using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Blocks;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.World;
using VRage;
using VRage.Entities;
using NLog;
using System.Threading;


namespace HaE_PBLimiter
{
    public class PBData
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private static Timer timer = new Timer(IteratePBs, null, 0, 1000);


        public static Dictionary<long, PBTracker> pbPair = new Dictionary<long, PBTracker>();

        public static void AddOrUpdatePair(MyProgrammableBlock entity, double runtime)
        {
            if (pbPair.ContainsKey(entity.EntityId))
            {
                pbPair[entity.EntityId].UpdatePerformance(runtime);
            }
            else
            {
                lock (pbPair)
                {
                    pbPair[entity.EntityId] = new PBTracker(entity, runtime);
                }
            }
        }

        public static void IteratePBs(object src)
        {
            foreach (var player in PBPlayerTracker.players.Values)
            {
                player.ms = 0;
            }

            

            lock (pbPair)
            {
                foreach (var tracker in pbPair.Values)
                {
                    long pbOwner = tracker.PB.OwnerId;
                    string ownerName = MySession.Static.Players.TryGetIdentity(pbOwner).DisplayName;
                    double overriddenMax = ProfilerConfig.maxTickTime;

                    if (PBPlayerTracker.playerOverrideDict.ContainsKey(ownerName))
                    {
                        var player = PBPlayerTracker.playerOverrideDict[ownerName];

                        if (player != null && player.OverrideEnabled)
                            overriddenMax = player.PersonalMaxMs;
                    }
                   

                    if (ProfilerConfig.perPlayer)
                    {

                        if (!PBPlayerTracker.players.ContainsKey(pbOwner))
                        {
                            PBPlayerTracker.players.Add(pbOwner, new Player());
                        }

                        tracker.CheckMax(pbOwner , overriddenMax);
                        continue;
                    }
                        

                    tracker.CheckMax(overriddenMax);
                }
            }
        }
    }
}
