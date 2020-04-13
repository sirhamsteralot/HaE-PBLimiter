using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Torch;
using System.Xml.Serialization;
using VRageMath;

namespace HaE_PBLimiter
{
    public class ProfilerConfig
    {
        private static int _startupTicks = 20;
        public static int startupTicks { get { return _startupTicks; } set { _startupTicks = value; PBLimiter_Logic.Save(); } }
        public int SerializeWrapTicks { get { return _startupTicks; } set { _startupTicks = value; } }

        private static double _violationsMult = 0.1;
        public static double violationsMult { get { return _violationsMult; } set { _violationsMult = value; PBLimiter_Logic.Save(); } }
        public double SerializeWrapViolationsMult { get { return _violationsMult; } set { _violationsMult = value; } }

        private static double _maxTickTime = 0.5;
        public static double maxTickTime { get { return _maxTickTime; } set { _maxTickTime = value; PBLimiter_Logic.Save(); } }
        public double SerializeWrapTime { get { return _maxTickTime; } set { _maxTickTime = value; } }

        private static double _timeOutTime = 300;
        public static double timeOutTime { get { return _timeOutTime; } set { _timeOutTime = value; PBLimiter_Logic.Save(); } }
        public double SerializeWrapTimeOutTime { get { return _timeOutTime; } set { _timeOutTime = value; } }

        private static double _tickSignificance = 0.005;
        public static double tickSignificance { get { return _tickSignificance; } set { _tickSignificance = MyMath.Clamp((float)value, 0, 1); PBLimiter_Logic.Save(); } }
        public double SerializeWrapSignificance { get { return _tickSignificance; } set { _tickSignificance = value; } }

        private static bool _perPlayer = false;
        public static bool perPlayer { get { return _perPlayer; } set { _perPlayer = value; PBLimiter_Logic.Save(); } }
        public bool SerializeWrapPerPlayer { get { return _perPlayer; } set { _perPlayer = value; } }

        private static bool _allowCleanup = false;
        public static bool allowCleanup { get { return _allowCleanup; } set { _allowCleanup = value; PBLimiter_Logic.Save(); } }
        public bool SerializeWrapAllowCleanup { get { return _allowCleanup; } set { _allowCleanup = value; } }

        private static ObservableCollection<Player> _playerOverrides = new ObservableCollection<Player>();
        public static ObservableCollection<Player> PlayerOverrides => _playerOverrides;
        public ObservableCollection<Player> SerializeWrapPlayerOverrides { get => _playerOverrides; set { _playerOverrides = value; } }


        private static bool _takeIngameMeasurement = true;
        public static bool takeIngameMeasurement { get => _takeIngameMeasurement; set => _takeIngameMeasurement = value; }
        public bool TakeIngameMeasurement { get => _takeIngameMeasurement; set => _takeIngameMeasurement = value; }

    }
}
