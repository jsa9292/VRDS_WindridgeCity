using System;
using UnityEngine;
using UnityEngine.Events;

namespace NWH.VehiclePhysics2.Modules.Trailer
{
    [Serializable]
    public class TrailerModule : VehicleModule
    {
        /// <summary>
        ///     True if object is trailer and is attached to a towing vehicle and also true if towing vehicle and has trailer
        ///     attached.
        /// </summary>
        [Tooltip(
            "True if object is trailer and is attached to a towing vehicle and also true if towing vehicle and has trailer\r\nattached.")]
        public bool attached;

        /// <summary>
        ///     If the vehicle is a trailer, this is the object placed at the point at which it will connect to the towing vehicle.
        ///     If the vehicle is towing, this is the object placed at point at which trailer will be coneected.
        /// </summary>
        [Tooltip(
            "If the vehicle is a trailer, this is the object placed at the point at which it will connect to the towing vehicle." +
            " If the vehicle is towing, this is the object placed at point at which trailer will be coneected.")]
        public Transform attachmentPoint;

        public UnityEvent onAttach;
        public UnityEvent onDetach;

        /// <summary>
        ///     Should the trailer input states be reset when trailer is detached?
        /// </summary>
        [Tooltip("    Should the trailer input states be reset when trailer is detached?")]
        public bool resetInputStatesOnDetach = true;

        /// <summary>
        ///     If enabled the trailer will keep in same gear as the tractor, assuming powertrain on trailer is enabled.
        /// </summary>
        [Tooltip(
            "If enabled the trailer will keep in same gear as the tractor, assuming powertrain on trailer is enabled.")]
        public bool synchronizeGearShifts = true;

        /// <summary>
        ///     Object that will be disabled when trailer is attached and disabled when trailer is detached.
        /// </summary>
        [Tooltip("    Object that will be disabled when trailer is attached and disabled when trailer is detached.")]
        public GameObject trailerStand;

        [NonSerialized]
        private TrailerHitchModule _trailerHitch;

        public TrailerHitchModule TrailerHitch
        {
            get { return _trailerHitch; }
            set { _trailerHitch = value; }
        }

        public override void Initialize()
        {
            initialized = true;
            vc.input.autoSettable = false;
        }

        public override void FixedUpdate()
        {
            if (Active && attached && _trailerHitch != null)
            {
                vc.powertrain.transmission.Gear = vc.powertrain.transmission.Gear;
            }
        }

        public override void Update()
        {
        }

        public override ModuleCategory GetModuleCategory()
        {
            return ModuleCategory.Trailer;
        }

        public void OnAttach(TrailerHitchModule trailerHitch)
        {
            _trailerHitch = trailerHitch;

            vc.Wake();

            vc.input.autoSettable = false;

            // Raise trailer stand
            if (trailerStand != null)
            {
                trailerStand.SetActive(false);
            }

            attached = true;

            onAttach.Invoke();
        }

        public void OnDetach()
        {
            if (resetInputStatesOnDetach)
            {
                vc.input.states.Reset();
            }

            vc.input.autoSettable = false;


            // Lower trailer stand
            if (trailerStand != null)
            {
                trailerStand.SetActive(true);
            }

            // Assume trailer does not have a power source, cut lights.
            vc.effectsManager.lightsManager.Disable();

            _trailerHitch = null;
            vc.Sleep();

            attached = false;

            onDetach.Invoke();
        }
    }
}