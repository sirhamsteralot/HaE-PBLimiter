using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using Sandbox.Game;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Blocks;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;
using Torch;
using Torch.Server;
using System.Xml;
using System.Xml.Serialization;
using HaE_PBLimiter.UI;

namespace HaE_PBLimiter
{
    public class PBTracker
    {
        public double PBID => PB.EntityId;
        public double AverageMS => averageMs;

        [XmlIgnore()]
        public MyProgrammableBlock PB;

        [XmlIgnore()]
        public double averageMs;

        public PBTracker()
        {

        }

        public PBTracker(MyProgrammableBlock PB, double average)
        {
            this.PB = PB;

            double Ms = average * 1000;
            this.averageMs += 0.01 * Ms;
        }

        public void UpdatePerformance(double dt)
        {
            var ms = dt * 1000;

            averageMs = 0.99 * averageMs + 0.01 * ms;
        }

        public void CheckMax(double maximumAverageMS)
        {
            if (averageMs > maximumAverageMS)
            {
                DamagePB();
            }
        }

        private void DamagePB()
        {
            if (!PB.IsFunctional)
                return;
            var damage = PB.SlimBlock.BlockDefinition.MaxIntegrity - PB.SlimBlock.BlockDefinition.MaxIntegrity * PB.SlimBlock.BlockDefinition.CriticalIntegrityRatio;
            TorchBase.Instance.Invoke(() => { PB.SlimBlock.DoDamage(damage, MyDamageType.Fire); });
        }
    }
}
