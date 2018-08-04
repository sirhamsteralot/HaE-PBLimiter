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
using System.IO;
using Sandbox.Game;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Blocks;
using VRage;
using VRage.Game;
using VRage.Game.Entity;
using NLog;
using Torch;
using Torch.API;
using Torch.API.Plugins;
using Torch.Server;
using Torch.Server.Views;
using Torch.Server.ViewModels;
using Torch.Utils;
using Torch.Views;
using Torch.ViewModels;

using HaE_PBLimiter.Equinox;
using System.Windows.Controls;

namespace HaE_PBLimiter
{
    public class PBLimiter_Logic : TorchPluginBase, IWpfPlugin
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();


        private PBLimiterUsercontrol _control;
        public UserControl GetControl() => _control ?? (_control = new PBLimiterUsercontrol(this));

        private ProfilerConfig _config;
        public ProfilerConfig Config => _config;

        public override void Init(ITorchBase torch)
        {
            torch.GameStateChanged += OnGameStateChanged;
            var pgmr = new PBProfilerManager(torch);
            torch.Managers.AddManager(pgmr);

            _config = new ProfilerConfig();
            Log.Info("PBLimiter loaded!");
        }

        private void OnGameStateChanged(MySandboxGame game, TorchGameState newState)
        {
            if ((newState & TorchGameState.Loaded) == TorchGameState.Loaded)
            {
                Torch.Managers.GetManager(typeof(PBProfilerManager)).Attach();
            }
            else if ((newState & TorchGameState.Unloading) != 0)
            {
                Torch.Managers.GetManager(typeof(PBProfilerManager)).Detach();
            }
        }

        public override void Update()
        {
            base.Update();
        }
    }
}
