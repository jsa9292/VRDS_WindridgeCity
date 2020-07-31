using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace NWH.VehiclePhysics2.Effects
{
    /// <summary>
    ///     Class for controlling all of the vehicle lights.
    /// </summary>
    [Serializable]
    public class LightsMananger : Effect
    {
        /// <summary>
        ///     Rear lights that will light up when brake is pressed. Always red.
        /// </summary>
        [FormerlySerializedAs("stopLights")]
        [Tooltip("    Rear lights that will light up when brake is pressed. Always red.")]
        public VehicleLight brakeLights = new VehicleLight();

        /// <summary>
        ///     Can be used for any type of special lights.
        /// </summary>
        [Tooltip("    Can be used for any type of special lights.")]
        public VehicleLight extraLights = new VehicleLight();

        /// <summary>
        ///     High (full) beam lights.
        /// </summary>
        [FormerlySerializedAs("fullBeams")]
        [Tooltip("    High (full) beam lights.")]
        public VehicleLight highBeamLights = new VehicleLight();

        /// <summary>
        ///     Blinkers on the left side of the vehicle.
        /// </summary>
        [Tooltip("    Blinkers on the left side of the vehicle.")]
        public VehicleLight leftBlinkers = new VehicleLight();

        /// <summary>
        ///     Low beam lights.
        /// </summary>
        [FormerlySerializedAs("headLights")]
        [Tooltip("    Low beam lights.")]
        public VehicleLight lowBeamLights = new VehicleLight();

        /// <summary>
        ///     Rear Lights that will light up when vehicle is in reverse gear(s). Usually white.
        /// </summary>
        [Tooltip("    Rear Lights that will light up when vehicle is in reverse gear(s). Usually white.")]
        public VehicleLight reverseLights = new VehicleLight();

        /// <summary>
        ///     Blinkers on the right side of the vehicle.
        /// </summary>
        [Tooltip("    Blinkers on the right side of the vehicle.")]
        public VehicleLight rightBlinkers = new VehicleLight();

        /// <summary>
        ///     Rear Lights that will light up when headlights are on. Always red.
        /// </summary>
        [FormerlySerializedAs("rearLights")]
        [Tooltip("    Rear Lights that will light up when headlights are on. Always red.")]
        public VehicleLight tailLights = new VehicleLight();

        private bool _leftBlinkerWasOn;
        private bool _rightBlinkerWasOn;

        /// <summary>
        ///     State in which blinker is at the moment.
        /// </summary>
        public bool BlinkerState
        {
            get { return (int) (Time.realtimeSinceStartup * 2) % 2 == 0; }
        }


        public override void Initialize()
        {
            initialized = true;
        }

        public override void FixedUpdate()
        {
        }

        public override void Update()
        {
            if (!Active)
            {
                return;
            }

            if (IsEnabled && vc != null)
            {
                // Stop lights
                if (brakeLights != null)
                {
                    if (vc.brakes.IsBraking)
                    {
                        brakeLights.TurnOn();
                    }
                    else
                    {
                        brakeLights.TurnOff();
                    }
                }

                // Reverse lights
                if (reverseLights != null)
                {
                    if (vc.powertrain.transmission.Gear < 0)
                    {
                        reverseLights.TurnOn();
                    }
                    else
                    {
                        reverseLights.TurnOff();
                    }
                }

                // Lights
                if (tailLights != null && lowBeamLights != null)
                {
                    if (vc.input.LowBeamLights)
                    {
                        tailLights.TurnOn();
                        lowBeamLights.TurnOn();
                    }
                    else
                    {
                        tailLights.TurnOff();
                        lowBeamLights.TurnOff();

                        if (highBeamLights != null && highBeamLights.On)
                        {
                            highBeamLights.TurnOff();
                            vc.input.HighBeamLights = false;
                        }
                    }
                }

                // Full beam lights
                if (highBeamLights != null)
                {
                    if (vc.input.HighBeamLights)
                    {
                        highBeamLights.TurnOn();

                        if (lowBeamLights != null && tailLights != null && !vc.input.LowBeamLights)
                        {
                            lowBeamLights.TurnOn();
                            tailLights.TurnOn();
                            vc.input.LowBeamLights = true;
                        }
                    }
                    else
                    {
                        highBeamLights.TurnOff();
                    }
                }

                if (leftBlinkers != null && rightBlinkers != null)
                {
                    // Left blinker lights
                    if (vc.input.LeftBlinker)
                    {
                        if (vc.input.RightBlinker && _rightBlinkerWasOn && !_leftBlinkerWasOn)
                        {
                            vc.input.RightBlinker = false;
                            rightBlinkers.TurnOff();
                        }
                        else
                        {
                            if (BlinkerState)
                            {
                                leftBlinkers.TurnOn();
                            }
                            else
                            {
                                leftBlinkers.TurnOff();
                            }

                            //leftBlinkers.On = true;
                        }
                    }

                    // Right blinker lights
                    if (vc.input.RightBlinker)
                    {
                        if (vc.input.LeftBlinker && _leftBlinkerWasOn && !_rightBlinkerWasOn)
                        {
                            vc.input.LeftBlinker = false;
                            leftBlinkers.TurnOff();
                        }
                        else
                        {
                            if (BlinkerState)
                            {
                                rightBlinkers.TurnOn();
                            }
                            else
                            {
                                rightBlinkers.TurnOff();
                            }

                            //rightBlinkers.On = true;
                        }
                    }

                    _leftBlinkerWasOn = vc.input.LeftBlinker;
                    _rightBlinkerWasOn = vc.input.RightBlinker;


                    // Hazards
                    if (vc.input.HazardLights)
                    {
                        if (BlinkerState)
                        {
                            leftBlinkers.TurnOn();
                            rightBlinkers.TurnOn();
                        }
                        else
                        {
                            leftBlinkers.TurnOff();
                            rightBlinkers.TurnOff();
                        }
                    }
                    else
                    {
                        if (!vc.input.LeftBlinker)
                        {
                            leftBlinkers.TurnOff();
                        }

                        if (!vc.input.RightBlinker)
                        {
                            rightBlinkers.TurnOff();
                        }
                    }
                }

                // Extra lights
                if (extraLights != null)
                {
                    if (vc.input.ExtraLights)
                    {
                        if (extraLights.On)
                        {
                            rightBlinkers.TurnOff();
                        }
                        else
                        {
                            rightBlinkers.TurnOn();
                        }
                    }
                }
            }
        }

        public override void Disable()
        {
            base.Disable();

            TurnOffAllLights();
        }

        /// <summary>
        ///     Retruns light states as a byte with each bit representing one light;
        /// </summary>
        /// <returns></returns>
        public byte GetByteState()
        {
            byte state = 0;

            if (brakeLights.On)
            {
                state |= 1 << 0;
            }

            if (tailLights.On)
            {
                state |= 1 << 1;
            }

            if (reverseLights.On)
            {
                state |= 1 << 2;
            }

            if (lowBeamLights.On)
            {
                state |= 1 << 3;
            }

            if (highBeamLights.On)
            {
                state |= 1 << 4;
            }

            if (leftBlinkers.On)
            {
                state |= 1 << 5;
            }

            if (rightBlinkers.On)
            {
                state |= 1 << 6;
            }

            if (extraLights.On)
            {
                state |= 1 << 7;
            }

            return state;
        }


        /// <summary>
        ///     Sets state of lights from a single byte where each bit represents one light.
        ///     To be used with GetByteState().
        /// </summary>
        /// <param name="state"></param>
        public void SetByteState(byte state)
        {
            if ((state & (1 << 0)) != 0)
            {
                brakeLights.TurnOn();
            }
            else
            {
                brakeLights.TurnOff();
            }

            if ((state & (1 << 1)) != 0)
            {
                tailLights.TurnOn();
            }
            else
            {
                tailLights.TurnOff();
            }

            if ((state & (1 << 2)) != 0)
            {
                reverseLights.TurnOn();
            }
            else
            {
                reverseLights.TurnOff();
            }

            if ((state & (1 << 3)) != 0)
            {
                lowBeamLights.TurnOn();
            }
            else
            {
                lowBeamLights.TurnOff();
            }

            if ((state & (1 << 4)) != 0)
            {
                highBeamLights.TurnOn();
            }
            else
            {
                highBeamLights.TurnOff();
            }

            if ((state & (1 << 5)) != 0)
            {
                leftBlinkers.TurnOn();
            }
            else
            {
                leftBlinkers.TurnOff();
            }

            if ((state & (1 << 6)) != 0)
            {
                rightBlinkers.TurnOn();
            }
            else
            {
                rightBlinkers.TurnOff();
            }
        }

        /// <summary>
        ///     Sets state of lights from a single byte where each bit represents one light.
        ///     To be used with GetByteState().
        /// </summary>
        /// <param name="state"></param>
        public void SetStatesFromByte(byte state)
        {
            if ((state & (1 << 0)) != 0)
            {
                brakeLights.TurnOn();
            }
            else
            {
                brakeLights.TurnOff();
            }

            if ((state & (1 << 1)) != 0)
            {
                tailLights.TurnOn();
            }
            else
            {
                tailLights.TurnOff();
            }

            if ((state & (1 << 2)) != 0)
            {
                reverseLights.TurnOn();
            }
            else
            {
                reverseLights.TurnOff();
            }

            if ((state & (1 << 3)) != 0)
            {
                lowBeamLights.TurnOn();
            }
            else
            {
                lowBeamLights.TurnOff();
            }

            if ((state & (1 << 4)) != 0)
            {
                highBeamLights.TurnOn();
            }
            else
            {
                highBeamLights.TurnOff();
            }

            if ((state & (1 << 5)) != 0)
            {
                leftBlinkers.TurnOn();
            }
            else
            {
                leftBlinkers.TurnOff();
            }

            if ((state & (1 << 6)) != 0)
            {
                rightBlinkers.TurnOn();
            }
            else
            {
                rightBlinkers.TurnOff();
            }

            if ((state & (1 << 7)) != 0)
            {
                extraLights.TurnOn();
            }
            else
            {
                extraLights.TurnOff();
            }
        }

        /// <summary>
        ///     Turns off all lights and emission on all meshes.
        /// </summary>
        public void TurnOffAllLights()
        {
            brakeLights.TurnOff();
            lowBeamLights.TurnOff();
            tailLights.TurnOff();
            reverseLights.TurnOff();
            highBeamLights.TurnOff();
            leftBlinkers.TurnOff();
            rightBlinkers.TurnOff();
            extraLights.TurnOff();
        }
    }
}