using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NWH.VehiclePhysics2.Input
{
    /// <summary>
    ///     Adds clicked and pressed flags to the standard Unity UI Button.
    /// </summary>
    public class MobileInputButton : Button
    {
        public bool hasBeenClicked;
        public bool isPressed;

        private void LateUpdate()
        {
            hasBeenClicked = false;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            isPressed = true;
            hasBeenClicked = true;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            isPressed = false;
        }
    }
}