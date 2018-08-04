using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Torch;

namespace HaE_PBLimiter
{
    public class ProfilerConfig
    {
        public static ObservableCollection<PBTracker> Trackers => new ObservableCollection<PBTracker>(PBData.pbPair.Values);
    }
}
