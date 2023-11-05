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

        public static string[] CarNames = { 
            "Aston Martin DBS", "AstonMartinDBX", "AstonMartinVantage", "Atom330", "AtomRenton", "autotuttolavetino",
            "BoltMosquitoMK5", "BoltMosquitoMK6", "BoltReptiliaR6", "BoltThornGrandMojave", "CastorAvalanche",
            "DodgeChallengerSrtHellcatWideboy", "DodgeChallengerSrtHellcat", "DodgeViperGts", "EchosCobra", 
            "EchosImperator", "ElentiIza", "EmdenLotz", "FMWROADSTER",
            "FordMustang", "GriffinTypo", "HammerdalStraleS", "HinataMoonMk2", "JaguarF-Type",
            "LacroixFret", "LandRoverDefender90", "LandRoverDiscovery", "LandRoverRangeRoverEVOQUE", "LandRoverRangeRoverSportSvr", "MayenM3", "MayenT8",
            "mercedes-benzslsamg", "mioveniurs", "nissan370znismo", "nissangt-r(r35)", "olsengrandclub", "OlsenGtr", "PaganiHuayraTempesta",
            "paganihuayra", "paganizondarevolucion", "porche911carrera4s991", "rinobianco", "royaletown", "royalecrown", "PaganiHuayraTempesta",
            "SakuraYaro", "SalemEarthquakeRex", "SalemEarthquake", "SalemGw500", "SalemSpectre"
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
