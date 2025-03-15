using System;
using System.Reflection;
using System.Diagnostics;
using NLog;
using Sandbox.Game.Entities.Blocks;
using Torch.Managers.PatchManager;
using Torch.Utils;
using Torch.Utils.Reflected;

namespace HaE_PBLimiter
{
    /// <summary>
    ///     Used to track GC generations during PB execution.
    ///     If we find that the number of collections for a generation has increased somewhere in between the start of the PB's execution and the end of the PB's execution,
    ///     we can look into discarding the measured time value because it's bogus (it includes the time taken by GC).
    /// </summary>
    internal static class GcStateManager
    {
        private static readonly int[] Counters;

        static GcStateManager()
        {
            Counters = new int[GC.MaxGeneration + 1];
        }
        
        public static void EnterCriticalSection()
        {
            var counts = Counters;
            
            for (var i = 0; i < counts.Length; i++)
            {
                counts[i] = GC.CollectionCount(i);
            }
        }

        public static void ExitCriticalSectionAndCheckGc(out int maxGcCount)
        {
            maxGcCount = 0;
            
            var counts = Counters;
            for (var i = 0; i < counts.Length; i++)
            {
                var diff = GC.CollectionCount(i) - counts[i];

                if (diff > maxGcCount)
                {
                    maxGcCount = diff;
                }
            }
        }
    }
    
    [ReflectedLazy]
    internal static class PBProfilerPatch
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        [ReflectedMethodInfo(typeof(MyProgrammableBlock), "ExecuteCode")]
        private static readonly MethodInfo _programmableRunSandboxed;

        [ReflectedMethodInfo(typeof(MyProgrammableBlock), "Compile")]
        private static readonly MethodInfo _programableRecompile;
        
        public static void Patch(PatchContext ctx)
        {
            ReflectedManager.Process(typeof(PBProfilerPatch));

            ctx.GetPattern(_programmableRunSandboxed).Prefixes.Add(ReflectionUtils.StaticMethod(typeof(PBProfilerPatch), nameof(PrefixProfilePb)));
            ctx.GetPattern(_programmableRunSandboxed).Suffixes.Add(ReflectionUtils.StaticMethod(typeof(PBProfilerPatch), nameof(SuffixProfilePb)));
            ctx.GetPattern(_programableRecompile).Suffixes.Add(ReflectionUtils.StaticMethod(typeof(PBProfilerPatch), nameof(PrefixRecompilePb)));

            Log.Info("Finished Patching!");
        }

        // ReSharper disable InconsistentNaming
        // ReSharper disable once SuggestBaseTypeForParameter
        private static void PrefixProfilePb(MyProgrammableBlock __instance, ref long __localTimingStart)
        {
            GcStateManager.EnterCriticalSection();
            
            if (ProfilerConfig.takeIngameMeasurement)
                return;

            __localTimingStart = Stopwatch.GetTimestamp();
        }

        static FieldInfo runtimeField = typeof(MyProgrammableBlock).GetField("m_runtime", BindingFlags.Instance | BindingFlags.NonPublic);
        static PropertyInfo lastruntimeMS = typeof(MyProgrammableBlock)
            .GetNestedType("RuntimeInfo", BindingFlags.NonPublic)
            .GetProperty("LastRunTimeMs", BindingFlags.Public | BindingFlags.Instance);
        private static void SuffixProfilePb(MyProgrammableBlock __instance, ref long __localTimingStart)
        {
            double dtInSeconds;
            if (!ProfilerConfig.takeIngameMeasurement)
                dtInSeconds = (Stopwatch.GetTimestamp() - __localTimingStart) / (double)Stopwatch.Frequency;
            else
                dtInSeconds = (double)lastruntimeMS.GetValue(runtimeField.GetValue(__instance)) / 1000.0;

            GcStateManager.ExitCriticalSectionAndCheckGc(out var maxGcCount);

            if (maxGcCount == 1)
            {
                Log.Warn($"Dropped \"{__instance.CustomName}\" for GC. Time would have been {dtInSeconds * 1e6:F}us");
                dtInSeconds = 0.0;
            }
            
            PBData.AddOrUpdatePair(__instance, dtInSeconds);
        }

        private static void PrefixRecompilePb(MyProgrammableBlock __instance)
        {
            if (PBData.pbPair.TryGetValue(__instance.EntityId, out var pbData))
                pbData.SetRecompiled();
        }
    }
}
