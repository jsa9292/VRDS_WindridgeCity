using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Effects
{
    [CustomPropertyDrawer(typeof(SurfaceParticleManager))]
    public class SurfaceParticleManagerDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            drawer.Field("longitudinalSlipParticleCoeff");
            drawer.Field("lateralSlipParticleCoeff");
            drawer.Field("particleSizeCoeff");
            drawer.Field("emissionRateCoeff");
            drawer.Info("Surface specific particle settings are adjusted through SurfacePresets.");

            drawer.Field("particleCount", false, null, "Total Particle Count");

            drawer.EndProperty();
            return true;
        }
    }
}