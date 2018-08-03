using Torch.API;
using Torch.Managers;
using Torch.Managers.PatchManager;
using NLog;


namespace HaE_PBLimiter
{
    public class PBProfilerManager : Manager
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

#pragma warning disable 649
        [Dependency(Ordered = false)]
        private readonly PatchManager _patchMgr;
#pragma warning restore 649

        public PBProfilerManager(ITorchBase torchInstance) : base(torchInstance)
        {
        }

        private static bool _patched = false;
        private PatchContext _patchContext;

        /// <inheritdoc cref="Manager.Attach"/>
        public override void Attach()
        {
            base.Attach();
            if (!_patched)
            {
                _patched = true;
                _patchContext = _patchMgr.AcquireContext();
                PBProfilerPatch.Patch(_patchContext);
            }

            Log.Info("Attached");
        }

        /// <inheritdoc cref="Manager.Detach"/>
        public override void Detach()
        {
            base.Detach();
            if (_patched)
            {
                _patched = false;
                _patchMgr.FreeContext(_patchContext);
            }

            Log.Info("Detached");
        }
    }
}
