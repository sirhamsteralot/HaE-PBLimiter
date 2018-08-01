using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaE_PBLimiter
{
    public class PBData
    {
        public static Dictionary<long, PBTracker> pbPair = new Dictionary<long, PBTracker>();

        public static void AddOrUpdatePair(long entityId, PBTracker pbTracker)
        {
            if (pbPair.ContainsKey(entityId))
            {

            }
        }

        
    }
}
