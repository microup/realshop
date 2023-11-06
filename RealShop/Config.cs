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

        public static string[] ListActiveBrands = { "trando", "voiz", "fierte", "shushutri", "bardogh" };

        public static string[] ListWhiteCarParts = { "I4 DT224", "I4 204PT", "V8 392 Hemi", "V10 OHV MT8", "V10 OHV", "V12 AM11", "B6 MA", "B6 M64", "eDen-1H", "Proton e-R", };

        public static string[] OldEngine = { "(I3)", "(R6)", "(I6 B)", "(V8)", "(V8 OHV)" }; 

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

        private const uint RemoveCarsOlderYear = 2005;

        private const string ConfigCategoryName = "MainSection";
        private const string ConfigCarSection = "CarSection";
        private const string ConfigRepairSection = "RepairSection";

        private readonly MelonPreferences_Category _config;
        private readonly MelonPreferences_Category _repair;
        private readonly MelonPreferences_Category _carSection;
        private readonly MelonPreferences_Entry<KeyCode> _isKeyBindEnableSwitchMode;
        private readonly MelonPreferences_Entry<KeyCode> _printStackBindKey;
        private readonly MelonPreferences_Entry<uint> _removeCarsOlderThanYear;
        private readonly MelonPreferences_Entry<bool> _isEnableBrandFilter;
        private readonly MelonPreferences_Entry<string[]> _carNames;
        private readonly MelonPreferences_Entry<string[]> _activeBrands;
        private readonly MelonPreferences_Entry<string[]> _whiteCarParts;
        private readonly MelonPreferences_Entry<string[]> _oldEngine;

        public KeyCode IsKeyBindEnableSwitchMode => _isKeyBindEnableSwitchMode.Value;
        public uint RemoveCarsOlderThanYear => _removeCarsOlderThanYear.Value;
        public string[] ActiveBrands => _activeBrands.Value;
        public string[] ListCarNames => _carNames.Value;
        public string[] ListOldEngine => _oldEngine.Value;
        public string[] WhiteCarParts => _whiteCarParts.Value;
        public bool IsEnableBrandFilter => _isEnableBrandFilter.Value;

        public Config()
        {
            _config = MelonPreferences.CreateCategory(ConfigCategoryName);
            _config.SetFilePath("Mods/RealShop.cfg");
            _isKeyBindEnableSwitchMode = _config.CreateEntry(nameof(IsKeyBindEnableSwitchMode), KeyCode.F9);

            _carSection = MelonPreferences.CreateCategory(ConfigCarSection);
            _carSection.SetFilePath("Mods/RealShop.cfg");
            _removeCarsOlderThanYear = _carSection.CreateEntry(nameof(RemoveCarsOlderThanYear), RemoveCarsOlderYear);
            _whiteCarParts = _carSection.CreateEntry(nameof(WhiteCarParts), ListWhiteCarParts);
            _isEnableBrandFilter = _carSection.CreateEntry(nameof(IsEnableBrandFilter), true);
            _activeBrands = _carSection.CreateEntry(nameof(ActiveBrands), ListActiveBrands);
            _oldEngine = _carSection.CreateEntry(nameof(ListOldEngine), OldEngine);

            _repair = MelonPreferences.CreateCategory(ConfigRepairSection);
            _repair.SetFilePath("Mods/RealShop.cfg");
            _carNames = _repair.CreateEntry(nameof(ListCarNames), CarNames);
        }

        public void Reload()
        {
            _config.LoadFromFile();

        }
    }
}
