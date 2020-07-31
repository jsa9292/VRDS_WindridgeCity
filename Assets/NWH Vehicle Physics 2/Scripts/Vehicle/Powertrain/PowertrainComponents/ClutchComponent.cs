using System;
using NWH.VehiclePhysics2.Demo;
using NWH.VehiclePhysics2.Utility;
using UnityEngine;

namespace NWH.VehiclePhysics2.Powertrain
{
    [Serializable]
    public class ClutchComponent : PowertrainComponent
    {
        /// <summary>
        ///     RPM at which automatic clutch will try to engage.
        /// </summary>
        [ShowInSettings("Base Engagement RPM", 1000f, 4000f, 200f)]
        [Tooltip("    RPM at which automatic clutch will try to engage.")]
        public float baseEngagementRPM = 1200f;

        /// <summary>
        ///     Clutch engagement in range [0,1] where 1 is fully engaged clutch.
        ///     Affected by Slip Torque field as the clutch can transfer [clutchEngagement * slipTorque] Nm
        ///     meaning that higher value of slipTorque will result in more sensitive clutch.
        /// </summary>
        [Range(0, 1)]
        [ShowInTelemetry]
        [Tooltip(
            "Clutch engagement in range [0,1] where 1 is fully engaged clutch.\r\nAffected by Slip Torque field as the clutch can transfer [clutchEngagement * slipTorque] Nm\r\nmeaning that higher value of slipTorque will result in more sensitive clutch.")]
        public float clutchEngagement;

        /// <summary>
        ///     RPM at which the clutch will engage. Equals baseEngagementRPM plus variable engagement range.
        /// </summary>
        [ShowInTelemetry]
        [Tooltip("    RPM at which the clutch will engage. Equals baseEngagementRPM plus variable engagement range.")]
        public float finalEngagementRPM = 2000f;

        public float fwdAcceleration;

        public int gear;

        /// <summary>
        ///     If true torqueConverterSlip torque will be used instead of slipTorque to calculate clutch/torque converter slip
        /// </summary>
        [ShowInSettings("Torque Converter")]
        [Tooltip(
            "If true torqueConverterSlip torque will be used instead of slipTorque to calculate clutch/torque converter slip")]
        public bool hasTorqueConverter;

        /// <summary>
        ///     Is the clutch automatic? If true any input set manually will be overridden by the result given by PID controller
        ///     based on the difference between engine and clutch RPM.
        /// </summary>
        [ShowInSettings]
        [Tooltip(
            "Is the clutch automatic? If true any input set manually will be overridden by the result given by PID controller\r\nbased on the difference between engine and clutch RPM.")]
        public bool isAutomatic = true;

        /// <summary>
        ///     Final result of PID controller is multiplied by this value. Used to adjust how fast PID reacts without
        ///     having to change individual coefficients.
        /// </summary>
        [Tooltip(
            "Final result of PID controller is multiplied by this value. Used to adjust how fast PID reacts without\r\nhaving to change individual coefficients.")]
        public float PID_Coefficient = 0.015f;

        /// <summary>
        ///     Derivative term of automatic clutch PID controller. Clutch engagement is adjusted based
        ///     on speed of change of the error between the clutch RPM and engine RPM.
        ///     Too high value will result in oscillation.
        /// </summary>
        [SerializeField]
        [Range(0f, 2f)]
        [Tooltip(
            "Derivative term of automatic clutch PID controller.")]
        public float PID_Kd = 0.5f;

        /// <summary>
        ///     Integral term of automatic clutch PID controller. Clutch engagement is adjusted based on
        ///     how long the error between the clutch RPM and engine RPM has persisted.
        ///     Too high value will result in oscillation.
        /// </summary>
        [SerializeField]
        [Range(0f, 5f)]
        [Tooltip(
            "Integral term of automatic clutch PID controller.")]
        public float PID_Ki = 2.7f;

        /// <summary>
        ///     Proportional term of automatic clutch PID controller. Clutch engagement is adjusted
        ///     proportionally to the error between the clutch RPM and engine RPM.
        ///     Low value will result in clutch engaging very slowly while high value will result in
        ///     clutch engaging fast - possibly stalling the engine in extreme cases.
        /// </summary>
        [SerializeField]
        [Range(0f, 10f)]
        [Tooltip(
            "Proportional term of automatic clutch PID controller.")]
        public float PID_Kp = 4f;

        public bool shiftSignal;

        /// <summary>
        ///     Torque at which the clutch will slip / maximum torque that the clutch can transfer.
        ///     This value also affects clutch engagement as higher slip value will result in clutch
        ///     that grabs higher up / sooner. Too high slip torque value combined with low inertia of
        ///     powertrain components might cause instability in powertrain solver.
        /// </summary>
        [SerializeField]
        [ShowInSettings("Slip Torque", 100f, 5000f, 200f)]
        [Tooltip(
            "Torque at which the clutch will slip / maximum torque that the clutch can transfer.\r\nThis value also affects clutch engagement as higher slip value will result in clutch\r\nthat grabs higher up / sooner. Too high slip torque value combined with low inertia of\r\npowertrain components might cause instability in powertrain solver.")]
        public float slipTorque = 3000f;

        /// <summary>
        ///     Torque at which the torque converter will slip. Lower value will result in longer and smoother shifts.
        /// </summary>
        [Tooltip(
            "Torque at which the torque converter will slip. Lower value will result in longer and smoother shifts.")]
        public float torqueConverterSlipTorque = 80f;

        /// <summary>
        ///     Maximum RPM value that variableEngagementIntensity field can add to engagement RPM.
        ///     Final clutch engagement RPM is calculated by multiplying this value by variableEngagementIntensity and adding the
        ///     result
        ///     engagementRPM.
        /// </summary>
        [SerializeField]
        [Tooltip(
            "Maximum RPM value that variableEngagementIntensity field can add to engagement RPM.\r\nFinal clutch engagement RPM is calculated by multiplying this value by variableEngagementIntensity and adding the result\r\nengagementRPM. ")]
        public float variableEngagementRPMRange = 1400f;

        private float _cachedTargetAngVel;
        private float _e, _ePrev;
        private float _ed;
        private float _ei;

        private float _smoothAcceleration;

        public override void OnPreSolve()
        {
            base.OnPreSolve();

            if (_smoothAcceleration < 0 && fwdAcceleration > 0 || _smoothAcceleration > 0 && fwdAcceleration < 0)
            {
                _smoothAcceleration = 0;
            }

            _smoothAcceleration = Mathf.Lerp(_smoothAcceleration, fwdAcceleration, 0.05f);
            int absGear = gear < 0 ? -gear : gear;
            absGear = absGear == 0 ? 1 : absGear;
            float gearFactor = 1f / (absGear * absGear);
            float variableRangeCoeff = Mathf.Clamp01(1f - _smoothAcceleration * gear * 0.2f) * gearFactor;
            finalEngagementRPM = baseEngagementRPM + variableEngagementRPMRange * variableRangeCoeff;
            _cachedTargetAngVel = UnitConverter.RPMToAngularVelocity(finalEngagementRPM);
        }

        public override float QueryAngularVelocity(float inputAngularVelocity, float dt)
        {
            angularVelocity = inputAngularVelocity;
            if (_outputAIsNull)
            {
                return inputAngularVelocity;
            }

            // Adjust engagement
            if (isAutomatic)
            {
                if (shiftSignal)
                {
                    clutchEngagement = 0f;
                }
                else
                {
                    _ePrev = _e;
                    _e = _cachedTargetAngVel - angularVelocity;
                    _ei += _e * dt;
                    if (_e <= 0)
                    {
                        _ei = 0;
                    }

                    _ed = (_e - _ePrev) / dt;
                    float diff = PID_Kp * _e * dt + PID_Ki * _ei * dt + PID_Kd * _ed * dt;
                    diff *= PID_Coefficient;
                    clutchEngagement -= diff;
                }

                clutchEngagement = clutchEngagement < 0 ? 0 : clutchEngagement > 1 ? 1 : clutchEngagement;
            }

            // Solver uses velocity based approach which is not ideal for clutch simulation
            float engSquared = clutchEngagement * clutchEngagement;
            float Wout = outputA.QueryAngularVelocity(inputAngularVelocity, dt) * engSquared;
            float Win = inputAngularVelocity * (1f - engSquared);
            float W = Wout + Win;

#if NVP_DEBUG_PT
            if (Powertrain.DEBUG) Debug.Log($"{name} (QueryAngularVelocity)\tC = {_clutchEngagement}\tW = {angularVelocity}\tWout = {Wout}\tWin = {Win}\tReturn W = {W}");
#endif
            return W;
        }

        public override float QueryInertia()
        {
            if (_outputAIsNull)
            {
#if NVP_DEBUG_PT
                if(Powertrain.DEBUG) Debug.Log($"{name} (QueryInertia)\tC = {_clutchEngagement}\tReturn I = {inertia}");
#endif
                return inertia;
            }

            float I = inertia + outputA.QueryInertia() * clutchEngagement;

#if NVP_DEBUG_PT
            if (Powertrain.DEBUG) Debug.Log($"{name} (QueryInertia)\tC = {_clutchEngagement}\tReturn I = {I}");
#endif

            return I;
        }

        public override float SendTorque(float torque, float inertiaSum, float dt)
        {
            if (_outputAIsNull)
            {
                return torque;
            }

            torque = torque > slipTorque ? slipTorque : torque < -slipTorque ? -slipTorque : torque;
            float returnTorque =
                outputA.SendTorque(torque * clutchEngagement, (inertiaSum + inertia) * clutchEngagement, dt) *
                clutchEngagement * clutchEngagement;
            returnTorque = returnTorque > slipTorque ? slipTorque :
                returnTorque < -slipTorque ? -slipTorque : returnTorque;
            return returnTorque;
        }

        public override void Validate(VehicleController vc)
        {
            base.Validate(vc);

            Debug.Assert(!string.IsNullOrEmpty(outputASelector.name),
                "Clutch is not connected to anything. Go to Powertrain > Clutch and set the output.");
        }

        public override void OnDisable()
        {
            base.OnDisable();
            
            clutchEngagement = 0;
        }
    }
}