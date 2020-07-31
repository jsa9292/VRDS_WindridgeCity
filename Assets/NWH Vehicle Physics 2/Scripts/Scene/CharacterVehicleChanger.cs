using System.Collections.Generic;
using System.Linq;
using NWH.VehiclePhysics2;
using NWH.VehiclePhysics2.Input;
using NWH.VehiclePhysics2.SceneManagement;
using UnityEngine;
using UnityEngine.Serialization;

namespace NWH.VehiclePhysics2
{
    /// <summary>
    ///     Allows character to enter or exit vehicle. Can be used with any first or 3rd person object.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(VehicleChanger))]
    public class CharacterVehicleChanger : MonoBehaviour
    {
        public static CharacterVehicleChanger Instance;

        /// <summary>
        ///     Game object representing a character. Can also be another vehicle.
        /// </summary>
        [FormerlySerializedAs("characterControllerObject")]
        [Tooltip("    Game object representing a character. Can also be another vehicle.")]
        public GameObject characterObject;

        /// <summary>
        ///     Maximum distance at which the character will be able to enter the vehicle.
        /// </summary>
        [Range(0.2f, 3f)]
        [Tooltip("    Maximum distance at which the character will be able to enter the vehicle.")]
        public float enterDistance = 2f;

        /// <summary>
        ///     Tag of the object representing the point from which the enter distance will be measured. Useful if you want to
        ///     enable you character to enter only when near the door.
        /// </summary>
        [Tooltip(
            "Tag of the object representing the point from which the enter distance will be measured. Useful if you want to enable you character to enter only when near the door.")]
        public string enterExitTag = "EnterExitPoint";

        /// <summary>
        ///     Maximum speed at which the character will be able to enter / exit the vehicle.
        /// </summary>
        [Tooltip("    Maximum speed at which the character will be able to enter / exit the vehicle.")]
        public float maxEnterExitVehicleSpeed = 2f;

        /// <summary>
        ///     True when character can enter the vehicle.
        /// </summary>
        [Tooltip("    True when character can enter the vehicle.")]
        public bool nearVehicle;

        private bool _insideVehicle;
        private GameObject _nearestEnterExitObject;
        private VehicleController _nearestVehicle;
        private Vector3 _relativeEnterPosition;
        private GameObject[] _enterExitPoints;
        private GameObject _nearestEnterExitPoint;

        public CharacterVehicleChanger(VehicleController nearestVehicle)
        {
            this._nearestVehicle = nearestVehicle;
        }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            if (enabled)
            {
                VehicleChanger.Instance.characterBased = true;
                VehicleChanger.Instance.deactivateAll = true;
            }
        }

        private void Update()
        {
            if (!_insideVehicle)
            {
                nearVehicle = false;

                if (!characterObject.activeSelf)
                {
                    characterObject.SetActive(true);
                }

                _enterExitPoints = GameObject.FindGameObjectsWithTag(enterExitTag);
                _nearestEnterExitPoint = null;
                float nearestSqrDist = Mathf.Infinity;
                foreach (GameObject eep in _enterExitPoints)
                {
                    float sqrDist = Vector3.SqrMagnitude(characterObject.transform.position - eep.transform.position);
                    if (sqrDist < nearestSqrDist)
                    {
                        nearestSqrDist = sqrDist;
                        _nearestEnterExitPoint = eep;
                    }
                }

                if (_nearestEnterExitPoint == null)
                {
                    return;
                }

                if (Vector3.Magnitude(Vector3.ProjectOnPlane(
                    _nearestEnterExitPoint.transform.position - characterObject.transform.position,
                    Vector3.up)) < enterDistance)
                {
                    nearVehicle = true;
                    _nearestVehicle = _nearestEnterExitPoint.GetComponentInParent<VehicleController>();
                }
            }
            
            bool any = false;
            foreach (InputProvider i in InputProvider.Instances)
            {
                if (i.ChangeVehicle())
                {
                    any = true;
                    break;
                }
            }

            if (InputProvider.Instances.Count > 0 && any)
            {
                EnterExitVehicle();
            }
        }

        public void EnterExitVehicle()
        {
            // Enter vehicle
            if (nearVehicle && !_insideVehicle && _nearestVehicle.Speed < maxEnterExitVehicleSpeed)
            {
                characterObject.SetActive(false);
                VehicleChanger.Instance.deactivateAll = false;
                _relativeEnterPosition =
                    _nearestVehicle.transform.InverseTransformPoint(characterObject.transform.position);
                _insideVehicle = true;
                VehicleChanger.Instance.ChangeVehicle(_nearestVehicle);
                nearVehicle = false;
            }
            // Exit vehicle
            else if (_insideVehicle && _nearestVehicle.Speed < maxEnterExitVehicleSpeed)
            {
                VehicleChanger.Instance.DeactivateAllIncludingActive(); // Call deactivate all to deactivate on the same frame, preventing 2 audio listeners warning.
                VehicleChanger.Instance.deactivateAll = true;
                _insideVehicle = false;
                characterObject.transform.position =
                    _nearestVehicle.transform.TransformPoint(_relativeEnterPosition);
                characterObject.SetActive(true);
            }
        }
    }
}