using System;
using System.Collections.Generic;
using NWH.VehiclePhysics2.Demo;
using NWH.VehiclePhysics2.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace NWH.VehiclePhysics2.Powertrain
{
    [Serializable]
    public class TransmissionComponent : PowertrainComponent
    {
        public delegate void Shift(bool upshiftSignal, bool downshiftSignal, int shiftIntoSignal, bool checksValid,
            float yAxis,
            float engineRPM, float minEngineRPM, float maxEngineRPM, float vehicleSpeed, Vector3 forward, Vector3 up);

        public delegate bool ShiftCheck();

        public enum ReverseType
        {
            Auto,
            DoubleTap
        }

        public enum Type
        {
            Manual,
            Automatic,
            AutomaticSequential,
            CVT,
            External
        }

        public float cvtMaxInputTorque = 300f;

        /// <summary>
        ///     Final gear multiplier. Each gear gets multiplied by this value.
        ///     Equivalent to axle/differential ratio in real life.
        /// </summary>
        [ShowInSettings("Final Gear Ratio", 1f, 8f, 0.5f)]
        [Tooltip(
            "    Final gear multiplier. Each gear gets multiplied by this value.\r\n    Equivalent to axle/differential ratio in real life.")]
        public float finalGearRatio = 3;

        /// <summary>
        ///     Currently active gearing profile.
        ///     Final gear ratio will be determined from this and final gear ratio.
        /// </summary>
        [Tooltip(
            "    Currently active gearing profile.\r\n    Final gear ratio will be determined from this and final gear ratio.")]
        public TransmissionGearingProfile gearingProfile;

        [Range(0, 4)]
        public float inclineEffectCoeff;

        [SerializeField]
        public ShiftEvent onDownshift;

        [SerializeField]
        public ShiftEvent onShift;

        [SerializeField]
        public ShiftEvent onUpshift;

        /// <summary>
        ///     Time after shifting in which shifting can not be done again.
        /// </summary>
        [Tooltip("    Time after shifting in which shifting can not be done again.")]
        public float postShiftBan = 0.5f;

        [Tooltip("Behavior when switching from neutral or forward gears to reverse gear. " +
                 "Auto - if the vehicle speed is low enough and vertical input negative, transmission will shift to reverse. " +
                 "DoubleTap - once all the requirements exist for shifting into reverse, user has to release the button and press it again to shift," +
                 " otherwise the vehicle will stand still in neutral with brakes applied.")]
        public ReverseType reverseType = ReverseType.Auto;

        public float shiftCheckCooldown = 0.1f;

        [HideInInspector]
        public List<ShiftCheck> shiftChecks = new List<ShiftCheck>();

        public bool shiftCheckValid = true;
        public Shift shiftDelegate;

        /// <summary>
        ///     Time it takes transmission to shift between gears.
        /// </summary>
        [ShowInSettings("Shift Duration", 0.01f, 0.5f, 0.05f)]
        [Tooltip("    Time it takes transmission to shift between gears.")]
        public float shiftDuration = 0.2f;

        [Range(0, 1)]
        public float variableShiftIntensity = 0.3f;

        /// <summary>
        ///     If enabled transmission will adjust both shift up and down points to match current load.
        /// </summary>
        [Tooltip("    If enabled transmission will adjust both shift up and down points to match current load.")]
        public bool variableShiftPoint = true;

        protected float _ratio;

        [SerializeField]
        private int _currentGearIndex;

        /// <summary>
        ///     RPM at which automatic transmission will shift down. If dynamic shift point is enabled this value will change
        ///     depending on load.
        /// </summary>
        [Tooltip(
            "RPM at which automatic transmission will shift down. If dynamic shift point is enabled this value will change depending on load.")]
        [SerializeField]
        private float _downshiftRPM = 1400;

        /// <summary>
        ///     Engine RPM at which transmission will shift down if dynamic shift point is enabled.
        /// </summary>
        [SerializeField]
        private float _targetDownshiftRPM;

        /// <summary>
        ///     Engine RPM at which transmission will shift up if dynamic shift point is enabled.
        /// </summary>
        [SerializeField]
        private float _targetUpshiftRPM;

        /// <summary>
        ///     Determines in which way gears can be changed.
        ///     Manual - gears can only be shifted by manual user input.
        ///     Automatic - automatic gear changing. Allows for gear skipping (e.g. 3rd->5th) which can be useful in trucks and
        ///     other high gear count vehicles.
        ///     AutomaticSequential - automatic gear changing but only one gear at the time can be shifted (e.g. 3rd->4th)
        /// </summary>
        [SerializeField]
        [Tooltip("Manual - gears can only be shifted by manual user input. " +
                 "Automatic - automatic gear changing. Allows for gear skipping (e.g. 3rd->5th) which can be useful in trucks and other high gear count vehicles. " +
                 "AutomaticSequential - automatic gear changing but only one gear at the time can be shifted (e.g. 3rd->4th)")]
        [ShowInSettings("Transmission Type")]
        private Type _transmissionType = Type.AutomaticSequential;

        /// <summary>
        ///     RPM at which automatic transmission will shift up. If dynamic shift point is enabled this value will change
        ///     depending on load.
        /// </summary>
        [Tooltip(
            "RPM at which automatic transmission will shift up. If dynamic shift point is enabled this value will change depending on load.")]
        [SerializeField]
        private float _upshiftRPM = 2800;

        private Shift cAutoShift;
        private Shift cCvtShift;

        [SerializeField]
        private bool clutchEngaged;

        private Shift cManualShift;

        [SerializeField]
        private bool externalShiftChecksValid = true;

        /// <summary>
        ///     List of gears starting with reverse gears, neutral and then forward gears. List is constructed on initialization
        ///     from forward and reverse gear lists.
        ///     This is so that things like Gear++ and Gear-- can be done.
        /// </summary>
        private List<float> gearRatios = new List<float>();

        /// <summary>
        ///     Time since the start of the scene when the last shift happened.
        /// </summary>
        private float lastShiftTime = -10f;

        [SerializeField]
        private bool noWheelAir;

        [SerializeField]
        private bool noWheelSkid;

        // Shift checks. Can shift if all positive.
        [SerializeField]
        private bool noWheelSpin;

        private float shiftCheckTime = -999f;

        private float smoothedYAxis01;
        private float totalReceivedTorque;

        private float verticalInputChangeVelocity;

        public int CurrentGearIndex
        {
            get { return _currentGearIndex; }
        }

        public float DownshiftAngularVelocity
        {
            get { return UnitConverter.RPMToAngularVelocity(_downshiftRPM); }
        }

        public float DownshiftRPM
        {
            get { return _downshiftRPM; }
            set { _downshiftRPM = Mathf.Clamp(value, 0, Mathf.Infinity); }
        }

        /// <summary>
        ///     Number of forward gears.
        /// </summary>
        public int ForwardGearCount
        {
            get { return gearRatios.Count - 1 - gearingProfile.reverseGears.Count; }
        }

        /// <summary>
        ///     List of forward gears. Gears list will be updated if new value is assigned.
        /// </summary>
        public List<float> ForwardGears
        {
            get { return gearingProfile.forwardGears; }
            set
            {
                gearingProfile.forwardGears = value;
                ReconstructGearList();
            }
        }

        /// <summary>
        ///     0 for neutral, less than 0 for reverse gears and lager than 0 for forward gears.
        /// </summary>
        public int Gear
        {
            get
            {
                int reverseGearCount = gearingProfile.reverseGears.Count;
                int gearRatioCount = gearRatios.Count;
                if (_currentGearIndex < 0 - reverseGearCount)
                {
                    return _currentGearIndex = 0 - reverseGearCount;
                }

                if (_currentGearIndex >= gearRatioCount - reverseGearCount - 1)
                {
                    return _currentGearIndex = gearRatioCount - reverseGearCount - 1;
                }

                return _currentGearIndex;
            }
            set
            {
                int reverseGearCount = gearingProfile.reverseGears.Count;
                int gearRatioCount = gearRatios.Count;
                if (value < 0 - reverseGearCount)
                {
                    _currentGearIndex = 0 - reverseGearCount;
                }
                else if (value >= gearRatioCount - reverseGearCount - 1)
                {
                    _currentGearIndex = ForwardGearCount;
                }
                else if (value < -100)
                {
                    // Do nothing
                }
                else
                {
                    _currentGearIndex = value;
                }
            }
        }

        /// <summary>
        ///     Returns current gear name as a string, e.g. "R", "R2", "N" or "1"
        /// </summary>
        [ShowInTelemetry]
        public string GearName
        {
            get
            {
                float gear = Gear;
                if (gear == 0)
                {
                    return "N";
                }

                if (gear > 0)
                {
                    return Gear.ToString();
                }

                if (gearingProfile.reverseGears.Count > 1)
                {
                    return "R" + (gear < 0 ? -gear : gear);
                }

                return "R";
            }
        }

        /// <summary>
        ///     List of all gear ratios including reverse, forward and neutral gears. e.g. -2nd, -1st, 0 (netural), 1st, 2nd, 3rd,
        ///     etc.
        /// </summary>
        public List<float> Gears
        {
            get { return gearRatios; }
        }

        public bool IsInReverse
        {
            get { return _ratio < 0; }
        }

        /// <summary>
        ///     True if currently shifting.
        /// </summary>
        public bool IsShifting
        {
            get
            {
                if (Time.realtimeSinceStartup < lastShiftTime + shiftDuration)
                {
                    return true;
                }

                return false;
            }
        }

        [ShowInTelemetry]
        public float Ratio
        {
            get { return _ratio; }
        }

        /// <summary>
        ///     Number of reverse gears.
        /// </summary>
        public int ReverseGearCount
        {
            get { return gearingProfile.reverseGears.Count; }
        }

        /// <summary>
        ///     List of reverse gears. Gears list will be updated if new value is assigned.
        /// </summary>
        public List<float> ReverseGears
        {
            get { return gearingProfile.reverseGears; }
            set
            {
                gearingProfile.reverseGears = value;
                ReconstructGearList();
            }
        }

        [ShowInTelemetry]
        public float TargetDownshiftRPM
        {
            get { return _targetDownshiftRPM; }
        }

        [ShowInTelemetry]
        public float TargetUpshiftRPM
        {
            get { return _targetUpshiftRPM; }
        }

        public Type TransmissionType
        {
            get { return _transmissionType; }
            set { _transmissionType = value; }
        }

        public float UpshiftAngularVelocity
        {
            get { return UnitConverter.RPMToAngularVelocity(_upshiftRPM); }
        }

        public float UpshiftRPM
        {
            get { return _upshiftRPM; }
            set { _upshiftRPM = Mathf.Clamp(value, 0, Mathf.Infinity); }
        }

        public override void Initialize()
        {
            ReconstructGearList();
            cAutoShift = AutomaticShift;
            cManualShift = ManualShift;
            cCvtShift = CVTShift;
        }

        public void CheckForShift(float throttle, bool wheelSpin, bool wheelSkid, bool wheelAir, float engineRPM,
            float minEngineRPM, float revLimiterRPM,
            float vehicleSpeed, float clutchEngagement, Vector3 forward, Vector3 up, bool upshiftSignal,
            bool downshiftSignal, int shiftIntoSignal = -999)
        {
            noWheelSpin = !wheelSpin;
            noWheelSkid = !wheelSkid;
            noWheelAir = !wheelAir;
            clutchEngaged = clutchEngagement > 0.8f;

            _ratio = GetCurrentGearRatio();

            bool currentShiftCheckValid = noWheelSpin && noWheelSkid && noWheelAir && clutchEngaged;
            if (!currentShiftCheckValid)
            {
                shiftCheckTime = Time.realtimeSinceStartup;
                shiftCheckValid = false;
            }
            else if (shiftCheckTime + shiftCheckCooldown <= Time.realtimeSinceStartup)
            {
                shiftCheckValid = true;
            }

            if (_transmissionType == Type.Manual)
            {
                shiftDelegate = cManualShift;
            }
            else if (_transmissionType == Type.Automatic || _transmissionType == Type.AutomaticSequential)
            {
                shiftDelegate = cAutoShift;
            }
            else if (_transmissionType == Type.CVT)
            {
                shiftDelegate = cCvtShift;
            }

            // Run shift checks
            externalShiftChecksValid = true;
            foreach (ShiftCheck check in shiftChecks)
            {
                if (!check.Invoke())
                {
                    externalShiftChecksValid = false;
                    break;
                }
            }

            shiftDelegate.Invoke(upshiftSignal, downshiftSignal, shiftIntoSignal, externalShiftChecksValid, throttle,
                engineRPM,
                minEngineRPM, revLimiterRPM, vehicleSpeed, forward, up);
        }

        /// <summary>
        ///     Total gear ratio of the transmission for current gear.
        /// </summary>
        public float GetCurrentGearRatio()
        {
            return gearRatios[GearToIndex(_currentGearIndex)] * finalGearRatio;
        }

        /// <summary>
        ///     Total gear ratio of the transmission for the specific gear.
        /// </summary>
        /// <returns></returns>
        public float GetGearRatio(int g)
        {
            return gearRatios[GearToIndex(g)] * finalGearRatio;
        }

        public override void OnPreSolve()
        {
            base.OnPreSolve();

            totalReceivedTorque = 0;
        }

        public override float QueryAngularVelocity(float inputAngularVelocity, float dt)
        {
            angularVelocity = inputAngularVelocity;
            if (_ratio == 0 || _outputAIsNull)
            {
                return inputAngularVelocity;
            }

            return outputA.QueryAngularVelocity(inputAngularVelocity, dt) * _ratio;
        }

        public override float QueryInertia()
        {
            if (_outputAIsNull || _ratio == 0)
            {
                return inertia;
            }

            return inertia + outputA.QueryInertia() / (_ratio * _ratio);
        }

        /// <summary>
        ///     Recreates gear list from the forward and reverse gears lists.
        /// </summary>
        public void ReconstructGearList()
        {
            gearRatios.Clear();
            gearRatios.AddRange(gearingProfile.reverseGears);
            gearRatios.Add(0);
            gearRatios.AddRange(gearingProfile.forwardGears);
        }

        /// <summary>
        ///     Converts axle RPM to engine RPM for given gear in Gears list.
        /// </summary>
        public float ReverseTransmitRPM(float inputRPM, int g)
        {
            float outRpm = inputRPM * gearRatios[GearToIndex(g)] * finalGearRatio;
            outRpm = outRpm < 0 ? -outRpm : outRpm;
            return outRpm;
        }

        public override float SendTorque(float torque, float inertiaSum, float dt)
        {
            totalReceivedTorque += torque;
            if (_outputAIsNull)
            {
                return torque;
            }

            if (_ratio == 0)
            {
                outputA.SendTorque(0, 0, dt);
                return torque;
            }

            // Always send torque to keep wheels updated
            return outputA.SendTorque(torque * _ratio, (inertiaSum + inertia) * (_ratio * _ratio), dt) / _ratio;
        }

        /// <summary>
        ///     Shifts into given gear. 0 for neutral, less than 0 for reverse and above 0 for forward gears.
        /// </summary>
        public void ShiftInto(int g)
        {
            if (g == Gear)
            {
                return;
            }

            int prevGear = Gear;

            if (externalShiftChecksValid && Time.realtimeSinceStartup > lastShiftTime + shiftDuration + postShiftBan)
            {
                if (prevGear < Gear)
                {
                    onUpshift.Invoke(g);
                }
                else if (prevGear > Gear)
                {
                    onDownshift.Invoke(g);
                }

                onShift.Invoke(g);

                if (Gear != 0)
                {
                    lastShiftTime = Time.realtimeSinceStartup;
                }

                Gear = g;
            }
        }

        public override void Validate(VehicleController vc)
        {
            base.Validate(vc);

            Debug.Assert(!string.IsNullOrEmpty(outputASelector.name),
                "Transmission is not connected to anything. Go to Powertrain > Transmission and set the output.");

            if (gearingProfile == null)
            {
                Debug.LogError("Transmission gearing profile not assigned.");
            }
            else
            {
                foreach (float gear in gearingProfile.reverseGears)
                {
                    Debug.Assert(gear < 0, "Reverse gears in gearing profile should have negative value.");
                }

                foreach (float gear in gearingProfile.forwardGears)
                {
                    Debug.Assert(gear > 0, "Forward gears in gearing profile should have positive value.");
                }
            }

            if (_upshiftRPM > vc.powertrain.engine.revLimiterRPM || _upshiftRPM > vc.powertrain.engine.maxRPM)
            {
                Debug.LogWarning(
                    "Transmission upshift RPM set to higher RPM than the engine can achieve (check engine maxRPM and revLimiterRPM).");
            }

            if (_downshiftRPM < vc.powertrain.engine.stallRPM || _downshiftRPM < vc.powertrain.engine.idleRPM)
            {
                Debug.LogWarning(
                    "Transmission downshift RPM set to lower RPM than the engine can achieve (check engine stallRPM and idleRPM).");
            }
        }

        /// <summary>
        ///     Handles automatic and automatic sequential shifting.
        /// </summary>
        private void AutomaticShift(bool upshiftSignal, bool downshiftSignal, int shiftIntoSignal, bool checksValid,
            float yAxis, float engineRPM, float minEngineRPM, float revLimiterRPM, float vehicleSpeed,
            Vector3 forward, Vector3 up)
        {
            float yAxis01 = yAxis < 0 ? 0 : yAxis;
            float damping = yAxis01 > smoothedYAxis01 ? 0.2f : 2f;
            smoothedYAxis01 = Mathf.SmoothDamp(smoothedYAxis01, yAxis01, ref verticalInputChangeVelocity, damping);

            _targetDownshiftRPM = _downshiftRPM;
            _targetUpshiftRPM = _upshiftRPM;

            if (variableShiftPoint)
            {
                _targetUpshiftRPM =
                    _upshiftRPM + Mathf.Clamp01(smoothedYAxis01 * variableShiftIntensity) * revLimiterRPM;
                _targetDownshiftRPM =
                    _downshiftRPM + Mathf.Clamp01(smoothedYAxis01 * variableShiftIntensity) * revLimiterRPM;

                _targetUpshiftRPM = Mathf.Clamp(_targetUpshiftRPM, _upshiftRPM, revLimiterRPM);
                _targetDownshiftRPM = Mathf.Clamp(_targetDownshiftRPM, minEngineRPM * 1.1f, _targetUpshiftRPM * 0.6f);

                // Intentionally allow incline modifier to go over rev limiter RPM to prevent upshifts on inclines
                float inclineModifier = Mathf.Clamp01(Vector3.Dot(forward, Vector3.up) * inclineEffectCoeff);
                inclineModifier *= inclineModifier;
                _targetUpshiftRPM += revLimiterRPM * (inclineModifier * 3f);
                _targetDownshiftRPM += revLimiterRPM * (inclineModifier * 1.8f);
            }

            int gear = Gear;

            // In neutral
            if (gear == 0)
            {
                // Shift into first
                if (yAxis > 0.1f)
                {
                    Gear = 1;
                }
                // Shift into reverse
                else if (yAxis < -0.1f)
                {
                    Gear = -1;
                }
            }
            // In reverse
            else if (gear < 0)
            {
                // Shift into 1st
                if (yAxis > 0.1f && vehicleSpeed < 1f)
                {
                    ShiftInto(1);
                }
                // Shift into neutral
                else if (yAxis > -0.1f && yAxis < 0.1f && vehicleSpeed < 1f)
                {
                    ShiftInto(0);
                }

                // Reverse upshift
                if (engineRPM > TargetUpshiftRPM && Mathf.Abs(gear - 1) < ReverseGearCount)
                {
                    ShiftInto(gear - 1);
                }
                // Reverse downshift
                else if (engineRPM < TargetDownshiftRPM)
                {
                    // 1st reverse gear to neutral
                    if (gear == -1 && yAxis > -0.1f && yAxis < 0.1f)
                    {
                        ShiftInto(0);
                    }
                    // To first gear
                    else if (gear < -1)
                    {
                        ShiftInto(gear + 1);
                    }
                }
            }
            // In forward
            else
            {
                if (vehicleSpeed > 0.2f)
                {
                    // Upshift
                    if (gear < ForwardGearCount && engineRPM > TargetUpshiftRPM && shiftCheckValid)
                    {
                        if (TransmissionType == Type.Automatic)
                        {
                            int g = gear;
                            
                            // Disallow gear skipping when under full throttle. Not very effective on binary inputs.
                            int maxGear = smoothedYAxis01 > 0.8f ? gear + 1 : gear + 2;

                            while (g < ForwardGearCount)
                            {
                                g++;

                                if (g == maxGear)
                                {
                                    break;
                                }

                                float wouldBeEngineRPM = ReverseTransmitRPM(RPM / _ratio, g);
                                if (wouldBeEngineRPM < TargetDownshiftRPM)
                                {
                                    g--;
                                    break;
                                }
                            }

                            if (g != gear)
                            {
                                ShiftInto(g);
                            }
                        }
                        else
                        {
                            ShiftInto(gear + 1);
                        }
                    }
                    // Downshift
                    else if (engineRPM < TargetDownshiftRPM)
                    {
                        // Non-sequential
                        if (_transmissionType == Type.Automatic)
                        {
                            if (gear != 1)
                            {
                                int g = gear;
                                while (g > 1)
                                {
                                    g--;
                                    float wouldBeEngineRPM = ReverseTransmitRPM(RPM / _ratio, g);
                                    if (wouldBeEngineRPM > TargetUpshiftRPM)
                                    {
                                        g++;
                                        break;
                                    }
                                }

                                if (g != gear)
                                {
                                    ShiftInto(g);
                                }
                            }
                            else if (vehicleSpeed < 1f && yAxis > -0.1f && yAxis < 0.1f)
                            {
                                ShiftInto(0);
                            }
                        }
                        // Sequential
                        else
                        {
                            if (Gear != 1)
                            {
                                ShiftInto(Gear - 1);
                            }
                            else if (vehicleSpeed < 1f && yAxis < 0.1f && yAxis > -0.1f)
                            {
                                ShiftInto(0);
                            }
                        }
                    }
                }
                // Shift into reverse
                else if (vehicleSpeed < 0.2f && yAxis <= -0.1f)
                {
                    ShiftInto(-1);
                }
                // Shift into neutral
                else if (yAxis < 0.1f && yAxis > -0.1f && vehicleSpeed > -0.2f && vehicleSpeed < 0.2f)
                {
                    ShiftInto(0);
                }
            }
        }

        private void CVTShift(bool upshiftSignal, bool downshiftSignal, int shiftIntoSignal, bool checksValid,
            float yAxis,
            float engineRPM,
            float minEngineRPM, float maxEngineRPM, float vehicleSpeed, Vector3 forward, Vector3 up)
        {
            float minRatio = gearingProfile.forwardGears[0];
            float maxRatio = gearingProfile.forwardGears[1];

            totalReceivedTorque = totalReceivedTorque * Time.fixedDeltaTime;
            float torqueFactor = (totalReceivedTorque < 0 ? -totalReceivedTorque : totalReceivedTorque) /
                                 cvtMaxInputTorque;
            torqueFactor = torqueFactor < 0 ? 0 : torqueFactor > 1f ? 1f : torqueFactor;
            Gear = 1;
            _ratio = minRatio * torqueFactor + maxRatio * (1f - torqueFactor);
        }

        private int GearToIndex(int g)
        {
            return g + gearingProfile.reverseGears.Count;
        }

        private void ManualShift(bool upshiftSignal, bool downshiftSignal, int shiftIntoSignal, bool checksValid,
            float yAxis,
            float engineRPM, float minEngineRPM,
            float maxEngineRPM, float vehicleSpeed, Vector3 forward, Vector3 up)
        {
            if (upshiftSignal)
            {
                ShiftInto(Gear + 1);
            }

            if (downshiftSignal)
            {
                ShiftInto(Gear - 1);
            }

            if (shiftIntoSignal > -100)
            {
                ShiftInto(shiftIntoSignal);
            }
        }

        [Serializable]
        public class ShiftEvent : UnityEvent<int>
        {
        }
    }
}