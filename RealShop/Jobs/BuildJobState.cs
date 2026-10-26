using MelonLoader;
using System.Collections.Generic;

namespace RealShop
{
    internal static class BuildJobState
    {
        internal const int ArmFrames = 20000;
        private const int ModelFallbackFrames = 1800;

        private static OrderGenerator _generator;
        private static int _offeredJobId = -1;
        private static float _offerExpiresAt = -1f;
        private static int _activeJobId = -1;
        private static int _activePlace = -1;
        private static string _activeCarFile;
        private static string _lastCarFile;
        private static int _advancePendingFor = -1;
        private static int _armFrames = -1;
        private static string _armCarFile;
        private static int _armJobId = -1;
        private static int _armPlace = -1;
        private static int _strippedJobId = -1;
        private static int _candidateJobId = -1;
        private static readonly HashSet<int> PaidJobs = new HashSet<int>();

        internal static OrderGenerator Generator
        {
            get { return _generator; }
        }

        internal static int OfferedJobId
        {
            get { return _offeredJobId; }
        }

        internal static float OfferExpiresAt
        {
            get { return _offerExpiresAt; }
        }

        internal static void ExtendOffer(float seconds)
        {
            _offerExpiresAt = UnityEngine.Time.realtimeSinceStartup + seconds;
        }

        internal static int ActiveJobId
        {
            get { return _activeJobId; }
        }

        internal static int ActivePlace
        {
            get { return _activePlace; }
        }

        internal static string ActiveCarFile
        {
            get { return _activeCarFile; }
        }

        internal static int AdvancePendingFor
        {
            get { return _advancePendingFor; }
        }

        internal static bool HasActiveJob
        {
            get { return _activeJobId >= 0; }
        }

        internal static bool HasOfferedJob
        {
            get { return _offeredJobId >= 0; }
        }

        internal static bool ArmActive
        {
            get { return _armFrames > 0; }
        }

        internal static bool ModelFallbackActive
        {
            get { return _armFrames > ArmFrames - ModelFallbackFrames; }
        }

        internal static int ArmJobId
        {
            get { return _armJobId; }
        }

        internal static int ArmPlace
        {
            get { return _armPlace; }
        }

        internal static int CandidateJobId
        {
            get { return _candidateJobId; }
            set { _candidateJobId = value; }
        }

        internal static bool IsStripped(int jobId)
        {
            return jobId >= 0 && _strippedJobId == jobId;
        }

        internal static void MarkStripped(int jobId)
        {
            if (jobId >= 0)
            {
                _strippedJobId = jobId;
            }
        }

        internal static void SetGenerator(OrderGenerator generator)
        {
            if (generator != null)
            {
                _generator = generator;
            }
        }

        internal static void SetOfferedJob(int jobId, float expiresAt)
        {
            _offeredJobId = jobId;
            _offerExpiresAt = expiresAt;
        }

        internal static void ClearOfferedJob(int jobId)
        {
            if (_offeredJobId != jobId)
            {
                return;
            }

            _offeredJobId = -1;
            _offerExpiresAt = -1f;
        }

        internal static void SetLastCarFile(string carFile)
        {
            if (!string.IsNullOrEmpty(carFile))
            {
                _lastCarFile = FilterJunkyardCarsByYearPatch.NormalizeCarId(carFile);
            }
        }

        internal static bool IsLastCarFile(string carFile)
        {
            string last = _lastCarFile;
            if (string.IsNullOrEmpty(last))
            {
                return false;
            }

            return string.Equals(FilterJunkyardCarsByYearPatch.NormalizeCarId(carFile), last, System.StringComparison.Ordinal);
        }

        internal static void SetActiveJob(int jobId, string carFile, int place)
        {
            _activeJobId = jobId;
            _activeCarFile = carFile;
            _activePlace = place;
            _offeredJobId = -1;
            _offerExpiresAt = -1f;
        }

        internal static void ClearActiveJob(int jobId)
        {
            if (_activeJobId != jobId)
            {
                return;
            }

            _activeJobId = -1;
            _activePlace = -1;
            _activeCarFile = null;
            _advancePendingFor = -1;
            _strippedJobId = -1;
            BuildJobPanels.ClearStrippedMarker(jobId);
        }

        internal static void SetAdvancePending(int jobId)
        {
            _advancePendingFor = jobId;
        }

        internal static void ClearAdvancePending()
        {
            _advancePendingFor = -1;
        }

        internal static void ArmStrip(int jobId, string carFile, int place)
        {
            _armJobId = jobId;
            _armPlace = place;
            _armCarFile = FilterJunkyardCarsByYearPatch.NormalizeCarId(carFile);
            _armFrames = ArmFrames;
            BuildJobStripTarget.ResetLoadMarkers();
            MelonLogger.Msg("[BuildJob] strip armed: job=" + jobId + " car=" + (_armCarFile ?? "<null>")
                + " place=" + place + " frames=" + ArmFrames);
        }

        internal static void DisarmStrip(string tag)
        {
            if (_armFrames <= 0)
            {
                return;
            }

            _armFrames = -1;
            _armCarFile = null;
            _armJobId = -1;
            _armPlace = -1;
            MelonLogger.Msg("[BuildJob] strip disarmed: " + tag);
        }

        internal static void Tick()
        {
            if (_armFrames <= 0)
            {
                return;
            }

            _armFrames--;
            if (_armFrames == 0)
            {
                int expiredJobId = _armJobId;
                bool stripped = IsStripped(expiredJobId);
                _armCarFile = null;
                _armJobId = -1;
                _armPlace = -1;
                MelonLogger.Msg("[BuildJob] strip window expired: job=" + expiredJobId + " stripped=" + (stripped ? "yes" : "no"));
            }
        }

        internal static bool MatchesArmedJob(int orderConnection)
        {
            return _armFrames > 0 && _armJobId >= 0 && orderConnection == _armJobId;
        }

        internal static bool MatchesArmedCar(string model)
        {
            if (_armFrames <= 0)
            {
                return false;
            }

            string armed = _armCarFile;
            string car = FilterJunkyardCarsByYearPatch.NormalizeCarId(model);
            return !string.IsNullOrEmpty(armed) && !string.IsNullOrEmpty(car) && car == armed;
        }

        internal static bool MarkPaid(int jobId)
        {
            return PaidJobs.Add(jobId);
        }

        internal static bool IsPaid(int jobId)
        {
            return PaidJobs.Contains(jobId);
        }
    }
}
