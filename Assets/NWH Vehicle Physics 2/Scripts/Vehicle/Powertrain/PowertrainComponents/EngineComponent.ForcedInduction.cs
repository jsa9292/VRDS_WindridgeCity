using System;
using NWH.VehiclePhysics2.Demo;
using UnityEngine;

namespace NWH.VehiclePhysics2.Powertrain
{
    public partial class EngineComponent
    {
        /// <summary>
        ///     Supercharger, turbocharger, etc.
        ///     Only an approximation. Engine capacity, air flow, turbo size, etc. are not taken into consideration.
        /// </summary>
        [Serializable]
        public class ForcedInduction
        {
            public enum ForcedInductionType
            {
                Turbocharger,
                Supercharger
            }

            /// <summary>
            ///     Boost value as percentage in 0 to 1 range. Unitless.
            ///     Can be used for boost gauges.
            /// </summary>
            [ShowInTelemetry]
            [Range(0, 1)]
            [Tooltip("    Boost value as percentage in 0 to 1 range. Unitless.\r\n    Can be used for boost gauges.")]
            public float boost;

            /// <summary>
            ///     Type of forced induction.
            /// </summary>
            [ShowInTelemetry]
            [ShowInSettings("Forced Induction Type")]
            [Tooltip("    Type of forced induction.")]
            public ForcedInductionType forcedInductionType = ForcedInductionType.Turbocharger;

            /// <summary>
            ///     Imitates wastegate in a turbo setup.
            ///     Enable if you want turbo flutter sound effects and/or boost to drop off faster after closing throttle.
            ///     Not used with superchargers.
            /// </summary>
            [Tooltip(
                "Imitates wastegate in a turbo setup.\r\nEnable if you want turbo flutter sound effects and/or boost to drop off faster after closing throttle.\r\nNot used with superchargers.")]
            public bool hasWastegate = true;

            /// <summary>
            ///     Use values towards 0 for large turbos, and near 1 for small turbos.
            ///     Lower linearity values will need higher engine RPM to spool up.
            ///     Forced to 1 for superchargers
            /// </summary>
            [Range(0, 1)]
            [Tooltip(
                "Use values towards 0 for large turbos, and near 1 for small turbos.\r\nLower linearity values will need higher engine RPM to spool up.\r\nForced to 1 for superchargers")]
            public float linearity = 0.7f;

            /// <summary>
            ///     Power coefficient that the maxPower of the engine will be multiplied by and represents power gained by
            ///     adding forced induction to the engine. E.g. 1.4 would mean that the engine will produce 140% of the maxPower.
            ///     Power gain is dependent on boost value.
            /// </summary>
            [Range(0, 2)]
            [Tooltip(
                "Additional power that will be added to the engine's power.\r\nThis is the maximum value possible and depends on spool percent.")]
            [ShowInSettings("Power Gain", 1f, 3f, 0.2f)]
            public float powerGainMultiplier = 1.4f;

            /// <summary>
            ///     Shortest time possible needed for turbo to spool up to its maximum RPM.
            ///     Use larger values for larger turbos and vice versa.
            ///     Forced to 0 for superchargers.
            /// </summary>
            [Range(0.1f, 2)]
            [ShowInSettings("Spool Up Time", 0f, 2f, 0.2f)]
            [Tooltip(
                "Shortest time possible needed for turbo to spool up to its maximum RPM.\r\nUse larger values for larger turbos and vice versa.\r\nForced to 0 for superchargers.")]
            public float spoolUpTime = 0.12f;

            /// <summary>
            ///     Should forced induction be used?
            /// </summary>
            [Tooltip("    Should forced induction be used?")]
            public bool useForcedInduction = true;

            /// <summary>
            ///     Cached boost value at the time of wastegate releasing pressure for sound effects.
            /// </summary>
            [Tooltip("    Cached boost value at the time of wastegate releasing pressure for sound effects.")]
            public float wastegateBoost;

            /// <summary>
            ///     Flag for sound effects.
            /// </summary>
            [Tooltip("    Flag for sound effects.")]
            public bool wastegateFlag;

            private float maxRPM = 120000;

            private float RPM;

            private float spoolVelocity;

            /// <summary>
            ///     Current power gained from forced induction.
            /// </summary>
            public float PowerGainMultiplier
            {
                get
                {
                    if (!useForcedInduction)
                    {
                        return 1f;
                    }

                    return RPM / maxRPM * powerGainMultiplier;
                }
            }

            public void Update(EngineComponent engine)
            {
                if (!useForcedInduction)
                {
                    return;
                }

                float throttle = engine.throttlePosition;
                float factor = throttle * engine.RPMPercent;
                float targetRPM = maxRPM * Mathf.Pow(factor, 1f + 1f - linearity);

                if (forcedInductionType == ForcedInductionType.Turbocharger)
                {
                    if (hasWastegate && engine.throttlePosition < 0.5f && boost > 0.3f)
                    {
                        wastegateFlag = true;
                        wastegateBoost = boost;
                        RPM = 0f;
                    }
                    else
                    {
                        RPM = Mathf.SmoothDamp(RPM, targetRPM, ref spoolVelocity, spoolUpTime);
                    }
                }
                else
                {
                    RPM = targetRPM;
                }


                RPM = RPM > maxRPM ? maxRPM : RPM < 0 ? 0 : RPM;
                boost = RPM * RPM / (maxRPM * maxRPM);
            }
        }
    }
}