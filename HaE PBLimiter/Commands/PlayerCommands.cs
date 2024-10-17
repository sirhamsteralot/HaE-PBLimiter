using Sandbox.Game.Multiplayer;
using System.Linq;
using System.Text;
using Torch.Commands;
using Torch.Commands.Permissions;
using Torch.Mod;
using Torch.Mod.Messages;
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
                double playerMax = ProfilerConfig.maxTickTime;
                if (PBPlayerTracker.players[player.IdentityId].OverrideEnabled)
                    playerMax = PBPlayerTracker.players[player.IdentityId].PersonalMaxMs;

                sb.AppendLine($"Player \"{player.DisplayName}\" Ms: {PBPlayerTracker.players[player.IdentityId].ms:F3}/{playerMax}\n");
            }

            foreach (var pb in PBData.pbPair.Values.Where(v => v.PB.OwnerId == player.IdentityId).OrderByDescending(v => v.AverageMS))
            {
                sb.AppendLine($"PB: \"{pb.PBID}\" Ms: {pb.AverageMS:F3}");
            }

            ModCommunication.SendMessageTo(new DialogMessage($"PBLimiter Status", null, sb.ToString()), Context.Player.SteamUserId);
        }

        [Command("investigate", "Lists pbs and their average runtimes owned a specific player")]
        [Permission(MyPromoteLevel.Moderator)]
        public void Investigate()
        {
            var player = Sync.Players.GetPlayerByName(Context.Args.First());

            if (player == null)
            {
                Context?.Respond("Could Not Find Player! Check Capitals and special characters!");

                return;
            }

            var playerIdentity = player.Identity;
            var playerIdentityId = playerIdentity.IdentityId;
            ulong playerId = Sync.Players.TryGetSteamId(playerIdentityId);
            var sb = new StringBuilder();

            if (ProfilerConfig.perPlayer && PBPlayerTracker.players.ContainsKey(playerIdentityId))
            {
                double playerMax = ProfilerConfig.maxTickTime;
                if (PBPlayerTracker.players[playerIdentityId].OverrideEnabled)
                    playerMax = PBPlayerTracker.players[playerIdentityId].PersonalMaxMs;

                sb.AppendLine($"Player \"{Sync.Players.TryGetPlayer(playerIdentityId)?.DisplayName ?? ""}\" Ms: {PBPlayerTracker.players[playerIdentityId].ms:F3}/{playerMax}\n");
            }

            foreach (var pb in PBData.pbPair.Values.Where(v => v.PB.OwnerId == playerIdentityId).OrderByDescending(v => v.AverageMS))
            {
                sb.AppendLine($"PB: \"{pb.PBID}\" Ms: {pb.AverageMS:F3}");
            }

            ModCommunication.SendMessageTo(new DialogMessage($"PBLimiter Status", null, sb.ToString()), Context.Player.SteamUserId);
        }
    }
}
