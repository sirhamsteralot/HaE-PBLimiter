using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Torch.Commands;
using Torch.Commands.Permissions;
using VRage.Game.ModAPI;

namespace HaE_PBLimiter.Commands
{
    [Category("pblimiter")]
    public class PlayerCommands : CommandModule
    {
        [Command("status", "Lists pbs and their average runtimes owned by the player")]
        [Permission(MyPromoteLevel.None)]
        public void PBStatus()
        {
            var player = Context?.Player;

            if (player == null)
                return;

            var sb = new StringBuilder();

            if (ProfilerConfig.perPlayer && PBPlayerTracker.players.ContainsKey(player.IdentityId))
            {
                sb.AppendLine($"Player \"{player.DisplayName}\" Ms: {PBPlayerTracker.players[player.IdentityId]?.ms}/{ProfilerConfig.maxTickTime}");
            }

            foreach (var pb in PBData.pbPair.Values.Where(v => v.PB.OwnerId == player.IdentityId).OrderByDescending(v => v.AverageMS))
            {
                sb.AppendLine($"PB: \"{pb.PBID}\" Ms: {pb.AverageMS:F3}");
            }

            Context.Respond(sb.ToString());
        }
    }
}
