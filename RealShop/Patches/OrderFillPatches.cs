using Harmony;
using MelonLoader;
using CMS.UI.Logic;

#pragma warning disable CS0618

namespace RealShop
{
    [HarmonyPatch(typeof(GlobalData), "GetMaxOrdersAmount")]
    internal static class OrderFillAllSlotsPatch
    {
        internal const int MaxOrderSlots = 8;

        private static bool _activeLogged;

        private static void Postfix(ref int __result)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null)
            {
                return;
            }

            if (!_activeLogged)
            {
                _activeLogged = true;
                MelonLogger.Msg("[Orders] hook active: GetMaxOrdersAmount fillAll=" + config.OrderFillAllSlots);
            }

            if (!config.OrderFillAllSlots)
            {
                return;
            }

            try
            {
                if (__result > 0 && __result < MaxOrderSlots)
                {
                    __result = MaxOrderSlots;
                }
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[Orders] GetMaxOrdersAmount hook failed: " + e.Message);
            }
        }
    }

    [HarmonyPatch(typeof(GlobalData), "IsOrderSlotUnlocked")]
    internal static class OrderUnlockAllSlotsPatch
    {
        private static bool _activeLogged;

        private static void Postfix(ref bool __result, ref OrderSlotLockReason lockReason, ref int levelCap)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null)
            {
                return;
            }

            if (!_activeLogged)
            {
                _activeLogged = true;
                MelonLogger.Msg("[Orders] hook active: IsOrderSlotUnlocked fillAll=" + config.OrderFillAllSlots);
            }

            if (!config.OrderFillAllSlots)
            {
                return;
            }

            try
            {
                if (!__result && lockReason == OrderSlotLockReason.Level && levelCap > 0)
                {
                    __result = true;
                }
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[Orders] IsOrderSlotUnlocked hook failed: " + e.Message);
            }
        }
    }

    [HarmonyPatch(typeof(OrderGenerator), "GenerateNewJob")]
    internal static class OrderRandomCarsMarkPatch
    {
        private static bool _activeLogged;

        private static void Prefix()
        {
            Config config = RealParts.ActiveConfig;
            if (config == null)
            {
                return;
            }

            if (!_activeLogged)
            {
                _activeLogged = true;
                MelonLogger.Msg("[Orders] hook active: GenerateNewJob randomCars=" + config.OrderRandomCars);
            }

            if (!config.OrderRandomCars)
            {
                return;
            }

            try
            {
                OrderRandomCarsPatch.GenerationFrame = UnityEngine.Time.frameCount;
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[Orders] GenerateNewJob mark hook failed: " + e.Message);
            }
        }
    }

    [HarmonyPatch(typeof(CarBundleLoader), "CarIsAvailableOnLevel")]
    internal static class OrderRandomCarsPatch
    {
        internal static int GenerationFrame = -1;

        private static bool _activeLogged;

        private static bool Prefix(ref bool __result)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null)
            {
                return true;
            }

            if (!_activeLogged)
            {
                _activeLogged = true;
                MelonLogger.Msg("[Orders] hook active: CarIsAvailableOnLevel randomCars=" + config.OrderRandomCars);
            }

            if (!config.OrderRandomCars)
            {
                return true;
            }

            try
            {
                if (GenerationFrame == UnityEngine.Time.frameCount)
                {
                    __result = true;
                    return false;
                }
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[Orders] CarIsAvailableOnLevel hook failed: " + e.Message);
            }

            return true;
        }
    }

    internal static class OrderFillService
    {
        private const int TickCooldownFrames = 300;

        private static bool _running;

        private static int _cooldownFrames;

        private static bool _generateResolved;

        private static System.Reflection.MethodInfo _generateNewJob;

        internal static void Tick()
        {
            Config config = RealParts.ActiveConfig;
            if (config == null || !config.OrderFillAllSlots)
            {
                return;
            }

            if (_cooldownFrames > 0)
            {
                _cooldownFrames--;
                return;
            }

            _cooldownFrames = TickCooldownFrames;
            OrderGenerator generator = BuildJobState.Generator;
            if (generator == null)
            {
                generator = ResolveGenerator();
                if (generator == null)
                {
                    return;
                }
            }

            TopUp(generator);
        }

        private static OrderGenerator ResolveGenerator()
        {
            try
            {
                UnityEngine.Object found;
                found = UnityEngine.Object.FindObjectOfType(OrderGenerator.Il2CppType);
                return found as OrderGenerator;
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[Orders] generator resolve failed: " + e.Message);
                return null;
            }
        }

        internal static void OnJobGenerated(OrderGenerator generator)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null || !config.OrderFillAllSlots || generator == null)
            {
                return;
            }

            TopUp(generator);
        }

        private static void TopUp(OrderGenerator generator)
        {
            if (_running)
            {
                return;
            }

            _running = true;
            try
            {
                ResolveGenerateMethod();
                if (_generateNewJob == null)
                {
                    return;
                }

                Il2CppSystem.Collections.Generic.List<Job> jobs = generator.Jobs;
                if (jobs == null)
                {
                    return;
                }

                int target = GlobalData.GetMaxOrdersAmount();
                if (target <= 0 || jobs.Count >= target)
                {
                    return;
                }

                int before = jobs.Count;
                int guard = 0;
                while (jobs.Count < target && guard < OrderFillAllSlotsPatch.MaxOrderSlots)
                {
                    guard++;
                    _generateNewJob.Invoke(generator, null);
                }

                if (jobs.Count > before)
                {
                    MelonLogger.Msg("[Orders] slots filled: " + before + " -> " + jobs.Count + " (target=" + target + ")");
                }
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[Orders] slot fill failed: " + e.Message);
            }
            finally
            {
                _running = false;
            }
        }

        private static void ResolveGenerateMethod()
        {
            if (_generateResolved)
            {
                return;
            }

            _generateResolved = true;
            try
            {
                _generateNewJob = typeof(OrderGenerator).GetMethod("GenerateNewJob",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic
                    | System.Reflection.BindingFlags.Instance);
            }
            catch
            {
            }

            if (_generateNewJob == null)
            {
                MelonLogger.Warning("[Orders] OrderGenerator.GenerateNewJob not found; slot fill disabled");
            }
        }
    }
}

#pragma warning restore CS0618
