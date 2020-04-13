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
using Sandbox.Game.Multiplayer;

namespace HaE_PBLimiter
{
    public class PBData
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private static Timer timer = new Timer(IteratePBs, null, 0, 1000);

        public static PBLimiterUsercontrol control;
        public static Dictionary<long, PBTracker> pbPair = new Dictionary<long, PBTracker>();

        public static void AddOrUpdatePair(MyProgrammableBlock entity, double runtime)
        {
            if (pbPair.ContainsKey(entity.EntityId))
            {
                pbPair[entity.EntityId].UpdatePerformance(runtime);
            }
            else
            {
                if (control == null)
                {
                    //Log.Warn("The Itemscontrol is NULL!");
                    return;
                }

                control.Dispatcher.Invoke(() =>
                {
                    lock (pbPair)
                    {
                        pbPair[entity.EntityId] = new PBTracker(entity, runtime);
                    }
                });
            }
        }

        public static void IteratePBs(object src)
        {
            //These shouldnt ever be null when the server is running but appearently one of them could be null?!

            if (MySession.Static == null)
            {
                //Log.Warn("MySession.Static is null!");
                return;
            }
            if (MySession.Static.Players == null)
            {
                Log.Warn("MySession.Static.Players is null!");
                return;
            }
            if (PBPlayerTracker.playerOverrideDict == null)
            {
                Log.Warn("PBPlayerTracker.playerOverrideDict is null!");
                return;
            }
            if (pbPair == null)
            {
                Log.Warn("pbPair is null!");
                return;
            }

            try
            {
                ResetPlayerMS();

                lock (pbPair)
                {
                    foreach (var tracker in pbPair.Values)
                    {
                        if (tracker.PB == null)
                            continue;

                        if (!tracker.PB.IsWorking)
                            continue;

                        if ((DateTime.Now - tracker.lastExecutionTime).TotalSeconds > ProfilerConfig.timeOutTime)
                            continue;

                        
                        MyPlayer.PlayerId id;
                        Sync.Players.TryGetPlayerId(tracker.PB.OwnerId, out id);
                        ulong pbOwner = id.SteamId;

                        double overriddenMax = ProfilerConfig.maxTickTime;

                        if (PBPlayerTracker.playerOverrideDict.ContainsKey(pbOwner))
                        {
                            CheckPlayerMax(pbOwner, ref overriddenMax);
                        }

                        tracker.UpdatePerformance();

                        if (ProfilerConfig.perPlayer)
                        {
                            if (!CheckPerPlayer(tracker, tracker.PB.OwnerId, overriddenMax))
                            {
                                DisablePbsTill(tracker.PB.OwnerId, overriddenMax);
                            }
                            continue;
                        }


                        tracker.CheckMax(overriddenMax);
                    }
                }
            } catch (Exception e)
            {
                Log.Error(e, $"Please report this crash with log on the github page!");
            }
        }

        private static void DisablePbsTill(long pbOwner, double maxRuntime)
        {
            Player owner;
            if (PBPlayerTracker.players.TryGetValue(pbOwner, out owner))
            {
                var id = owner.GetSlowestID();

                foreach(var tracker in pbPair.Values)
                {
                    if (id == tracker.PB.EntityId)
                        tracker.DamagePB();
                }
            }
        }

        private static bool CheckPerPlayer(PBTracker tracker, long pbOwner, double overriddenMax)
        {
            if (!PBPlayerTracker.players.ContainsKey(pbOwner))
            {
                PBPlayerTracker.players.Add(pbOwner, new Player());
            }

            return tracker.CheckMax(pbOwner, overriddenMax);
        }

        private static void CheckPlayerMax(ulong owner, ref double overriddenMax)
        {
            var player = PBPlayerTracker.playerOverrideDict[owner];

            if (player != null && player.OverrideEnabled)
                overriddenMax = player.PersonalMaxMs;
        }

        private static void ResetPlayerMS()
        {
            foreach (var player in PBPlayerTracker.players.Values)
            {
                player.ms = 0;
            }
        }
    }
}
