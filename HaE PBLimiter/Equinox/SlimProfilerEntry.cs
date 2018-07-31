using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace HaE_PBLimiter.Equinox
{
    public class SlimProfilerEntry
    {
        private long _passes;
        private long _totalTime;
        private long _watchStartTime;
        private int _watchStarts;

        private ulong _startTick;
        private int _activeCount;

        public readonly string PassUnits;
        public readonly bool CounterOnly;

        public SlimProfilerEntry(string passUnits = null, bool counterOnly = false)
        {
            PassUnits = passUnits;
            CounterOnly = counterOnly;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Increment(long count)
        {
            Debug.Assert(CounterOnly, "Increment used on non-counter");
            Interlocked.Add(ref _passes, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void FastForward(TimeSpan ts, long passes = 1)
        {
            Debug.Assert(!CounterOnly, "Timing used on counter");
            Interlocked.Add(ref _passes, passes);
            Interlocked.Add(ref _totalTime, ts.Ticks * Stopwatch.Frequency / System.TimeSpan.TicksPerSecond);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Start()
        {
            Debug.Assert(!CounterOnly, "Timing used on counter");
            if (Interlocked.Add(ref _watchStarts, 1) == 1)
            {
                _watchStartTime = Stopwatch.GetTimestamp();
                Interlocked.Increment(ref _passes);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Stop()
        {
            Debug.Assert(!CounterOnly, "Timing used on counter");
            Debug.Assert(_watchStarts > 0);
            if (Interlocked.Add(ref _watchStarts, -1) == 0)
                Interlocked.Add(ref _totalTime, Stopwatch.GetTimestamp() - _watchStartTime);
        }

        internal bool IsActive
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _activeCount > 0;
        }

        internal void PushProfiler(ulong tickId)
        {
            if (Interlocked.Add(ref _activeCount, 1) != 1) return;
            _totalTime = 0;
            _startTick = tickId;
            _passes = 0;
        }

        /// <summary>
        /// Returns time per tick, in ms
        /// </summary>
        /// <param name="tickId"></param>
        /// <param name="hits">hits per tick</param>
        /// <returns></returns>
        internal double PopProfiler(ulong tickId, out double hits)
        {
            Debug.Assert(_activeCount > 0);
            Interlocked.Add(ref _activeCount, -1);
            var deltaTicks = (double)unchecked(tickId - _startTick);
            hits = (ulong)_passes / deltaTicks;
            var loadTimeMs = _totalTime * 1000D / Stopwatch.Frequency;
            return loadTimeMs / deltaTicks;
        }
    }
}
