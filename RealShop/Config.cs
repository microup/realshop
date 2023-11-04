using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RealShop
{
    public class Config
    {
        public const string ModName = "RealShop";

        public static string[] Brands = { "trando", "voiz", "fierte", "shushutri", "bardogh" };

        public static string[] TargetRepairParts = { "mirror", "window", "seat", "taillight", "pasek", "lancuch",
            "srodkowy", "katalizator", "koncowy", "rura", "slizg", "filtr", "inverter", "wheel", "turbo", "kable", "challenger",
            "washer", "reservoir", "srodkowy", "bak", "rolka", "walka", "pasek", "poczatkowy", "charging", "cewka", "tlumik", "wzdluzny"};
        public static int[] RepairCost = {150, 250, 400};

        private const string ConfigCategoryName = "MainSection";
        private const string ConfigRepairSection = "RepairSection";

        private readonly MelonPreferences_Category _config;
        private readonly MelonPreferences_Category _repair;
        private readonly MelonPreferences_Entry<KeyCode> _isEnableSwitchModeBindKey;
        private readonly MelonPreferences_Entry<KeyCode> _printStackBindKey;
        private readonly MelonPreferences_Entry<KeyCode> _repairBindKey;
        private readonly MelonPreferences_Entry<bool> _isShowDebugInfo;
        private readonly MelonPreferences_Entry<int> _removeCarsOlderThanYear;
        private readonly MelonPreferences_Entry<bool> _isEnableBrandFilter;
        private readonly MelonPreferences_Entry<string[]> _brandsEntry;
        private readonly MelonPreferences_Entry<string[]> _repairParts;
        private readonly MelonPreferences_Entry<int[]> _repairCost;

        public KeyCode IsEnableSwitchMode => _isEnableSwitchModeBindKey.Value;
        public KeyCode PrintStack => _printStackBindKey.Value;
        public KeyCode Repair => _repairBindKey.Value;
        public bool IsShowDebugInfo => _isShowDebugInfo.Value;
        public int RemoveCarsOlderThanYear => _removeCarsOlderThanYear.Value;
        public string[] PartBrands => _brandsEntry.Value;
        public string[] RepairParts => _repairParts.Value;
        public int[] ListRepairCost => _repairCost.Value;
        public bool IsEnableBrandFilter => _isEnableBrandFilter.Value;

        public Config()
        {
            _config = MelonPreferences.CreateCategory(ConfigCategoryName);
            _config.SetFilePath("Mods/RealShop.cfg");

            _isEnableSwitchModeBindKey = _config.CreateEntry(nameof(IsEnableSwitchMode), KeyCode.F9);

            _isShowDebugInfo = _config.CreateEntry(nameof(IsShowDebugInfo), false);
            _removeCarsOlderThanYear = _config.CreateEntry(nameof(RemoveCarsOlderThanYear), 2000);
            _isEnableBrandFilter = _config.CreateEntry(nameof(IsEnableBrandFilter), true);
            _brandsEntry = _config.CreateEntry(nameof(PartBrands), Brands);

            _repair = MelonPreferences.CreateCategory(ConfigRepairSection);
            _repair.SetFilePath("Mods/RealShop.cfg");

            _repairBindKey = _repair.CreateEntry(nameof(Repair), KeyCode.F8);
            _repairParts = _repair.CreateEntry(nameof(RepairParts), TargetRepairParts);
            _repairCost = _repair.CreateEntry(nameof(ListRepairCost), RepairCost);
        }

        public void Reload()
        {
            _config.LoadFromFile();

        }
    }
}
