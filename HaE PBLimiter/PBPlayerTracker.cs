using NLog;
using Sandbox.Game.Multiplayer;
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

        public static Dictionary<ulong, Player> playerOverrideDict = new Dictionary<ulong, Player>();

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();


        public static void OnListChanged()
        {
            playerOverrideDict.Clear();

            foreach (var entry in ProfilerConfig.PlayerOverrides)
            {
                var player = new Player(entry.Name, entry.SteamId, entry.PersonalMaxMs, entry.OverrideEnabled);

                if (player.SteamId == 0)
                {
                    if (player.Name == null && player.SteamId == 0)
                    {
                        return;
                    }
                    else if (!string.IsNullOrEmpty(player.Name))
                    {
                        try {
                            player.SteamId = Sync.Players.GetPlayerByName(player.Name).Id.SteamId;
                        } catch(Exception e)
                        {
                            Log.Warn($"Players name {player.Name} did not resolve to a steam ID!");
                        }

                    }
                }



                playerOverrideDict.Add(player.SteamId, player);
            }
        }
    }
}
