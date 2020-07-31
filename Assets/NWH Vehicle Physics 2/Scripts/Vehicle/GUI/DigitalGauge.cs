using UnityEngine;
using UnityEngine.UI;

namespace NWH.VehiclePhysics2.VehicleGUI
{
    [RequireComponent(typeof(Text))]
    public class DigitalGauge : MonoBehaviour
    {
        public enum GaugeType
        {
            Numerical,
            Textual
        }

        /// <summary>
        ///     Numerical value formatting.
        /// </summary>
        [Tooltip("    Numerical value formatting.")]
        public string format = "0.0";

        /// <summary>
        ///     Should the stringValue or numericalValue be used. String value is useful for e.g. gear (R, N, 1, 2, 3) and
        ///     numerical
        ///     for
        ///     speed, RPM or similar.
        /// </summary>
        [Tooltip(
            "Should the stringValue or numericalValue be used. String value is useful for e.g. gear (R, N, 1, 2, 3) and numerical\r\nfor\r\nspeed, RPM or similar.")]
        public GaugeType gaugeType = GaugeType.Numerical;

        /// <summary>
        ///     Maximum value that the gauge can display. Only used if showProgressBar enabled.
        /// </summary>
        [Tooltip("    Maximum value that the gauge can display. Only used if showProgressBar enabled.")]
        public float maxValue;

        /// <summary>
        ///     Time over which the numerical value will be smoothed.
        /// </summary>
        [Range(0, 1)]
        [Tooltip("    Time over which the numerical value will be smoothed.")]
        public float numericalSmoothing = 0.5f;

        /// <summary>
        ///     Numerical value that will be displayed on the gauge.
        /// </summary>
        [Tooltip("    Numerical value that will be displayed on the gauge.")]
        public float numericalValue;

        /// <summary>
        ///     Should the progress line/bar be displayed for better visualization?
        /// </summary>
        [Tooltip("    Should the progress line/bar be displayed for better visualization?")]
        public bool showProgressBar;

        /// <summary>
        ///     String value that will be displayed on the gauge.
        /// </summary>
        [Tooltip("    String value that will be displayed on the gauge.")]
        public string stringValue;

        /// <summary>
        ///     Unit displayed after the value, e.g. km/h.
        /// </summary>
        [Tooltip("    Unit displayed after the value, e.g. km/h.")]
        public string unit;

        private float _fullLineWidth;
        private Image _line;
        private float _prevNumericalValue;
        private Text _readout;

        private void Start()
        {
            // Find readout
            Transform readoutTransform = transform.Find("Readout");
            if (readoutTransform != null)
            {
                _readout = readoutTransform.gameObject.GetComponent<Text>();
            }

            // Find line
            Transform lineTransform = transform.Find("Line");
            if (lineTransform != null)
            {
                _line = lineTransform.gameObject.GetComponent<Image>();
            }

            // Disable dynamic line if non-numerical display
            if (gaugeType == GaugeType.Textual)
            {
                showProgressBar = false;
            }

            // Remember initial line width
            if (_line != null)
            {
                _fullLineWidth = _line.rectTransform.sizeDelta.x;
            }
        }

        private void Update()
        {
            // Update readout
            if (_readout != null)
            {
                _readout.text = "";
                if (gaugeType == GaugeType.Numerical)
                {
                    numericalValue = Mathf.SmoothStep(_prevNumericalValue, numericalValue, 1.01f - numericalSmoothing);
                    _readout.text += numericalValue.ToString(format);
                    _prevNumericalValue = numericalValue;
                }

                string space = "";
                if (unit != "")
                {
                    space = " ";
                }

                _readout.text += stringValue + space + unit;
            }

            // Update line
            if (_line != null && showProgressBar)
            {
                float percentage = Mathf.Clamp01(numericalValue / maxValue);
                _line.rectTransform.sizeDelta =
                    new Vector2(percentage * _fullLineWidth, _line.rectTransform.sizeDelta.y);
            }
        }
    }
}