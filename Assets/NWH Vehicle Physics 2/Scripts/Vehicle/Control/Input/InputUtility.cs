using UnityEngine;

namespace NWH.VehiclePhysics2.Input
{
    /// <summary>
    ///     Input utilities.
    /// </summary>
    public static class InputUtility
    {
        public static float GetMouseHorizontal()
        {
            float percent = Mathf.Clamp(UnityEngine.Input.mousePosition.x / Screen.width, -1f, 1f);
            if (percent < 0.5f)
            {
                return -(0.5f - percent) * 2.0f;
            }

            return (percent - 0.5f) * 2.0f;
        }

        public static float GetMouseVertical()
        {
            float percent = Mathf.Clamp(UnityEngine.Input.mousePosition.y / Screen.height, -1f, 1f);
            if (percent < 0.5f)
            {
                return -(0.5f - percent) * 2.0f;
            }

            return (percent - 0.5f) * 2.0f;
        }
    }
}