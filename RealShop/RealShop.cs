using CMS.UI.Logic;
using CMS.UI.Logic.Map;
using CMS.UI.Logic.RepairPart;
using CMS.UI.Windows;
using CMS.Containers;
using CMS.Helpers;
using CMS.Extensions;
using Harmony;
using MelonLoader;
using System;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable CS0618

namespace RealShop
{
    public class RealParts : MelonMod
    {
        internal const string JunkyardMapDestinationButtonName = "MapDestinationButton (5)";
        internal const float RedPartConditionThreshold = 15f;
        internal const int FullyRepairedPartConditionToShow = 100;
        internal const float FullyRepairedPartConditionThreshold = 99.5f;
        internal const float MinimumRepairConditionIncrease = 1f;
        internal const int RepairGroupAvailableFromStart = 1;
        internal const int DefaultJunkyardTravelCost = 500;

        internal static Config ActiveConfig { get; private set; }

        internal static bool IsFullyRepaired(float value)
        {
            return (value >= FullyRepairedPartConditionThreshold / 100f && value <= 1.0001f) || value >= FullyRepairedPartConditionThreshold;
        }

        internal static bool IsDamagedRed(float value)
        {
            if (value <= 0f)
            {
                return false;
            }

            return value <= 1f ? value < RedPartConditionThreshold / 100f : value < RedPartConditionThreshold;
        }

        private Config _config;
        private bool _modIsEnable = true;
        private bool _isRunOnce = false;
        private readonly SceneUiController _scene = new SceneUiController();

        [Obsolete]
        public override void OnApplicationStart()
        {
            MelonLogger.Msg("initializing...");

            _config = new Config();
            _config.Reload();
            ActiveConfig = _config;

            PatchBootstrap.Install(this.Harmony);
        }

        public override void OnUpdate()
        {
            if (_config == null)
            {
                return;
            }

            if (Input.GetKeyDown(_config.IsKeyBindEnableSwitchMode))
            {
                if (_modIsEnable)
                {
                    _modIsEnable = false;
                }
                else
                {
                    _modIsEnable = true;
                }
                UIManager.Get().ShowPopup("[RealShop]", $"the mod has been status: {_modIsEnable}.", PopupType.Normal);

                MelonLogger.Msg($"the mod has been {_modIsEnable}");
            }

            if (!_modIsEnable)
            {
                return;
            }

            _scene.Update(_config);
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            GameSettings.CanShowPopups = true;
            FreeTravelService.PendingFreeTravelCost = 0;

            if (buildIndex == 1)
            {
                var buildVersion = GameSettings.BuildVersion;

                MelonLogger.Msg($"GameSettings.BuildVersion = {buildVersion} ");
            }

            if (buildIndex == 10 && !_isRunOnce)
            {
                RetroPartCatalog.Initialize();

                _isRunOnce = true;
            }
        }
    }
}

#pragma warning restore CS0618
