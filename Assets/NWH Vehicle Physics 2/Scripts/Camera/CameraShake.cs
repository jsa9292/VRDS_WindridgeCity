using UnityEngine;

namespace NWH.VehiclePhysics2.Cameras
{
    /// <summary>
    /// Simple script that shakes the camera on vehicle collision.
    /// </summary>
    [RequireComponent(typeof(VehicleCamera))]
    public class CameraShake : MonoBehaviour
    {
        public float shakeAmount = 0.2f;
        public float shakeDuration = 0.5f;

        private float _elapsed = 0f;
        private VehicleCamera _vehicleCamera;
        private VehicleController _targetVehicle;

        private void Start()
        {
            _vehicleCamera = GetComponent<VehicleCamera>();

            if (_vehicleCamera.target != null)
            {
                _vehicleCamera.target.damageHandler.OnCollision.AddListener(Shake);
            }
        }

        private void Update()
        {
            if (_elapsed > 0)
            {
                _elapsed -= Time.deltaTime;
                transform.localPosition += Random.insideUnitSphere * (shakeAmount * (_elapsed / shakeDuration));
            }
        }

        public void Shake(Collision collision)
        {
            _elapsed = shakeDuration;
        }
    }
}