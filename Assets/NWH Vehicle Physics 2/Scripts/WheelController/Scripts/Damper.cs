using System;
using NWH.VehiclePhysics2.Demo;
using UnityEngine;
using UnityEngine.Serialization;

namespace NWH.WheelController3D
{
    /// <summary>
    ///     Suspension damper.
    /// </summary>
    [Serializable]
    public class Damper
    {
        public const float maxVelocity = 100f;

        /// <summary>
        ///     Bump force of the damper.
        /// </summary>
        [FormerlySerializedAs("unitBumpForce")]
        [ShowInSettings("Damper Bump Force", 500, 5000, 200)]
        [Tooltip("    Bump force of the damper.")]
        public float bumpForce = 2000.0f;

        /// <summary>
        ///     Curve where X axis represents speed of travel of the suspension and Y axis represents resultant force.
        ///     Both values are normalized to [0,1].
        /// </summary>
        [Tooltip(
            "Curve where X axis represents speed of travel of the suspension and Y axis represents resultant force.\r\nBoth values are normalized to [0,1].")]
        public AnimationCurve curve;

        /// <summary>
        ///     Current damper force.
        /// </summary>
        [ShowInTelemetry]
        [Tooltip("    Current damper force.")]
        public float force;

        /// <summary>
        ///     Rebound force of the damper.
        /// </summary>
        [FormerlySerializedAs("unitReboundForce")]
        [ShowInSettings("Damper Rebound Force", 500, 5000, 200)]
        [Tooltip("    Rebound force of the damper.")]
        public float reboundForce = 2400.0f;
    }
}