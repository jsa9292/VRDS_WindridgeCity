using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NWH.VehiclePhysics2.Input
{
    /// <summary>
    ///     Script for controlling the GUI steering wheel for mobile controls.
    ///     Credits go to yasirkula from Unity Forums for original code.
    /// </summary>
    public class SteeringWheel : MonoBehaviour
    {
        /// <summary>
        ///     Maximum angle that the steering wheel can be turned to towards either side in degrees.
        /// </summary>
        [Tooltip("    Maximum angle that the steering wheel can be turned to towards either side in degrees.")]
        public float maximumSteeringAngle = 200f;

        /// <summary>
        ///     Speed at which wheel is returned to center in degrees per second.
        /// </summary>
        [Tooltip("    Speed at which wheel is returned to center in degrees per second.")]
        public float returnToCenterSpeed = 400f;

        public Graphic steeringWheelGraphic;

        private Vector2 _centerPoint;
        private RectTransform _rectT;
        private float _wheelAngle;
        private bool _wheelBeingHeld;
        private float _wheelPrevAngle;

        private void Start()
        {
            _rectT = steeringWheelGraphic.rectTransform;

            InitEventsSystem();
            UpdateRect();
        }

        private void Update()
        {
            // If the wheel is released, reset the rotation
            // to initial (zero) rotation by wheelReleasedSpeed degrees per second
            if (!_wheelBeingHeld && !Mathf.Approximately(0f, _wheelAngle))
            {
                float deltaAngle = returnToCenterSpeed * Time.deltaTime;
                if (Mathf.Abs(deltaAngle) > Mathf.Abs(_wheelAngle))
                {
                    _wheelAngle = 0f;
                }
                else if (_wheelAngle > 0f)
                {
                    _wheelAngle -= deltaAngle;
                }
                else
                {
                    _wheelAngle += deltaAngle;
                }
            }

            // Rotate the wheel image
            _rectT.localEulerAngles = Vector3.back * _wheelAngle;
        }

        private void UpdateRect()
        {
            // Credit to: mwk888 from unityAnswers
            Vector3[] corners = new Vector3[4];
            _rectT.GetWorldCorners(corners);

            for (int i = 0; i < 4; i++)
            {
                corners[i] = RectTransformUtility.WorldToScreenPoint(null, corners[i]);
            }

            Vector3 bottomLeft = corners[0];
            Vector3 topRight = corners[2];
            float width = topRight.x - bottomLeft.x;
            float height = topRight.y - bottomLeft.y;

            Rect _rect = new Rect(bottomLeft.x, topRight.y, width, height);
            _centerPoint = new Vector2(_rect.x + _rect.width * 0.5f, _rect.y - _rect.height * 0.5f);
        }

        public void DragEvent(BaseEventData eventData)
        {
            // Executed when mouse/finger is dragged over the steering wheel
            Vector2 pointerPos = ((PointerEventData) eventData).position;

            float wheelNewAngle = Vector2.Angle(Vector2.up, pointerPos - _centerPoint);
            // Do nothing if the pointer is too close to the center of the wheel
            if (Vector2.Distance(pointerPos, _centerPoint) > 20f)
            {
                if (pointerPos.x > _centerPoint.x)
                {
                    _wheelAngle += wheelNewAngle - _wheelPrevAngle;
                }
                else
                {
                    _wheelAngle -= wheelNewAngle - _wheelPrevAngle;
                }
            }

            // Make sure wheel angle never exceeds maximumSteeringAngle
            _wheelAngle = Mathf.Clamp(_wheelAngle, -maximumSteeringAngle, maximumSteeringAngle);
            _wheelPrevAngle = wheelNewAngle;
        }

        /// <summary>
        ///     Returns a value in range [-1,1] similar to GetAxis("Horizontal")
        /// </summary>
        public float GetClampedValue()
        {
            return _wheelAngle / maximumSteeringAngle;
        }

        public void PressEvent(BaseEventData eventData)
        {
            // Executed when mouse/finger starts touching the steering wheel
            Vector2 pointerPos = ((PointerEventData) eventData).position;

            _wheelBeingHeld = true;
            _wheelPrevAngle = Vector2.Angle(Vector2.up, pointerPos - _centerPoint);
        }

        public void ReleaseEvent(BaseEventData eventData)
        {
            // Executed when mouse/finger stops touching the steering wheel
            // Performs one last DragEvent, just in case
            DragEvent(eventData);

            _wheelBeingHeld = false;
        }

        private void InitEventsSystem()
        {
            EventTrigger events = steeringWheelGraphic.gameObject.GetComponent<EventTrigger>();

            if (events == null)
            {
                events = steeringWheelGraphic.gameObject.AddComponent<EventTrigger>();
            }

            if (events.triggers == null)
            {
                events.triggers = new List<EventTrigger.Entry>();
            }

            EventTrigger.Entry entry = new EventTrigger.Entry();
            EventTrigger.TriggerEvent callback = new EventTrigger.TriggerEvent();
            UnityAction<BaseEventData> functionCall = PressEvent;
            callback.AddListener(functionCall);
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback = callback;

            events.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            callback = new EventTrigger.TriggerEvent();
            functionCall = DragEvent;
            callback.AddListener(functionCall);
            entry.eventID = EventTriggerType.Drag;
            entry.callback = callback;

            events.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            callback = new EventTrigger.TriggerEvent();
            functionCall = ReleaseEvent; //
            callback.AddListener(functionCall);
            entry.eventID = EventTriggerType.PointerUp;
            entry.callback = callback;

            events.triggers.Add(entry);
        }
    }
}