using MelonLoader;
using System;
using System.Text;
using UnhollowerBaseLib;
using UnityEngine;

namespace RealShop
{
    internal static class BuildJobMountCensus
    {
        private const int MaxFailureLogs = 3;

        private static int _loggedJobId = -1;

        private static int _failureJobId = -1;

        private static int _failureCount;

        internal static void LogPreparedMountAvailability(CarLoader loader, Job job)
        {
            if (loader == null || job == null)
            {
                return;
            }

            int jobId;
            try
            {
                jobId = job.id;
            }
            catch (System.Exception)
            {
                return;
            }

            if (_loggedJobId == jobId)
            {
                return;
            }

            Config config = RealParts.ActiveConfig;
            if (config == null || !RealParts.ModIsEnabled || !config.EnableBuildFromScratchJobs
                || !BuildJobState.IsOursJobId(jobId))
            {
                return;
            }

            try
            {
                Transform root = loader.GetRootTransform();
                Il2CppReferenceArray<JobTask> tasks = job.jobTasks;
                if (root == null || tasks == null)
                {
                    Warn(jobId, " root=" + (root == null ? "null" : "ok")
                        + " tasks=" + (tasks == null ? "null" : "ok") + " (transient, will retry)");
                    return;
                }

                if (tasks.Length == 0)
                {
                    Warn(jobId, " tasks=0 (transient, will retry)");
                    return;
                }

                PartScript[] parts = root.GetComponentsInChildren<PartScript>(true);
                if (parts == null || parts.Length == 0)
                {
                    Warn(jobId, " parts=" + (parts == null ? "null" : "0") + " (transient, will retry)");
                    return;
                }

                JobPart sample = BuildJobMountIndex.FirstPart(tasks);
                if (sample == null)
                {
                    Warn(jobId, " no job parts to probe (transient, will retry)");
                    return;
                }

                BuildJobPartMemberProbe.Resolve(sample);
                if (BuildJobPartMemberProbe.IdentityUnresolved)
                {
                    Warn(jobId, " jobpart identity unresolved, census skipped (see jobpart members)");
                    return;
                }

                int keyFailures = 0;
                System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<int>> index =
                    BuildJobMountIndex.Build(parts, ref keyFailures);

                int[] kinds = new int[parts.Length];
                string[] names = new string[parts.Length];
                for (int i = 0; i < parts.Length; i++)
                {
                    kinds[i] = BuildJobMountIndex.Classify(parts[i]);
                    names[i] = BuildJobMountIndex.SafeName(parts, i);
                }

                bool[] referenced = new bool[parts.Length];
                int declared = 0;
                int listed = 0;
                int available = 0;
                int installed = 0;
                int hidden = 0;
                int absent = 0;
                int unknown = 0;
                int availableDropped = 0;
                int installedDropped = 0;
                int hiddenDropped = 0;
                int absentDropped = 0;
                StringBuilder availableNames = new StringBuilder();
                StringBuilder installedNames = new StringBuilder();
                StringBuilder hiddenNames = new StringBuilder();
                StringBuilder absentNames = new StringBuilder();
                StringBuilder detail = new StringBuilder();
                StringBuilder taskLabels = new StringBuilder();
                int labelsDropped = 0;

                for (int i = 0; i < tasks.Length; i++)
                {
                    JobTask task = tasks[i];
                    if (task == null)
                    {
                        continue;
                    }

                    declared += task.partsCount;
                    int taskListed = task.Parts != null ? task.Parts.Count : 0;
                    listed += taskListed;

                    int taskAvailable = 0;
                    int taskInstalled = 0;
                    int taskHidden = 0;
                    int taskAbsent = 0;

                    StringBuilder taskText = new StringBuilder();

                    if (task.Parts != null)
                    {
                        for (int k = 0; k < task.Parts.Count; k++)
                        {
                            JobPart entry = task.Parts[k];
                            string id = BuildJobPartMemberProbe.IdOf(entry);
                            string label = BuildJobPartMemberProbe.NameOf(entry);
                            int slot = BuildJobMountIndex.Resolve(index, referenced, id, label);

                            BuildJobMountIndex.Append(taskText, label + ":"
                                + (slot < 0 ? "-" : KindText(kinds[slot])), ref labelsDropped);

                            if (slot < 0)
                            {
                                taskAbsent++;
                                absent++;
                                BuildJobMountIndex.Append(absentNames, label, ref absentDropped);
                                continue;
                            }

                            referenced[slot] = true;

                            if (kinds[slot] == BuildJobMountIndex.KindMountable)
                            {
                                taskAvailable++;
                                available++;
                                BuildJobMountIndex.Append(availableNames, names[slot], ref availableDropped);
                            }
                            else if (kinds[slot] == BuildJobMountIndex.KindInstalled)
                            {
                                taskInstalled++;
                                installed++;
                                BuildJobMountIndex.Append(installedNames, names[slot], ref installedDropped);
                            }
                            else if (kinds[slot] == BuildJobMountIndex.KindHidden)
                            {
                                taskHidden++;
                                hidden++;
                                BuildJobMountIndex.Append(hiddenNames, names[slot], ref hiddenDropped);
                            }
                            else
                            {
                                unknown++;
                            }
                        }
                    }

                    if (detail.Length > 0)
                    {
                        detail.Append('|');
                    }

                    detail.Append(task.type).Append('/').Append(task.subtype)
                        .Append('=').Append(taskAvailable).Append('+').Append(taskInstalled)
                        .Append('+').Append(taskHidden).Append('-').Append(taskAbsent)
                        .Append('/').Append(taskListed).Append('/').Append(task.partsCount);

                    BuildJobMountIndex.Append(taskLabels, task.type + "/" + task.subtype + "="
                        + taskText.ToString(), ref labelsDropped);
                }

                if (listed == 0)
                {
                    if (declared == 0)
                    {
                        _loggedJobId = jobId;
                        MelonLogger.Warning("[BuildJob] mount census: job=" + jobId + " tasks="
                            + tasks.Length + " declared=0 listed=0 (the job lists no parts at all)");
                        return;
                    }

                    Warn(jobId, " listed=0 declared=" + declared + " (transient, will retry)");
                    return;
                }

                int carMounted = 0;
                int carMissing = 0;
                int carHidden = 0;
                int carDropped = 0;
                StringBuilder notListed = new StringBuilder();

                for (int i = 0; i < parts.Length; i++)
                {
                    if (parts[i] == null || referenced[i])
                    {
                        continue;
                    }

                    if (kinds[i] == BuildJobMountIndex.KindMountable)
                    {
                        carMissing++;
                        BuildJobMountIndex.Append(notListed, names[i] + "-off", ref carDropped);
                    }
                    else if (kinds[i] == BuildJobMountIndex.KindInstalled)
                    {
                        carMounted++;
                        BuildJobMountIndex.Append(notListed, names[i] + "-on", ref carDropped);
                    }
                    else if (kinds[i] == BuildJobMountIndex.KindHidden)
                    {
                        carHidden++;
                        BuildJobMountIndex.Append(notListed, names[i] + "-hidden", ref carDropped);
                    }
                }

                MelonLogger.Msg("[BuildJob] mount census: job=" + jobId + " tasks=" + tasks.Length
                    + " declared=" + declared + " listed=" + listed
                    + " mountable=" + available + " alreadyMounted=" + installed + " hidden=" + hidden
                    + " absent=" + absent + " unknown=" + unknown
                    + " carParts=" + parts.Length + " keyFailures=" + keyFailures
                    + " idMember=" + BuildJobPartMemberProbe.IdMemberName
                    + " nameMember=" + BuildJobPartMemberProbe.NameMemberName
                    + " detail=type/subtype=mountable+mounted+hidden-absent/listed/declared " + detail);

                MelonLogger.Msg("[BuildJob] mount census tasks: job=" + jobId
                    + " truncated=" + labelsDropped + " labels=" + taskLabels);

                MelonLogger.Msg("[BuildJob] mount census mountable: job=" + jobId + " count=" + available
                    + " truncated=" + availableDropped + " names=" + availableNames);

                MelonLogger.Msg("[BuildJob] mount census installed: job=" + jobId + " count=" + installed
                    + " truncated=" + installedDropped + " names=" + installedNames);

                MelonLogger.Msg("[BuildJob] mount census hidden: job=" + jobId + " count=" + hidden
                    + " truncated=" + hiddenDropped
                    + " (not activeInHierarchy: the game candidate snapshot cannot see them) names=" + hiddenNames);

                if (absent > 0)
                {
                    MelonLogger.Warning("[BuildJob] mount census absent: job=" + jobId + " count=" + absent
                        + " truncated=" + absentDropped
                        + " (listed by the job, no free object in the car) names=" + absentNames);

                    if (absent == listed)
                    {
                        MelonLogger.Warning("[BuildJob] mount census: job=" + jobId
                            + " match rate zero: no listed part matched any car object (idMember="
                            + BuildJobPartMemberProbe.IdMemberName + " nameMember="
                            + BuildJobPartMemberProbe.NameMemberName
                            + "), verify the identity members before trusting the split");
                    }
                }

                MelonLogger.Msg("[BuildJob] mount census carNotListed: job=" + jobId
                    + " mounted=" + carMounted + " unmounted=" + carMissing + " hidden=" + carHidden
                    + " truncated=" + carDropped + " names=" + notListed);

                _loggedJobId = jobId;

                LogInteriorParts(jobId, parts);
            }
            catch (System.Exception e)
            {
                Warn(jobId, " failed " + e.Message);
            }
        }

        internal static void ClearLoggedMarker(int jobId)
        {
            if (_loggedJobId == jobId)
            {
                _loggedJobId = -1;
            }

            if (_failureJobId == jobId)
            {
                _failureJobId = -1;
                _failureCount = 0;
            }
        }

        private static void LogInteriorParts(int jobId, PartScript[] parts)
        {
            int count = 0;
            int dropped = 0;
            GameInventory inventory = null;
            try
            {
                inventory = Singleton<GameInventory>.Instance;
            }
            catch (System.Exception)
            {
            }

            StringBuilder detail = new StringBuilder();
            for (int i = 0; i < parts.Length; i++)
            {
                PartScript part = parts[i];
                string name = BuildJobMountIndex.SafeName(parts, i);
                if (part == null || !BuildJobMissingInteractiveObjects.IsInteriorName(name))
                {
                    continue;
                }

                count++;
                string ioState = "?";
                try
                {
                    InteractiveObject io = part.GetComponent<InteractiveObject>();
                    ioState = io == null ? "none" : (io.gameObject.activeSelf ? "on" : "off");
                }
                catch (System.Exception)
                {
                }

                BuildJobMountIndex.Append(detail, name + ":" + KindText(BuildJobMountIndex.Classify(part))
                    + ":io" + ioState + ":t" + BuildJobPartPropertyProbe.TypeOf(inventory,
                        BuildJobPartPropertyProbe.PartId(part)), ref dropped);
            }

            MelonLogger.Msg("[BuildJob] mount census interior: job=" + jobId + " count=" + count
                + " truncated=" + dropped + " detail=" + detail);
        }

        private static string KindText(int kind)
        {
            if (kind == BuildJobMountIndex.KindMountable)
            {
                return "mountable";
            }

            if (kind == BuildJobMountIndex.KindInstalled)
            {
                return "installed";
            }

            if (kind == BuildJobMountIndex.KindHidden)
            {
                return "hidden";
            }

            return "unknown";
        }

        private static void Warn(int jobId, string tail)
        {
            if (_failureJobId != jobId)
            {
                _failureJobId = jobId;
                _failureCount = 0;
            }

            if (_failureCount >= MaxFailureLogs)
            {
                return;
            }

            _failureCount++;
            MelonLogger.Warning("[BuildJob] mount census: job=" + jobId + tail);
        }
    }
}
