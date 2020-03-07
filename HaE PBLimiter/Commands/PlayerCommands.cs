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

            foreach (var pb in PBData.pbPair.Values)
            {
                if (pb.PB.OwnerId == player.IdentityId)
                    sb.AppendLine($"PB: \"{pb.PBID}\" Ms: {pb.AverageMS:F3}");
            }

            Context.Respond(sb.ToString());
        }
    }
}
