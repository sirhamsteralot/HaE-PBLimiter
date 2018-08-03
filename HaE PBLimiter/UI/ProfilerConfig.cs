using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Torch;

namespace HaE_PBLimiter.UI
{
    public class ProfilerConfig
    {
        public static ObservableCollection<PBTracker> Trackers { get; } = new ObservableCollection<PBTracker>();
    }
}
