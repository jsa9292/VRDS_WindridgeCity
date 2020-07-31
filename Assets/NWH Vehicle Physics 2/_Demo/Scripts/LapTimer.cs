using UnityEngine;
using UnityEngine.UI;

namespace NWH.VehiclePhysics2.Demo
{
    public class LapTimer : MonoBehaviour
    {
        public float bestLapTime = 9999f;
        public Text bestLapTimeText;

        public float currentLapTime = 9999f;
        public Text currentLapTimeText;
        public float previousLapTime = 9999f;
        public Text previousLapTimeText;

        private bool _started;

        private void OnTriggerEnter(Collider other)
        {
            _started = true;

            if (currentLapTime < 5f)
            {
                return;
            }

            if (currentLapTime < bestLapTime)
            {
                bestLapTime = currentLapTime;
            }

            previousLapTime = currentLapTime;
            currentLapTime = 0f;
        }

        private void Start()
        {
            currentLapTime = 9999f;
            bestLapTime = 9999f;
            previousLapTime = 9999f;
        }

        private void Update()
        {
            if (!_started)
            {
                return;
            }

            currentLapTime += Time.deltaTime;

            if (currentLapTime < 9998f)
            {
                currentLapTimeText.text = currentLapTime.ToString("F2");
            }

            if (previousLapTime < 9998f)
            {
                previousLapTimeText.text = previousLapTime.ToString("F2");
            }

            if (bestLapTime < 9998f)
            {
                bestLapTimeText.text = bestLapTime.ToString("F2");
            }
        }
    }
}
