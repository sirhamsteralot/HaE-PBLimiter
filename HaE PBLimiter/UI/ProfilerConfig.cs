using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Torch;
using System.Xml.Serialization;

namespace HaE_PBLimiter
{
    public class ProfilerConfig
    {
        private static int _startupTicks;
        public static int startupTicks { get { return _startupTicks; } set { _startupTicks = value; PBLimiter_Logic.Save(); } }
        public int SerializeWrapTicks { get { return _startupTicks; } set { _startupTicks = value;} }

        private static double _maxTickTime;
        public static double maxTickTime { get { return _maxTickTime; } set { _maxTickTime = value; PBLimiter_Logic.Save(); } }
        public double SerializeWrapTime { get { return _maxTickTime; } set { _maxTickTime = value;} }

        [XmlIgnore()]
        public static ObservableCollection<PBTracker> Trackers => new ObservableCollection<PBTracker>(PBData.pbPair.Values);
    }
}
