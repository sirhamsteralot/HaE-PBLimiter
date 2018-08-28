using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaE_PBLimiter
{
    public class PBPlayerTracker
    {
        public static Dictionary<long, Player> players = new Dictionary<long, Player>();

        public class Player {
            public double ms;

            public void Reset()
            {
                ms = 0;
            }
        }
    }
}
