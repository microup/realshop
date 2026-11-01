using MelonLoader;

namespace RealShop
{
    public partial class Config
    {
        private int MigrateBuildFromScratchOn()
        {
            if (_enableBuildFromScratchJobs == null || _enableBuildFromScratchJobs.Value)
            {
                return 0;
            }

            MelonLogger.Msg("[BuildJob] cfg EnableBuildFromScratchJobs migrated to true (build-from-scratch orders are on by default)");
            _enableBuildFromScratchJobs.Value = true;
            return 1;
        }
    }
}
