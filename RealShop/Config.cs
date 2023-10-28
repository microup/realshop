using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealShop
{
    public class Config
    {
        public const string ModName = "RealShop";

        private const string ConfigCategoryName = "MainSection";

        private readonly MelonPreferences_Category _config;
        private readonly MelonPreferences_Entry<int> _removeCarsOlderThanYear;

        public int RemoveCarsOlderThanYear => _removeCarsOlderThanYear.Value;
        public Config()
        {
            _config = MelonPreferences.CreateCategory(ConfigCategoryName);
            _config.SetFilePath("Mods/RealShop.cfg");

            _removeCarsOlderThanYear = _config.CreateEntry(nameof(RemoveCarsOlderThanYear), 2000);
        }

        public void Reload()
        {
            _config.LoadFromFile();

        }
    }
}
