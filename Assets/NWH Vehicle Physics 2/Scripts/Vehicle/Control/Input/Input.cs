using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NWH.VehiclePhysics2.Input
{
    /// <summary>
    ///     Manages vehicle input by retrieving it from the active InputProvider and filling in the InputStates with the
    ///     fetched data.
    /// </summary>
    [Serializable]
    public class Input : VehicleComponent
    {
        public enum ThrottleType
        {
            WForwardSReverse,
            WForwardWReverse
        }

        public bool autoSettable = true;
        public InputStates states;
        public ThrottleType throttleType = ThrottleType.WForwardSReverse;
        private List<InputProvider>  _inputProviders = new List<InputProvider>();

        private delegate bool BinaryInputDelegate();

        public float Clutch
        {
            get { return states.clutch; }
            set { states.clutch = value < 0f ? 0f : value > 1f ? 1f : value; }
        }

        public bool EngineStartStop
        {
            get { return states.engineStartStop; }
            set { states.engineStartStop = value || states.engineStartStop; }
        }

        public bool ExtraLights
        {
            get { return states.extraLights; }
            set { states.extraLights = value; }
        }

        public bool HighBeamLights
        {
            get { return states.highBeamLights; }
            set { states.highBeamLights = value; }
        }

        public float Handbrake
        {
            get { return states.handbrake; }
            set { states.handbrake = value < 0f ? 0f : value > 1f ? 1f : value; }
        }

        public bool HazardLights
        {
            get { return states.hazardLights; }
            set { states.hazardLights = value; }
        }

        public float Horizontal
        {
            get { return states.horizontal; }
            set { states.horizontal = value < -1f ? -1f : value > 1f ? 1f : value; }
        }

        public bool Horn
        {
            get { return states.horn; }
            set { states.horn = value; }
        }

        public bool LeftBlinker
        {
            get { return states.leftBlinker; }
            set { states.leftBlinker = value; }
        }

        public bool LowBeamLights
        {
            get { return states.lowBeamLights; }
            set { states.lowBeamLights = value; }
        }

        public bool RightBlinker
        {
            get { return states.rightBlinker; }
            set { states.rightBlinker = value; }
        }

        public bool ShiftDown
        {
            get { return states.shiftDown; }
            set { states.shiftDown = value || states.shiftDown; }
        }

        public int ShiftInto
        {
            get { return states.shiftInto; }
            set { states.shiftInto = value; }
        }

        public bool ShiftUp
        {
            get { return states.shiftUp; }
            set { states.shiftUp = value || states.shiftUp; }
        }

        public bool TrailerAttachDetach
        {
            get { return states.trailerAttachDetach; }
            set { states.trailerAttachDetach = value || states.trailerAttachDetach; }
        }

        public float Vertical
        {
            get { return states.vertical; }
            set { states.vertical = value < -1f ? -1f : value > 1f ? 1f : value; }
        }

        public bool CruiseControl
        {
            get { return states.cruiseControl; }
            set { states.cruiseControl = value; }
        }
        
        public bool Boost
        {
            get { return states.boost; }
            set { states.boost = value; }
        }

        public bool FlipOver
        {
            get { return states.flipOver; }
            set { states.flipOver = value; }
        }

        public override void Initialize()
        {
            _inputProviders = InputProvider.Instances;

            if (_inputProviders == null || _inputProviders.Count == 0)
            {
                Debug.LogWarning(
                    "No InputProviders are present in the scene. Make sure that one or more InputProviders are present (DesktopInputProvider, MobileInputProvider, etc.).");
                return;
            }

            initialized = true;
        }

        public void ResetEngineStartStopFlag()
        {
            states.engineStartStop = false;
        }

        public override void FixedUpdate()
        {
        }

        public override void Update()
        {
            if (!Active || !autoSettable)
            {
                return;
            }

            Horizontal = GetHorizontal();
            Vertical = GetVertical();
            Clutch = GetClutch();
            Handbrake = GetHandbrake();
                        
            ShiftInto = GetShiftInto();

            // Avoid using Linq Any as it causes GC in this case
            bool any = false;
            foreach (InputProvider i in _inputProviders)
            {
                if (i.ShiftUp())
                {
                    any = true;
                    break;
                }
            }
            ShiftUp = any;
            
            any = false;
            foreach (InputProvider i in _inputProviders)
            {
                if (i.ShiftDown())
                {
                    any = true;
                    break;
                }
            }
            ShiftDown = any;

            any = false;
            foreach (InputProvider i in _inputProviders)
            {
                if (i.LeftBlinker())
                {
                    any = true;
                    break;
                }
            }
            LeftBlinker = any ? !LeftBlinker : LeftBlinker;

            any = false;
            foreach (InputProvider i in _inputProviders)
            {
                if (i.RightBlinker())
                {
                    any = true;
                    break;
                }
            }
            RightBlinker = any ? !RightBlinker : RightBlinker;

            any = false;
            foreach (InputProvider i in _inputProviders)
            {
                if (i.LowBeamLights())
                {
                    any = true;
                    break;
                }
            }
            LowBeamLights = any ? !LowBeamLights : LowBeamLights;
            
            any = false;
            foreach (InputProvider i in _inputProviders)
            {
                if (i.HighBeamLights())
                {
                    any = true;
                    break;
                }
            }
            HighBeamLights =  any ? !HighBeamLights : HighBeamLights;
            
            any = false;
            foreach (InputProvider i in _inputProviders)
            {
                if (i.HazardLights())
                {
                    any = true;
                    break;
                }
            }
            HazardLights = any ? !HazardLights : HazardLights;
            
            any = false;
            foreach (InputProvider i in _inputProviders)
            {
                if (i.ExtraLights())
                {
                    any = true;
                    break;
                }
            }
            ExtraLights = any ? !ExtraLights : ExtraLights;
            
            any = false;
            foreach (InputProvider i in _inputProviders)
            {
                if (i.TrailerAttachDetach())
                {
                    any = true;
                    break;
                }
            }
            TrailerAttachDetach = any;

            any = false;
            foreach (InputProvider i in _inputProviders)
            {
                if (i.Horn())
                {
                    any = true;
                    break;
                }
            }
            Horn = any;
            
            any = false;
            foreach (InputProvider i in _inputProviders)
            {
                if (i.EngineStartStop())
                {
                    any = true;
                    break;
                }
            }
            EngineStartStop = any;
            
            any = false;
            foreach (InputProvider i in _inputProviders)
            {
                if (i.CruiseControl())
                {
                    any = true;
                    break;
                }
            }
            CruiseControl = any;
            
            any = false;
            foreach (InputProvider i in _inputProviders)
            {
                if (i.Boost())
                {
                    any = true;
                    break;
                }
            }
            Boost = any;
            
            any = false;
            foreach (InputProvider i in _inputProviders)
            {
                if (i.FlipOver())
                {
                    any = true;
                    break;
                }
            }
            FlipOver = any;
        }

        int GetShiftInto()
        {
            foreach (InputProvider ip in _inputProviders)
            {
                if (ip.ShiftInto() > -998)
                {
                    return ip.ShiftInto();
                }
            }
            
            return -999;
        }

        float GetHorizontal()
        {
            float value = 0;
            foreach (InputProvider ip in _inputProviders)
            {
                value += ip.Horizontal();
            }
            return value;
        }
        
        float GetVertical()
        {
            float value = 0;
            foreach (InputProvider ip in _inputProviders)
            {
                value += ip.Vertical();
            }
            return value;
        }
        
        float GetClutch()
        {
            float value = 0;
            foreach (InputProvider ip in _inputProviders)
            {
                value += ip.Clutch();
            }
            return value;
        }
        
        float GetHandbrake()
        {
            float value = 0;
            foreach (InputProvider ip in _inputProviders)
            {
                value += ip.Handbrake();
            }
            return value;
        }

        public override void Disable()
        {
            base.Disable();
            states.Reset();
        }

        public void ResetShiftDownFlag()
        {
            states.shiftDown = false;
        }

        public void ResetShiftUpFlag()
        {
            states.shiftUp = false;
        }

        public void ResetTrailerAttachDetachFlag()
        {
            states.trailerAttachDetach = false;
        }
    }
}