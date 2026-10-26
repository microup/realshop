using MelonLoader;
using System;
using System.Text;
using UnhollowerBaseLib;
using UnityEngine;

namespace RealShop
{
    internal static partial class BuildJobService
    {
        internal const int Magic = 20261025;
        private const uint Salt = 0x51B710B5;

        private static readonly string[] TaskSpecs =
        {
            "Engine|General",
            "Engine|Oil",
            "Engine|Timing",
            "Gearbox|General",
            "Gearbox|Clutch",
            "Susp|General",
            "Brakes|General",
            "Exhaust|General",
            "Body|General",
            "Body|PaintOriginal",
            "Additionals|WheelsAlignment",
        };

        internal static void OnOrderAccepted(Job job, string tag)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null || job == null || !IsBuildJob(job))
            {
                return;
            }

            if (BuildJobState.ActiveJobId == job.id)
            {
                return;
            }

            BuildJobState.SetActiveJob(job.id, job.carFile, job.carLoaderID);
            if (config.BuildJobAdvancePercent > 0)
            {
                BuildJobState.SetAdvancePending(job.id);
            }

            if (!string.Equals(config.BuildJobStripMode, "off", StringComparison.OrdinalIgnoreCase))
            {
                MelonLogger.Msg("[BuildJob] strip " + (BuildJobState.IsStripped(job.id) ? "applied" : "not applied before commit")
                    + ": id=" + job.id + " - the active job id covers the rest of the load");
                BuildJobState.DisarmStrip("accepted");
            }

            MelonLogger.Msg("[BuildJob] accepted: id=" + job.id + " car=" + job.carFile
                + " place=" + job.carLoaderID + " via=" + tag
                + " deadline=" + config.BuildJobDeadlineDays + "d (" + (config.BuildJobDeadlineDays * 24) + "h)"
                + " advance=" + config.BuildJobAdvancePercent + "%");
            BuildJobPopup.Show(BuildJobText.Pick("difficulty", config.BuildJobDifficultyLabel) + ": "
                + BuildJobText.Pick("takePopup", config.BuildJobTakePopup)
                + (config.BuildJobAdvancePercent > 0
                    ? " | " + BuildJobText.Pick("advance", config.BuildJobLabelAdvance) + " "
                        + config.BuildJobAdvancePercent + "% - "
                        + BuildJobText.Pick("advanceSuffix", config.BuildJobLabelAdvanceSuffix)
                    : string.Empty));
        }

        internal static bool IsBuildJob(Job job)
        {
            return job != null && !job.IsMission && job.MissionID == Magic;
        }

        internal static int BodyRequirement(int jobId)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null)
            {
                return 0;
            }

            return Roll(config.BuildJobBodyMinQuality, config.BuildJobBodyMaxQuality, jobId, 1);
        }

        internal static int PartsRequirement(int jobId)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null)
            {
                return 0;
            }

            return Roll(config.BuildJobPartsMinQuality, config.BuildJobPartsMaxQuality, jobId, 2);
        }

        internal static int InteriorRequirement(int jobId)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null)
            {
                return 0;
            }

            return Roll(config.BuildJobInteriorMinQuality, config.BuildJobInteriorMaxQuality, jobId, 3);
        }

        internal static int FrameRequirement(int jobId)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null)
            {
                return 0;
            }

            return Roll(config.BuildJobFrameMinQuality, config.BuildJobFrameMaxQuality, jobId, 4);
        }

        internal static bool TryPickRetroCar(out string carFile)
        {
            carFile = null;
            Config config = RealParts.ActiveConfig;
            if (config == null)
            {
                return false;
            }

            int cutoff = (int)config.RemoveCarsOlderThanYear;
            if (cutoff <= 0)
            {
                MelonLogger.Warning("[BuildJob] build job not created: RemoveCarsOlderThanYear=" + cutoff + " does not define a retro cutoff");
                return false;
            }

            string[] pool = CarData.GetOrderCars(cutoff);
            if (pool == null || pool.Length == 0)
            {
                MelonLogger.Warning("[BuildJob] retro order pool is empty");
                return false;
            }

            int pick = UnityEngine.Random.Range(0, pool.Length);
            if (pool.Length > 1 && BuildJobState.IsLastCarFile(pool[pick]))
            {
                pick = (pick + 1 + UnityEngine.Random.Range(0, pool.Length - 1)) % pool.Length;
            }

            carFile = pool[pick];
            return true;
        }

        internal static bool TryCreateJob(OrderGenerator generator, out Job job)
        {
            job = null;
            Config config = RealParts.ActiveConfig;
            if (config == null || !config.EnableBuildFromScratchJobs)
            {
                return false;
            }

            Il2CppSystem.Collections.Generic.List<Job> jobs;
            try
            {
                jobs = generator.Jobs;
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] Jobs getter failed: " + e.Message);
                return false;
            }

            if (jobs == null || jobs.Count == 0)
            {
                MelonLogger.Warning("[BuildJob] no vanilla job to copy the job shape from; skipped");
                return false;
            }

            Job template = null;
            int maxId = 0;
            for (int i = 0; i < jobs.Count; i++)
            {
                Job existing = jobs[i];
                if (existing == null)
                {
                    continue;
                }

                if (existing.id > maxId)
                {
                    maxId = existing.id;
                }

                if (template == null && !IsBuildJob(existing))
                {
                    template = existing;
                }
            }

            if (template == null)
            {
                MelonLogger.Warning("[BuildJob] no vanilla template job; skipped");
                return false;
            }

            string carFile;
            if (!TryPickRetroCar(out carFile))
            {
                return false;
            }

            int place = FreePlaceIndex();
            if (place < 0)
            {
                MelonLogger.Warning("[BuildJob] job skipped: no free garage place");
                return false;
            }

            int jobId = maxId + 1;
            if (config.BuildJobLastIssuedId >= jobId)
            {
                jobId = config.BuildJobLastIssuedId + 1;
            }

            while (IdExists(jobs, jobId))
            {
                jobId++;
            }

            int body = BodyRequirement(jobId);
            int parts = PartsRequirement(jobId);
            int frame = FrameRequirement(jobId);
            int interior = InteriorRequirement(jobId);

            Job created = new Job();
            created.id = jobId;
            created.MissionID = Magic;
            created.IsMission = false;
            created.carFile = carFile;
            created.carLoaderID = place;
            created.configVersion = template.configVersion;
            created.LocalizationID = template.LocalizationID;
            created.carColor = template.carColor;
            created.carFactoryColor = template.carFactoryColor;
            created.PaintType = template.PaintType;
            created.carFactoryPaintType = template.carFactoryPaintType;
            created.oilLevel = template.oilLevel;
            created.forXP = template.forXP;
            created.jobType = CopyJobTypeFlags(template);
            created.Mileage = 0;
            created.jobPartsCount = 0;
            created.globalCondition = Mathf.Min(frame, Mathf.Min(body, Mathf.Min(parts, interior))) / 100f;
            created.otherPartsCondition = parts / 100f;
            created.timeToEnd = config.BuildJobDeadlineDays * 24;
            created.TotalPayout = 0;
            created.JobBonus = 0;
            created.TaskBonus = 0;
            created.MoneySpent = 0;
            created.MoneySpentWithDifficultyMod = 0;
            created.XP = config.BuildJobXpBonus;
            created.BonusToExp = true;
            created.BonusToMoney = true;
            created.CanDelete = true;
            created.IsCompleted = false;
            created.IconTypeEngine = true;
            created.IconTypeTiming = true;
            created.IconTypeSuspension = true;
            created.IconTypeBrakes = true;
            created.IconTypeExhaust = true;
            created.IconTypeGearbox = true;
            created.IconTypeOil = true;
            created.IconTypeBody = true;
            created.IconTypeTuning = true;
            created.jobTasks = BuildTasks();

            job = created;
            config.RememberBuildJobId(jobId);
            MelonLogger.Msg("[BuildJob] built job id=" + jobId + " car=" + carFile + " place=" + created.carLoaderID
                + " body>=" + body + "% parts>=" + parts + "% frame>=" + frame + "% interior>=" + interior + "%"
                + " deadline=" + config.BuildJobDeadlineDays + "d/" + (config.BuildJobDeadlineDays * 24) + "h"
                + " multiplier=x" + config.BuildJobRewardMultiplier);
            return true;
        }

        internal static bool TryOfferJob(Config config, bool force)
        {
            OrderGenerator generator = BuildJobState.Generator;
            if (config == null || !config.EnableBuildFromScratchJobs || generator == null)
            {
                return false;
            }

            if (BuildJobState.HasActiveJob)
            {
                return false;
            }

            int level;
            try
            {
                level = GlobalData.RealPlayerLevel;
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] player level unavailable: " + e.Message);
                return false;
            }

            if (level < config.BuildJobMinLevel || level > config.BuildJobMaxLevel)
            {
                MelonLogger.Msg("[BuildJob] offer skipped: level=" + level + " outside "
                    + config.BuildJobMinLevel + ".." + config.BuildJobMaxLevel);
                return false;
            }

            if (CountBuildJobs(generator) > 0)
            {
                return false;
            }

            int roll = (int)(Ticket() % 100u);
            if (!force && roll >= config.BuildJobChance)
            {
                MelonLogger.Msg("[BuildJob] offer skipped: chance roll=" + roll + "% >= " + config.BuildJobChance + "%");
                return false;
            }

            if (!TryCreateJob(generator, out Job job))
            {
                return false;
            }

            int listed = 0;
            try
            {
                Il2CppSystem.Collections.Generic.List<Job> jobs = generator.Jobs;
                jobs.Insert(0, job);
                listed = jobs.Count;
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] add to job list failed: " + e.Message);
                return false;
            }

            float expires = Time.realtimeSinceStartup + (config.BuildJobOfferSeconds > 0 ? config.BuildJobOfferSeconds : 1);
            BuildJobState.SetOfferedJob(job.id, expires);
            BuildJobState.SetLastCarFile(job.carFile);
            MelonLogger.Msg("[BuildJob] job offered: id=" + job.id + " level=" + level + " list=" + listed
                + " offerSeconds=" + config.BuildJobOfferSeconds);
            return true;
        }

    }
}
