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

namespace HaE_PBLimiter
{
    public class PBTracker
    {
        public MyProgrammableBlock PB;
            

        public double sum;
        public double average;

        public double[] performanceHistory = new double[100];

        public void UpdatePerformance()
        {

        }


        private void CalculateAverage()
        {
            average = sum / performanceHistory.Length;
        }
    }
}
