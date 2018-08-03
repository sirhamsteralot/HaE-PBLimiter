using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Blocks;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.World;

namespace HaE_PBLimiter
{
    public class PBData
    {
        public static Dictionary<long, PBTracker> pbPair = new Dictionary<long, PBTracker>();

        public static void AddOrUpdatePair(MyProgrammableBlock entity, double runtime)
        {
            if (pbPair.ContainsKey(entity.EntityId))
            {
                pbPair[entity.EntityId].UpdatePerformance(runtime);
            } else
            {
                pbPair[entity.EntityId] = new PBTracker(entity, runtime);
            }
        }

        public static void IteratePBs(double maxAvgMs)
        {
            lock (pbPair)
            {
                foreach (var tracker in pbPair.Values)
                {
                    tracker.CheckMax(maxAvgMs);
                }
            }
        }
    }
}
