using System;
using System.Collections.Generic;
using System.Linq;
using NWH.VehiclePhysics2.Powertrain.Wheel;
using NWH.WheelController3D;
using UnityEngine;

namespace NWH.VehiclePhysics2.Powertrain
{
    [Serializable]
    public class Powertrain : VehicleComponent
    {
        // Powertrain components
        [Tooltip("Powertrain components")]
        public ClutchComponent clutch = new ClutchComponent();

        public List<DifferentialComponent> differentials = new List<DifferentialComponent>();
        public EngineComponent engine = new EngineComponent();
        public Solver solver = new Solver();
        public TransmissionComponent transmission = new TransmissionComponent();
        public List<WheelGroup> wheelGroups = new List<WheelGroup>();
        public List<WheelComponent> wheels = new List<WheelComponent>();
        private int _frameCount;

        private int _groundCheckFrameIndex = -999;
        private int _prevFrameCount = -999;
        private bool engineWasRunning;

        public bool HasWheelAir { get; private set; }

        public bool HasWheelSkid { get; private set; }

        public bool HasWheelSpin { get; private set; }

        public override void Initialize()
        {
            List<PowertrainComponent> components = GetPowertrainComponents();

            solver.Initialize(components);

            foreach (PowertrainComponent pc in components)
            {
                pc.FindOutputs(solver);
            }

            foreach (PowertrainComponent pc in components)
            {
                pc.FindInput(solver);
            }

            foreach (WheelComponent wheel in wheels)
            {
                wheel.vc = vc;
            }

            foreach (WheelGroup axle in wheelGroups)
            {
                axle.Initialize(vc);
            }

            initialized = true;
        }

        public override void FixedUpdate()
        {
            _frameCount++;

            // Always calculate camber to prevent camber being wrong when vehicle is moved while inactive, i.e. player bumps into the vehicle.
            foreach (WheelGroup wheelGroup in wheelGroups)
            {
                wheelGroup.CalculateCamber();
            }
            
            if (!Active)
            {
                return;
            }
            
            // Update wheel groups even when inactive to maintain correct camber
            bool initSyncTransforms = Physics.autoSyncTransforms;
            Physics.autoSyncTransforms = false;
            foreach (WheelGroup wheelGroup in wheelGroups)
            {
                wheelGroup.CalculateARB();
            }
            Physics.autoSyncTransforms = initSyncTransforms;

            // Cache skid, spin and air values
            HasWheelSkid = false;
            HasWheelSpin = false;
            HasWheelAir = false;

            int wheelCount = wheels.Count;

            if (_frameCount >= _groundCheckFrameIndex + wheelCount)
            {
                _groundCheckFrameIndex = _frameCount;
            }

            int currentIndex = _frameCount - _groundCheckFrameIndex;

            for (int i = 0; i < wheelCount; i++)
            {
                WheelComponent wheel = wheels[i];
                if (wheel.HasLongitudinalSlip)
                {
                    HasWheelSpin = true;
                }

                if (wheel.HasLateralSlip)
                {
                    HasWheelSkid = true;
                }

                if (!wheel.IsGrounded)
                {
                    HasWheelAir = true;
                }

                if (_prevFrameCount != _frameCount && currentIndex == i)
                {
                    vc.groundDetection.GetCurrentSurfaceMap(wheel.wheelController, ref wheel.surfaceMapIndex,
                        ref wheel.surfacePreset);
                }
            }

            float verticalInput = vc.input.Vertical;
            transmission.CheckForShift(verticalInput, HasWheelSpin, HasWheelSkid, HasWheelAir, engine.RPM,
                engine.minRPM, engine.revLimiterRPM,
                vc.vehicleRigidbody.velocity.magnitude, clutch.clutchEngagement, vc.transform.forward, vc.transform.up,
                vc.input.ShiftUp, vc.input.ShiftDown, vc.input.ShiftInto);

            vc.input.ResetShiftUpFlag();
            vc.input.ResetShiftDownFlag();
            vc.input.ShiftInto = -999;

            int transmissionGear = transmission.Gear;

            float throttlePosition = vc.input.Vertical;
            if (transmission.IsShifting)
            {
                throttlePosition = 0;
            }
            else
            {
                if (vc.input.throttleType == Input.Input.ThrottleType.WForwardSReverse)
                {
                    throttlePosition = transmissionGear < 0 ? -verticalInput : verticalInput;
                }
            }

            engine.ThrottlePosition = throttlePosition;
            engine.slipTorque = clutch.hasTorqueConverter ? clutch.torqueConverterSlipTorque : clutch.slipTorque;
            clutch.fwdAcceleration = vc.ForwardAcceleration;
            clutch.gear = transmissionGear;
            clutch.shiftSignal = transmission.IsShifting;
            if (!clutch.isAutomatic)
            {
                clutch.clutchEngagement = vc.input.Clutch;
            }

            if (vc.input.EngineStartStop)
            {
                engine.StartStop();
                vc.input.ResetEngineStartStopFlag();
            }

            foreach (WheelComponent wheel in wheels)
            {
                wheel.PreUpdate();
            }

            solver.Solve();
            foreach (WheelComponent wheel in wheels)
            {
                wheel.PostUpdate();
            }

            if (_frameCount > 9999999)
            {
                _frameCount = 0;
            }

            _prevFrameCount = _frameCount;
        }

        public override void Update()
        {
        }

        public override void Enable()
        {
            base.Enable();

            foreach (PowertrainComponent pc in solver.Components)
            {
                pc.OnEnable();
            }
        }

        public override void Disable()
        {
            base.Disable();
            
            foreach (PowertrainComponent pc in solver.Components)
            {
                pc.OnDisable();
            }
        }

        public List<string> GetPowertrainComponentNames(List<PowertrainComponent> powertrainComponents)
        {
            return powertrainComponents.Select(c => c.name).ToList();
        }

        public List<string> GetPowertrainComponentNames()
        {
            return GetPowertrainComponentNames(GetPowertrainComponents());
        }

        public List<string> GetPowertrainComponentNamesWithType(List<PowertrainComponent> powertrainComponents)
        {
            return powertrainComponents.Select(c =>
                $"[{c.GetType().ToString().Split('.').LastOrDefault()?.Replace("Component", "")}] {c.name} ").ToList();
        }

        public List<PowertrainComponent> GetPowertrainComponents()
        {
            List<PowertrainComponent> powertrainComponents = new List<PowertrainComponent>
                {engine, clutch, transmission};

            foreach (DifferentialComponent diff in differentials)
            {
                powertrainComponents.Add(diff);
            }

            foreach (WheelComponent wheel in wheels)
            {
                powertrainComponents.Add(wheel);
            }

            return powertrainComponents;
        }

        public void GetWheelStates(out bool wheelSpin, out bool wheelSkid, out bool wheelAir)
        {
            wheelSpin = false;
            wheelSkid = false;
            wheelAir = false;

            foreach (WheelComponent wheel in wheels)
            {
                if (wheel.HasLongitudinalSlip)
                {
                    wheelSpin = true;
                }

                if (wheel.HasLateralSlip)
                {
                    wheelSkid = true;
                }

                if (!wheel.IsGrounded)
                {
                    wheelAir = true;
                }
            }
        }


        public override void SetDefaults(VehicleController vc)
        {
            base.SetDefaults(vc);

            engine.name = "Engine";
            engine.inertia = 0.2f;
            clutch.name = "Clutch";
            transmission.name = "Transmission";

            engine.SetOutput(clutch);
            clutch.SetOutput(transmission);

            engine.powerCurve = new AnimationCurve
            {
                keys = new[]
                {
                    new Keyframe(0f, 0f, 0, 1f),
                    new Keyframe(0.3f, 0.53f, 1f, 1f),
                    new Keyframe(0.5f, 0.8f, 1f, 1f),
                    new Keyframe(1f, 1f)
                }
            };

            transmission.gearingProfile =
                Resources.Load(VehicleController.DEFAULT_RESOURCES_PATH + "DefaultGearingProfile")
                    as TransmissionGearingProfile;

            // Find wheels
            wheels = new List<WheelComponent>();
            foreach (WheelController wheelController in vc.GetComponentsInChildren<WheelController>())
            {
                wheels.Add(new WheelComponent
                {
                    name = "Wheel" + wheelController.name,
                    wheelController = wheelController
                });
                Debug.Log($"VehicleController setup: Found WheelController '{wheelController.name}'");
            }

            if (wheels.Count == 0)
            {
                Debug.LogWarning("No WheelControllers found, skipping powertrain auto-setup.");
                return;
            }

            // Order wheels in left-right, front to back order.
            wheels = wheels.OrderByDescending(w => w.wheelController.transform.localPosition.z).ToList();
            List<int> wheelGroupIndices = new List<int>();
            int wheelGroupCount = 1;
            float prevWheelZ = wheels[0].wheelController.transform.localPosition.z;
            for (int i = 0; i < wheels.Count; i++)
            {
                WheelComponent wheel = wheels[i];
                float wheelZ = wheel.wheelController.transform.localPosition.z;

                // Wheels are on different axes, add new axis/wheel group.
                if (Mathf.Abs(wheelZ - prevWheelZ) > 0.2f)
                {
                    wheelGroupCount++;
                }
                // Wheels are on the same axle, order left to right.
                else if (i > 0)
                {
                    if (wheels[i].wheelController.transform.localPosition.x <
                        wheels[i - 1].wheelController.transform.localPosition.x)
                    {
                        WheelComponent tmp = wheels[i - 1];
                        wheels[i - 1] = wheels[i];
                        wheels[i] = tmp;
                    }
                }

                wheelGroupIndices.Add(wheelGroupCount - 1);
                prevWheelZ = wheelZ;
            }

            // Add wheel groups
            wheelGroups = new List<WheelGroup>();
            for (int i = 0; i < wheelGroupCount; i++)
            {
                string appendix = i == 0 ? "Front" : i == wheelGroupCount - 1 ? "Rear" : "Middle";
                string groupName = $"{appendix} Axle {i}";
                wheelGroups.Add(new WheelGroup
                {
                    name = groupName,
                    brakeCoefficient = i == 0 || wheelGroupCount > 2 ? 1f : 0.7f,
                    handbrakeCoefficient = i == wheelGroupCount - 1 ? 1f : 0f,
                    steerCoefficient = i == 0 ? 1f : i == 1 && wheelGroupCount > 2 ? 0.5f : 0f,
                    ackermanPercent = i == 0 ? 0.12f : 0f,
                    antiRollBarForce = 3000f,
                    isSolid = false
                });
                Debug.Log($"VehicleController setup: Creating WheelGroup '{groupName}'");
            }

            // Add differentials
            differentials = new List<DifferentialComponent>();
            Debug.Log("[Powertrain] Adding 'Front Differential'");
            differentials.Add(new DifferentialComponent {name = "Front Differential"});
            Debug.Log("[Powertrain] Adding 'Rear Differential'");
            differentials.Add(new DifferentialComponent {name = "Rear Differential"});
            Debug.Log("[Powertrain] Adding 'Center Differential'");
            differentials.Add(new DifferentialComponent {name = "Center Differential"});
            differentials[2].SetOutput(differentials[0], differentials[1]);

            // Connect transmission to differentials
            Debug.Log($"[Powertrain] Setting transmission output to '{differentials[2].name}'");
            transmission.SetOutput(differentials[2]);

            // Add wheels to wheel groups
            for (int i = 0; i < wheels.Count; i++)
            {
                int wheelGroupIndex = wheelGroupIndices[i];
                wheels[i].wheelGroupSelector = new WheelGroupSelector {index = wheelGroupIndex};
                Debug.Log($"[Powertrain] Adding '{wheels[i].name}' to '{wheelGroups[wheelGroupIndex].name}'");
            }

            // Connect wheels to differentials
            int diffCount = differentials.Count;
            int wheelGroupsCount = wheelGroups.Count;
            wheelGroupsCount =
                wheelGroupCount > 2
                    ? 2
                    : wheelGroupCount; // Prevent from resetting diffs on vehicles with more than 2 axles
            for (int i = 0; i < wheelGroupsCount; i++)
            {
                WheelGroup group = wheelGroups[i];
                List<WheelComponent> belongingWheels = group.FindWheelsBelongingToGroup(ref wheels, i);

                if (belongingWheels.Count == 2)
                {
                    Debug.Log(
                        $"[Powertrain] Setting output of '{differentials[i].name}' to '{belongingWheels[0].name}'");
                    if (belongingWheels[0].wheelController.vehicleSide == WheelController.Side.Left)
                    {
                        differentials[i].SetOutput(belongingWheels[0], belongingWheels[1]);
                    }
                    else if (belongingWheels[0].wheelController.vehicleSide == WheelController.Side.Right)
                    {
                        differentials[i].SetOutput(belongingWheels[1], belongingWheels[0]);
                    }
                    else
                    {
                        Debug.LogWarning(
                            "[Powertrain] Powertrain settings for center wheels have to be manually set up. If powered either connect it directly to transmission (motorcycle) or to one side of center differential (trike).");
                    }
                }
            }
        }

        public override void Validate(VehicleController vc)
        {
            base.Validate(vc);

            engine.Validate(vc);
            clutch.Validate(vc);
            transmission.Validate(vc);
            foreach (DifferentialComponent diff in differentials)
            {
                diff.Validate(vc);
            }

            foreach (WheelComponent wheel in wheels)
            {
                wheel.Validate(vc);
            }
        }
    }
}