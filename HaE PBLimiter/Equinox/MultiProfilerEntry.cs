using System;
using System.Reflection;

namespace HaE_PBLimiter
{
    public struct MultiProfilerEntry
    {
        internal static readonly MethodInfo ProfilerEntryStart = ReflectionUtils.InstanceMethod(typeof(MultiProfilerEntry), nameof(Start));

        internal static readonly MethodInfo ProfilerEntryFastForward = ReflectionUtils.InstanceMethod(typeof(MultiProfilerEntry), nameof(FastForward));

        internal static readonly MethodInfo ProfilerEntryIncrement = ReflectionUtils.InstanceMethod(typeof(MultiProfilerEntry), nameof(Increment));

        internal static readonly MethodInfo ProfilerEntryStop = ReflectionUtils.InstanceMethod(typeof(MultiProfilerEntry), nameof(Stop));

        private int _count;
        private SlimProfilerEntry _entry0, _entry1, _entry2, _entry3, _entry4, _entry5;

        public bool Add(SlimProfilerEntry target)
        {
            if (target == null || !target.IsActive)
                return false;
            switch (_count++)
            {
                case 0:
                    _entry0 = target;
                    return true;
                case 1:
                    _entry1 = target;
                    return true;
                case 2:
                    _entry2 = target;
                    return true;
                case 3:
                    _entry3 = target;
                    return true;
                case 4:
                    _entry4 = target;
                    return true;
                case 5:
                    _entry5 = target;
                    return true;
                default:
                    return false;
            }
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public void Start()
        {
            if (_entry0 == null)
                return;
            _entry0.Start();
            if (_entry1 == null)
                return;
            _entry1.Start();
            if (_entry2 == null)
                return;
            _entry2.Start();
            if (_entry3 == null)
                return;
            _entry3.Start();
            if (_entry4 == null)
                return;
            _entry4.Start();
            // ReSharper disable once UseNullPropagation
            if (_entry5 == null)
                return;
            _entry5.Start();
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public void FastForward(TimeSpan ts, long passes = 1)
        {
            if (_entry0 == null)
                return;
            _entry0.FastForward(ts, passes);
            if (_entry1 == null)
                return;
            _entry1.FastForward(ts, passes);
            if (_entry2 == null)
                return;
            _entry2.FastForward(ts, passes);
            if (_entry3 == null)
                return;
            _entry3.FastForward(ts, passes);
            if (_entry4 == null)
                return;
            _entry4.FastForward(ts, passes);
            // ReSharper disable once UseNullPropagation
            if (_entry5 == null)
                return;
            _entry5.FastForward(ts, passes);
        }


        // ReSharper disable once MemberCanBePrivate.Global
        public void Increment(long ts)
        {
            if (_entry0 == null)
                return;
            _entry0.Increment(ts);
            if (_entry1 == null)
                return;
            _entry1.Increment(ts);
            if (_entry2 == null)
                return;
            _entry2.Increment(ts);
            if (_entry3 == null)
                return;
            _entry3.Increment(ts);
            if (_entry4 == null)
                return;
            _entry4.Increment(ts);
            // ReSharper disable once UseNullPropagation
            if (_entry5 == null)
                return;
            _entry5.Increment(ts);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public void Stop()
        {
            if (_entry0 == null)
                return;
            _entry0.Stop();
            if (_entry1 == null)
                return;
            _entry1.Stop();
            if (_entry2 == null)
                return;
            _entry2.Stop();
            if (_entry3 == null)
                return;
            _entry3.Stop();
            if (_entry4 == null)
                return;
            _entry4.Stop();
            // ReSharper disable once UseNullPropagation
            if (_entry5 == null)
                return;
            _entry5.Stop();
        }
    }
}
