using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using Sandbox;
using Sandbox.Game;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Blocks;
using VRage;
using VRage.Game;
using VRage.Game.Entity;
using Torch.API.Plugins;
using Torch.API;
using Torch;
using HaE_PBLimiter.Equinox;

namespace HaE_PBLimiter
{
    public class PBLimiter_Logic : TorchPluginBase
    {
        public override void Init(ITorchBase torch)
        {
            torch.GameStateChanged += OnGameStateChanged;
            var pgmr = new PBProfilerManager(torch);
            torch.Managers.AddManager(pgmr);
        }

        private void OnGameStateChanged(MySandboxGame game, TorchGameState newState)
        {
            if ((newState & TorchGameState.Loaded) != 0)
            {
                Torch.Managers.GetManager(typeof(PBProfilerManager)).Attach();
            }
            else
            {
                Torch.Managers.GetManager(typeof(PBProfilerManager)).Detach();
            }
        }
    }
}
