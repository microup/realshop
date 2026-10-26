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

        public void RememberBuildJobId(int id)
        {
            _buildJobLastIssuedId.Value = id;
            MelonPreferences.Save();
        }
        public string BuildJobStripMode => _buildJobStripMode.Value;
        public string BuildJobStripMethods => _buildJobStripMethods.Value;
        public int BuildJobStripPanelChance => _buildJobStripPanelChance.Value;
        public string BuildJobPanelNamePrefixes => _buildJobPanelNamePrefixes.Value;
        public string BuildJobDifficultyLabel => _buildJobDifficultyLabel.Value;
        public string BuildJobLabelBody => _buildJobLabelBody.Value;
        public string BuildJobLabelParts => _buildJobLabelParts.Value;
        public string BuildJobLabelInterior => _buildJobLabelInterior.Value;
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
            _buildJobAdvanceAmount = _cfgSectionCars.CreateEntry(nameof(BuildJobAdvanceAmount), ValueBuildJobAdvanceAmount);
            _buildJobLabelAdvance = _cfgSectionCars.CreateEntry(nameof(BuildJobLabelAdvance), ValueBuildJobLabelAdvance);
            _buildJobLabelAdvanceSuffix = _cfgSectionCars.CreateEntry(nameof(BuildJobLabelAdvanceSuffix), ValueBuildJobLabelAdvanceSuffix);
            _buildJobAdvancePopup = _cfgSectionCars.CreateEntry(nameof(BuildJobAdvancePopup), ValueBuildJobAdvancePopup);
            _buildJobStripMode = _cfgSectionCars.CreateEntry(nameof(BuildJobStripMode), ValueBuildJobStripMode);
            _buildJobStripMethods = _cfgSectionCars.CreateEntry(nameof(BuildJobStripMethods), ValueBuildJobStripMethods);
            _buildJobStripPanelChance = _cfgSectionCars.CreateEntry(nameof(BuildJobStripPanelChance), ValueBuildJobStripPanelChance);
            _buildJobPanelNamePrefixes = _cfgSectionCars.CreateEntry(nameof(BuildJobPanelNamePrefixes), ValueBuildJobPanelNamePrefixes);
            _buildJobDifficultyLabel = _cfgSectionCars.CreateEntry(nameof(BuildJobDifficultyLabel), ValueBuildJobDifficultyLabel);
            _buildJobLabelBody = _cfgSectionCars.CreateEntry(nameof(BuildJobLabelBody), ValueBuildJobLabelBody);
            _buildJobLabelParts = _cfgSectionCars.CreateEntry(nameof(BuildJobLabelParts), ValueBuildJobLabelParts);
            _buildJobLabelInterior = _cfgSectionCars.CreateEntry(nameof(BuildJobLabelInterior), ValueBuildJobLabelInterior);
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
