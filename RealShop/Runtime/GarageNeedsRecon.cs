using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace RealShop
{
    internal static partial class GarageNeedsRecon
    {
        private const int MaxPerList = 80;

        internal static object CachedProfileManager;

        private static void AwakePostfix(object __instance)
        {
            CachedProfileManager = __instance;
            MelonLogger.Msg("[Recon] cached ProfileManager instance (Awake)");
        }

        private static readonly HashSet<string> NeededComponents = new HashSet<string>();
        private static readonly Dictionary<string, HashSet<string>> NeededBodyByModel = new Dictionary<string, HashSet<string>>();
        private static readonly Dictionary<string, List<string>> NeedModelsByComponent = new Dictionary<string, List<string>>();
        private static readonly Dictionary<string, int> PartCountByModel = new Dictionary<string, int>();
        private static readonly Dictionary<string, int> ModelBuyPrice = new Dictionary<string, int>();
        private static readonly HashSet<string> OwnedModels = new HashSet<string>();
        private static readonly Dictionary<string, float> OwnedBestCond = new Dictionary<string, float>();
        private static readonly Dictionary<string, Dictionary<string, float>> OwnedBodyBestCond = new Dictionary<string, Dictionary<string, float>>();
        private static readonly Dictionary<Type, Dictionary<string, MemberInfo>> MemberCache = new Dictionary<Type, Dictionary<string, MemberInfo>>();
        private static long lastRefreshTicks = 0;

        internal static void Dump()
        {
            MelonLogger.Msg("[Recon] === garage needs dump ===");
            DumpSavedCars();
            DumpLiveCars();
            object pdp = GetProfileData(false);
            if (pdp != null)
            {
                int pkCount = 0;
                foreach (var c in Enumerate(Get(pdp.GetType(), pdp, "carsOnParking")))
                {
                    if (c != null && !string.IsNullOrEmpty(AsStr(Get(c.GetType(), c, "carToLoad"))))
                    {
                        pkCount++;
                    }
                }

                MelonLogger.Msg("[Recon] carsOnParking(non-empty)=" + pkCount);
            }
            RefreshNeeded();
            MelonLogger.Msg("[Recon] needed components=" + NeededComponents.Count + " bodyModels=" + NeededBodyByModel.Count);
            int shown = 0;
            foreach (var id in NeededComponents)
            {
                if (shown++ >= 20)
                {
                    break;
                }
                MelonLogger.Msg("[Recon]   NEED-COMP " + id);
            }
            foreach (var kv in NeededBodyByModel)
            {
                MelonLogger.Msg("[Recon]   NEED-BODY " + kv.Key + " -> " + string.Join(",", kv.Value));
            }
            MelonLogger.Msg("[Recon] === end ===");
        }

        internal static bool IsNeeded(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }
            MaybeRefresh();
            if (NeededComponents.Contains(id))
            {
                return true;
            }
            if (id.StartsWith("car_"))
            {
                foreach (var m in OwnedModels)
                {
                    if (id.StartsWith(m + "-"))
                    {
                        string rest = id.Substring(m.Length + 1);
                        HashSet<string> slots;
                        if (NeededBodyByModel.TryGetValue(m, out slots) && slots.Contains(BaseSlot(rest)))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        internal static bool IsBetter(string id, float cond)
        {
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }
            MaybeRefresh();
            float best;
            if (OwnedBestCond.TryGetValue(id, out best))
            {
                return cond > best + 0.0001f;
            }
            if (id.StartsWith("car_"))
            {
                foreach (var m in OwnedModels)
                {
                    if (id.StartsWith(m + "-"))
                    {
                        string rest = id.Substring(m.Length + 1);
                        Dictionary<string, float> bestSlots;
                        if (OwnedBodyBestCond.TryGetValue(m, out bestSlots))
                        {
                            float b;
                            if (bestSlots.TryGetValue(BaseSlot(rest), out b))
                            {
                                return cond > b + 0.0001f;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private static void MaybeRefresh()
        {
            long now = DateTime.Now.Ticks;
            if (lastRefreshTicks == 0 || (now - lastRefreshTicks) > 3000L * TimeSpan.TicksPerMillisecond)
            {
                RefreshNeeded();
            }
        }

        private static void RefreshNeeded()
        {
            NeededComponents.Clear();
            NeededBodyByModel.Clear();
            NeedModelsByComponent.Clear();
            PartCountByModel.Clear();
            ModelBuyPrice.Clear();
            OwnedModels.Clear();
            OwnedBestCond.Clear();
            OwnedBodyBestCond.Clear();
            try
            {
                object pd = GetProfileData(false);
                if (pd != null)
                {
                    AddNeedsFrom(pd, "carsInGarage");
                    AddNeedsFrom(pd, "carsOnParking");
                }
            }
            catch (Exception e)
            {
                MelonLogger.Warning("[Needs] refresh err: " + e.Message);
            }
            lastRefreshTicks = DateTime.Now.Ticks;
        }

        private static void AddNeedsFrom(object pd, string field)
        {
            var arr = Get(pd.GetType(), pd, field) as IEnumerable;
            if (arr == null)
            {
                return;
            }

            foreach (var car in arr)
            {
                AddCarNeeds(car);
            }
        }

        private static void AddCarNeeds(object car)
        {
            if (car == null)
            {
                return;
            }

            Type ct = car.GetType();
            string model = AsStr(Get(ct, car, "carToLoad"));
            if (string.IsNullOrEmpty(model))
            {
                return;
            }

            OwnedModels.Add(model);
            object info = Get(ct, car, "CarInfoData");
            if (info != null)
            {
                int buyPrice = (int)ToFloat(Get(info.GetType(), info, "BuyPrice"));
                if (buyPrice > 0)
                {
                    ModelBuyPrice[model] = buyPrice;
                }
            }

            CollectComponents(Get(ct, car, "PartData"), model);
            CollectBody(Get(ct, car, "BodyPartsData"), model);
        }

    }
}
