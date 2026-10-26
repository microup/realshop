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
        private int _moneyStableFrames = 0;
        private int _moneyPendingFrames = 0;
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

            if (StartingMoneyPatch.Pending)
            {
                _moneyPendingFrames++;
                if (_moneyPendingFrames > 3000)
                {
                    StartingMoneyPatch.Pending = false;
                }
                else if (_config.StartingMoney > 0)
                {
                    int cur = GlobalData.PlayerMoney;
                    if (cur != _config.StartingMoney)
                    {
                        GlobalData.AddPlayerMoney(_config.StartingMoney - cur);
                        MelonLogger.Msg("[Money] hold: " + cur + " -> " + _config.StartingMoney);
                        _moneyStableFrames = 0;
                    }
                    else if (StartingMoneyPatch.SceneLoaded)
                    {
                        _moneyStableFrames++;
                        if (_moneyStableFrames > 150)
                        {
                            StartingMoneyPatch.Pending = false;
                            MelonLogger.Msg("[Money] hold released");
                        }
                    }
                }
                else
                {
                    StartingMoneyPatch.Pending = false;
                }
            }
            else
            {
                _moneyPendingFrames = 0;
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

            if (_config.EnableMoneyHotkey && Input.GetKeyDown(_config.AddMoneyKey) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
            {
                int amount = _config.AddMoneyAmount;
                GlobalData.AddPlayerMoney(amount);
                UIManager.Get().ShowPopup("[RealShop]", $"+{amount} CR", PopupType.Normal);
                MelonLogger.Msg($"[Money] Ctrl+{_config.AddMoneyKey}: +{amount} (balance={GlobalData.PlayerMoney})");
            }

            if (Input.GetKeyDown(KeyCode.G) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
            {
                GarageNeedsRecon.Dump();
            }

            if (!_modIsEnable)
            {
                BuildJobState.Tick();
                BuildJobAcceptPatch.AbandonCandidate("mod disabled");
                return;
            }

            BuildJobState.Tick();
            BuildJobAcceptPatch.TickCommit();
            BuildJobService.TickOffers();
            _scene.Update(_config);
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            GameSettings.CanShowPopups = true;
            FreeTravelService.PendingFreeTravelCost = 0;

            if (StartingMoneyPatch.Pending)
            {
                MelonLogger.Msg("[Money] scene loaded: " + sceneName + " (idx=" + buildIndex + ")");
                if (sceneName != "SceneLoader")
                {
                    StartingMoneyPatch.SceneLoaded = true;
                }
            }

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
