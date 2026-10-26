using MelonLoader;
using UnityEngine;

namespace RealShop
{
    public partial class Config
    {
        public void RememberAdvance(int jobId, int amount)
        {
            _buildJobAdvanceJobId.Value = jobId;
            _buildJobAdvanceAmount.Value = amount;
            MelonPreferences.Save();
        }
        public void ClearAdvance()
        {
            _buildJobAdvanceJobId.Value = -1;
            _buildJobAdvanceAmount.Value = 0;
            MelonPreferences.Save();
        }

        public void RememberConditionJob(int jobId)
        {
            _buildJobConditionJobId.Value = jobId;
            MelonPreferences.Save();
        }
        public void ClearConditionJob()
        {
            _buildJobConditionJobId.Value = -1;
            MelonPreferences.Save();
        }

        public void RememberBuildJobId(int id)
        {
            _buildJobLastIssuedId.Value = id;
            MelonPreferences.Save();
        }

        public int RepairBuildJobConfig()
        {
            int repaired = 0;
            repaired += RepairTextEntry(_buildJobDifficultyLabel, "difficulty");
            repaired += RepairTextEntry(_buildJobLabelBody, "body");
            repaired += RepairTextEntry(_buildJobLabelParts, "parts");
            repaired += RepairTextEntry(_buildJobLabelInterior, "interior");
            repaired += RepairTextEntry(_buildJobLabelEngine, "engine");
            repaired += RepairTextEntry(_buildJobLabelReward, "reward");
            repaired += RepairTextEntry(_buildJobTakePopup, "takePopup");
            repaired += RepairTextEntry(_buildJobNotReadyPrefix, "notReadyPrefix");
            repaired += RepairTextEntry(_buildJobLabelAdvance, "advance");
            repaired += RepairTextEntry(_buildJobLabelAdvanceSuffix, "advanceSuffix");
            repaired += RepairTextEntry(_buildJobAdvancePopup, "advancePopup");

            int migrated = 0;
            int guard = 0;
            int frame = 0;
            int wheels = 0;
            int susp = 0;
            int prefixes = 0;
            int block = 0;
            if (_buildJobConfigSchema == null || _buildJobConfigSchema.Value < ValueBuildJobConfigSchema)
            {
                migrated = MigrateStripMethods();
                guard = MigrateGuardSeconds();
                frame = MigrateKeepFrame();
                wheels = MigrateHideWheels();
                susp = MigrateSuspensionObjects();
                prefixes = MigrateUnmountPrefixes();
                block = MigrateBlockCancelOff();
                if (_buildJobConfigSchema != null)
                {
                    _buildJobConfigSchema.Value = ValueBuildJobConfigSchema;
                }
            }

            if (repaired > 0 || migrated > 0 || guard > 0 || frame > 0 || wheels > 0 || susp > 0 || prefixes > 0
                || block > 0)
            {
                MelonPreferences.Save();
                MelonLogger.Msg("[BuildJob] cfg repaired: textKeys=" + repaired + " stripMethods=" + migrated
                    + " guardSeconds=" + guard + " keepFrame=" + frame + " hideWheels=" + wheels
                    + " suspensionObjects=" + susp + " unmountPrefixes=" + prefixes + " blockCancel=" + block);
            }

            return repaired + migrated + guard + frame + wheels + susp + prefixes + block;
        }

        private static int RepairTextEntry(MelonPreferences_Entry<string> entry, string key)
        {
            if (entry == null || BuildJobTextRu.IsUsable(entry.Value))
            {
                return 0;
            }

            string russian = BuildJobTextRu.Get(key);
            if (string.IsNullOrEmpty(russian))
            {
                return 0;
            }

            MelonLogger.Warning("[BuildJob] cfg text is not text (ASCII save destroyed it): " + entry.Identifier
                + " = \"" + entry.Value + "\", restored code default");
            entry.Value = russian;
            return 1;
        }

        private int MigrateStripMethods()
        {
            if (_buildJobStripMethods == null)
            {
                return 0;
            }

            string current = _buildJobStripMethods.Value;
            if (current == null)
            {
                return 0;
            }

            current = current.Trim();
            if (System.String.Equals(current, ValueBuildJobStripMethods, System.StringComparison.Ordinal))
            {
                return 0;
            }

            if (!System.String.Equals(current, LegacyBuildJobStripMethods, System.StringComparison.Ordinal)
                && !System.String.Equals(current, LegacyBuildJobStripMethodsDev23, System.StringComparison.Ordinal)
                && !System.String.Equals(current, LegacyBuildJobStripMethodsDev25, System.StringComparison.Ordinal))
            {
                return 0;
            }

            MelonLogger.Msg("[BuildJob] cfg strip methods migrated from the previous default: "
                + ValueBuildJobStripMethods);
            _buildJobStripMethods.Value = ValueBuildJobStripMethods;
            return 1;
        }

        private int MigrateGuardSeconds()
        {
            if (_buildJobConditionGuardSeconds == null)
            {
                return 0;
            }

            if (_buildJobConditionGuardSeconds.Value != LegacyBuildJobConditionGuardSeconds)
            {
                return 0;
            }

            MelonLogger.Msg("[BuildJob] cfg guard seconds migrated from the previous default: "
                + LegacyBuildJobConditionGuardSeconds + " -> " + ValueBuildJobConditionGuardSeconds);
            _buildJobConditionGuardSeconds.Value = ValueBuildJobConditionGuardSeconds;
            return 1;
        }

        private int MigrateKeepFrame()
        {
            if (_buildJobStripKeepFrame == null || _buildJobStripKeepFrame.Value != LegacyBuildJobStripKeepFrame)
            {
                return 0;
            }

            MelonLogger.Msg("[BuildJob] cfg keep-frame migrated from the previous default: true -> "
                + ValueBuildJobStripKeepFrame);
            _buildJobStripKeepFrame.Value = ValueBuildJobStripKeepFrame;
            return 1;
        }

        private int MigrateHideWheels()
        {
            if (_buildJobStripHideWheels == null || _buildJobStripHideWheels.Value != LegacyBuildJobStripHideWheelsTrue)
            {
                return 0;
            }

            MelonLogger.Msg("[BuildJob] cfg hide-wheels migrated from the previous default: true -> "
                + ValueBuildJobStripHideWheels + " (hiding is cosmetic: the game cannot mount wheels it does not see)");
            _buildJobStripHideWheels.Value = ValueBuildJobStripHideWheels;
            return 1;
        }

        private int MigrateSuspensionObjects()
        {
            if (_buildJobStripSuspensionObjects == null
                || _buildJobStripSuspensionObjects.Value != LegacyBuildJobStripSuspensionObjectsFalse)
            {
                return 0;
            }

            MelonLogger.Msg("[BuildJob] cfg suspension objects migrated from the previous default: false -> "
                + ValueBuildJobStripSuspensionObjects + " (user: remove the suspension, accept the blockers)");
            _buildJobStripSuspensionObjects.Value = ValueBuildJobStripSuspensionObjects;
            return 1;
        }

        private int MigrateBlockCancelOff()
        {
            if (_buildJobBlockCancel == null || _buildJobBlockCancel.Value != LegacyBuildJobBlockCancelTrue)
            {
                return 0;
            }

            MelonLogger.Msg("[BuildJob] cfg block-cancel migrated back: true -> " + ValueBuildJobBlockCancel
                + " (the ~8s CancelJob is the game finishing the accept: blocking it keeps the offer in the list,"
                + " live accept not confirmed ... stillOffered)");
            _buildJobBlockCancel.Value = ValueBuildJobBlockCancel;
            return 1;
        }

        private int MigrateUnmountPrefixes()
        {
            if (_buildJobStripUnmountPrefixes == null
                || !string.Equals(_buildJobStripUnmountPrefixes.Value, LegacyBuildJobStripUnmountPrefixes,
                    System.StringComparison.Ordinal))
            {
                return 0;
            }

            MelonLogger.Msg("[BuildJob] cfg unmount prefixes migrated from the previous default: "
                + "window_front added (the game keeps the front window mounted, live mountedPanels=1)");
            _buildJobStripUnmountPrefixes.Value = ValueBuildJobStripUnmountPrefixes;
            return 1;
        }

        public void RememberBuildJobEngine(int jobId, string engineType)
        {
            _buildJobEngineJobId.Value = jobId;
            _buildJobEngineType.Value = engineType == null ? "" : engineType;
            MelonPreferences.Save();
        }

        public void ClearBuildJobEngine()
        {
            _buildJobEngineJobId.Value = -1;
            _buildJobEngineType.Value = ValueBuildJobEngineType;
            MelonPreferences.Save();
        }

        public string BuildJobStripMode => _buildJobStripMode.Value;
        public string BuildJobStripMethods => _buildJobStripMethods.Value;
        public int BuildJobStripPanelChance => _buildJobStripPanelChance.Value;
        public int BuildJobStripConditionPercent => _buildJobStripConditionPercent.Value;
        public int BuildJobStripDustPercent => _buildJobStripDustPercent.Value;
        public int BuildJobConditionGuardSeconds => _buildJobConditionGuardSeconds.Value;
        public int BuildJobConditionReapplyMax => _buildJobConditionReapplyMax.Value;
        public string BuildJobPanelNamePrefixes => _buildJobPanelNamePrefixes.Value;
        public string BuildJobDifficultyLabel => _buildJobDifficultyLabel.Value;
        public string BuildJobLabelBody => _buildJobLabelBody.Value;
        public string BuildJobLabelParts => _buildJobLabelParts.Value;
        public string BuildJobLabelInterior => _buildJobLabelInterior.Value;
        public string BuildJobLabelEngine => _buildJobLabelEngine.Value;
        public bool BuildJobStripKeepFrame => _buildJobStripKeepFrame.Value;
        public bool BuildJobStripEngineObject => _buildJobStripEngineObject.Value;
        public int BuildJobConfigSchema => _buildJobConfigSchema.Value;
        public bool BuildJobStripWheels => _buildJobStripWheels.Value;
        public bool BuildJobStripHideWheels => _buildJobStripHideWheels.Value;
        public int BuildJobStripSuspensionParts => _buildJobStripSuspensionParts.Value;
        public int BuildJobGuardKeepSeconds => _buildJobGuardKeepSeconds.Value;
        public string BuildJobStripSuspensionNames => _buildJobStripSuspensionNames.Value;
        public bool BuildJobStripSuspensionObjects => _buildJobStripSuspensionObjects.Value;
        public bool BuildJobKeepGuardOnCancel => _buildJobKeepGuardOnCancel.Value;
        public bool BuildJobBlockCancel => _buildJobBlockCancel.Value;
        public string BuildJobSuspensionToggleKey => _buildJobSuspensionToggleKey.Value;
        public string BuildJobStripUnmountPrefixes => _buildJobStripUnmountPrefixes.Value;
        public int BuildJobEngineJobId => _buildJobEngineJobId.Value;
        public string BuildJobEngineType => _buildJobEngineType.Value;
        public string BuildJobLabelReward => _buildJobLabelReward.Value;
        public string BuildJobTakePopup => _buildJobTakePopup.Value;
        public string BuildJobNotReadyPrefix => _buildJobNotReadyPrefix.Value;

        public bool EnableDonorCondition => _enableDonorCondition.Value;
        public float DonorConditionScale => _donorConditionScale.Value;        public bool EnableMaxQuality => _enableMaxQuality.Value;
        public int MaxQualityCommonMin => _maxQualityCommonMin.Value;
        public int MaxQualityCommonMax => _maxQualityCommonMax.Value;
        public int MaxQualityRareMin => _maxQualityRareMin.Value;
        public int MaxQualityRareMax => _maxQualityRareMax.Value;
        public int MaxQualityUniqueMin => _maxQualityUniqueMin.Value;
        public int MaxQualityUniqueMax => _maxQualityUniqueMax.Value;
        public int MaxQualityBodyMin => _maxQualityBodyMin.Value;
        public int MaxQualityBodyMax => _maxQualityBodyMax.Value;
        public int MaxQualityCommonWeight => _maxQualityCommonWeight.Value;
        public int MaxQualityRareWeight => _maxQualityRareWeight.Value;
        public int MaxQualityUniqueWeight => _maxQualityUniqueWeight.Value;
        public bool MaxQualityRevealAfterRepair => _maxQualityRevealAfterRepair.Value;


        public string[] ListCarNames => _carNames.Value;
        public string[] ListRetroParts => _retroParts.Value;
        public string[] WhiteCarParts => _whiteCarParts.Value;
        public bool IsEnableBrandFilter => _isEnableBrandFilter.Value;

        public bool IsEnableShop => _isEnableShop.Value;
        public bool IsEnableAdditionalAuction => _isEnableAdditionalAuction.Value;
        public bool IsEnableShopCarParts => _isEnableShopCarParts.Value;

        public string[] LstCarPartsCantBeRepair => _carPartsCantBeRepair.Value;

        public string[] ListRetroPartYears => _retroPartYears.Value;
        public string[] ListSportGT => _sportGT.Value;
        public string[] ListSpecialRetro => _specialRetro.Value;
        public Config()
        {
            _config = MelonPreferences.CreateCategory(ConfigCategoryName);
            _config.SetFilePath("Mods/RealShop.cfg");
            _isKeyBindEnableSwitchMode = _config.CreateEntry(nameof(IsKeyBindEnableSwitchMode), KeyCode.F9);
            _addMoneyKey = _config.CreateEntry(nameof(AddMoneyKey), KeyCode.S);
            _addMoneyAmount = _config.CreateEntry(nameof(AddMoneyAmount), 1000);
            _enableMoneyHotkey = _config.CreateEntry(nameof(EnableMoneyHotkey), true);
            _startingMoney = _config.CreateEntry(nameof(StartingMoney), 20000);
            _isEnableShop = _config.CreateEntry(nameof(IsEnableShop), true);
            _isEnableAdditionalAuction = _config.CreateEntry(nameof(IsEnableAdditionalAuction), false);
            _isEnableShopCarParts = _config.CreateEntry(nameof(IsEnableShopCarParts), false);

            _cfgSectionCars = MelonPreferences.CreateCategory(ConfigCarSection);
            _cfgSectionCars.SetFilePath("Mods/RealShop.cfg");

            _removeCarsOlderThanYear = _cfgSectionCars.CreateEntry(nameof(RemoveCarsOlderThanYear), RemoveCarsOlderYear);

            _priceParts1950_1959 = _cfgSectionCars.CreateEntry(nameof(PriceParts1950_1959), ValuePriceParts1950_1959);
            _priceParts1960_1969 = _cfgSectionCars.CreateEntry(nameof(PriceParts1960_1969), ValuePriceParts1960_1969);
            _priceParts1970_1979 = _cfgSectionCars.CreateEntry(nameof(PriceParts1970_1979), ValuePriceParts1970_1979);
            _priceParts1980_1990 = _cfgSectionCars.CreateEntry(nameof(PriceParts1980_1990), ValuePriceParts1980_1990);
            _priceParts1991_2000 = _cfgSectionCars.CreateEntry(nameof(PriceParts1991_2000), ValuePriceParts1991_2000);
            _priceParts2001_2005 = _cfgSectionCars.CreateEntry(nameof(PriceParts2001_2005), ValuePriceParts2001_2005);
            _pricePartsGT = _cfgSectionCars.CreateEntry(nameof(PricePartsSportGT), ValuePriceSportGT);
            _pricePartsSpecialRetro = _cfgSectionCars.CreateEntry(nameof(PricePartsSpecialRetro), ValuePriceSpecialRetro);
            _pricePartsOther = _cfgSectionCars.CreateEntry(nameof(PricePartsOther), ValuePriceOther);

            _priceRarityCommon = _cfgSectionCars.CreateEntry(nameof(PriceRarityCommon), ValuePriceRarityCommon);
            _priceRarityUncommon = _cfgSectionCars.CreateEntry(nameof(PriceRarityUncommon), ValuePriceRarityUncommon);
            _priceRarityRare = _cfgSectionCars.CreateEntry(nameof(PriceRarityRare), ValuePriceRarityRare);
            _priceRarityUnique = _cfgSectionCars.CreateEntry(nameof(PriceRarityUnique), ValuePriceRarityUnique);

            _enableSellCap = _cfgSectionCars.CreateEntry(nameof(EnableSellCap), ValueEnableSellCap);
            _sellRatio = _cfgSectionCars.CreateEntry(nameof(SellRatio), ValueSellRatio);

            _enableLaborCost = _cfgSectionCars.CreateEntry(nameof(EnableLaborCost), ValueEnableLaborCost);
            _laborCostBase = _cfgSectionCars.CreateEntry(nameof(LaborCostBase), ValueLaborCostBase);
            _enableSellConditionFactor = _cfgSectionCars.CreateEntry(nameof(EnableSellConditionFactor), ValueEnableSellConditionFactor);
            _sellConditionFloor = _cfgSectionCars.CreateEntry(nameof(SellConditionFloor), ValueSellConditionFloor);
            _sellConditionSpan = _cfgSectionCars.CreateEntry(nameof(SellConditionSpan), ValueSellConditionSpan);

            _enableLaborCurve = _cfgSectionCars.CreateEntry(nameof(EnableLaborCurve), ValueEnableLaborCurve);
            _laborCurveK = _cfgSectionCars.CreateEntry(nameof(LaborCurveK), ValueLaborCurveK);
            _laborCurveMaxCond = _cfgSectionCars.CreateEntry(nameof(LaborCurveMaxCond), ValueLaborCurveMaxCond);

            _enableCarSaleCondition = _cfgSectionCars.CreateEntry(nameof(EnableCarSaleCondition), ValueEnableCarSaleCondition);
            _carSaleConditionFloor = _cfgSectionCars.CreateEntry(nameof(CarSaleConditionFloor), ValueCarSaleConditionFloor);
            _carSaleConditionSpan = _cfgSectionCars.CreateEntry(nameof(CarSaleConditionSpan), ValueCarSaleConditionSpan);

            _enableRepairCostPreview = _cfgSectionCars.CreateEntry(nameof(EnableRepairCostPreview), ValueEnableRepairCostPreview);
            _enableProfitMarker = _cfgSectionCars.CreateEntry(nameof(EnableProfitMarker), ValueEnableProfitMarker);
            _enableAuctionRatingCap = _cfgSectionCars.CreateEntry(nameof(EnableAuctionRatingCap), ValueEnableAuctionRatingCap);
            _auctionMaxRating = _cfgSectionCars.CreateEntry(nameof(AuctionMaxRating), ValueAuctionMaxRating);
            _enableBuildFromScratchJobs = _cfgSectionCars.CreateEntry(nameof(EnableBuildFromScratchJobs), ValueEnableBuildFromScratchJobs);
            _buildJobMinLevel = _cfgSectionCars.CreateEntry(nameof(BuildJobMinLevel), ValueBuildJobMinLevel);
            _buildJobMaxLevel = _cfgSectionCars.CreateEntry(nameof(BuildJobMaxLevel), ValueBuildJobMaxLevel);
            _buildJobChance = _cfgSectionCars.CreateEntry(nameof(BuildJobChance), ValueBuildJobChance);
            _buildJobDeadlineDays = _cfgSectionCars.CreateEntry(nameof(BuildJobDeadlineDays), ValueBuildJobDeadlineDays);
            _buildJobRewardMultiplier = _cfgSectionCars.CreateEntry(nameof(BuildJobRewardMultiplier), ValueBuildJobRewardMultiplier);
            _buildJobRewardFloor = _cfgSectionCars.CreateEntry(nameof(BuildJobRewardFloor), ValueBuildJobRewardFloor);
            _buildJobRewardCap = _cfgSectionCars.CreateEntry(nameof(BuildJobRewardCap), ValueBuildJobRewardCap);
            _buildJobXpBonus = _cfgSectionCars.CreateEntry(nameof(BuildJobXpBonus), ValueBuildJobXpBonus);
            _buildJobBodyMinQuality = _cfgSectionCars.CreateEntry(nameof(BuildJobBodyMinQuality), ValueBuildJobBodyMinQuality);
            _buildJobBodyMaxQuality = _cfgSectionCars.CreateEntry(nameof(BuildJobBodyMaxQuality), ValueBuildJobBodyMaxQuality);
            _buildJobPartsMinQuality = _cfgSectionCars.CreateEntry(nameof(BuildJobPartsMinQuality), ValueBuildJobPartsMinQuality);
            _buildJobPartsMaxQuality = _cfgSectionCars.CreateEntry(nameof(BuildJobPartsMaxQuality), ValueBuildJobPartsMaxQuality);
            _buildJobInteriorMinQuality = _cfgSectionCars.CreateEntry(nameof(BuildJobInteriorMinQuality), ValueBuildJobInteriorMinQuality);
            _buildJobInteriorMaxQuality = _cfgSectionCars.CreateEntry(nameof(BuildJobInteriorMaxQuality), ValueBuildJobInteriorMaxQuality);
            _buildJobFrameMinQuality = _cfgSectionCars.CreateEntry(nameof(BuildJobFrameMinQuality), ValueBuildJobFrameMinQuality);
            _buildJobFrameMaxQuality = _cfgSectionCars.CreateEntry(nameof(BuildJobFrameMaxQuality), ValueBuildJobFrameMaxQuality);
            _buildJobUnlockAllOrderSlots = _cfgSectionCars.CreateEntry(nameof(BuildJobUnlockAllOrderSlots), ValueBuildJobUnlockAllOrderSlots);
            _buildJobTextLanguage = _cfgSectionCars.CreateEntry(nameof(BuildJobTextLanguage), ValueBuildJobTextLanguage);
            _buildJobRequireAllPartsMounted = _cfgSectionCars.CreateEntry(nameof(BuildJobRequireAllPartsMounted), ValueBuildJobRequireAllPartsMounted);
            _buildJobBlockEarlyFinish = _cfgSectionCars.CreateEntry(nameof(BuildJobBlockEarlyFinish), ValueBuildJobBlockEarlyFinish);
            _buildJobUseDedicatedSlot = _cfgSectionCars.CreateEntry(nameof(BuildJobUseDedicatedSlot), ValueBuildJobUseDedicatedSlot);
            _buildJobMinMountedParts = _cfgSectionCars.CreateEntry(nameof(BuildJobMinMountedParts), ValueBuildJobMinMountedParts);
            _buildJobLastIssuedId = _cfgSectionCars.CreateEntry(nameof(BuildJobLastIssuedId), ValueBuildJobLastIssuedId);
            _buildJobOfferSeconds = _cfgSectionCars.CreateEntry(nameof(BuildJobOfferSeconds), ValueBuildJobOfferSeconds);
            _buildJobAdvancePercent = _cfgSectionCars.CreateEntry(nameof(BuildJobAdvancePercent), ValueBuildJobAdvancePercent);
            _buildJobAdvanceJobId = _cfgSectionCars.CreateEntry(nameof(BuildJobAdvanceJobId), ValueBuildJobAdvanceJobId);
            _buildJobConditionJobId = _cfgSectionCars.CreateEntry(nameof(BuildJobConditionJobId), ValueBuildJobConditionJobId);
            _buildJobAdvanceAmount = _cfgSectionCars.CreateEntry(nameof(BuildJobAdvanceAmount), ValueBuildJobAdvanceAmount);
            _buildJobLabelAdvance = _cfgSectionCars.CreateEntry(nameof(BuildJobLabelAdvance), ValueBuildJobLabelAdvance);
            _buildJobLabelAdvanceSuffix = _cfgSectionCars.CreateEntry(nameof(BuildJobLabelAdvanceSuffix), ValueBuildJobLabelAdvanceSuffix);
            _buildJobAdvancePopup = _cfgSectionCars.CreateEntry(nameof(BuildJobAdvancePopup), ValueBuildJobAdvancePopup);
            _buildJobStripMode = _cfgSectionCars.CreateEntry(nameof(BuildJobStripMode), ValueBuildJobStripMode);
            _buildJobStripMethods = _cfgSectionCars.CreateEntry(nameof(BuildJobStripMethods), ValueBuildJobStripMethods);
            _buildJobStripPanelChance = _cfgSectionCars.CreateEntry(nameof(BuildJobStripPanelChance), ValueBuildJobStripPanelChance);
            _buildJobStripConditionPercent = _cfgSectionCars.CreateEntry(nameof(BuildJobStripConditionPercent), ValueBuildJobStripConditionPercent);
            _buildJobStripDustPercent = _cfgSectionCars.CreateEntry(nameof(BuildJobStripDustPercent), ValueBuildJobStripDustPercent);
            _buildJobConditionGuardSeconds = _cfgSectionCars.CreateEntry(nameof(BuildJobConditionGuardSeconds), ValueBuildJobConditionGuardSeconds);
            _buildJobConditionReapplyMax = _cfgSectionCars.CreateEntry(nameof(BuildJobConditionReapplyMax), ValueBuildJobConditionReapplyMax);
            _buildJobStripKeepFrame = _cfgSectionCars.CreateEntry(nameof(BuildJobStripKeepFrame), ValueBuildJobStripKeepFrame);
            _buildJobStripEngineObject = _cfgSectionCars.CreateEntry(nameof(BuildJobStripEngineObject), ValueBuildJobStripEngineObject);
            _buildJobConfigSchema = _cfgSectionCars.CreateEntry(nameof(BuildJobConfigSchema), 0);
            _buildJobStripWheels = _cfgSectionCars.CreateEntry(nameof(BuildJobStripWheels), ValueBuildJobStripWheels);
            _buildJobStripHideWheels = _cfgSectionCars.CreateEntry(nameof(BuildJobStripHideWheels), ValueBuildJobStripHideWheels);
            _buildJobStripSuspensionParts = _cfgSectionCars.CreateEntry(nameof(BuildJobStripSuspensionParts), ValueBuildJobStripSuspensionParts);
            _buildJobGuardKeepSeconds = _cfgSectionCars.CreateEntry(nameof(BuildJobGuardKeepSeconds), ValueBuildJobGuardKeepSeconds);
            _buildJobStripSuspensionNames = _cfgSectionCars.CreateEntry(nameof(BuildJobStripSuspensionNames), ValueBuildJobStripSuspensionNames);
            _buildJobStripSuspensionObjects = _cfgSectionCars.CreateEntry(nameof(BuildJobStripSuspensionObjects), ValueBuildJobStripSuspensionObjects);
            _buildJobKeepGuardOnCancel = _cfgSectionCars.CreateEntry(nameof(BuildJobKeepGuardOnCancel), ValueBuildJobKeepGuardOnCancel);
            _buildJobBlockCancel = _cfgSectionCars.CreateEntry(nameof(BuildJobBlockCancel), ValueBuildJobBlockCancel);
            _buildJobSuspensionToggleKey = _cfgSectionCars.CreateEntry(nameof(BuildJobSuspensionToggleKey), ValueBuildJobSuspensionToggleKey);
            _buildJobStripUnmountPrefixes = _cfgSectionCars.CreateEntry(nameof(BuildJobStripUnmountPrefixes), ValueBuildJobStripUnmountPrefixes);
            _buildJobEngineType = _cfgSectionCars.CreateEntry(nameof(BuildJobEngineType), ValueBuildJobEngineType);
            _buildJobEngineJobId = _cfgSectionCars.CreateEntry(nameof(BuildJobEngineJobId), ValueBuildJobEngineJobId);
            _buildJobPanelNamePrefixes = _cfgSectionCars.CreateEntry(nameof(BuildJobPanelNamePrefixes), ValueBuildJobPanelNamePrefixes);
            _buildJobDifficultyLabel = _cfgSectionCars.CreateEntry(nameof(BuildJobDifficultyLabel), ValueBuildJobDifficultyLabel);
            _buildJobLabelBody = _cfgSectionCars.CreateEntry(nameof(BuildJobLabelBody), ValueBuildJobLabelBody);
            _buildJobLabelParts = _cfgSectionCars.CreateEntry(nameof(BuildJobLabelParts), ValueBuildJobLabelParts);
            _buildJobLabelInterior = _cfgSectionCars.CreateEntry(nameof(BuildJobLabelInterior), ValueBuildJobLabelInterior);
            _buildJobLabelEngine = _cfgSectionCars.CreateEntry(nameof(BuildJobLabelEngine), ValueBuildJobLabelEngine);
            _buildJobLabelReward = _cfgSectionCars.CreateEntry(nameof(BuildJobLabelReward), ValueBuildJobLabelReward);
            _buildJobTakePopup = _cfgSectionCars.CreateEntry(nameof(BuildJobTakePopup), ValueBuildJobTakePopup);
            _buildJobNotReadyPrefix = _cfgSectionCars.CreateEntry(nameof(BuildJobNotReadyPrefix), ValueBuildJobNotReadyPrefix);

            _enableDonorCondition = _cfgSectionCars.CreateEntry(nameof(EnableDonorCondition), ValueEnableDonorCondition);
            _donorConditionScale = _cfgSectionCars.CreateEntry(nameof(DonorConditionScale), ValueDonorConditionScale);            _enableMaxQuality = _cfgSectionCars.CreateEntry(nameof(EnableMaxQuality), true);
            _maxQualityCommonMin = _cfgSectionCars.CreateEntry(nameof(MaxQualityCommonMin), 30);
            _maxQualityCommonMax = _cfgSectionCars.CreateEntry(nameof(MaxQualityCommonMax), 50);
            _maxQualityRareMin = _cfgSectionCars.CreateEntry(nameof(MaxQualityRareMin), 50);
            _maxQualityRareMax = _cfgSectionCars.CreateEntry(nameof(MaxQualityRareMax), 70);
            _maxQualityUniqueMin = _cfgSectionCars.CreateEntry(nameof(MaxQualityUniqueMin), 70);
            _maxQualityUniqueMax = _cfgSectionCars.CreateEntry(nameof(MaxQualityUniqueMax), 100);
            _maxQualityBodyMin = _cfgSectionCars.CreateEntry(nameof(MaxQualityBodyMin), 50);
            _maxQualityBodyMax = _cfgSectionCars.CreateEntry(nameof(MaxQualityBodyMax), 100);
            _maxQualityRevealAfterRepair = _cfgSectionCars.CreateEntry(nameof(MaxQualityRevealAfterRepair), true);
            _maxQualityCommonWeight = _cfgSectionCars.CreateEntry(nameof(MaxQualityCommonWeight), 65);
            _maxQualityRareWeight = _cfgSectionCars.CreateEntry(nameof(MaxQualityRareWeight), 25);
            _maxQualityUniqueWeight = _cfgSectionCars.CreateEntry(nameof(MaxQualityUniqueWeight), 10);

            _whiteCarParts = _cfgSectionCars.CreateEntry(nameof(WhiteCarParts), ListWhiteCarParts);
            _isEnableBrandFilter = _cfgSectionCars.CreateEntry(nameof(IsEnableBrandFilter), true);
            _retroParts = _cfgSectionCars.CreateEntry(nameof(ListRetroParts), RetroParts);

            _cfgSectionRepair = MelonPreferences.CreateCategory(ConfigRepairSection);
            _cfgSectionRepair.SetFilePath("Mods/RealShop.cfg");
            _carPartsCantBeRepair = _cfgSectionRepair.CreateEntry(nameof(ListCarPartsCantBeRepair), ListCarPartsCantBeRepair);
            _carNames = _cfgSectionRepair.CreateEntry(nameof(ListCarNames), CarNames);

            _cfgSectionParts = MelonPreferences.CreateCategory(ConfigCategory);
            _cfgSectionParts.SetFilePath("Mods/RealShop.cfg");
            _retroPartYears = _cfgSectionParts.CreateEntry(nameof(ListRetroPartYears), RetroPartYearsDefault());

            _sportGT = _cfgSectionParts.CreateEntry(nameof(ListSportGT), SportGT);
            _specialRetro = _cfgSectionParts.CreateEntry(nameof(ListSpecialRetro), SpecialRetro);
        }

        public void Reload()
        {
            _config.LoadFromFile();
        }
    }
}
