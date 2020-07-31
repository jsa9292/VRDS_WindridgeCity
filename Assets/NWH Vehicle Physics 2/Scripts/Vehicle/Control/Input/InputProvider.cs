using System.Collections.Generic;
using UnityEngine;

namespace NWH.VehiclePhysics2.Input
{
    /// <summary>
    ///     Base abstract class from which all input providers inherit.
    /// </summary>
    public abstract class InputProvider : MonoBehaviour
    {
        /// <summary>
        /// List of all InputProviders in the scene.
        /// </summary>
        public static List<InputProvider> Instances = new List<InputProvider>();

        public void Awake()
        {
            Instances.Add(this);
        }

        public abstract bool EngineStartStop();
        public abstract bool ChangeCamera();
        public abstract bool ChangeVehicle();
        public abstract float Clutch();
        public abstract bool ExtraLights();
        public abstract bool HighBeamLights();
        public abstract float Handbrake();
        public abstract bool HazardLights();
        public abstract float Horizontal();
        public abstract bool Horn();
        public abstract bool LeftBlinker();
        public abstract bool LowBeamLights();
        public abstract bool RightBlinker();
        public abstract bool ShiftDown();
        public abstract int ShiftInto();
        public abstract bool ShiftUp();
        public abstract bool TrailerAttachDetach();
        public abstract float Vertical();
        public abstract bool FlipOver();
        public abstract bool Boost();
        public abstract bool CruiseControl();
    }
}