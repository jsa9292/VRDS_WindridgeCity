using UnityEngine;
using UnityEngine.UI;

namespace NWH.VehiclePhysics2.VehicleGUI
{
    /// <summary>
    ///     A simple script that switches Color of an Image between On Color and Off Color, usually white and black.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class DashLight : MonoBehaviour
    {
        /// <summary>
        ///     Time it takes for the light to turn off, once Hold Time expires. Imitates dash lights that use conventional bulbs.
        /// </summary>
        [Tooltip(
            "Time it takes for the light to turn off, once Hold Time expires. Imitates dash lights that use conventional bulbs.")]
        public float fadeTime = 0.5f;

        /// <summary>
        ///     Time it takes for the light to start turning off after the state has changed. Useful to remove flicker from signals
        ///     that are not persistent but rather change on frame-to-frame basis, e.g. ABS and TCS.
        /// </summary>
        [Tooltip(
            "Time it takes for the light to start turning off after the state has changed. Useful to remove flicker from signals that are not persistent but rather change on frame-to-frame basis, e.g. ABS and TCS. ")]
        public float holdTime = 0.2f;

        /// <summary>
        ///     Color of the ''Image'' when the light is off. Black by default.
        /// </summary>
        [Tooltip("    Color of the ''Image'' when the light is off. Black by default.")]
        public Color offColor = Color.black;

        /// <summary>
        ///     Color of the ''Image'' when the light is on. White by default.
        /// </summary>
        [Tooltip("    Color of the ''Image'' when the light is on. White by default.")]
        public Color onColor = Color.white;

        private bool _active;
        private float _fadeOutTimer;
        private Image _icon;
        private bool _wasActive;

        public bool Active
        {
            get { return _active; }
            set
            {
                _wasActive = _active;
                _active = value;

                if (!_active && _wasActive)
                {
                    if (_fadeOutTimer < 0)
                    {
                        _fadeOutTimer = 0;
                    }
                }
                else if (_active && !_wasActive)
                {
                    _icon.color = onColor;
                }
            }
        }

        private void Start()
        {
            _active = false;
            _icon = GetComponent<Image>();
            _icon.color = offColor;

            _fadeOutTimer = -1f;
        }

        private void Update()
        {
            if (fadeTime > 0)
            {
                if (_fadeOutTimer >= 0 && _fadeOutTimer <= fadeTime + holdTime)
                {
                    _icon.color = Color.Lerp(onColor, offColor, (_fadeOutTimer - holdTime) / fadeTime);
                }
            }
            else if (_fadeOutTimer >= 0)
            {
                _icon.color = offColor;
            }


            if (_fadeOutTimer >= 0)
            {
                _fadeOutTimer += Time.deltaTime;
                if (_fadeOutTimer > fadeTime + holdTime)
                {
                    _fadeOutTimer = -1;
                }
            }
        }
    }
}