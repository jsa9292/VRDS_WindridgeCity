using System;
using NWH.VehiclePhysics2.Demo;
using UnityEngine;

namespace NWH.WheelController3D
{
    /// <summary>
    ///     All info related to longitudinal force calculation.
    /// </summary>
    [Serializable]
    public class Friction
    {
        /// <summary>
        ///     Current force in friction direction.
        /// </summary>
        [ShowInTelemetry]
        [Tooltip("    Current force in friction direction.")]
        public float force;

        /// <summary>
        ///     Modifies force value.
        /// </summary>
        [Tooltip("    Modifies force value.")]
        public float forceCoefficient = 1f;

        public float PI_error;

        /// <summary>
        ///     Current slip in friction direction.
        /// </summary>
        [ShowInTelemetry]
        [Tooltip("    Current slip in friction direction.")]
        public float slip;

        /// <summary>
        ///     Modifies slip value.
        /// </summary>
        [Tooltip("    Modifies slip value.")]
        public float slipCoefficient = 1;

        /// <summary>
        ///     Speed at the point of contact with the surface.
        /// </summary>
        [Tooltip("    Speed at the point of contact with the surface.")]
        public float speed;

        [SerializeField]
        private float Ki = 0.06f;

        [SerializeField]
        private float Kp = 0.16f;

        private float PI_integral;

        public void Initialize()
        {
            Ki = 0.08f * (0.02f / Time.fixedDeltaTime);
            Kp = 0.26f * (0.02f / Time.fixedDeltaTime);
        }

        public static void CalculateLateralSlip(float dt, float velocityMagnitude, float angularVelocity,
            float loadCoefficient,
            float forwardSpeed, ref FrictionPreset frictionPreset, ref Friction friction, bool hasHit,
            out float surfaceForce)
        {
            surfaceForce = 0;
            float sideSpeed = friction.speed;
            float absForwardSpeed = forwardSpeed < 0 ? -forwardSpeed : forwardSpeed;
            float absAngVel = angularVelocity < 0 ? -angularVelocity : angularVelocity;

            if (hasHit)
            {
                if (velocityMagnitude < 0.35f && absAngVel < 1f)
                {
                    friction.PI_error = friction.speed;
                    friction.PI_integral += friction.PI_error;
                    friction.slip = friction.Kp * friction.PI_error
                                    + friction.Ki * friction.PI_integral;
                    friction.slip = friction.slip < -1f ? -1f : friction.slip > 1f ? 1f : friction.slip;
                }
                else
                {
                    if (velocityMagnitude < 0.8f && absAngVel < 6f)
                    {
                        friction.slip = sideSpeed * 0.25f;
                    }
                    else
                    {
                        friction.slip = Mathf.Atan2(sideSpeed, absForwardSpeed) * Mathf.Rad2Deg / 80.0f;
                    }

                    friction.PI_error = 0;
                    friction.PI_integral = 0;
                }

                friction.slip *= friction.slipCoefficient;
                friction.slip = friction.slip < -1f ? -1f : friction.slip > 1f ? 1f : friction.slip;
                float absSlip = friction.slip < 0 ? -friction.slip : friction.slip;
                float slipSign = friction.slip < 0 ? -1f : 1f;
                float curveVal = frictionPreset.Curve.Evaluate(absSlip);
                surfaceForce = slipSign * curveVal * loadCoefficient * friction.forceCoefficient;
            }
        }

        public static float CalculateLongitudinalSlip(float torque, float brakeTorque, float dragTorque, float wheelRadius,
            float wheelInertia,
            float dt, float fixedDeltaTime, float loadCoefficient, float BCDEz, ref Friction friction,
            ref float angularVelocity, ref float outSurfaceTorque)
        {
            float speed = friction.speed;
            float absSpeed = friction.speed < 0 ? -friction.speed : friction.speed;
            float initialAngVel = angularVelocity;
            angularVelocity += torque / wheelInertia * dt;

            brakeTorque += dragTorque;
            brakeTorque = brakeTorque * (angularVelocity > 0 ? -1f : 1f);
            float brakeTorqueCap = (angularVelocity < 0 ? -angularVelocity : angularVelocity) * wheelInertia / dt;
            brakeTorque = brakeTorque > brakeTorqueCap ? brakeTorqueCap :
                brakeTorque < -brakeTorqueCap ? -brakeTorqueCap : brakeTorque;
            angularVelocity += brakeTorque / wheelInertia * dt;

            float freeAngularVelocity = speed / wheelRadius;
            float errorAngularVelocity = angularVelocity - freeAngularVelocity;
            float errorTorque = errorAngularVelocity * wheelInertia / dt;
            float maxTorque = loadCoefficient * BCDEz * friction.forceCoefficient * 0.8f;
            float groundTorque = errorTorque < -maxTorque ? -maxTorque :
                errorTorque > maxTorque ? maxTorque : errorTorque;
            float thresholdVelocity = 0.8f;

            if (absSpeed > thresholdVelocity)
            {
                friction.slip = (speed - angularVelocity * wheelRadius) / absSpeed;
            }
            else
            {
                float Vsx = speed - angularVelocity * wheelRadius;
                friction.slip = 2f * Vsx / (thresholdVelocity + speed * speed / thresholdVelocity);
            }

            friction.slip *= friction.slipCoefficient;
            friction.slip = friction.slip < -1f ? -1f : friction.slip > 1f ? 1f : friction.slip;

            angularVelocity -= groundTorque / wheelInertia * dt;

            float deltaOmegaTorque = (angularVelocity - initialAngVel) * wheelInertia / dt;

            outSurfaceTorque += groundTorque * (dt / fixedDeltaTime);

            float counterTorque = -groundTorque + brakeTorque - deltaOmegaTorque;

#if NVP_DEBUG_PT
            if (Powertrain.DEBUG) Debug.Log($"{name} (SendTorque)\tTreceived = {torque}\tTbrake =\tTreact = {Tground}\tslip = {_slip}\t" +
                $"W = {angularVelocity}\tIsum = {inertiaSum}\t Returning T = {Treturn}");
#endif

            return counterTorque * 0.9f;
        }
    }
}