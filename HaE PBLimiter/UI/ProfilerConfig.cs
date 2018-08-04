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
        [XmlIgnore()]
        public static ObservableCollection<PBTracker> Trackers => new ObservableCollection<PBTracker>(PBData.pbPair.Values);

        private static int _startupTicks = 10;
        public static int startupTicks => _startupTicks;

        private static double _maxTickTime = 0.5;
        public static double maxTickTime => _maxTickTime;
    }
}
