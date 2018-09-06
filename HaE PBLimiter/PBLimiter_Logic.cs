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
using System.Windows.Controls;

namespace HaE_PBLimiter
{
    public class PBLimiter_Logic : TorchPluginBase, IWpfPlugin
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        internal static ITorchBase server;


        private PBLimiterUsercontrol _control;
        public UserControl GetControl() => _control ?? (_control = new PBLimiterUsercontrol(this));

        private static Persistent<ProfilerConfig> _config;
        public ProfilerConfig Config => _config?.Data;

        public override void Init(ITorchBase torch)
        {
            torch.GameStateChanged += OnGameStateChanged;
            var pgmr = new PBProfilerManager(torch);
            torch.Managers.AddManager(pgmr);

            _config = Persistent<ProfilerConfig>.Load(Path.Combine(StoragePath, "PBLimiter.cfg"));

            server = torch;

            ProfilerConfig.PlayerOverrides.CollectionChanged += PlayerOverrides_CollectionChanged;

            Log.Info("PBLimiter loaded!");
        }

        private void PlayerOverrides_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PBPlayerTracker.OnListChanged();
            Save();
        }

        public static void Save()
        {
            try
            {
                _config.Save();
                Log.Info("Configuration Saved.");
            }
            catch (IOException)
            {
                Log.Warn("Configuration failed to save");
            }
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

        /// <inheritdoc />
        public override void Dispose()
        {
            Save();
        }
    }
}
