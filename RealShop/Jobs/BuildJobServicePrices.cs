using MelonLoader;
using System;
using System.Text;
using UnhollowerBaseLib;
using UnityEngine;


namespace RealShop
{
    internal static partial class BuildJobService
    {
        internal static bool TrySalePrice(CarLoader loader, out int price, out string source)
        {
            return TryPriceAt(loader, LiveGlobalCondition(loader), out price, out source);
        }

        internal static bool TryFinishedPrice(CarLoader loader, out int price, out string source)
        {
            return TryPriceAt(loader, 1f, out price, out source);
        }

        private static float LiveGlobalCondition(CarLoader loader)
        {
            try
            {
                return SafeCondition(loader.CalcGlobalCondition());
            }
            catch
            {
                return 1f;
            }
        }

        private static bool TryPriceAt(CarLoader loader, float cond, out int price, out string source)
        {
            price = 0;
            source = null;
            Config config = RealParts.ActiveConfig;
            if (config == null || loader == null)
            {
                return false;
            }

            string model = null;
            try
            {
                model = loader.carToLoad;
            }
            catch
            {
            }

            int basePrice = 0;
            string from = "CarInfoData";
            try
            {
                basePrice = loader.CarInfoData.BuyPrice;
            }
            catch
            {
            }

            if (basePrice <= 0 && !string.IsNullOrEmpty(model) && CarPriceCache.TryGet(model, out basePrice))
            {
                from = "CarPriceCache";
            }

            if (basePrice <= 0 && !string.IsNullOrEmpty(model) && GarageNeedsRecon.TryGetModelBuyPrice(model, out basePrice))
            {
                from = "ProfileBuyPrice";
            }

            if (basePrice <= 0)
            {
                return false;
            }

            float factor = config.EnableCarSaleCondition
                ? config.CarSaleConditionFloor + config.CarSaleConditionSpan * cond
                : 1f;

            price = Mathf.RoundToInt(basePrice * factor);
            source = from + " base=" + basePrice + " cond=" + Percent(cond) + "% factor=" + factor.ToString("0.###");
            return true;
        }

        internal static int ClampReward(int reward)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null)
            {
                return reward;
            }

            int value = reward;
            if (config.BuildJobRewardFloor > 0 && value < config.BuildJobRewardFloor)
            {
                value = config.BuildJobRewardFloor;
            }

            if (config.BuildJobRewardCap > 0 && value > config.BuildJobRewardCap)
            {
                value = config.BuildJobRewardCap;
            }

            return value;
        }

        internal static CarLoader FindJobCarLoader(Job job)
        {
            return job == null ? null : FindCarLoaderByOrder(job.id);
        }

        private static bool IdExists(Il2CppSystem.Collections.Generic.List<Job> jobs, int jobId)
        {
            try
            {
                for (int i = 0; i < jobs.Count; i++)
                {
                    Job job = jobs[i];
                    if (job != null && job.id == jobId)
                    {
                        return true;
                    }
                }
            }
            catch
            {
            }

            return false;
        }

        private static Il2CppReferenceArray<JobTask> BuildTasks()
        {
            System.Collections.Generic.List<JobTask> tasks = new System.Collections.Generic.List<JobTask>();
            for (int i = 0; i < TaskSpecs.Length; i++)
            {
                string[] spec = TaskSpecs[i].Split('|');

                JobTask task = new JobTask();
                task.type = spec[0];
                task.subtype = spec.Length > 1 ? spec[1] : "General";
                task.desc = string.Empty;
                task.partsCount = 0;
                task.Parts = new Il2CppSystem.Collections.Generic.List<JobPart>();
                task.easyMode = false;
                task.moneySpent = 0;
                task.IncreaseTuneValue = 0;
                task.Done = false;
                tasks.Add(task);
            }

            MelonLogger.Msg("[BuildJob] tasks built: " + tasks.Count + " of " + TaskSpecs.Length
                + " bodyTasks=included (job tasks are never omitted)");
            return new Il2CppReferenceArray<JobTask>(tasks.ToArray());
        }

        private static int FreePlaceIndex()
        {
            try
            {
                CarLoaderPlaces places = CarLoaderPlaces.Get();
                if (places == null)
                {
                    return -1;
                }

                CarLoader free = places.GetPlaceForLoadCar();
                if (free == null)
                {
                    return -1;
                }

                int index = places.GetCarLoaderIndex(free);
                return index >= 0 ? index : -1;
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] free place lookup failed: " + e.Message);
                return -1;
            }
        }

        private static Il2CppStructArray<bool> CopyJobTypeFlags(Job template)
        {
            try
            {
                Il2CppStructArray<bool> source = template.jobType;
                if (source == null || source.Length <= 0)
                {
                    return source;
                }

                Il2CppStructArray<bool> flags = new Il2CppStructArray<bool>(source.Length);
                for (int i = 0; i < flags.Length; i++)
                {
                    flags[i] = true;
                }

                return flags;
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] jobType copy failed, reusing template shape: " + e.Message);
                return template.jobType;
            }
        }

        private static int Roll(int min, int max, int jobId, int stream)
        {
            if (max < min)
            {
                int swap = min;
                min = max;
                max = swap;
            }

            int span = max - min + 1;
            uint hash = Hash(jobId * 7919 + stream * 104729, Salt);
            return min + (int)(hash % (uint)span);
        }

        private static uint Hash(int value, uint salt)
        {
            unchecked
            {
                uint h = (uint)value ^ salt;
                h *= 2654435761u;
                h ^= h >> 16;
                h *= 2246822519u;
                h ^= h >> 13;
                return h;
            }
        }
    }
}
