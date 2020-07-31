using UnityEngine;

namespace NWH.VehiclePhysics2.Utility
{
    public class HysteresisSmoothedValue
    {
        private readonly float _fallTime = 1;
        private readonly float _riseTime = 1;

        private float _velocity;

        public HysteresisSmoothedValue(float initial, float riseTime, float fallTime)
        {
            Value = initial;
            _riseTime = riseTime;
            _fallTime = fallTime;
        }

        public float Value { get; private set; }

        public void Tick(float target)
        {
            float speed = target < Value ? _fallTime : _riseTime;
            Value = Mathf.SmoothDamp(Value, target, ref _velocity, speed);
        }
    }
}