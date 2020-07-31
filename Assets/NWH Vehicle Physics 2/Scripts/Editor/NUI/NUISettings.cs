using UnityEngine;

namespace NWH.NUI
{
    public static class NUISettings
    {
        public const string DOCUMENTATION_BASE_URL = "http://nwhvehiclephysics.com/doku.php/";
        public const float FIELD_HEIGHT = 23f;
        public const float FIELD_SPACING = 3f;
        public const string RESOURCES_PATH = "NUI/";
        public const float textMargin = 2f;

        public static Color scriptableObjectHeaderColor = new Color32(220, 122, 32, 255);
        public static Color editorHeaderColor = new Color32(20, 125, 211, 255);
        public static Color propertyHeaderColor = new Color32(78, 152, 213, 255);

        public static Color disabledColor = new Color(1f, 0.5f, 0.5f);
        public static Color enabledColor = new Color(0.5f, 1f, 0.5f);
        public static Color lightBlueColor = new Color32(70, 170, 220, 255);
        public static Color lightGreyColor = new Color32(180, 180, 180, 255);
    }
}