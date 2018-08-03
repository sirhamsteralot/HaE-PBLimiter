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


        public static void Patch(PatchContext ctx)
        {
            ReflectedManager.Process(typeof(PBProfilerPatch));

            ctx.GetPattern(_programmableRunSandboxed).Prefixes.Add(ReflectionUtils.StaticMethod(typeof(PBProfilerPatch), nameof(PrefixProfilePb)));
            ctx.GetPattern(_programmableRunSandboxed).Suffixes.Add(ReflectionUtils.StaticMethod(typeof(PBProfilerPatch), nameof(SuffixProfilePb)));

            Log.Info("Finished Patching!");
        }

        // ReSharper disable InconsistentNaming
        // ReSharper disable once SuggestBaseTypeForParameter
        private static void PrefixProfilePb(MyProgrammableBlock __instance, ref long __localTimingStart)
        {
            __localTimingStart = Stopwatch.GetTimestamp();
            Log.Info("PBRun");
        }

        private static void SuffixProfilePb(MyProgrammableBlock __instance, ref long __localTimingStart)
        {
            var dtInSeconds = (Stopwatch.GetTimestamp() - __localTimingStart) / (double)Stopwatch.Frequency;

            PBData.AddOrUpdatePair(__instance, dtInSeconds);
        }

    }
}
