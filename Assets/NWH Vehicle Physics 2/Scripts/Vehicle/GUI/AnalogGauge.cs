using UnityEngine;

namespace NWH.VehiclePhysics2.VehicleGUI
{
    public class AnalogGauge : MonoBehaviour
    {
        /// <summary>
        ///     Angle of the needle at the highest value. You can use lock at end option to adjust this value while in play mode.
        /// </summary>
        [Tooltip(
            "Angle of the needle at the highest value. You can use lock at end option to adjust this value while in play mode.")]
        public float endAngle = 330;

        /// <summary>
        ///     Locks the needle position at the end angle (play mode only).
        /// </summary>
        [Tooltip("    Locks the needle position at the end angle (play mode only).")]
        public bool lockAtEnd;

        /// <summary>
        ///     Locks the needle position at the start angle (play mode only).
        /// </summary>
        [Tooltip("    Locks the needle position at the start angle (play mode only).")]
        public bool lockAtStart;

        /// <summary>
        ///     Value at the end of needle travel, at the end angle.
        /// </summary>
        [Tooltip("    Value at the end of needle travel, at the end angle.")]
        public float maxValue;

        /// <summary>
        ///     Smooths the travel of the needle making it more inert, as if actually had some mass and resistance.
        /// </summary>
        [Range(0, 1)]
        [Tooltip(
            "    Smooths the travel of the needle making it more inert, as if actually had some mass and resistance.")]
        public float needleSmoothing;

        /// <summary>
        ///     Angle of the needle at the lowest value. You can use lock at start option to adjust this value while in play mode.
        /// </summary>
        [Tooltip(
            "Angle of the needle at the lowest value. You can use lock at start option to adjust this value while in play mode.")]
        public float startAngle = 574;

        private float _angle;
        private float _currentValue;
        private GameObject _needle;
        private float _percent;
        private float _prevAngle;

        public float Value
        {
            get { return _currentValue; }
            set { _currentValue = Mathf.Clamp(value, 0, maxValue); }
        }

        private void Awake()
        {
            _needle = transform.Find("Needle").gameObject;
        }

        private void Start()
        {
            _angle = startAngle;
        }

        private void Update()
        {
            _percent = Mathf.Clamp01(_currentValue / maxValue);
            _prevAngle = _angle;
            _angle = Mathf.Lerp(startAngle + (endAngle - startAngle) * _percent, _prevAngle, needleSmoothing);

            if (lockAtEnd)
            {
                _angle = endAngle;
            }

            if (lockAtStart)
            {
                _angle = startAngle;
            }

            _needle.transform.eulerAngles =
                new Vector3(_needle.transform.eulerAngles.x, _needle.transform.eulerAngles.y, _angle);
        }
    }
}