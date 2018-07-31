using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Torch;
using Torch.API;
using Torch.API.Plugins;

namespace HaE_PBLimiter
{
    public class Class1 : TorchPluginBase//, IWpfPlugin - uncomment for GUI in torch
    {
        /*===========| Optional |===========*/
        //private Persistent<ConfigClass> _config;  - uncomment for Config
        //public UserControl GetControl() => _control ?? (_control = new ControlClass(this));   - uncomment for GUI in torch


        private static readonly Logger Log = LogManager.GetCurrentClassLogger();


        public void Save()
        {
            /*===========| Optional |===========*\
            try
            {
                
                _config.Save();
                Log.Info("Configuration Saved.");
            }
            catch (IOException)
            {
                Log.Warn("Configuration failed to save");
            }
                uncomment for Saving Config
            \*==================================*/
        }

        public override void Init(ITorchBase torch)
        {
            base.Init(torch);

            Log.Debug("loading _config");
            //_config = Persistent<BackupConfig>.Load(Path.Combine(StoragePath, "Backup.cfg")); - uncomment for Loading Config on plugin load
            Log.Debug("_config loaded");

            //All Initialization stuff goes here
        }

        public override void Update()
        {
            base.Update();

            //All updatey stuff goes here
        }

        public override void Dispose()
        {
            //_config.Save(); -uncomment for saving Config on plugin dispose
        }
    }
}
