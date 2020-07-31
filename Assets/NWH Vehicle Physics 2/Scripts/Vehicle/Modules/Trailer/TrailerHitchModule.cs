using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace NWH.VehiclePhysics2.Modules.Trailer
{
    /// <summary>
    ///     Module representing the towing vehicle.
    ///     When a trailer is instantiated after initialization, running SyncTrailers() manually is required for the script to
    ///     find trailers.
    /// </summary>
    [Serializable]
    public class TrailerHitchModule : VehicleModule
    {
        /// <summary>
        ///     Maximum distance between towing vehicle's attachment point and trailer's attachment point.
        /// </summary>
        [Tooltip("    Maximum distance between towing vehicle's attachment point and trailer's attachment point.")]
        public float attachDistanceThreshold = 0.5f;

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

        /// <summary>
        ///     If a trailer is in range when the scene is started it will be attached.
        /// </summary>
        [Tooltip("    If a trailer is in range when the scene is started it will be attached.")]
        public bool attachOnPlay;

        /// <summary>
        ///     Breaking force of the generated joint.
        /// </summary>
        [Tooltip("    Breaking force of the generated joint.")]
        public float breakForce = Mathf.Infinity;

        /// <summary>
        ///     Can the trailer be detached once it is attached?
        /// </summary>
        [Tooltip("    Can the trailer be detached once it is attached?")]
        public bool detachable = true;

        /// <summary>
        ///     Power reduction that will be applied when vehicle has no trailer to avoid wheel spin when controlled with a binary
        ///     controller.
        /// </summary>
        [Tooltip(
            "Power reduction that will be applied when vehicle has no trailer to avoid wheel spin when controlled with a binary controller.")]
        public float noTrailerPowerCoefficient = 1f;

        public UnityEvent onTrailerAttach;
        public UnityEvent onTrailerDetach;

        /// <summary>
        ///     Is trailer's attachment point close enough to be attached to the towing vehicle?
        /// </summary>
        [Tooltip("    Is trailer's attachment point close enough to be attached to the towing vehicle?")]
        public bool trailerInRange;

        /// <summary>
        ///     Use for articulated busses and equipment where rotation around vertical axis is not wanted.
        /// </summary>
        [Tooltip("    Use for articulated busses and equipment where rotation around vertical axis is not wanted.")]
        public bool useHingeJoint;

        [NonSerialized]
        private ConfigurableJoint _configurableJoint;

        [NonSerialized]
        private TrailerModule _nearestTrailerModule;

        /// <summary>
        ///     A trailer that is attached to this trailer hitch
        /// </summary>
        private TrailerModule _trailer;

        [NonSerialized]
        private List<TrailerModule> _trailerModules = new List<TrailerModule>();

        public TrailerModule Trailer
        {
            get { return _trailer; }
            set { _trailer = value; }
        }

        public override void Initialize()
        {
            FindSceneTrailerModules(ref _trailerModules);

            initialized = true;

            if (attachOnPlay)
            {
                AttachTrailer(vc);
            }
        }

        public override void FixedUpdate()
        {
            if (_trailer == null) // Check if already attached
            {
                if (attachmentPoint != null) // Check if attachment point exists
                {
                    if (_trailerModules.Count > 0)
                    {
                        _nearestTrailerModule = null;
                        trailerInRange = false;
                        foreach (TrailerModule trailerModule in _trailerModules)
                        {
                            if (trailerModule.Active)
                            {
                                Vector3 dir = trailerModule.attachmentPoint.transform.position -
                                              attachmentPoint.transform.position;
                                float dist = dir.sqrMagnitude;

                                if (dist < attachDistanceThreshold)
                                {
                                    _nearestTrailerModule = trailerModule;
                                    trailerInRange = true;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        _nearestTrailerModule = null;
                        trailerInRange = false;
                    }
                }
            }

            // Attach trailer
            if (_nearestTrailerModule != null && _trailer == null && vc.input.TrailerAttachDetach)
            {
                AttachTrailer(vc);
            }
            // Detach trailer
            else if (_trailer != null && vc.input.TrailerAttachDetach)
            {
                DetachTrailer(vc);
            }

            // Detach trailer if joint broke
            if (_trailer != null && _configurableJoint == null)
            {
                DetachTrailer(vc);
            }

            vc.input.ResetTrailerAttachDetachFlag();
        }

        public override void Update()
        {
            if (_trailer != null && _trailer.VehicleController != null && _trailer.VehicleController.input.Active)
            {
                _trailer.VehicleController.input.states = vc.input.states;
            }
        }

        public override void Enable()
        {
            base.Enable();

            if (vc != null)
            {
                vc.powertrain.engine.powerModifiers.Add(NoTrailerPowerModifier);
            }
        }

        public override void Disable()
        {
            base.Disable();

            if (vc != null)
            {
                vc.powertrain.engine.powerModifiers.Remove(NoTrailerPowerModifier);
            }
        }

        public override ModuleCategory GetModuleCategory()
        {
            return ModuleCategory.Trailer;
        }

        public float NoTrailerPowerModifier()
        {
            if (!attached)
            {
                return 1f;
            }

            return noTrailerPowerCoefficient;
        }

        /// <summary>
        ///     Updates the list of trailers in the scene.
        ///     Should be called each time a new trailer is instantiated if 'autoSyncTrailers' is turned off.
        /// </summary>
        public void SyncTrailers()
        {
            FindSceneTrailerModules(ref _trailerModules);
        }

        private void AttachTrailer(VehicleController vc)
        {
            if (_nearestTrailerModule != null)
            {
                _trailer = _nearestTrailerModule;
                if (_trailer == null)
                {
                    return;
                }

                VehicleController trailerVc = _trailer.VehicleController;

                // Position trailer
                trailerVc.vehicleTransform.position = trailerVc.transform.position -
                                                      (_trailer.attachmentPoint.transform.position -
                                                       attachmentPoint.transform.position);

                // Configure joint
                _configurableJoint = trailerVc.gameObject.GetComponent<ConfigurableJoint>();
                if (_configurableJoint == null)
                {
                    _configurableJoint = trailerVc.gameObject.AddComponent<ConfigurableJoint>();
                }

                _configurableJoint.connectedBody = vc.vehicleRigidbody;
                _configurableJoint.anchor =
                    trailerVc.transform.InverseTransformPoint(attachmentPoint.transform.position);
                _configurableJoint.xMotion = ConfigurableJointMotion.Locked;
                _configurableJoint.yMotion = ConfigurableJointMotion.Locked;
                _configurableJoint.zMotion = ConfigurableJointMotion.Locked;
                _configurableJoint.angularZMotion =
                    useHingeJoint ? ConfigurableJointMotion.Locked : ConfigurableJointMotion.Free;
                _configurableJoint.enableCollision = true;
                _configurableJoint.breakForce = breakForce;

                // Reset input flag
                vc.input.ResetTrailerAttachDetachFlag();

                // Enable lights if tractor has them enabled
                if (vc.effectsManager.lightsManager.IsEnabled)
                {
                    trailerVc.effectsManager.lightsManager.Enable();
                }
                else
                {
                    trailerVc.effectsManager.lightsManager.Disable();
                }

                attached = true;

                _trailer.OnAttach(this);

                onTrailerAttach.Invoke();
            }
        }


        private void DetachTrailer(VehicleController vc)
        {
            if (!detachable || _trailer == null || _trailer.VehicleController == null)
            {
                return;
            }

            attached = false;

            if (_configurableJoint != null)
            {
                Object.Destroy(_configurableJoint);
                _configurableJoint = null;
            }

            _trailer.OnDetach();
            _trailer = null;

            vc.input.ResetTrailerAttachDetachFlag();

            onTrailerDetach.Invoke();
        }

        private void FindSceneTrailerModules(ref List<TrailerModule> trailerModules)
        {
            trailerModules = new List<TrailerModule>();

            foreach (TrailerModuleWrapper trailerModuleWrapper in Object.FindObjectsOfType<TrailerModuleWrapper>())
            {
                TrailerModule trailerModule = trailerModuleWrapper.module;
                if (trailerModule != null && vc.gameObject.scene == SceneManager.GetActiveScene())
                {
                    trailerModules.Add(trailerModule);
                }
            }
        }
    }
}