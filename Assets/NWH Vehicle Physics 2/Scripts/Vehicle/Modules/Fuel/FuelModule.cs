using System;
using NWH.VehiclePhysics2.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace NWH.VehiclePhysics2.Modules.Fuel
{
    /// <summary>
    ///     Module for simulating the fuel system in a vehicle.
    ///     Prevents engine from running when out of fuel.
    /// </summary>
    [Serializable]
    public class FuelModule : VehicleModule
    {
        /// <summary>
        ///     Maximum amount in liters that the fuel tank can hold.
        /// </summary>
        [Tooltip("    Maximum amount in liters that the fuel tank can hold.")]
        public float amount = 50f;

        /// <summary>
        ///     Fuel capacity in liters.
        /// </summary>
        [Tooltip("    Fuel capacity in liters.")]
        public float capacity = 50f;

        /// <summary>
        ///     In case you do not need physically accurate fuel consumption you can lower/rise the consumption in here.
        /// </summary>
        [Tooltip(
            "In case you do not need physically accurate fuel consumption you can lower/rise the consumption in here.")]
        public float consumptionMultiplier = 1f;

        /// <summary>
        ///     Engine efficiency (in percent). 1 would mean that all the energy contained in fuel would go into output power.
        /// </summary>
        [Tooltip(
            "Engine efficiency (in percent). 1 would mean that all the energy contained in fuel would go into output power.")]
        public float efficiency = 0.45f;

        /// <summary>
        ///     Consumption when idling indicated in percentage of max consumption. 0.05f = 5% out of maximum.
        /// </summary>
        [Tooltip("    Consumption when idling indicated in percentage of max consumption. 0.05f = 5% out of maximum.")]
        public float idleConsumption = 0.1f;

        public float maxConsumptionPerHour = 20f;

        /// <summary>
        ///     Called when vehicle runs out of fuel.
        /// </summary>
        [Tooltip("    Called when vehicle runs out of fuel.")]
        public UnityEvent onOutOfFuel;

        private float _consumptionThisFrame;
        private float _distanceTraveled;

        private float _prevAmount;

        [SerializeField]
        private float consumptionLPer100km;

        [SerializeField]
        private float consumptionPerHour;

        /// <summary>
        ///     Fuel consumption in kilometers per liter.
        /// </summary>
        public float ConsumptionKilometersPerLiter
        {
            get { return UnitConverter.L100kmToKml(consumptionLPer100km); }
        }

        /// <summary>
        ///     Fuel consumption in liters per 100 kilometers.
        /// </summary>
        public float ConsumptionLitersPer100Kilometers
        {
            get { return consumptionLPer100km; }
        }

        /// <summary>
        ///     Fuel consumption in liters per second.
        /// </summary>
        public float ConsumptionLitersPerSecond
        {
            get { return consumptionPerHour / 3600f; }
        }

        /// <summary>
        ///     Fuel consumption in miles per galon.
        /// </summary>
        public float ConsumptionMPG
        {
            get { return UnitConverter.L100kmToMpg(consumptionLPer100km); }
        }

        /// <summary>
        ///     Percentage of fuel from the max amount the tank can hold.
        /// </summary>
        public float FuelPercentage
        {
            get { return Mathf.Clamp01(amount / capacity); }
        }

        // HP EM 10 2858267 HR

        /// <summary>
        ///     True if has fuel or if use fuel is false.
        /// </summary>
        public bool HasFuel
        {
            get
            {
                if (!Active)
                {
                    return true;
                }

                if (amount > 0)
                {
                    return true;
                }

                return false;
            }
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

            if (vc.powertrain.engine.IsRunning)
            {
                // Assuming fuel has 36 MJ/L. 1KWh = 3.6MJ. 1L = 36MJ = 10kWH.
                maxConsumptionPerHour = vc.powertrain.engine.maxPower / 10f * Mathf.Clamp01(1f - efficiency);

                consumptionPerHour = vc.powertrain.engine.generatedPower / vc.powertrain.engine.maxPower *
                                     maxConsumptionPerHour;
                consumptionPerHour =
                    Mathf.Clamp(consumptionPerHour, maxConsumptionPerHour * idleConsumption, Mathf.Infinity) *
                    consumptionMultiplier;

                // Reduce fuel amount until empty.
                amount -= consumptionPerHour / 3600 * vc.fixedDeltaTime;
                amount = Mathf.Clamp(amount, 0f, capacity);

                if (amount == 0 && vc.powertrain.engine.IsRunning)
                {
                    vc.powertrain.engine.Stop();
                }

                // Calculate consumption per distance for mpg, km/l and l/100km values.
                _distanceTraveled = vc.Speed * vc.fixedDeltaTime;
                _consumptionThisFrame = consumptionPerHour / 3600f * Time.fixedDeltaTime;

                float framesPerHour = 3600f / Time.fixedDeltaTime;
                float measuredConsPerHour = _consumptionThisFrame * framesPerHour;
                float measuredDistPerHour = _distanceTraveled * framesPerHour / 100000f;
                consumptionLPer100km = measuredDistPerHour == 0
                    ? 0
                    : Mathf.Clamp(measuredConsPerHour / measuredDistPerHour, 0f, 99.9f);
            }
            else
            {
                consumptionPerHour = 0f;
                consumptionLPer100km = 0;
            }

            if (amount == 0 && _prevAmount > 0)
            {
                onOutOfFuel.Invoke();
            }

            _prevAmount = amount;
        }

        public override void Enable()
        {
            base.Enable();
            _prevAmount = amount;
        }

        public override ModuleCategory GetModuleCategory()
        {
            return ModuleCategory.Powertrain;
        }
    }
}