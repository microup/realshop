// Добавить список н ремонтируемых запчастей


using MelonLoader;
using UnityEngine;

namespace RealShop
{
    public partial class Config
    {
        public const string ModName = "RealShop";

        private const uint RemoveCarsOlderYear = 2005;

        public const int ValuePriceParts1950_1959 = 8;
        public const int ValuePriceParts1960_1969 = 7;
        public const int ValuePriceParts1970_1979 = 6;
        public const int ValuePriceParts1980_1990 = 4;
        public const int ValuePriceParts1991_2000 = 3;
        public const int ValuePriceParts2001_2005 = 2;
        public const int ValuePriceSportGT = 6;
        public const int ValuePriceSpecialRetro = 10;
        public const int ValuePriceOther = 2;

        public const float ValuePriceRarityCommon = 1.0f;
        public const float ValuePriceRarityUncommon = 1.5f;
        public const float ValuePriceRarityRare = 2.5f;
        public const float ValuePriceRarityUnique = 4.0f;

        private const bool ValueEnableSellCap = true;
        public const float ValueSellRatio = 0.35f;

        private const bool ValueEnableLaborCost = true;
        public const int ValueLaborCostBase = 100;

        private const bool ValueEnableDonorCondition = true;
        public const float ValueDonorConditionScale = 0.6f;
        private const bool ValueEnableSellConditionFactor = true;
        public const float ValueSellConditionFloor = 0.4f;
        public const float ValueSellConditionSpan = 0.6f;

        private const bool ValueEnableLaborCurve = true;
        public const float ValueLaborCurveK = 0.25f;
        public const float ValueLaborCurveMaxCond = 0.98f;

        private const bool ValueEnableCarSaleCondition = true;
        public const float ValueCarSaleConditionFloor = 0.5f;
        public const float ValueCarSaleConditionSpan = 0.5f;

        private const bool ValueEnableRepairCostPreview = true;

        private const bool ValueEnableProfitMarker = true;
        private const bool ValueEnableAuctionRatingCap = true;
        public const int ValueAuctionMaxRating = 1;
        private const bool ValueEnableBuildFromScratchJobs = false;
        public const int ValueBuildJobMinLevel = 1;
        public const int ValueBuildJobMaxLevel = 80;
        public const int ValueBuildJobChance = 35;
        public const int ValueBuildJobDeadlineDays = 7;
        public const int ValueBuildJobRewardMultiplier = 5;
        public const int ValueBuildJobRewardFloor = 20000;
        public const int ValueBuildJobRewardCap = 500000;
        public const int ValueBuildJobXpBonus = 4000;
        public const int ValueBuildJobBodyMinQuality = 60;
        public const int ValueBuildJobBodyMaxQuality = 85;
        public const int ValueBuildJobPartsMinQuality = 50;
        public const int ValueBuildJobPartsMaxQuality = 75;
        public const int ValueBuildJobInteriorMinQuality = 50;
        public const int ValueBuildJobInteriorMaxQuality = 75;
        public const int ValueBuildJobFrameMinQuality = 50;
        public const int ValueBuildJobFrameMaxQuality = 75;
        public const bool ValueBuildJobUnlockAllOrderSlots = true;
        private const string ValueBuildJobTextLanguage = "auto";
        public const bool ValueBuildJobRequireAllPartsMounted = true;
        public const bool ValueBuildJobBlockEarlyFinish = true;
        public const bool ValueBuildJobUseDedicatedSlot = true;
        public const int ValueBuildJobMinMountedParts = 25;
        public const int ValueBuildJobLastIssuedId = 0;
        public const int ValueBuildJobOfferSeconds = 120;
        public const int ValueBuildJobAdvancePercent = 30;
        public const int ValueBuildJobAdvanceJobId = -1;
        public const int ValueBuildJobAdvanceAmount = 0;
        private const string ValueBuildJobLabelAdvance = "ПРЕДОПЛАТА";
        private const string ValueBuildJobLabelAdvanceSuffix = "при принятии; удерживается из итоговой выплаты";
        private const string ValueBuildJobAdvancePopup = "предоплата по заданию";
        private const string ValueBuildJobStripMode = "suppress";
        public const string ValueBuildJobStripMethods = "CreateEngine,CreateEngineAsync,CreateInterior,CreateParts,CreateDriveshaft,CreateExterior,CreateCarLightParts,CreateBonusParts";
        private const int ValueBuildJobStripPanelChance = 100;
        public const string ValueBuildJobPanelNamePrefixes = "door,hood,trunk,fender,bumper,clamshell,window,mirror,headlight,taillight,tailight,license_plate,parkingdoor,front_end,roof,quarter";
        private const string ValueBuildJobDifficultyLabel = "КРАЙНЕ СЛОЖНОЕ";
        private const string ValueBuildJobLabelBody = "КУЗОВ";
        private const string ValueBuildJobLabelParts = "ДЕТАЛИ";
        private const string ValueBuildJobLabelInterior = "ИНТЕРЬЕР";
        private const string ValueBuildJobLabelReward = "цена готовой машины";
        private const string ValueBuildJobTakePopup = "принято: собери автомобиль с нуля. Кузов, детали и интерьер имеют минимальное качество.";
        private const string ValueBuildJobNotReadyPrefix = "автомобиль не соответствует требованиям задания";

        private const string ConfigCategoryName = "MainSection";
        private const string ConfigCarSection = "CarSection";
        private const string ConfigRepairSection = "RepairSection";
        private const string ConfigCategory = "Category";

        private readonly MelonPreferences_Category _config;
        private readonly MelonPreferences_Category _cfgSectionRepair;
        private readonly MelonPreferences_Category _cfgSectionCars;
        private readonly MelonPreferences_Category _cfgSectionParts;
        private readonly MelonPreferences_Entry<KeyCode> _isKeyBindEnableSwitchMode;
        private readonly MelonPreferences_Entry<KeyCode> _addMoneyKey;
        private readonly MelonPreferences_Entry<int> _addMoneyAmount;
        private readonly MelonPreferences_Entry<int> _startingMoney;
        private readonly MelonPreferences_Entry<bool> _enableMoneyHotkey;

        private readonly MelonPreferences_Entry<uint> _removeCarsOlderThanYear;

        public readonly MelonPreferences_Entry<int> _priceParts1950_1959;
        public readonly MelonPreferences_Entry<int> _priceParts1960_1969;
        public readonly MelonPreferences_Entry<int> _priceParts1970_1979;
        public readonly MelonPreferences_Entry<int> _priceParts1980_1990;
        public readonly MelonPreferences_Entry<int> _priceParts1991_2000;
        public readonly MelonPreferences_Entry<int> _priceParts2001_2005;
        public readonly MelonPreferences_Entry<int> _pricePartsGT;
        public readonly MelonPreferences_Entry<int> _pricePartsSpecialRetro;
        public readonly MelonPreferences_Entry<int> _pricePartsOther;

        public readonly MelonPreferences_Entry<float> _priceRarityCommon;
        public readonly MelonPreferences_Entry<float> _priceRarityUncommon;
        public readonly MelonPreferences_Entry<float> _priceRarityRare;
        public readonly MelonPreferences_Entry<float> _priceRarityUnique;

        private readonly MelonPreferences_Entry<bool> _enableSellCap;
        private readonly MelonPreferences_Entry<float> _sellRatio;

        private readonly MelonPreferences_Entry<bool> _enableLaborCost;
        private readonly MelonPreferences_Entry<int> _laborCostBase;

        private readonly MelonPreferences_Entry<bool> _enableDonorCondition;
        private readonly MelonPreferences_Entry<float> _donorConditionScale;
        private readonly MelonPreferences_Entry<bool> _enableSellConditionFactor;
        private readonly MelonPreferences_Entry<float> _sellConditionFloor;
        private readonly MelonPreferences_Entry<float> _sellConditionSpan;

        private readonly MelonPreferences_Entry<bool> _enableLaborCurve;
        private readonly MelonPreferences_Entry<float> _laborCurveK;
        private readonly MelonPreferences_Entry<float> _laborCurveMaxCond;

        private readonly MelonPreferences_Entry<bool> _enableCarSaleCondition;
        private readonly MelonPreferences_Entry<float> _carSaleConditionFloor;
        private readonly MelonPreferences_Entry<float> _carSaleConditionSpan;

        private readonly MelonPreferences_Entry<bool> _enableRepairCostPreview;
        private readonly MelonPreferences_Entry<bool> _enableProfitMarker;        private readonly MelonPreferences_Entry<bool> _enableMaxQuality;
        private readonly MelonPreferences_Entry<int> _maxQualityCommonMin;
        private readonly MelonPreferences_Entry<int> _maxQualityCommonMax;
        private readonly MelonPreferences_Entry<int> _maxQualityRareMin;
        private readonly MelonPreferences_Entry<int> _maxQualityRareMax;
        private readonly MelonPreferences_Entry<int> _maxQualityUniqueMin;
        private readonly MelonPreferences_Entry<int> _maxQualityUniqueMax;
        private readonly MelonPreferences_Entry<int> _maxQualityBodyMin;
        private readonly MelonPreferences_Entry<int> _maxQualityBodyMax;
        private readonly MelonPreferences_Entry<bool> _maxQualityRevealAfterRepair;
        private readonly MelonPreferences_Entry<int> _maxQualityCommonWeight;
        private readonly MelonPreferences_Entry<int> _maxQualityRareWeight;
        private readonly MelonPreferences_Entry<int> _maxQualityUniqueWeight;
        private readonly MelonPreferences_Entry<bool> _enableAuctionRatingCap;
        private readonly MelonPreferences_Entry<int> _auctionMaxRating;
        private readonly MelonPreferences_Entry<bool> _enableBuildFromScratchJobs;
        private readonly MelonPreferences_Entry<int> _buildJobMinLevel;
        private readonly MelonPreferences_Entry<int> _buildJobMaxLevel;
        private readonly MelonPreferences_Entry<int> _buildJobChance;
        private readonly MelonPreferences_Entry<int> _buildJobDeadlineDays;
        private readonly MelonPreferences_Entry<int> _buildJobRewardMultiplier;
        private readonly MelonPreferences_Entry<int> _buildJobRewardFloor;
        private readonly MelonPreferences_Entry<int> _buildJobRewardCap;
        private readonly MelonPreferences_Entry<int> _buildJobXpBonus;
        private readonly MelonPreferences_Entry<int> _buildJobBodyMinQuality;
        private readonly MelonPreferences_Entry<int> _buildJobBodyMaxQuality;
        private readonly MelonPreferences_Entry<int> _buildJobPartsMinQuality;
        private readonly MelonPreferences_Entry<int> _buildJobPartsMaxQuality;
        private readonly MelonPreferences_Entry<int> _buildJobInteriorMinQuality;
        private readonly MelonPreferences_Entry<int> _buildJobInteriorMaxQuality;
        private readonly MelonPreferences_Entry<int> _buildJobFrameMinQuality;
        private readonly MelonPreferences_Entry<int> _buildJobFrameMaxQuality;
        private readonly MelonPreferences_Entry<bool> _buildJobUnlockAllOrderSlots;
        private readonly MelonPreferences_Entry<string> _buildJobTextLanguage;
        private readonly MelonPreferences_Entry<bool> _buildJobRequireAllPartsMounted;
        private readonly MelonPreferences_Entry<bool> _buildJobBlockEarlyFinish;
        private readonly MelonPreferences_Entry<bool> _buildJobUseDedicatedSlot;
        private readonly MelonPreferences_Entry<int> _buildJobMinMountedParts;
        private readonly MelonPreferences_Entry<int> _buildJobLastIssuedId;
        private readonly MelonPreferences_Entry<int> _buildJobOfferSeconds;
        private readonly MelonPreferences_Entry<int> _buildJobAdvancePercent;
        private readonly MelonPreferences_Entry<int> _buildJobAdvanceJobId;
        private readonly MelonPreferences_Entry<int> _buildJobAdvanceAmount;
        private readonly MelonPreferences_Entry<string> _buildJobLabelAdvance;
        private readonly MelonPreferences_Entry<string> _buildJobLabelAdvanceSuffix;
        private readonly MelonPreferences_Entry<string> _buildJobAdvancePopup;
        private readonly MelonPreferences_Entry<string> _buildJobStripMode;
        private readonly MelonPreferences_Entry<string> _buildJobStripMethods;
        private readonly MelonPreferences_Entry<int> _buildJobStripPanelChance;
        private readonly MelonPreferences_Entry<string> _buildJobPanelNamePrefixes;
        private readonly MelonPreferences_Entry<string> _buildJobDifficultyLabel;
        private readonly MelonPreferences_Entry<string> _buildJobLabelBody;
        private readonly MelonPreferences_Entry<string> _buildJobLabelParts;
        private readonly MelonPreferences_Entry<string> _buildJobLabelInterior;
        private readonly MelonPreferences_Entry<string> _buildJobLabelReward;
        private readonly MelonPreferences_Entry<string> _buildJobTakePopup;
        private readonly MelonPreferences_Entry<string> _buildJobNotReadyPrefix;

        private readonly MelonPreferences_Entry<bool> _isEnableBrandFilter;
        private readonly MelonPreferences_Entry<bool> _isEnableShop;
        private readonly MelonPreferences_Entry<bool> _isEnableAdditionalAuction;
        private readonly MelonPreferences_Entry<bool> _isEnableShopCarParts;

        private readonly MelonPreferences_Entry<string[]> _carNames;
        private readonly MelonPreferences_Entry<string[]> _whiteCarParts;

        private readonly MelonPreferences_Entry<string[]> _carPartsCantBeRepair;

        private readonly MelonPreferences_Entry<string[]> _retroParts;

        private readonly MelonPreferences_Entry<string[]> _retroPartYears;
        private readonly MelonPreferences_Entry<string[]> _sportGT;
        private readonly MelonPreferences_Entry<string[]> _specialRetro;

        public KeyCode IsKeyBindEnableSwitchMode => _isKeyBindEnableSwitchMode.Value;
        public KeyCode AddMoneyKey => _addMoneyKey.Value;
        public int AddMoneyAmount => _addMoneyAmount.Value;
        public int StartingMoney => _startingMoney.Value;
        public bool EnableMoneyHotkey => _enableMoneyHotkey.Value;
        public uint RemoveCarsOlderThanYear => _removeCarsOlderThanYear.Value;


        public int PriceParts1950_1959 => _priceParts1950_1959.Value;
        public int PriceParts1960_1969 => _priceParts1960_1969.Value;
        public int PriceParts1970_1979 => _priceParts1970_1979.Value;
        public int PriceParts1980_1990 => _priceParts1980_1990.Value;
        public int PriceParts1991_2000 => _priceParts1991_2000.Value;
        public int PriceParts2001_2005 => _priceParts2001_2005.Value;
        public int PricePartsSportGT => _pricePartsGT.Value;
        public int PricePartsSpecialRetro => _pricePartsSpecialRetro.Value;
        public int PricePartsOther => _pricePartsOther.Value;

        public float PriceRarityCommon => _priceRarityCommon.Value;
        public float PriceRarityUncommon => _priceRarityUncommon.Value;
        public float PriceRarityRare => _priceRarityRare.Value;
        public float PriceRarityUnique => _priceRarityUnique.Value;

        public bool EnableSellCap => _enableSellCap.Value;
        public float SellRatio => _sellRatio.Value;

        public bool EnableLaborCost => _enableLaborCost.Value;
        public int LaborCostBase => _laborCostBase.Value;
        public bool EnableSellConditionFactor => _enableSellConditionFactor.Value;
        public float SellConditionFloor => _sellConditionFloor.Value;
        public float SellConditionSpan => _sellConditionSpan.Value;

        public bool EnableLaborCurve => _enableLaborCurve.Value;
        public float LaborCurveK => _laborCurveK.Value;
        public float LaborCurveMaxCond => _laborCurveMaxCond.Value;

        public bool EnableCarSaleCondition => _enableCarSaleCondition.Value;
        public float CarSaleConditionFloor => _carSaleConditionFloor.Value;
        public float CarSaleConditionSpan => _carSaleConditionSpan.Value;

        public bool EnableRepairCostPreview => _enableRepairCostPreview.Value;
        public bool EnableProfitMarker => _enableProfitMarker.Value;
        public bool EnableAuctionRatingCap => _enableAuctionRatingCap.Value;
        public int AuctionMaxRating => _auctionMaxRating.Value;
        public bool EnableBuildFromScratchJobs => _enableBuildFromScratchJobs.Value;
        public int BuildJobMinLevel => _buildJobMinLevel.Value;
        public int BuildJobMaxLevel => _buildJobMaxLevel.Value;
        public int BuildJobChance => _buildJobChance.Value;
        public int BuildJobDeadlineDays => _buildJobDeadlineDays.Value;
        public int BuildJobRewardMultiplier => _buildJobRewardMultiplier.Value;
        public int BuildJobRewardFloor => _buildJobRewardFloor.Value;
        public int BuildJobRewardCap => _buildJobRewardCap.Value;
        public int BuildJobXpBonus => _buildJobXpBonus.Value;
        public int BuildJobBodyMinQuality => _buildJobBodyMinQuality.Value;
        public int BuildJobBodyMaxQuality => _buildJobBodyMaxQuality.Value;
        public int BuildJobPartsMinQuality => _buildJobPartsMinQuality.Value;
        public int BuildJobPartsMaxQuality => _buildJobPartsMaxQuality.Value;
        public int BuildJobInteriorMinQuality => _buildJobInteriorMinQuality.Value;
        public int BuildJobInteriorMaxQuality => _buildJobInteriorMaxQuality.Value;
        public int BuildJobFrameMinQuality => _buildJobFrameMinQuality.Value;
        public int BuildJobFrameMaxQuality => _buildJobFrameMaxQuality.Value;
        public bool BuildJobUnlockAllOrderSlots => _buildJobUnlockAllOrderSlots.Value;
        public string BuildJobTextLanguage => _buildJobTextLanguage.Value;
        public bool BuildJobRequireAllPartsMounted => _buildJobRequireAllPartsMounted.Value;
        public bool BuildJobBlockEarlyFinish => _buildJobBlockEarlyFinish.Value;
        public bool BuildJobUseDedicatedSlot => _buildJobUseDedicatedSlot.Value;
        public int BuildJobMinMountedParts => _buildJobMinMountedParts.Value;
        public int BuildJobLastIssuedId => _buildJobLastIssuedId.Value;
        public int BuildJobOfferSeconds => _buildJobOfferSeconds.Value;
        public int BuildJobAdvancePercent => _buildJobAdvancePercent.Value;
        public int BuildJobAdvanceJobId => _buildJobAdvanceJobId.Value;
        public int BuildJobAdvanceAmount => _buildJobAdvanceAmount.Value;
        public string BuildJobLabelAdvance => _buildJobLabelAdvance.Value;
        public string BuildJobLabelAdvanceSuffix => _buildJobLabelAdvanceSuffix.Value;
        public string BuildJobAdvancePopup => _buildJobAdvancePopup.Value;
    }
}
