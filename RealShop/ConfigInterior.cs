using MelonLoader;

namespace RealShop
{
    public partial class Config
    {
        private MelonPreferences_Entry<bool> _buildJobStripInteriorUnmount;
        private MelonPreferences_Entry<bool> _buildJobStripInteriorShow;
        private MelonPreferences_Entry<bool> _buildJobStripInteriorInstant;
        private MelonPreferences_Entry<int> _buildJobInteriorStrippedJobId;

        partial void CreateInteriorStripEntries();

        public bool BuildJobStripInteriorUnmount => _buildJobStripInteriorUnmount.Value;
        public bool BuildJobStripInteriorShow => _buildJobStripInteriorShow.Value;
        public bool BuildJobStripInteriorInstant => _buildJobStripInteriorInstant.Value;
        public int BuildJobInteriorStrippedJobId => _buildJobInteriorStrippedJobId.Value;

        public void RememberInteriorStripped(int jobId)
        {
            _buildJobInteriorStrippedJobId.Value = jobId;
            MelonPreferences.Save();
        }

        public void ClearInteriorStripped()
        {
            _buildJobInteriorStrippedJobId.Value = ValueBuildJobInteriorStrippedJobId;
            MelonPreferences.Save();
        }

        partial void CreateInteriorStripEntries()
        {
            _buildJobStripInteriorUnmount = _cfgSectionCars.CreateEntry(nameof(BuildJobStripInteriorUnmount), ValueBuildJobStripInteriorUnmount);
            _buildJobStripInteriorShow = _cfgSectionCars.CreateEntry(nameof(BuildJobStripInteriorShow), ValueBuildJobStripInteriorShow);
            _buildJobStripInteriorInstant = _cfgSectionCars.CreateEntry(nameof(BuildJobStripInteriorInstant), ValueBuildJobStripInteriorInstant);
            _buildJobInteriorStrippedJobId = _cfgSectionCars.CreateEntry(nameof(BuildJobInteriorStrippedJobId), ValueBuildJobInteriorStrippedJobId);
        }
    }
}
