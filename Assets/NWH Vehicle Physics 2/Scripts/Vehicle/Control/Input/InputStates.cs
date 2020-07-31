using System;
using UnityEngine;

namespace NWH.VehiclePhysics2.Input
{
    /// <summary>
    ///     Struct for storing input states of the vehicle.
    ///     Allows for input to be copied between the vehicles.
    /// </summary>
    [Serializable]
    public struct InputStates
    {
        [Range(0f, 1f)]
        public float clutch;

        public bool engineStartStop;
        public bool extraLights;
        public bool highBeamLights;

        [Range(0f, 1f)]
        public float handbrake;

        public bool hazardLights;

        [Range(-1f, 1f)]
        public float horizontal;

        public bool horn;
        public bool leftBlinker;
        public bool lowBeamLights;
        public bool rightBlinker;
        public bool shiftDown;
        public int shiftInto;
        public bool shiftUp;
        public bool trailerAttachDetach;
        public bool cruiseControl;
        public bool boost;
        public bool flipOver;

        [Range(-1f, 1f)]
        public float vertical;

        public void Reset()
        {
            horizontal = 0;
            vertical = 0;
            clutch = 0;
            handbrake = 0;
            shiftUp = false;
            shiftDown = false;
            shiftInto = -999;
            leftBlinker = false;
            rightBlinker = false;
            lowBeamLights = false;
            highBeamLights = false;
            hazardLights = false;
            extraLights = false;
            trailerAttachDetach = false;
            horn = false;
            engineStartStop = false;
            cruiseControl = false;
            boost = false;
            flipOver = false;
        }
    }
}