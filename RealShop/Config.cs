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

        public static string[] OldEngine = { "(I3)", "(I6 B)", "(V8)", "(V8 OHV)" };

        public static string[] TargetRepairParts = { "mirror", "window", "seat", "taillight", "pasek", "lancuch",
            "srodkowy", "katalizator", "koncowy", "rura", "slizg", "filtr", "inverter", "wheel", "turbo", "kable", "challenger",
            "washer", "reservoir", "srodkowy", "bak", "rolka", "walka", "pasek", "poczatkowy", "charging", "cewka", "tlumik", "wzdluzny"};

        public static string[] CarNames = { 
            "Aston Martin DBS", "Aston Martin DBX", "Aston Martin Vantage", "Atom 330", "Atom Renton", "Autotutto Lavetino",
            "Bolt Mosquito MK5", "Bolt Mosquito MK6", "Bolt Reptilia R6", "Bolthorn Grand Mojave", "Castor Avalanche",
            "Dodge Challenger SRT Hellcat Widebody", "Dodge Challenger SRT Hellcat", "Dodge Viper GTS", "Echos Cobra", 
            "Echos Imperator", "Elenti Izi", "Emden Lotz", "FMW Roadster", "Ford F350 Super Duty", "Ford Mustang",
            "Griffin Tyro", "Hammerdal Strale S", "Hinata Moon MK2", "Jaguar F-Type", "Lacroix Fret",
            "Land Rover Defender 90", "Land Rover Discovery", "Land Rover Range Rover Evoque", "Land Rover Range Rover Sport SVR",
            "Mayen M3", "Mayen T8", "Mercedes-Benz SLS AMG", "Mioveni Urs", "Nissan 370z Nismo", "Nissan GT-R (R35)",
            "Olsen Grand Club", "Olsen GTR", "Pagani Huayra Tempesta", "Pagani Huayra", "Pagani Zonda Revolucion",
            "Porsche 911 Carrera 4S (991)", "Rino Bianco", "Rino Piccolo", "Royale Town", "Royale Crown", "Pagani Huayra Tempesta",
            "Sakura Yaro", "Salem Earthquake Rex", "Salem Earthquake", "Salem GW500", "Salem Spectre"
        };

        public static int[] RepairCost = {150, 250, 400};

        private const uint RemoveCarsOlderYear = 2005;

        private const string ConfigCategoryName = "MainSection";
        private const string ConfigCarSection = "CarSection";
        private const string ConfigRepairSection = "RepairSection";

        private readonly MelonPreferences_Category _config;
        private readonly MelonPreferences_Category _repair;
        private readonly MelonPreferences_Category _carSection;
        private readonly MelonPreferences_Entry<KeyCode> _isKeyBindEnableSwitchMode;
        private readonly MelonPreferences_Entry<KeyCode> _printStackBindKey;
        private readonly MelonPreferences_Entry<KeyCode> _isKeyBindRepair;
        private readonly MelonPreferences_Entry<bool> _isKeyBindShowDebugInfo;
        private readonly MelonPreferences_Entry<uint> _removeCarsOlderThanYear;
        private readonly MelonPreferences_Entry<bool> _isEnableBrandFilter;
        private readonly MelonPreferences_Entry<string[]> _carNames;
        private readonly MelonPreferences_Entry<string[]> _brandsEntry;
        private readonly MelonPreferences_Entry<string[]> _repairParts;
        private readonly MelonPreferences_Entry<string[]> _oldEngine;
        private readonly MelonPreferences_Entry<int[]> _repairCost;

        public KeyCode IsKeyBindEnableSwitchMode => _isKeyBindEnableSwitchMode.Value;
        public KeyCode PrintStack => _printStackBindKey.Value;
        public KeyCode IsKeyBindRepair => _isKeyBindRepair.Value;
        public bool IsKeyBindShowDebugInfo => _isKeyBindShowDebugInfo.Value;
        public uint RemoveCarsOlderThanYear => _removeCarsOlderThanYear.Value;
        public string[] PartBrands => _brandsEntry.Value;
        public string[] RepairParts => _repairParts.Value;
        public string[] ListCarNames => _carNames.Value;
        public int[] ListRepairCost => _repairCost.Value;
        public string[] ListOldEngine => _oldEngine.Value;
        public bool IsEnableBrandFilter => _isEnableBrandFilter.Value;

        public Config()
        {
            _config = MelonPreferences.CreateCategory(ConfigCategoryName);
            _config.SetFilePath("Mods/RealShop.cfg");
            _isKeyBindEnableSwitchMode = _config.CreateEntry(nameof(IsKeyBindEnableSwitchMode), KeyCode.F9);
            _isKeyBindShowDebugInfo = _config.CreateEntry(nameof(IsKeyBindShowDebugInfo), false);

            _carSection = MelonPreferences.CreateCategory(ConfigCarSection);
            _carSection.SetFilePath("Mods/RealShop.cfg");
            _removeCarsOlderThanYear = _carSection.CreateEntry(nameof(RemoveCarsOlderThanYear), RemoveCarsOlderYear);
            _isEnableBrandFilter = _carSection.CreateEntry(nameof(IsEnableBrandFilter), true);
            _brandsEntry = _carSection.CreateEntry(nameof(PartBrands), Brands);
            _oldEngine = _carSection.CreateEntry(nameof(ListOldEngine), OldEngine);

            _repair = MelonPreferences.CreateCategory(ConfigRepairSection);
            _repair.SetFilePath("Mods/RealShop.cfg");
            _isKeyBindRepair = _repair.CreateEntry(nameof(IsKeyBindRepair), KeyCode.F8);
            _repairCost = _repair.CreateEntry(nameof(ListRepairCost), RepairCost);
            _repairParts = _repair.CreateEntry(nameof(RepairParts), TargetRepairParts);
            _carNames = _repair.CreateEntry(nameof(ListCarNames), CarNames);
        }

        public void Reload()
        {
            _config.LoadFromFile();

        }
    }
}
