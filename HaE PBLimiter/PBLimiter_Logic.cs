using Sandbox;
using System.IO;
using NLog;
using Torch;
using Torch.API;
using Torch.API.Plugins;
using System.Windows.Controls;

namespace HaE_PBLimiter
{
    public class PBLimiter_Logic : TorchPluginBase, IWpfPlugin
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        internal static ITorchBase server;


        private PBLimiterUsercontrol _control;
        public UserControl GetControl()
        {
            if (_control == null)
            {
                _control = new PBLimiterUsercontrol(this);
                PBData.control = _control;
            }

            return _control;
        }

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

            PBPlayerTracker.OnListChanged();
            Log.Info("PBLimiter loaded!");
        }

        private void PlayerOverrides_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PBPlayerTracker.OnListChanged();
            Log.Info("Configuration changed!");
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
