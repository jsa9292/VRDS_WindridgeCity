using System;
using System.Collections.Generic;
using NWH.VehiclePhysics2.Demo;
using UnityEngine;

namespace NWH.VehiclePhysics2.Powertrain.Wheel
{
    [Serializable]
    public class WheelGroup
    {
        [Tooltip(
            "Set to positive for Pro-Ackerman steering (inner wheel steers more) or to negative for Anti-Ackerman steering.")]
        [Range(-1f, 1f)]
        [ShowInSettings("Ackerman Percent", -0.5f, 0.5f, 0.05f)]
        public float ackermanPercent = 0.15f;

        [Tooltip(
            "Used to reduce roll in the vehicle. Should not exceed max spring force setting. Another way to reduce roll is to" +
            " adjust center of mass to be lower.")]
        [ShowInSettings("ARB Force", 1000, 12000, 1000)]
        public float antiRollBarForce;

        /// <summary>
        ///     If set to 1 group will receive full brake torque as set by Max Torque parameter under Brake section while 0
        ///     means no breaking at all.
        /// </summary>
        [Tooltip(
            "If set to 1 axle will receive full brake torque as set by Max Torque parameter under Brake section while " +
            "0 means no breaking at all.")]
        [Range(0f, 1f)]
        [ShowInSettings("Brake Coefficient", 0f, 1f)]
        public float brakeCoefficient = 1f;

        [Tooltip("Camber at the bottom of the spring travel (wheel is at the lowest point).")]
        [Range(-16f, 16f)]
        [ShowInSettings("Wheel Camber At Bottom", -10f, 10f, 0.5f)]
        public float camberAtBottom = 1f;

        [Tooltip(
            "Camber at the top of the spring travel (wheel is at the highest point). Set to other than 0 to override WC3D's settings," +
            "and set to 0 if you want to use camber settings and curve from WC3D inpector.")]
        [Range(-16f, 16f)]
        [ShowInSettings("Wheel Camber At Top", -10f, 10f, 0.5f)]
        public float camberAtTop = -5f;

        [Tooltip(
            "Positive caster means that whe wheel will be angled towards the front of the vehicle while negative " +
            " caster will angle the wheel in opposite direction (shopping cart wheel).")]
        [Range(-8f, 8f)]
        [ShowInSettings("Caster Angle", -8f, 8f, 0.5f)]
        public float casterAngle;

        /// <summary>
        ///     If set to 1 axle will receive full brake torque when handbrake is used.
        /// </summary>
        [Range(0f, 1f)]
        [ShowInSettings("Handbrake Coefficient", 0f, 1f)]
        [Tooltip("    If set to 1 axle will receive full brake torque when handbrake is used.")]
        public float handbrakeCoefficient;

        [Tooltip(
            "Setting to true will override camber settings and camber will be calculated from position of the (imaginary) axle object instead.")]
        public bool isSolid;

        public string name;

        [Tooltip(
            "Determines what percentage of the steer angle will be applied to the wheel. If set to negative value" +
            " wheels will turn in direction opposite of input.")]
        [Range(-1f, 1f)]
        [ShowInSettings("Steer Coefficient", -1f, 1f)]
        public float steerCoefficient;

        [Tooltip(
            "Positive toe angle means that the wheels will face inwards (front of the wheel angled toward longitudinal center of the vehicle).")]
        [Range(-8f, 8f)]
        [ShowInSettings("Toe Angle", -5f, 5f, 0.2f)]
        public float toeAngle;
        
        [SerializeField]
        private List<WheelComponent> wheels = new List<WheelComponent>();
        private float _arbForce;
        private Vector3 _direction;
        private Vector3 _position;
        private VehicleController _vc;

        public WheelComponent LeftWheel
        {
            get { return wheels.Count == 0 ? null : wheels[0]; }
        }

        public WheelComponent RightWheel
        {
            get { return wheels.Count <= 1 ? null : wheels[1]; }
        }

        public WheelComponent Wheel
        {
            get { return wheels.Count == 0 ? null : wheels[0]; }
        }

        public List<WheelComponent> Wheels
        {
            get { return wheels; }
        }

        public void Initialize(VehicleController vc)
        {
            this._vc = vc;

            int groupIndex = vc.powertrain.wheelGroups.IndexOf(this);
            wheels.Clear();
            foreach (WheelComponent wheel in FindWheelsBelongingToGroup(ref vc.powertrain.wheels, groupIndex))
            {
                AddWheel(wheel);
            }
        }

        public void CalculateCamber()
        {
            if (isSolid && Wheels.Count == 2) // Calculate and set solid axle camber
            {
                WheelComponent leftWheel = Wheels[0];
                WheelComponent rightWheel = Wheels[1];
            
                // Center point of imaginary axle
                _position = (leftWheel.wheelController.springTravelPoint +
                             rightWheel.wheelController.springTravelPoint) / 2f;
                _direction = _position - leftWheel.wheelController.springTravelPoint;

                // Calculate camber from the mid point
                float camberAngle = Vector3.SignedAngle(_vc.transform.right, _direction, _vc.transform.forward);
                camberAngle = Mathf.Clamp(camberAngle, -25f, 25f);

                // Set camber
                leftWheel.wheelController.SetCamber(camberAngle);
                rightWheel.wheelController.SetCamber(-camberAngle);
                camberAtBottom = camberAtTop = camberAngle;
            }
            else // Set normal camber
            {
                foreach (WheelComponent wheel in Wheels)
                {
                    wheel.wheelController.SetCamber(camberAtTop, camberAtBottom);
                }
            }
        }

        public void CalculateARB()
        {
            if (Wheels.Count != 2)
            {
                return;
            }
            
            WheelComponent leftWheel = Wheels[0];
            WheelComponent rightWheel = Wheels[1];

            // Apply anti roll bar
            if (antiRollBarForce > 0)
            {
                float leftTravel = leftWheel.SpringTravel;
                float rightTravel = rightWheel.SpringTravel;

                // Cylindrical steel shaft of which ARBs are usually made have ~linear torque vs. angle of twist characteristic
                // for the low twist angle values.
                float springLengthDiff = leftWheel.wheelController.spring.length -
                                         rightWheel.wheelController.spring.length;
                _arbForce = springLengthDiff * antiRollBarForce;

                if (leftWheel.IsGrounded || rightWheel.IsGrounded)
                {
                    // Apply the ARB force at the shock anchor point.
                    _vc.vehicleRigidbody.AddForceAtPosition(leftWheel.ControllerTransform.up * -_arbForce,
                        leftWheel.ControllerTransform.position);
                    _vc.vehicleRigidbody.AddForceAtPosition(rightWheel.ControllerTransform.up * _arbForce,
                        rightWheel.ControllerTransform.position);
                }
            }
        }

        public void AddWheel(WheelComponent wheel)
        {
            Wheels.Add(wheel);
            wheel.wheelGroup = this;
        }

        public List<WheelComponent> FindWheelsBelongingToGroup(ref List<WheelComponent> wheels, int thisGroupIndex)
        {
            List<WheelComponent> belongingWheels = new List<WheelComponent>();
            foreach (WheelComponent wheel in wheels)
            {
                if (wheel.wheelGroupSelector.index == thisGroupIndex)
                {
                    belongingWheels.Add(wheel);
                }
            }

            return belongingWheels;
        }

        public void RemoveWheel(WheelComponent wheel)
        {
            Wheels.Remove(wheel);
        }

        public void SetWheels(List<WheelComponent> wheels)
        {
            this.wheels = wheels;
            foreach (WheelComponent wheel in wheels)
            {
                wheel.wheelGroup = this;
            }
        }
    }
}