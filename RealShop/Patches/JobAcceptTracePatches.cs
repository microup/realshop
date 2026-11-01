using CMS.UI.Logic;
using Harmony;
using MelonLoader;

#pragma warning disable CS0618

namespace RealShop
{
    internal static class BuildJobAcceptTrace
    {
        private const int LimitPerTag = 3;

        private static int _plateNull;
        private static int _plateOk;
        private static int _livery;
        private static int _dent;
        private static int _otherParts;
        private static int _timer;

        internal static bool Active()
        {
            Config config = RealParts.ActiveConfig;
            if (config == null || !config.EnableBuildFromScratchJobs)
            {
                return false;
            }

            return BuildJobState.CandidateJobId >= 0 || BuildJobState.ActiveJobId >= 0
                || BuildJobState.ArmActive;
        }

        internal static void Reset()
        {
            _plateNull = 0;
            _plateOk = 0;
            _livery = 0;
            _dent = 0;
            _otherParts = 0;
            _timer = 0;
        }

        internal static bool PlateNullSlot()
        {
            if (_plateNull >= LimitPerTag)
            {
                return false;
            }

            _plateNull++;
            return true;
        }

        internal static bool PlateOkSlot()
        {
            if (_plateOk >= LimitPerTag)
            {
                return false;
            }

            _plateOk++;
            return true;
        }

        internal static bool TailSlot(string tag)
        {
            int used;
            if (tag == "livery")
            {
                used = _livery++;
            }
            else if (tag == "dent")
            {
                used = _dent++;
            }
            else
            {
                used = _otherParts++;
            }

            return used < LimitPerTag;
        }

        internal static bool TimerSlot()
        {
            if (_timer >= LimitPerTag)
            {
                return false;
            }

            _timer++;
            return true;
        }

        internal static string State()
        {
            return " candidate=" + BuildJobState.CandidateJobId + " active=" + BuildJobState.ActiveJobId
                + " offered=" + BuildJobState.OfferedJobId + " arm=" + BuildJobState.ArmJobId;
        }
    }

    [HarmonyPatch(typeof(CarLoader), "GetCarPart", new[] { typeof(string) })]
    internal static class BuildJobPlatePartTracePatch
    {
        private static void Postfix(ref CarPart __result, [HarmonyArgument(0)] string partName)
        {
            try
            {
                if (!BuildJobAcceptTrace.Active() || partName == null
                    || partName.IndexOf("license_plate", System.StringComparison.OrdinalIgnoreCase) < 0)
                {
                    return;
                }

                if (__result != null)
                {
                    if (BuildJobAcceptTrace.PlateOkSlot())
                    {
                        MelonLogger.Msg("[BuildJob] accept trace: licence plate part present: name=" + partName
                            + BuildJobAcceptTrace.State());
                    }

                    return;
                }

                if (BuildJobAcceptTrace.PlateNullSlot())
                {
                    MelonLogger.Warning("[BuildJob] accept trace: licence plate part missing: name=" + partName
                        + BuildJobAcceptTrace.State()
                        + " (CreateExterior is suppressed: the game's TakeJob coroutine hands this null part to"
                        + " ChangeLicencePlateTexture and dies, so PrepareJob is never called - the order can never"
                        + " build its task list)");
                }
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] licence plate trace hook failed: " + e.Message);
            }
        }
    }

    [HarmonyPatch(typeof(CarLoader), "SetRandomCarLivery")]
    internal static class BuildJobLiveryTracePatch
    {
        private static void Postfix()
        {
            try
            {
                if (BuildJobAcceptTrace.Active() && BuildJobAcceptTrace.TailSlot("livery"))
                {
                    MelonLogger.Msg("[BuildJob] accept trace: take job tail SetRandomCarLivery"
                        + BuildJobAcceptTrace.State() + " (the accept coroutine passed PrepareJob)");
                }
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] livery trace hook failed: " + e.Message);
            }
        }
    }

    [HarmonyPatch(typeof(CarLoader), "SetRandomDent")]
    internal static class BuildJobDentTracePatch
    {
        private static void Postfix()
        {
            try
            {
                if (BuildJobAcceptTrace.Active() && BuildJobAcceptTrace.TailSlot("dent"))
                {
                    MelonLogger.Msg("[BuildJob] accept trace: take job tail SetRandomDent"
                        + BuildJobAcceptTrace.State());
                }
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] dent trace hook failed: " + e.Message);
            }
        }
    }

    [HarmonyPatch(typeof(CarLoader), "GetOtherPartsCondition")]
    internal static class BuildJobOtherPartsTracePatch
    {
        private static void Postfix([HarmonyArgument(0)] Job job)
        {
            try
            {
                if (BuildJobAcceptTrace.Active() && BuildJobAcceptTrace.TailSlot("otherParts"))
                {
                    MelonLogger.Msg("[BuildJob] accept trace: take job tail GetOtherPartsCondition job="
                        + (job != null ? job.id : -1) + BuildJobAcceptTrace.State()
                        + " (the accept coroutine reaches CancelJob next)");
                }
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] other parts trace hook failed: " + e.Message);
            }
        }
    }

    [HarmonyPatch(typeof(Job), "StartTimer")]
    internal static class BuildJobJobTimerTracePatch
    {
        private static void Prefix(Job __instance)
        {
            try
            {
                if (__instance == null || !BuildJobAcceptTrace.Active())
                {
                    return;
                }

                int jobId = __instance.id;
                if (jobId != BuildJobState.CandidateJobId && jobId != BuildJobState.ActiveJobId
                    && jobId != BuildJobState.OfferedJobId)
                {
                    return;
                }

                if (BuildJobAcceptTrace.TimerSlot())
                {
                    MelonLogger.Msg("[BuildJob] accept trace: game job timer started: id=" + jobId
                        + BuildJobAcceptTrace.State()
                        + " (Job.StartTimer calls OrderGenerator.CancelJob when the timer runs out)");
                }
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] job timer trace hook failed: " + e.Message);
            }
        }
    }
}

#pragma warning restore CS0618
