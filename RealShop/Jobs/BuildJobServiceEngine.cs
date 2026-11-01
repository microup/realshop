using MelonLoader;
using System;

namespace RealShop
{
    internal static partial class BuildJobService
    {
        internal static string EngineTypeFor(int jobId)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null || config.BuildJobEngineJobId != jobId)
            {
                return string.Empty;
            }

            return config.BuildJobEngineType;
        }

        internal static string InstalledEngineType(CarLoader loader)
        {
            if (loader == null)
            {
                return string.Empty;
            }

            try
            {
                EngineParams parameters = loader.EngineParams;
                string type = parameters.Type;
                return type != null ? type.Trim() : string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        internal static bool EngineRequirementMet(int jobId, CarLoader loader)
        {
            string required = EngineTypeFor(jobId);
            if (string.IsNullOrEmpty(required))
            {
                return true;
            }

            return string.Equals(InstalledEngineType(loader), required, StringComparison.OrdinalIgnoreCase);
        }

        private static string EngineLine(int jobId, CarLoader loader)
        {
            string id = EngineTypeFor(jobId);
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            Config config = RealParts.ActiveConfig;
            string label = BuildJobText.Pick("engine", config != null ? config.BuildJobLabelEngine : "ДВИГАТЕЛЬ");
            string name = CarData.GetEngineDisplayName(id);
            string installed = InstalledEngineType(loader);
            string state;
            if (loader == null)
            {
                state = string.Empty;
            }
            else if (string.Equals(installed, id, StringComparison.OrdinalIgnoreCase))
            {
                state = " - " + BuildJobText.Pick("engineInstalled", "установлен");
            }
            else if (!string.IsNullOrEmpty(installed))
            {
                state = " - " + CarData.GetEngineDisplayName(installed);
            }
            else
            {
                state = " - " + BuildJobText.Pick("engineMissing", "отсутствует");
            }

            return label + ": " + name + state;
        }
    }
}
