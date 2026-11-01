using MelonLoader;

namespace RealShop
{
    public partial class Config
    {
        private int MigrateAdvancePercent()
        {
            if (_buildJobAdvancePercent == null || _buildJobAdvancePercent.Value != LegacyBuildJobAdvancePercent)
            {
                return 0;
            }

            MelonLogger.Msg("[BuildJob] cfg advance migrated from the previous default: "
                + LegacyBuildJobAdvancePercent + "% -> " + ValueBuildJobAdvancePercent
                + "% (BuildJobAdvancePercent drives the card line, accept/advance popups and the final payout)");
            _buildJobAdvancePercent.Value = ValueBuildJobAdvancePercent;
            return 1;
        }
    }
}
