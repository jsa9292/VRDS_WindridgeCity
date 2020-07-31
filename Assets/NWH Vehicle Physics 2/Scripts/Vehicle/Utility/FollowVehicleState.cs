using UnityEngine;
using UnityEngine.Serialization;

namespace NWH.VehiclePhysics2.Utility
{
    /// <summary>
    ///     Enables the object when vehicle is awake, disables it when vehicle is sleeping.
    /// </summary>
    public class FollowVehicleState : MonoBehaviour
    {
        private VehicleController _vc;

        private void Awake()
        {
            _vc = GetComponentInParent<VehicleController>();
            if (_vc == null)
            {
                Debug.LogError("VehicleController not found.");
            }
            
            _vc.onWake.AddListener(OnVehicleWake);
            _vc.onSleep.AddListener(OnVehicleSleep);

            if (_vc.IsAwake)
            {
                OnVehicleWake();
            }
            else
            {
                OnVehicleSleep();
            }
        }

        private void OnVehicleWake()
        {
            gameObject.SetActive(true);
        }

        private void OnVehicleSleep()
        {
            gameObject.SetActive(false);
        }
    }
}