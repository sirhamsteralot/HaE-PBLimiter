using Sandbox.ModAPI;
using System.Reflection;
using System;
using Sandbox.Game.World;
using Sandbox.Game.Entities.Blocks;
using VRage.Game;
using Torch;
using Torch.API.Managers;
using NLog;
using VRageMath;

namespace HaE_PBLimiter
{
    public class PBTracker
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public string PBID { get { return $"{PB.CubeGrid.DisplayName}-{PB.CustomName}"; } }        
        public bool IsEnabled => PB.Enabled;
        public double AverageMS => Math.Round(averageMs, 5);
        public string Owner => MySession.Static.Players.TryGetIdentity(PB.OwnerId).DisplayName;


        private int StartupTicks => ProfilerConfig.startupTicks;
        public MyProgrammableBlock PB;
        public double averageMs;
        public DateTime lastExecutionTime;
        
        private int startTick;
        private int violations;
        private int LastPerformanceUpdateFrame = MySession.Static.GameplayFrameCounter;

        public PBTracker(PBTracker clone)
        {
            this.PB = clone.PB;
            this.averageMs = clone.averageMs;
            this.lastExecutionTime = clone.lastExecutionTime;
            this.violations = clone.violations;
            this.startTick = clone.startTick;
            this.LastPerformanceUpdateFrame = clone.LastPerformanceUpdateFrame;
        }

        public PBTracker(MyProgrammableBlock PB, double average)
        {
            this.PB = PB;
        }

        public void UpdatePerformance(double dt)
        {
            lastExecutionTime = DateTime.Now;

            if (startTick < StartupTicks)
            {
                startTick++;
                LastPerformanceUpdateFrame = MySession.Static.GameplayFrameCounter;
                return;
            }
 

            var ms = dt * 1000;

            UpdatePerformance();

            averageMs += ProfilerConfig.tickSignificance * ms;
        }

        public void UpdatePerformance() {
            int frame = MySession.Static.GameplayFrameCounter;

            if (frame == LastPerformanceUpdateFrame) {
                return;
            }

            averageMs = averageMs * Math.Pow(1 - ProfilerConfig.tickSignificance, frame - LastPerformanceUpdateFrame);
            
            LastPerformanceUpdateFrame = frame;
        }

        public void CheckMax(double maximumAverageMS)
        {
            if (averageMs > maximumAverageMS)
            {
                violations++;

                if (violations > ProfilerConfig.maxViolations)
                {
                    DamagePB();
                }
            }
        }

        public bool CheckMax(long owner, double maximumAverageMS)
        {
            PBPlayerTracker.players[owner].ms += averageMs;
            PBPlayerTracker.players[owner].UpdatePB(PB, maximumAverageMS);

            if (PBPlayerTracker.players[owner].ms > maximumAverageMS)
            {
                return false;
            }

            return true;
        }

        public void SetRecompiled()
        {
            averageMs = 0;
            startTick = 0;
        }

        static FieldInfo needsInstansiationField = typeof(MyProgrammableBlock).GetField("m_needsInstantiation", BindingFlags.NonPublic | BindingFlags.Instance);
        static MethodInfo Terminate = typeof(MyProgrammableBlock).GetMethod("OnProgramTermination", BindingFlags.NonPublic | BindingFlags.Instance);


        public bool DamagePB()
        {
            if (PB == null || !PB.IsFunctional)
            {
                SetRecompiled();
                return false;
            }


            float damage = PB.SlimBlock.BlockDefinition.MaxIntegrity - PB.SlimBlock.BlockDefinition.MaxIntegrity * PB.SlimBlock.BlockDefinition.CriticalIntegrityRatio;
            damage += (float)(damage * (violations * ProfilerConfig.violationsMult));
            TorchBase.Instance.Invoke(() => 
            {
                try {
                    PB.SlimBlock.DoDamage(damage, MyDamageType.Fire, true, null, 0);
                    PB.Enabled = false;

                    if (ProfilerConfig.allowCleanup)
                    {
                        PB.RunSandboxedProgramAction(delegate (IMyGridProgram program)
                        {
                            program.Save();
                        }, out string response);
                    }

                    needsInstansiationField.SetValue(PB, false);
                    Terminate.Invoke(PB, new object[] 
                    {
                        MyProgrammableBlock.ScriptTerminationReason.InstructionOverflow
                    });
                } catch (NullReferenceException)
                { 

                }
            });

            Player owner;
            if (PBPlayerTracker.players.TryGetValue(PB.OwnerId, out owner))
            {
                owner.ms -= averageMs;
            }
            
            averageMs = 0;
            startTick = 0;
            ulong PBOwnerID = MySession.Static.Players.TryGetSteamId(PB.OwnerId);
            
            if (PBOwnerID != 0) {
                var chatManager = PBLimiter_Logic.server?.CurrentSession.Managers.GetManager<IChatManagerServer>();
                chatManager?.SendMessageAsOther("Server", $"Your PB {PBID} has overheated due to excessive usage!", Color.Red, PBOwnerID); 
            }

            return true;
        }
    }
}
