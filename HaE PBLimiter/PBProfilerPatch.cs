using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Diagnostics;
using Havok;
using NLog;
using Sandbox.Engine.Physics;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Blocks;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.World;
using Torch.Managers.PatchManager;
using Torch.Managers.PatchManager.MSIL;
using Torch.Utils;
using Torch.Utils.Reflected;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.Entity;
using VRage.Game.Entity.EntityComponents.Interfaces;
using VRage.ModAPI;
using HaE_PBLimiter.Equinox;

namespace HaE_PBLimiter
{
    [ReflectedLazy]
    internal static class PBProfilerPatch
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        [ReflectedMethodInfo(typeof(MyProgrammableBlock), "RunSandboxedProgramAction")]
        private static readonly MethodInfo _programmableRunSandboxed;

        [ReflectedMethodInfo(typeof(MyGameLogic), nameof(MyGameLogic.UpdateBeforeSimulation))]
        private static readonly MethodInfo _gameLogicUpdateBeforeSimulation;

        [ReflectedMethodInfo(typeof(MyGameLogic), nameof(MyGameLogic.UpdateAfterSimulation))]
        private static readonly MethodInfo _gameLogicUpdateAfterSimulation;

        [ReflectedMethodInfo(typeof(MyGameLogic), nameof(MyGameLogic.UpdateOnceBeforeFrame))]
        private static readonly MethodInfo _gameLogicUpdateOnceBeforeFrame;

        [ReflectedMethodInfo(typeof(MyEntities), nameof(MyEntities.UpdateBeforeSimulation))]
        private static readonly MethodInfo _entitiesUpdateBeforeSimulation;

        [ReflectedMethodInfo(typeof(MyEntities), nameof(MyEntities.UpdateAfterSimulation))]
        private static readonly MethodInfo _entitiesUpdateAfterSimulation;

        private static MethodInfo _distributedUpdaterIterate;

        public static void Patch(PatchContext ctx)
        {
            ReflectedManager.Process(typeof(PBProfilerPatch));

            _distributedUpdaterIterate = typeof(MyDistributedUpdater<,>).GetMethod("Iterate");
            var duiP = _distributedUpdaterIterate?.GetParameters();
            if (_distributedUpdaterIterate == null || duiP == null || duiP.Length != 1 ||
                typeof(Action<>) != duiP[0].ParameterType.GetGenericTypeDefinition())
            {
                Log.Error(
                    $"Unable to find MyDistributedUpdater.Iterate(Delegate) method.  Profiling will not function.  (Found {_distributedUpdaterIterate}");
                return;
            }

            PatchDistributedUpdate(ctx, _gameLogicUpdateBeforeSimulation);
            PatchDistributedUpdate(ctx, _gameLogicUpdateAfterSimulation);
            PatchDistributedUpdate(ctx, _entitiesUpdateBeforeSimulation);
            PatchDistributedUpdate(ctx, _entitiesUpdateAfterSimulation);

            var patcher = typeof(PBProfilerPatch).GetMethod(nameof(TranspilerForUpdate),
        BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)?
    .MakeGenericMethod(typeof(MyGameLogicComponent));
            if (patcher == null)
            {
                Log.Error($"Failed to make generic patching method for composite updates");
            }

            ctx.GetPattern(_gameLogicUpdateOnceBeforeFrame).PostTranspilers.Add(patcher);

            ctx.GetPattern(_programmableRunSandboxed).Prefixes.Add(ReflectionUtils.StaticMethod(typeof(PBProfilerPatch), nameof(PrefixProfilePb)));
            ctx.GetPattern(_programmableRunSandboxed).Suffixes.Add(ReflectionUtils.StaticMethod(typeof(PBProfilerPatch), nameof(SuffixProfilePb)));
        }

        // ReSharper disable InconsistentNaming
        // ReSharper disable once SuggestBaseTypeForParameter
        private static void PrefixProfilePb(MyProgrammableBlock __instance, ref long __localTimingStart)
        {
            __localTimingStart = Stopwatch.GetTimestamp();
        }

        private static void SuffixProfilePb(MyProgrammableBlock __instance, ref long __localTimingStart)
        {
            var dtInSeconds = (Stopwatch.GetTimestamp() - __localTimingStart) / (double)Stopwatch.Frequency;

            PBData.AddOrUpdatePair(__instance, dtInSeconds);
        }

        private static void PatchDistributedUpdate(PatchContext ctx, MethodBase callerMethod)
        {
            var foundAnyIterate = false;
            var msil = PatchUtilities.ReadInstructions(callerMethod).ToList();
            for (var i = 0; i < msil.Count; i++)
            {
                var insn = msil[i];
                if ((insn.OpCode != OpCodes.Callvirt && insn.OpCode != OpCodes.Call) ||
                    !IsDistributedIterate((insn.Operand as MsilOperandInline<MethodBase>)?.Value as MethodInfo)) continue;

                foundAnyIterate = true;
                // Call to Iterate().  Backtrace up the instruction stack to find the statement creating the delegate.
                var foundNewDel = false;
                for (var j = i - 1; j >= 1; j--)
                {
                    var insn2 = msil[j];
                    if (insn2.OpCode != OpCodes.Newobj) continue;
                    var ctorType = (insn2.Operand as MsilOperandInline<MethodBase>)?.Value?.DeclaringType;
                    if (ctorType == null || !ctorType.IsGenericType || ctorType.GetGenericTypeDefinition() != typeof(Action<>)) continue;
                    foundNewDel = true;
                    // Find the instruction loading the function pointer this delegate is created with
                    var ldftn = msil[j - 1];
                    if (ldftn.OpCode != OpCodes.Ldftn ||
                        !(ldftn.Operand is MsilOperandInline<MethodBase> targetMethod))
                    {
                        Log.Error(
                            $"Unable to find ldftn instruction for call to Iterate in {callerMethod.DeclaringType}#{callerMethod}");
                    }
                    else
                    {
                        Log.Debug(
                            $"Patching {targetMethod.Value.DeclaringType}#{targetMethod.Value} for {callerMethod.DeclaringType}#{callerMethod}");
                        PatchDistUpdateDel(ctx, targetMethod.Value);
                    }

                    break;
                }

                if (!foundNewDel)
                {
                    Log.Error(
                        $"Unable to find new Action() call for Iterate in {callerMethod.DeclaringType}#{callerMethod}");
                }
            }

            if (!foundAnyIterate)
                Log.Error($"Unable to find any calls to {_distributedUpdaterIterate} in {callerMethod.DeclaringType}#{callerMethod}");
        }

        private static bool IsDistributedIterate(MethodInfo info)
        {
            if (info == null)
                return false;
            if (!info.DeclaringType?.IsGenericType ?? true)
                return false;
            if (info.DeclaringType?.GetGenericTypeDefinition() != _distributedUpdaterIterate.DeclaringType)
                return false;
            var aps = _distributedUpdaterIterate.GetParameters();
            var ops = info.GetParameters();
            if (aps.Length != ops.Length)
                return false;
            for (var i = 0; i < aps.Length; i++)
                if (aps[i].ParameterType.GetGenericTypeDefinition() != ops[i].ParameterType.GetGenericTypeDefinition())
                    return false;
            return true;
        }

        private static void PatchDistUpdateDel(PatchContext ctx, MethodBase method)
        {
            var pattern = ctx.GetPattern(method);
            var patcher = typeof(PBProfilerPatch).GetMethod(nameof(TranspilerForUpdate),
                    BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)?
                .MakeGenericMethod(method.GetParameters()[0].ParameterType);
            if (patcher == null)
            {
                Log.Error($"Failed to make generic patching method for {method}");
            }

            pattern.PostTranspilers.Add(patcher);
        }

        private static bool ShouldProfileMethodCall<T>(MethodBase info)
        {
            if (info.IsStatic)
                return false;

            if (typeof(T) != typeof(MyCubeGridSystems) && !typeof(T).IsAssignableFrom(info.DeclaringType) &&
                (!typeof(MyGameLogicComponent).IsAssignableFrom(typeof(T)) || !typeof(IMyGameLogicComponent).IsAssignableFrom(info.DeclaringType)))
                return false;
            if (typeof(T) == typeof(MySessionComponentBase) && info.Name.Equals("Simulate", StringComparison.OrdinalIgnoreCase))
                return true;
            return info.Name.StartsWith("UpdateBeforeSimulation", StringComparison.OrdinalIgnoreCase) ||
                   info.Name.StartsWith("UpdateAfterSimulation", StringComparison.OrdinalIgnoreCase) ||
                   info.Name.StartsWith("UpdateOnceBeforeFrame", StringComparison.OrdinalIgnoreCase);
        }

        private static IEnumerable<MsilInstruction> TranspilerForUpdate<T>(IEnumerable<MsilInstruction> instructions,
            // ReSharper disable once InconsistentNaming
            Func<Type, MsilLocal> __localCreator,
            // ReSharper disable once InconsistentNaming
            MethodBase __methodBase)
        {


            var usedLocals = new List<MsilLocal>();
            var tmpArgument = new Dictionary<Type, Stack<MsilLocal>>();

            var foundAny = false;
            foreach (var i in instructions)
            {
                {
                    var target = ((MsilOperandInline<MethodBase>)i.Operand).Value;
                    var parameters = target.GetParameters();
                    usedLocals.Clear();
                    foreach (var pam in parameters)
                    {
                        if (!tmpArgument.TryGetValue(pam.ParameterType, out var stack))
                            tmpArgument.Add(pam.ParameterType, stack = new Stack<MsilLocal>());
                        var local = stack.Count > 0 ? stack.Pop() : __localCreator(pam.ParameterType);
                        usedLocals.Add(local);
                        yield return local.AsValueStore();
                    }

                    Log.Debug(
                        $"Attaching profiling to {target?.DeclaringType?.FullName}#{target?.Name} in {__methodBase.DeclaringType?.FullName}#{__methodBase.Name} targeting {typeof(T)}");
                    yield return new MsilInstruction(OpCodes.Dup); // duplicate the object the update is called on
                    if (typeof(MyCubeGridSystems) == typeof(T) && __methodBase.DeclaringType == typeof(MyCubeGridSystems))
                    {
                        yield return new MsilInstruction(OpCodes.Ldarg_0);
                    }

                    for (var j = usedLocals.Count - 1; j >= 0; j--)
                    {
                        yield return usedLocals[j].AsValueLoad();
                        tmpArgument[usedLocals[j].Type].Push(usedLocals[j]);
                    }

                    yield return i;


                    foundAny = true;
                    continue;
                }

                yield return i;
            }

            if (!foundAny)
                Log.Warn($"Didn't find any update profiling targets for {typeof(T)} in {__methodBase.DeclaringType?.FullName}#{__methodBase.Name}");
        }
    }
}
