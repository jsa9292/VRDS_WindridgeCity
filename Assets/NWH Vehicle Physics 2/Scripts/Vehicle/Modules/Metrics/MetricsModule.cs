using System;
using UnityEngine;

namespace NWH.VehiclePhysics2.Modules.Metrics
{
    /// <summary>
    ///     Class for holding metrics such as odometer, top speed and drift time.
    /// </summary>
    [Serializable]
    public class MetricsModule : VehicleModule
    {
        public Metric averageSpeed = new Metric();
        public Metric continousDriftDistance = new Metric();
        public Metric continousDriftTime = new Metric();
        public Metric odometer = new Metric();
        public Metric topSpeed = new Metric();
        public Metric totalDriftDistance = new Metric();
        public Metric totalDriftTime = new Metric();

        private float driftEndTime;
        private float driftTimeout = 0.75f;

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

            bool hasWheelSkid = vc.powertrain.HasWheelSkid;
            float realtimeSinceStartup = Time.realtimeSinceStartup;

            // Odometer
            odometer.Update(delegate { return vc.Speed * Time.fixedDeltaTime; }, true);

            // Top speed
            topSpeed.Update(
                delegate
                {
                    if (vc.Speed > topSpeed.value)
                    {
                        return vc.Speed;
                    }

                    return topSpeed.value;
                }, false);

            // Average speed
            averageSpeed.Update(
                delegate { return odometer.value / realtimeSinceStartup; }, false);

            // Total drift time
            totalDriftTime.Update(
                delegate
                {
                    if (hasWheelSkid)
                    {
                        return vc.fixedDeltaTime;
                    }

                    return 0;
                }, true);

            // Continous drift time
            continousDriftTime.Update(
                delegate
                {
                    if (hasWheelSkid)
                    {
                        driftEndTime = realtimeSinceStartup;
                        return vc.fixedDeltaTime;
                    }

                    if (realtimeSinceStartup < driftEndTime + driftTimeout)
                    {
                        return vc.fixedDeltaTime;
                    }

                    return -continousDriftTime.value;
                }, true);

            // Total drift distance
            totalDriftDistance.Update(
                delegate
                {
                    if (hasWheelSkid)
                    {
                        return vc.fixedDeltaTime * vc.Speed;
                    }

                    return 0;
                }, true);

            // Continous drift distance
            continousDriftDistance.Update(
                delegate
                {
                    if (hasWheelSkid)
                    {
                        driftEndTime = realtimeSinceStartup;
                        return vc.fixedDeltaTime * vc.Speed;
                    }

                    if (realtimeSinceStartup < driftEndTime + driftTimeout)
                    {
                        return vc.fixedDeltaTime * vc.Speed;
                    }

                    return -continousDriftDistance.value;
                }, true);
        }

        public override ModuleCategory GetModuleCategory()
        {
            return ModuleCategory.Vehicle;
        }

        [Serializable]
        public class Metric
        {
            public delegate float UpdateDelegate();

            public float value;

            public void Update(UpdateDelegate del, bool increment)
            {
                if (increment)
                {
                    value += del();
                }
                else
                {
                    value = del();
                }
            }

            public void Reset()
            {
                value = 0;
            }
        }
    }
}