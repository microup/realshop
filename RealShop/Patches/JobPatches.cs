using CMS.UI.Logic;
using CMS.UI.Logic.Orders;
using CMS.UI.Windows;
using Harmony;
using MelonLoader;
using System;
using UnhollowerBaseLib;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable CS0618

namespace RealShop
{
    [HarmonyPatch(typeof(OrdersWindow), "UpdateJobs", new[] { typeof(Il2CppSystem.Collections.Generic.List<Job>) })]
    internal static class BuildJobSlotPatch
    {
        private static bool _activeLogged;

        private static void Postfix(OrdersWindow __instance, Il2CppSystem.Collections.Generic.List<Job> jobs)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null || !config.EnableBuildFromScratchJobs)
            {
                return;
            }

            if (!_activeLogged)
            {
                _activeLogged = true;
                MelonLogger.Msg("[BuildJob] hook active: UpdateJobs");
            }

            BuildJobAcceptPatch.TryCommit();

            try
            {
                Il2CppReferenceArray<OrderItem> items = __instance.orderItems;
                if (items == null || items.Length == 0)
                {
                    return;
                }

                int ourIndex = -1;
                for (int i = 0; i < items.Length; i++)
                {
                    OrderItem item = items[i];
                    if (item == null)
                    {
                        continue;
                    }

                    if (BuildJobService.IsBuildJob(item.job))
                    {
                        ourIndex = i;
                    }
                    else
                    {
                        HideBadge(item);
                    }

                    if (config.BuildJobUnlockAllOrderSlots && item.isLocked)
                    {
                        item.SetLocked(false);
                    }
                }

                if (ourIndex < 0)
                {
                    return;
                }

                int target = DedicatedSlotIndex(items, config);
                OrderItem ours = items[ourIndex];
                if (ours != null && ours.transform.GetSiblingIndex() != target)
                {
                    ours.transform.SetSiblingIndex(target);
                }

                OrderItem buildItem = items[ourIndex];
                if (buildItem == null)
                {
                    return;
                }

                ShowBadge(buildItem, config);
                ApplyLevelLock(buildItem, config);
                MelonLogger.Msg("[BuildJob] slot: index=" + ourIndex + " movedTo=" + target + " of " + items.Length
                    + " level=" + GlobalData.RealPlayerLevel);
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] UpdateJobs hook failed: " + e.Message);
            }
        }

        private static int LastUnlockedIndex(Il2CppReferenceArray<OrderItem> items)
        {
            for (int i = items.Length - 1; i >= 0; i--)
            {
                OrderItem item = items[i];
                if (item == null)
                {
                    continue;
                }

                if (!item.isLocked)
                {
                    return i;
                }
            }

            return items.Length - 1;
        }

        private static int DedicatedSlotIndex(Il2CppReferenceArray<OrderItem> items, Config config)
        {
            int last = LastUnlockedIndex(items);
            if (!config.BuildJobUseDedicatedSlot)
            {
                return last;
            }

            int realJobs = 0;
            for (int i = 0; i < items.Length; i++)
            {
                OrderItem item = items[i];
                if (item != null && item.job != null && !BuildJobService.IsBuildJob(item.job))
                {
                    realJobs++;
                }
            }

            if (realJobs > last)
            {
                realJobs = last;
            }

            return realJobs < 0 ? 0 : realJobs;
        }

        private static void ApplyLevelLock(OrderItem item, Config config)
        {
            if (GlobalData.RealPlayerLevel >= config.BuildJobMinLevel)
            {
                return;
            }

            item.SetLocked(true);
            item.SetLockDetails(OrderSlotLockReason.Level, config.BuildJobMinLevel);
        }

        private static void ShowBadge(OrderItem item, Config config)
        {
            Text label = BadgeText(item);
            if (label == null)
            {
                return;
            }

            label.text = BuildJobText.Pick("difficulty", config.BuildJobDifficultyLabel);
            label.gameObject.SetActive(true);
        }

        private static void HideBadge(OrderItem item)
        {
            Transform found = item.transform.Find(BadgeName);
            if (found != null)
            {
                found.gameObject.SetActive(false);
            }
        }

        private const string BadgeName = "RealShopBuildBadge";

        private static Text BadgeText(OrderItem item)
        {
            if (item == null || item.carName == null)
            {
                return null;
            }

            Transform existing = item.transform.Find(BadgeName);
            if (existing != null)
            {
                return existing.GetComponent<Text>();
            }

            GameObject root = new GameObject(BadgeName);
            root.transform.SetParent(item.transform, false);
            Text text = root.AddComponent<Text>();
            text.font = item.carName.font;
            text.fontSize = 17;
            text.fontStyle = FontStyle.Bold;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = new Color(1f, 0.34f, 0.22f, 1f);
            text.raycastTarget = false;
            text.supportRichText = true;
            text.text = string.Empty;

            RectTransform rect = text.rectTransform;
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2(0f, -2f);
            rect.sizeDelta = new Vector2(0f, 20f);

            root.transform.SetAsLastSibling();
            return text;
        }
    }

    [HarmonyPatch(typeof(JobHelper), "SetupIssues")]
    internal static class BuildJobIssuesPatch
    {
        private static bool _activeLogged;

        private static void Postfix(Job job, Il2CppReferenceArray<IIssueObject> issueObjects)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null || !config.EnableBuildFromScratchJobs)
            {
                return;
            }

            if (!_activeLogged)
            {
                _activeLogged = true;
                MelonLogger.Msg("[BuildJob] hook active: SetupIssues");
            }

            if (!BuildJobService.IsBuildJob(job) || issueObjects == null || issueObjects.Length == 0)
            {
                return;
            }

            try
            {
                CarLoader loader = BuildJobService.FindJobCarLoader(job);
                int advance = BuildJobService.AdvanceFor(job.id);
                string[] lines = BuildJobService.RequirementLines(job.id, loader, advance);
                int count = Math.Min(lines.Length, issueObjects.Length);
                int shown = 0;
                for (int i = 0; i < count; i++)
                {
                    string line = lines[i];
                    if (string.IsNullOrEmpty(line))
                    {
                        IIssueObject empty = issueObjects[i];
                        if (empty != null)
                        {
                            empty.SetActive(false);
                        }

                        continue;
                    }

                    IIssueObject issue = issueObjects[i];
                    if (issue == null)
                    {
                        continue;
                    }

                    issue.Setup(line);
                    issue.SetActive(true);
                    issue.SetDone(i < 4 ? BuildJobService.IsRequirementMet(job.id, i, loader) : advance > 0);
                    shown++;
                }

                for (int i = count; i < issueObjects.Length; i++)
                {
                    IIssueObject extra = issueObjects[i];
                    if (extra != null)
                    {
                        extra.SetActive(false);
                    }
                }

                MelonLogger.Msg("[BuildJob] issues: id=" + job.id + " lines=" + shown + " car=" + (loader != null)
                    + " advance=" + advance);
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] SetupIssues hook failed: " + e.Message);
            }
        }
    }

    [HarmonyPatch(typeof(GameScript), "EndJob")]
    internal static class BuildJobEndPatch
    {
        private static bool _activeLogged;

        private static bool Prefix(GameScript __instance, Job job, CarLoader carLoader)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null || !config.EnableBuildFromScratchJobs || !BuildJobService.IsBuildJob(job))
            {
                return true;
            }

            if (!_activeLogged)
            {
                _activeLogged = true;
                MelonLogger.Msg("[BuildJob] hook active: EndJob");
            }

            try
            {
                string reason;
                if (BuildJobService.Validate(job, carLoader, out reason))
                {
                    return true;
                }

                MelonLogger.Msg("[BuildJob] finish blocked: id=" + job.id + " " + reason);
                if (config.BuildJobBlockEarlyFinish)
                {
                    BuildJobPopup.Show(BuildJobText.Pick("difficulty", config.BuildJobDifficultyLabel) + ": "
                        + BuildJobText.Pick("notReadyPrefix", config.BuildJobNotReadyPrefix) + " - " + reason);
                    return false;
                }
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] EndJob gate failed: " + e.Message);
                if (config.BuildJobBlockEarlyFinish)
                {
                    return false;
                }
            }

            return true;
        }

        private static void Postfix(GameScript __instance, Job job, CarLoader carLoader, ref bool __runOriginal)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null || !config.EnableBuildFromScratchJobs || !BuildJobService.IsBuildJob(job))
            {
                return;
            }

            if (!__runOriginal)
            {
                return;
            }

            try
            {
                if (BuildJobState.IsPaid(job.id))
                {
                    return;
                }

                int price;
                string source;
                if (!BuildJobService.TrySalePrice(carLoader, out price, out source))
                {
                    MelonLogger.Warning("[BuildJob] reward skipped: car price unknown id=" + job.id);
                    return;
                }

                int reward = BuildJobService.ClampReward(price * config.BuildJobRewardMultiplier);
                int advance = BuildJobService.AdvanceFor(job.id);
                int payout = reward - advance;
                if (payout < 0)
                {
                    payout = 0;
                }

                GlobalData.AddPlayerMoney(payout);
                config.ClearAdvance();
                BuildJobState.MarkPaid(job.id);
                BuildJobState.ClearActiveJob(job.id);
                BuildJobState.DisarmStrip("EndJob");

                MelonLogger.Msg("[BuildJob] reward: id=" + job.id + " price=" + price
                    + " x" + config.BuildJobRewardMultiplier + " = " + reward + " (" + source + ")"
                    + " advance=" + advance + " payout=" + payout);
                BuildJobPopup.Show(BuildJobText.Pick("difficulty", config.BuildJobDifficultyLabel) + ": +" + payout + " CR ("
                    + BuildJobText.Pick("reward", config.BuildJobLabelReward)
                    + " " + price + " x" + config.BuildJobRewardMultiplier
                    + (advance > 0 ? " - " + BuildJobText.Pick("advance", config.BuildJobLabelAdvance) + " " + advance : "") + ")");
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] EndJob reward failed: " + e.Message);
            }
        }
    }

    internal static class BuildJobPopup
    {
        internal static void Show(string text)
        {
            try
            {
                UIManager.Get().ShowPopup("[RealShop]", text, PopupType.Normal);
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] popup failed: " + e.Message);
            }
        }
    }
}

#pragma warning restore CS0618
