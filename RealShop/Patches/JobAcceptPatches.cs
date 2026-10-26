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
    [HarmonyPatch(typeof(OrdersWindow), "AcceptOrderAction")]
    internal static class BuildJobAcceptPatch
    {
        private const int CandidateMaxWaits = 120;
        private const double CandidateTimeoutSeconds = 60.0;

        private static bool _activeLogged;
        private static Job _candidate;
        private static int _candidateWaits;
        private static System.DateTime _candidateDeadline = System.DateTime.MinValue;

        private static int _tickFrames;

        internal static void TickCommit()
        {
            if (_candidate == null)
            {
                _tickFrames = 0;
                return;
            }

            _tickFrames++;
            if (_tickFrames % 30 != 0)
            {
                return;
            }

            TryCommit();
        }

        private static void Prefix(OrdersWindow __instance)
        {
            _candidate = null;

            Config config = RealParts.ActiveConfig;
            if (config == null || !config.EnableBuildFromScratchJobs)
            {
                return;
            }

            if (!_activeLogged)
            {
                _activeLogged = true;
                MelonLogger.Msg("[BuildJob] hook active: AcceptOrderAction");
            }

            try
            {
                _candidateWaits = 0;
                _candidateDeadline = System.DateTime.UtcNow.AddSeconds(CandidateTimeoutSeconds);
                _candidate = ResolveAcceptedJob();
                BuildJobState.CandidateJobId = _candidate != null ? _candidate.id : -1;
                if (_candidate == null)
                {
                    BuildJobStripArm.DisarmCandidateWindow("accept is not a build job");
                    return;
                }

                BuildJobStripArm.ArmCandidate(_candidate, "AcceptOrderAction prefix");
            }
            catch (System.Exception e)
            {
                ClearCandidate();
                MelonLogger.Warning("[BuildJob] AcceptOrderAction prefix failed: " + e.Message);
            }
        }

        private static void Postfix()
        {
            TryCommit();
        }

        internal static void TryCommit()
        {
            Job candidate = _candidate;
            if (candidate == null)
            {
                return;
            }

            Config config = RealParts.ActiveConfig;
            if (config == null || !config.EnableBuildFromScratchJobs)
            {
                ClearCandidate();
                return;
            }

            try
            {
                OrderGenerator generator = BuildJobState.Generator;
                bool expired = System.DateTime.UtcNow > _candidateDeadline;
                if (generator == null)
                {
                    if (expired)
                    {
                        MelonLogger.Warning("[BuildJob] accept not confirmed: id=" + candidate.id
                            + " abandoned, no generator after " + CandidateTimeoutSeconds + "s");
                        ClearCandidate();
                    }

                    return;
                }

                if (!IsAcceptConfirmed(generator, candidate.id))
                {
                    _candidateWaits++;
                    if (_candidateWaits == 8 || _candidateWaits == 40)
                    {
                        MelonLogger.Warning("[BuildJob] accept not confirmed: id=" + candidate.id
                            + " stillOffered waits=" + _candidateWaits);
                    }

                    if (_candidateWaits >= CandidateMaxWaits || expired)
                    {
                        MelonLogger.Warning("[BuildJob] accept not confirmed: id=" + candidate.id
                            + " abandoned after waits=" + _candidateWaits);
                        ClearCandidate();
                    }

                    return;
                }

                _candidate = null;
                _candidateWaits = 0;
                BuildJobState.CandidateJobId = -1;
                BuildJobService.OnOrderAccepted(candidate, "AcceptOrderAction");
            }
            catch (System.Exception e)
            {
                ClearCandidate();
                MelonLogger.Warning("[BuildJob] AcceptOrderAction commit failed: " + e.Message);
            }
        }

        private static bool IsAcceptConfirmed(OrderGenerator generator, int jobId)
        {
            if (IsOffered(generator, jobId))
            {
                return false;
            }

            try
            {
                Il2CppSystem.Collections.Generic.List<Job> selected = generator.selectedJobs;
                if (selected != null)
                {
                    for (int i = 0; i < selected.Count; i++)
                    {
                        Job job = selected[i];
                        if (job != null && job.id == jobId)
                        {
                            return true;
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] selectedJobs read failed: " + e.Message);
            }

            return BuildJobService.HasCarForOrder(jobId);
        }

        internal static void AbandonCandidate(string tag)
        {
            if (_candidate == null)
            {
                return;
            }

            MelonLogger.Msg("[BuildJob] accept candidate abandoned: id=" + _candidate.id + " (" + tag + ")");
            ClearCandidate();
        }

        private static bool IsOffered(OrderGenerator generator, int jobId)
        {
            try
            {
                Il2CppSystem.Collections.Generic.List<Job> jobs = generator.Jobs;
                if (jobs == null)
                {
                    return true;
                }

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
                return true;
            }

            return false;
        }

        internal static void ClearCandidate()
        {
            _candidate = null;
            _candidateWaits = 0;
            BuildJobState.CandidateJobId = -1;
            BuildJobState.DisarmStrip("accept candidate cleared");
        }

        private static Job ResolveAcceptedJob()
        {
            OrderGenerator generator = BuildJobState.Generator;
            if (generator == null || BuildJobSelectPatch.SelectedJobId < 0)
            {
                return null;
            }

            Job selected = BuildJobTakePatch.FindJob(generator, BuildJobSelectPatch.SelectedJobId);
            return BuildJobService.IsBuildJob(selected) ? selected : null;
        }
    }

    [HarmonyPatch(typeof(OrderGenerator), "CancelJob")]
    internal static class BuildJobCancelPatch
    {
        private static bool _activeLogged;

        private static void Postfix([HarmonyArgument(0)] int jobId)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null || !config.EnableBuildFromScratchJobs)
            {
                return;
            }

            if (!_activeLogged)
            {
                _activeLogged = true;
                MelonLogger.Msg("[BuildJob] hook active: CancelJob");
            }

            if (BuildJobState.CandidateJobId == jobId)
            {
                BuildJobAcceptPatch.ClearCandidate();
                MelonLogger.Msg("[BuildJob] cancelled before commit: id=" + jobId);
            }

            if (BuildJobState.ActiveJobId == jobId)
            {
                BuildJobAcceptPatch.ClearCandidate();
                BuildJobService.ForfeitAdvance(jobId, "cancel");
                BuildJobState.ClearActiveJob(jobId);
                BuildJobState.DisarmStrip("CancelJob");
                MelonLogger.Msg("[BuildJob] cancelled: id=" + jobId);
                return;
            }

            if (BuildJobService.AdvanceFor(jobId) > 0)
            {
                BuildJobService.ForfeitAdvance(jobId, "cancel, job not active (reloaded save)");
                MelonLogger.Msg("[BuildJob] cancelled: id=" + jobId + " not active, advance clawed back");
            }
        }
    }
}

#pragma warning restore CS0618
