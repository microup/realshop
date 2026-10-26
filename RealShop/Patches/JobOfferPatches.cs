using CMS.UI.Logic;
using CMS.UI.Logic.Orders;
using CMS.UI.Windows;
using Harmony;
using MelonLoader;
using System;
using UnhollowerBaseLib;

#pragma warning disable CS0618

namespace RealShop
{
    [HarmonyPatch(typeof(OrderGenerator), "GenerateNewJob")]
    internal static class BuildJobGeneratePatch
    {
        private static bool _firstSeenLogged;

        private static void Postfix(OrderGenerator __instance)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null || !config.EnableBuildFromScratchJobs)
            {
                return;
            }

            if (!_firstSeenLogged)
            {
                _firstSeenLogged = true;
                MelonLogger.Msg("[BuildJob] hook active: GenerateNewJob level=" + config.BuildJobMinLevel
                    + ".." + config.BuildJobMaxLevel + " chance=" + config.BuildJobChance
                    + "% offerSeconds=" + config.BuildJobOfferSeconds);
            }

            try
            {
                if (BuildJobState.Generator == null)
                {
                    BuildJobState.SetGenerator(__instance);
                    BuildJobService.ResetOfferSchedule();
                }

                ReconcileActiveJob(__instance);
                BuildJobService.TryOfferJob(config, false);
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] GenerateNewJob hook failed: " + e.Message);
            }
        }

        private static void ReconcileActiveJob(OrderGenerator generator)
        {
            if (!BuildJobState.HasActiveJob)
            {
                return;
            }

            try
            {
                Il2CppSystem.Collections.Generic.List<Job> offered = generator.Jobs;
                Il2CppSystem.Collections.Generic.List<Job> selected = generator.selectedJobs;
                if (offered == null || selected == null)
                {
                    return;
                }

                if (IsListed(offered, BuildJobState.ActiveJobId))
                {
                    return;
                }

                if (IsListed(selected, BuildJobState.ActiveJobId))
                {
                    if (!_selectedListLogged)
                    {
                        _selectedListLogged = true;
                        MelonLogger.Msg("[BuildJob] active job " + BuildJobState.ActiveJobId
                            + " is in selectedJobs (accepted orders)");
                    }

                    return;
                }

                MelonLogger.Msg("[BuildJob] active job " + BuildJobState.ActiveJobId + " is neither offered nor selected; state cleared");
                BuildJobService.ForfeitAdvance(BuildJobState.ActiveJobId, "job gone");
                BuildJobState.ClearActiveJob(BuildJobState.ActiveJobId);
                BuildJobState.DisarmStrip("active job gone");
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] active job reconcile failed: " + e.Message);
            }
        }

        private static bool _selectedListLogged;

        private static bool IsListed(Il2CppSystem.Collections.Generic.List<Job> jobs, int jobId)
        {
            if (jobs == null)
            {
                return false;
            }

            for (int i = 0; i < jobs.Count; i++)
            {
                Job job = jobs[i];
                if (job != null && job.id == jobId)
                {
                    return true;
                }
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(OrderGenerator), "TakeJob")]
    internal static class BuildJobTakePatch
    {
        private static bool _activeLogged;

        private static void Postfix(OrderGenerator __instance, [HarmonyArgument(0)] int jobId)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null || !config.EnableBuildFromScratchJobs)
            {
                return;
            }

            if (!_activeLogged)
            {
                _activeLogged = true;
                MelonLogger.Msg("[BuildJob] hook active: TakeJob");
            }

            try
            {
                BuildJobService.OnOrderAccepted(FindJob(__instance, jobId), "TakeJob");
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] TakeJob hook failed: " + e.Message);
            }
        }

        internal static Job FindJob(OrderGenerator generator, int jobId)
        {
            try
            {
                Il2CppSystem.Collections.Generic.List<Job> jobs = generator.Jobs;
                if (jobs == null)
                {
                    return null;
                }

                for (int i = 0; i < jobs.Count; i++)
                {
                    Job job = jobs[i];
                    if (job != null && job.id == jobId)
                    {
                        return job;
                    }
                }
            }
            catch
            {
            }

            return null;
        }
    }

    [HarmonyPatch(typeof(OrdersWindow), "OnOrderItemSelect", new[] { typeof(OrderItem) })]
    internal static class BuildJobSelectPatch
    {
        internal static int SelectedJobId = -1;

        private static void Postfix(OrderItem item)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null || !config.EnableBuildFromScratchJobs)
            {
                return;
            }

            try
            {
                Job job = item != null ? item.job : null;
                SelectedJobId = job != null ? job.id : -1;
            }
            catch
            {
                SelectedJobId = -1;
            }
        }
    }

    [HarmonyPatch(typeof(OrdersWindow), "OnItemSelect", new[] { typeof(int) })]
    internal static class BuildJobIndexSelectPatch
    {
        private static bool _activeLogged;

        private static void Postfix(OrdersWindow __instance, [HarmonyArgument(0)] int index)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null || !config.EnableBuildFromScratchJobs)
            {
                return;
            }

            if (!_activeLogged)
            {
                _activeLogged = true;
                MelonLogger.Msg("[BuildJob] hook active: OnItemSelect");
            }

            try
            {
                Il2CppReferenceArray<OrderItem> items = __instance != null ? __instance.orderItems : null;
                if (items == null || index < 0 || index >= items.Length)
                {
                    BuildJobSelectPatch.SelectedJobId = -1;
                    return;
                }

                OrderItem item = items[index];
                Job job = item != null ? item.job : null;
                BuildJobSelectPatch.SelectedJobId = job != null ? job.id : -1;
            }
            catch
            {
                BuildJobSelectPatch.SelectedJobId = -1;
            }
        }
    }

}

#pragma warning restore CS0618
