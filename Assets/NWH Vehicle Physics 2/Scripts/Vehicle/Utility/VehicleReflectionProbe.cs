using UnityEngine;
using UnityEngine.Rendering;

namespace NWH.VehiclePhysics2.Utility
{
    /// <summary>
    ///     Manages reflection probe for the vehicle and switches between probe types as needed.
    /// </summary>
    [RequireComponent(typeof(ReflectionProbe))]
    public class VehicleReflectionProbe : MonoBehaviour
    {
        public enum ProbeType { Baked, Realtime }
        public ProbeType awakeProbeType = ProbeType.Realtime;
        public ProbeType asleepProbeType = ProbeType.Baked;
        public bool bakeOnStart = true;
        public bool bakeOnSleep = true;

        private ReflectionProbe _reflectionProbe;
        private VehicleController _vc;

        private void Start()
        {
            _vc = GetComponentInParent<VehicleController>();
            if (_vc == null)
            {
                Debug.LogError("VehicleController not found.");
            }

            _reflectionProbe = GetComponent<ReflectionProbe>();
            _vc.onWake.AddListener(OnVehicleWake);
            _vc.onSleep.AddListener(OnVehicleSleep);

            if (bakeOnStart)
            {
                _reflectionProbe.RenderProbe();
            }
        }

        private void OnVehicleWake()
        {
            _reflectionProbe.mode = awakeProbeType == ProbeType.Baked
                ? _reflectionProbe.mode = ReflectionProbeMode.Baked
                : ReflectionProbeMode.Realtime;
        }

        private void OnVehicleSleep()
        {
            _reflectionProbe.mode = asleepProbeType == ProbeType.Baked
                ? _reflectionProbe.mode = ReflectionProbeMode.Baked
                : ReflectionProbeMode.Realtime;
            
            if(bakeOnSleep)
            {
                _reflectionProbe.RenderProbe();
            }
        }
    }
}