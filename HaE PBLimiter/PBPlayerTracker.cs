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

        public static Dictionary<string, Player> playerOverrideDict = new Dictionary<string, Player>();

        public static void OnListChanged()
        {
            playerOverrideDict.Clear();

            foreach (var entry in ProfilerConfig.PlayerOverrides)
            {
                var player = new Player(entry.Name, entry.PersonalMaxMs, entry.OverrideEnabled);

                playerOverrideDict.Add(player.Name, player);
            }
        }
    }
}
