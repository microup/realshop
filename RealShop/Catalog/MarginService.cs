using CMS.Containers;
using CMS.Helpers;
using System.Collections.Generic;
using UnityEngine;

namespace RealShop
{
    internal static class MarginService
    {
        internal const float EstimatedCycleDelta = 0.15f;

        internal static bool TryCarGain(string id, float cond, float cap, out float gain, out string model)
        {
            gain = 0f;
            model = null;

            Config config = RealParts.ActiveConfig;
            if (config == null || !config.EnableProfitMarker || string.IsNullOrEmpty(id))
            {
                return false;
            }

            float delta = cap - cond;
            if (delta <= 0f)
            {
                return false;
            }

            if (delta > EstimatedCycleDelta)
            {
                delta = EstimatedCycleDelta;
            }

            List<string> models;
            if (!GarageNeedsRecon.TryGetNeedModels(id, out models))
            {
                return false;
            }

            float best = 0f;
            foreach (var m in models)
            {
                int basePrice;
                if (!CarPriceCache.TryGet(m, out basePrice) && !GarageNeedsRecon.TryGetModelBuyPrice(m, out basePrice))
                {
                    continue;
                }

                int n = GarageNeedsRecon.PartCountFor(m);
                float g = basePrice * config.CarSaleConditionSpan * delta / (n > 0 ? n : 1);
                if (g > best)
                {
                    best = g;
                    model = m;
                }
            }

            if (model == null)
            {
                return false;
            }

            gain = best;
            return true;
        }

        internal static float ResaleGain(string id, float delta)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null || string.IsNullOrEmpty(id))
            {
                return 0f;
            }

            PartProperty property = Singleton<GameInventory>.Instance.GetItemProperty(id);
            if (property == null || property.Price <= 0)
            {
                return 0f;
            }

            return property.Price * config.SellRatio * config.SellConditionSpan * delta;
        }

        internal static bool IsCycleProfitable(Item item, float totalCost, float delta, out string model, out float gain)
        {
            model = null;
            gain = 0f;

            if (item == null)
            {
                return false;
            }

            float cond = item.GetConditionToShow();
            if (cond < 0f)
            {
                cond = 0f;
            }

            if (cond > 1f)
            {
                cond = 1f;
            }

            float cap = MaxQualityService.MaxFraction(item);
            float carGain;
            string carModel;
            bool hasCar = TryCarGain(item.ID, cond, cap, out carGain, out carModel);
            float resale = ResaleGain(item.ID, delta);

            gain = resale;
            if (hasCar && carGain > gain)
            {
                gain = carGain;
                model = carModel;
            }

            return gain > totalCost;
        }

        internal static bool HasCarChannel(Item item)
        {
            if (item == null)
            {
                return false;
            }

            float cond = item.GetConditionToShow();
            float cap = MaxQualityService.MaxFraction(item);
            string model;
            float gain;
            return TryCarGain(item.ID, cond, cap, out gain, out model);
        }
    }
}
