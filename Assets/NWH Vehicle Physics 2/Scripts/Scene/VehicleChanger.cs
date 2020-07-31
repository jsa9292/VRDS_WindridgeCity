using System.Collections.Generic;
using System.Linq;
using NWH.VehiclePhysics2.Input;
using UnityEngine;

namespace NWH.VehiclePhysics2.SceneManagement
{
    public class VehicleChanger : MonoBehaviour
    {
        /// <summary>
        ///     Is vehicle changing character based? When true changing vehicles will require getting close to them
        ///     to be able to enter, opposed to pressing a button to switch between vehicles.
        /// </summary>
        [Tooltip(
            "Is vehicle changing character based? When true changing vehicles will require getting close to them\r\nto be able to enter, opposed to pressing a button to switch between vehicles.")]
        public bool characterBased;

        /// <summary>
        ///     Index of the current vehicle in vehicles list.
        /// </summary>
        [Tooltip("    Index of the current vehicle in vehicles list.")]
        public int currentVehicleIndex;

        /// <summary>
        ///     If true no vehicle will be active. Used when character controller has focus instead of vehicle controller.
        /// </summary>
        [Tooltip(
            "If true no vehicle will be active. Used when character controller has focus instead of vehicle controller.")]
        public bool deactivateAll;

        /// <summary>
        ///     Should the vehicles that the player is currently not using be put to sleep to improve performance?
        /// </summary>
        [Tooltip(
            "    Should the vehicles that the player is currently not using be put to sleep to improve performance?")]
        public bool putOtherVehiclesToSleep = true;

        /// <summary>
        ///     List of all of the vehicles that can be selected and driven in the scene.
        /// </summary>
        [Tooltip("List of all of the vehicles that can be selected and driven in the scene. " +
                 "If set to 0 script will try to auto-find all the vehicles in the scene with a tag define by VehiclesTag parameter.")]
        [SerializeField]
        public List<VehicleController> vehicles = new List<VehicleController>();

        /// <summary>
        ///     Tag that the script will search for if vehicles list is empty. Can be left empty if vehicles have already been
        ///     assigned manually.
        /// </summary>
        [Tooltip(
            "Tag that the script will search for if vehicles list is empty. Can be left empty if vehicles have already been assigned manually.")]
        public string vehicleTag = "Vehicle";

        public static VehicleChanger Instance { get; private set; }

        public static VehicleController ActiveVehicleController { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            if (vehicles.Count == 0)
            {
                FindVehicles();
            }

            if (deactivateAll)
            {
                DeactivateAllIncludingActive();
            }
            else
            {
                DeactivateAllExceptActive();
            }

            if (characterBased && CharacterVehicleChanger.Instance != null)
            {
                DeactivateAllIncludingActive();
            }
        }


        private void Update()
        {
            if (!characterBased)
            {
                bool changeVehiclePressed = false;
                try
                {
                    changeVehiclePressed = UnityEngine.Input.GetButtonDown("ChangeVehicle");
                }
                catch
                {
                    changeVehiclePressed = false;
                    foreach (InputProvider i in InputProvider.Instances)
                    {
                        if (i.ChangeVehicle())
                        {
                            changeVehiclePressed = true;
                            break;
                        }
                    }

                    Debug.LogWarning("'ChangeVehicle' input binding is not set under Project Settings > Input, falling back to default. " +
                                     "Check input section of manual on which input bindings need to be set up for NVP to work properly.");
                }

                if (changeVehiclePressed)
                {
                    NextVehicle();
                }
            }

            if (vehicles.Count > 0)
            {
                ActiveVehicleController = deactivateAll ? null : vehicles[currentVehicleIndex];
            }
            else
            {
                ActiveVehicleController = null;
            }

            if (deactivateAll)
            {
                for (int i = 0; i < vehicles.Count; i++)
                {
                    if (vehicles[i].IsAwake)
                    {
                        vehicles[i].Sleep();
                    }
                }
            }
        }

        /// <summary>
        ///     Changes vehicle to requested vehicle.
        /// </summary>
        /// <param name="index">Index of a vehicle in Vehicles list.</param>
        public void ChangeVehicle(int index)
        {
            currentVehicleIndex = index;
            if (currentVehicleIndex >= vehicles.Count)
            {
                currentVehicleIndex = 0;
            }

            DeactivateAllExceptActive();
        }

        /// <summary>
        ///     Finds nearest vehicle on the vehicles list.
        /// </summary>
        public VehicleController NearestVehicleFrom(GameObject go)
        {
            VehicleController nearest = null;

            int minIndex = -1;
            float minDist = Mathf.Infinity;
            if (vehicles.Count > 0)
            {
                for (int i = 0; i < vehicles.Count; i++)
                {
                    if (!vehicles[i].gameObject.activeInHierarchy)
                    {
                        continue;
                    }

                    float distance = Vector3.Distance(go.transform.position, vehicles[i].transform.position);
                    if (distance < minDist)
                    {
                        minIndex = i;
                        minDist = distance;
                    }
                }

                nearest = vehicles[minIndex];
            }

            return nearest;
        }

        /// <summary>
        ///     Changes vehicle to a vehicle with the requested name if there is such a vehicle.
        /// </summary>
        public void ChangeVehicle(VehicleController vc)
        {
            int vehicleIndex = vehicles.IndexOf(vc);

            if (vehicleIndex >= 0)
            {
                ChangeVehicle(vehicleIndex);
            }
        }

        /// <summary>
        ///     Changes vehicle to a next vehicle on the Vehicles list.
        /// </summary>
        public void NextVehicle()
        {
            if (vehicles.Count == 1)
            {
                return;
            }

            ChangeVehicle(currentVehicleIndex + 1);
        }

        public void PreviousVehicle()
        {
            if (vehicles.Count == 1)
            {
                return;
            }

            int previousIndex = currentVehicleIndex == 0 ? vehicles.Count - 1 : currentVehicleIndex - 1;


            ChangeVehicle(previousIndex);
        }

        public void DeactivateAllExceptActive()
        {
            for (int i = 0; i < vehicles.Count; i++)
            {
                if (i == currentVehicleIndex && !deactivateAll)
                {
                    vehicles[i].Wake();
                }
                else
                {
                    if (putOtherVehiclesToSleep)
                    {
                        vehicles[i].Sleep();
                    }
                }
            }
        }

        public void DeactivateAllIncludingActive()
        {
            for (int i = 0; i < vehicles.Count; i++)
            {
                vehicles[i].Sleep();
            }
        }


        public void FindVehicles()
        {
            GameObject[] candidateGOs = GameObject.FindGameObjectsWithTag(vehicleTag);
            if (vehicles == null)
            {
                vehicles = new List<VehicleController>();
            }

            foreach (GameObject go in candidateGOs)
            {
                VehicleController vc = go.GetComponent<VehicleController>();
                if (vc != null)
                {
                    vehicles.Add(vc);
                }
            }
        }
    }
}