using MelonLoader;
using System;
using System.Text;
using UnhollowerBaseLib;
using UnityEngine;


namespace RealShop
{
    internal static partial class BuildJobService
    {
        internal static bool Validate(Job job, CarLoader loader, out string reason)
        {
            reason = null;
            Config config = RealParts.ActiveConfig;
            if (config == null)
            {
                reason = BuildJobText.Pick("modOff", "мод выключен");
                return false;
            }

            if (job == null)
            {
                reason = BuildJobText.Pick("jobMissing", "задание не найдено");
                return false;
            }

            if (loader == null)
            {
                reason = BuildJobText.Pick("carMissing", "автомобиль задания не найден в гараже");
                return false;
            }

            int body = BodyRequirement(job.id);
            int parts = PartsRequirement(job.id);
            int frame = FrameRequirement(job.id);
            int interior = InteriorRequirement(job.id);

            float bodyCond = BodyConditionFor(loader);
            float detailsCond = SafeDetailsCondition(loader);
            float frameCond = FrameCondition(loader);
            float interiorCond = SafeInteriorCondition(loader);

            StringBuilder sb = new StringBuilder();
            if (bodyCond >= 0f && Percent(bodyCond) < body)
            {
                sb.Append(BuildJobText.Pick("body", config.BuildJobLabelBody)).Append(' ')
                    .Append(Percent(bodyCond)).Append("% < ").Append(body).Append("%; ");
            }

            if (Percent(detailsCond) < parts)
            {
                sb.Append(BuildJobText.Pick("parts", config.BuildJobLabelParts)).Append(' ')
                    .Append(Percent(detailsCond)).Append("% < ").Append(parts).Append("%; ");
            }

            if (frameCond >= 0f && Percent(frameCond) < frame)
            {
                sb.Append(BuildJobText.Pick("frame", "РАМА")).Append(' ')
                    .Append(Percent(frameCond)).Append("% < ").Append(frame).Append("%; ");
            }

            if (!InteriorNotDelivered() && Percent(interiorCond) < interior)
            {
                sb.Append(BuildJobText.Pick("interior", config.BuildJobLabelInterior)).Append(' ')
                    .Append(Percent(interiorCond)).Append("% < ").Append(interior).Append("%; ");
            }

            string requiredEngine = EngineTypeFor(job.id);
            if (!string.IsNullOrEmpty(requiredEngine) && !EngineRequirementMet(job.id, loader))
            {
                sb.Append(BuildJobText.Pick("engine", config.BuildJobLabelEngine)).Append(": ")
                    .Append(CarData.GetEngineDisplayName(requiredEngine)).Append(' ')
                    .Append(BuildJobText.Pick("engineMissing", "отсутствует")).Append("; ");
            }

            if (config.BuildJobRequireAllPartsMounted)
            {
                int unmounted = CountUnmounted(loader);
                if (unmounted > 0)
                {
                    sb.Append(BuildJobText.Pick("unmounted", "снято деталей")).Append(": ").Append(unmounted).Append("; ");
                }
            }

            int minMounted = config.BuildJobMinMountedParts;
            if (minMounted > 0)
            {
                int mounted = CountMounted(loader);
                if (mounted < minMounted)
                {
                    sb.Append(BuildJobText.Pick("mounted", "установлено деталей")).Append(": ").Append(mounted)
                        .Append(BuildJobText.Russian ? " из " : " of ").Append(minMounted).Append("; ");
                }
            }

            if (sb.Length == 0)
            {
                return true;
            }

            reason = sb.ToString();
            return false;
        }

        internal static string[] RequirementLines(int jobId, CarLoader loader, int advanceAmount)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null)
            {
                return new string[0];
            }

            int body = BodyRequirement(jobId);
            int parts = PartsRequirement(jobId);
            int interior = InteriorRequirement(jobId);

            int frame = FrameRequirement(jobId);
            string bodyLabel = BuildJobText.Pick("body", config.BuildJobLabelBody);
            string partsLabel = BuildJobText.Pick("parts", config.BuildJobLabelParts);
            string frameLabel = BuildJobText.Pick("frame", "РАМА");
            string interiorLabel = BuildJobText.Pick("interior", config.BuildJobLabelInterior);

            return new string[]
            {
                bodyLabel + " ≥ " + body + "%" + LiveSuffix(loader, 0, body),
                partsLabel + " ≥ " + parts + "%" + LiveSuffix(loader, 1, parts),
                frameLabel + " ≥ " + frame + "%" + LiveSuffix(loader, 2, frame),
                InteriorNotDelivered()
                    ? interiorLabel + ": " + BuildJobText.Pick("interiorAbsent", "не приезжает")
                    : interiorLabel + " ≥ " + interior + "%" + LiveSuffix(loader, 3, interior),
                AdvanceLine(config, advanceAmount),
                EngineLine(jobId, loader),
            };
        }

        private static string AdvanceLine(Config config, int advanceAmount)
        {
            if (config.BuildJobAdvancePercent <= 0)
            {
                return null;
            }

            string label = BuildJobText.Pick("advance", config.BuildJobLabelAdvance);
            string suffix = BuildJobText.Pick("advanceSuffix", config.BuildJobLabelAdvanceSuffix);
            if (advanceAmount > 0)
            {
                return label + " " + config.BuildJobAdvancePercent + "% = " + advanceAmount
                    + " CR - " + suffix;
            }

            return label + " " + config.BuildJobAdvancePercent + "% - " + suffix;
        }

        internal static bool IsRequirementMet(int jobId, int index, CarLoader loader)
        {
            if (loader == null)
            {
                return false;
            }

            int required;
            float current;
            switch (index)
            {
                case 0:
                    required = BodyRequirement(jobId);
                    current = LivePercent(loader, 0);
                    if (current < 0f)
                    {
                        return true;
                    }

                    break;
                case 1:
                    required = PartsRequirement(jobId);
                    current = LivePercent(loader, 1);
                    break;
                case 2:
                    required = FrameRequirement(jobId);
                    current = LivePercent(loader, 2);
                    if (current < 0f)
                    {
                        return true;
                    }

                    break;
                case 3:
                    if (InteriorNotDelivered())
                    {
                        return true;
                    }

                    required = InteriorRequirement(jobId);
                    current = LivePercent(loader, 3);
                    break;
                default:
                    return false;
            }

            return current >= required;
        }

        internal static uint Ticket()
        {
            _ticket = _ticket + 1u;
            return Hash((int)_ticket, Salt);
        }

        private static uint _ticket = 0x5A17BEEF;

        private static bool InteriorNotDelivered()
        {
            return BuildJobCondition.StripSuppresses("CreateInterior");
        }

        private static string LiveSuffix(CarLoader loader, int index, int required)
        {
            if (loader == null)
            {
                return string.Empty;
            }

            float current = LivePercent(loader, index);
            if (current < 0f)
            {
                return string.Empty;
            }

            return "  (" + Mathf.RoundToInt(current) + "/" + required + "%)";
        }

        private static float LivePercent(CarLoader loader, int index)
        {
            if (loader == null)
            {
                return -1f;
            }

            try
            {
                switch (index)
                {
                    case 0:
                        float body = BodyConditionFor(loader);
                        return body < 0f ? -1f : Percent(body);
                    case 1:
                        return Percent(SafeCondition(loader.GetDetailsCondition()));
                    case 2:
                        float frame = FrameCondition(loader);
                        return frame < 0f ? -1f : Percent(frame);
                    case 3:
                        return Percent(SafeCondition(loader.GetInteriorCondition()));
                    default:
                        return -1f;
                }
            }
            catch
            {
                return -1f;
            }
        }

        internal static float BodyConditionFor(CarLoader loader)
        {
            if (loader == null)
            {
                return -1f;
            }

            if (BuildJobPanels.PanelsStrippedOn(loader))
            {
                return -1f;
            }

            try
            {
                return SafeCondition(loader.GetBodyCondition());
            }
            catch
            {
                return -1f;
            }
        }

        internal static float FrameCondition(CarLoader loader)
        {
            if (loader == null)
            {
                return -1f;
            }

            try
            {
                Il2CppSystem.Collections.Generic.List<CarPart> parts = loader.carParts;
                if (parts == null || parts.Count == 0)
                {
                    return -1f;
                }

                float sum = 0f;
                int counted = 0;
                for (int i = 0; i < parts.Count; i++)
                {
                    CarPart part = parts[i];
                    if (part == null || part.Unmounted)
                    {
                        continue;
                    }

                    sum += part.StructureCondition;
                    counted++;
                }

                if (counted == 0)
                {
                    return -1f;
                }

                return sum / counted;
            }
            catch
            {
                return -1f;
            }
        }

        private static int CountMounted(CarLoader loader)
        {
            if (BuildJobPanels.PanelsStrippedOn(loader))
            {
                return BuildJobPanels.CountMountedOther(loader);
            }

            try
            {
                Il2CppSystem.Collections.Generic.List<CarPart> parts = loader.carParts;
                if (parts == null)
                {
                    return 0;
                }

                int mounted = 0;
                for (int i = 0; i < parts.Count; i++)
                {
                    CarPart part = parts[i];
                    if (part != null && !part.Unmounted)
                    {
                        mounted++;
                    }
                }

                return mounted;
            }
            catch
            {
                return 0;
            }
        }

        private static int CountUnmounted(CarLoader loader)
        {
            if (BuildJobPanels.PanelsStrippedOn(loader))
            {
                return BuildJobPanels.CountUnmountedOther(loader);
            }

            int unmounted = 0;
            try
            {
                Il2CppSystem.Collections.Generic.List<CarPart> parts = loader.carParts;
                if (parts == null)
                {
                    return 0;
                }

                for (int i = 0; i < parts.Count; i++)
                {
                    CarPart part = parts[i];
                    if (part != null && part.Unmounted)
                    {
                        unmounted++;
                    }
                }
            }
            catch
            {
            }

            return unmounted;
        }

        private static float SafeDetailsCondition(CarLoader loader)
        {
            try
            {
                return SafeCondition(loader.GetDetailsCondition());
            }
            catch
            {
                return 0f;
            }
        }

        private static float SafeInteriorCondition(CarLoader loader)
        {
            try
            {
                return SafeCondition(loader.GetInteriorCondition());
            }
            catch
            {
                return 0f;
            }
        }

        private static float SafeCondition(float value)
        {
            if (value < 0f)
            {
                return 0f;
            }

            if (value > 1f)
            {
                return value / 100f;
            }

            return value;
        }

        private static int Percent(float fraction)
        {
            return Mathf.RoundToInt(Mathf.Clamp01(fraction) * 100f);
        }

    }
}
