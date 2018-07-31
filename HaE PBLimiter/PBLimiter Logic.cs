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

namespace HaE_PBLimiter
{
    public class PBLimiter_Logic : ITorchPlugin
    {
        public Guid Id => new Guid("9bb557f4-867d-4766-9907-4a1c28347db7");

        public string Version => "0.1";

        public string Name => "HaE PBLimiter";

        public HashSet<MyProgrammableBlock> PBs = new HashSet<MyProgrammableBlock>();
        public bool gameLoaded;

        public void Init(ITorchBase torchBase)
        {
            torchBase.GameStateChanged += OnGameStateChanged;
            MyEntities.OnEntityAdd += OnEntityAdded;
        }

        private void OnEntityAdded(MyEntity entity)
        {

        }

        private void OnGameStateChanged(MySandboxGame game, TorchGameState newState)
        {
            if ((newState & TorchGameState.Loaded) != 0)
                gameLoaded = true;
            if (!gameLoaded)
                return;

                
        }

        public void Update()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
