using NWH.NUI;
using UnityEditor;
using UnityEngine;

namespace NWH.WheelController3D
{
    /// <summary>
    ///     Editor for WheelController.
    /// </summary>
    [CustomEditor(typeof(WheelController))]
    [CanEditMultipleObjects]
    public class WheelControllerEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
            {
                return false;
            }

            float logoHeight = 40f;
            Rect texRect = drawer.positionRect;
            texRect.height = logoHeight;
            drawer.DrawEditorTexture(texRect, "Wheel Controller 3D/Editor/logo_wc3d", ScaleMode.ScaleToFit);
            drawer.Space(logoHeight);

            drawer.HorizontalRuler();

            drawer.BeginSubsection("Wheel");
            drawer.Field("vehicleSide");
            drawer.Field("wheel.radius", true, "m");
            drawer.Field("wheel.width", true, "m");
            drawer.Field("wheel.mass", true, "kg");
            drawer.Field("wheel.rimOffset", true, "m");
            drawer.Field("dragTorque", true, "Nm");
            drawer.Field("parent");
            drawer.Field("useRimCollider");
            drawer.EndSubsection();

            drawer.BeginSubsection("Wheel Model");
            drawer.Field("wheel.visual");
            drawer.Field("wheel.visualPositionOffset");
            drawer.Field("wheel.visualRotationOffset");
            drawer.Field("wheel.nonRotatingVisual");
            drawer.EndSubsection();

            drawer.BeginSubsection("Spring");
            drawer.Field("spring.maxForce", true, "Nm");
            if (Application.isPlaying)
            {
                WheelController wc = target as WheelController;
                if (wc != null && wc.vehicleWheelCount > 0)
                {
                    float minRecommended = wc.parentRigidbody.mass * -Physics.gravity.y * 2f / wc.vehicleWheelCount;
                    if (wc.spring.maxForce < minRecommended)
                    {
                        drawer.Info(
                            "MaxForce of Spring is most likely too low for the vehicle mass. Minimum recommended for current configuration is" +
                            $" {minRecommended}N.", MessageType.Warning);
                    }
                }
            }

            if (drawer.Field("spring.maxLength", true, "m").floatValue < Time.fixedDeltaTime * 10f)
            {
                drawer.Info(
                    $"Minimum recommended spring length for Time.fixedDeltaTime of {Time.fixedDeltaTime} is {Time.fixedDeltaTime * 10f}");
            }

            drawer.Field("spring.forceCurve");
            drawer.Info("X: Spring compression [%], Y: Force coefficient");
            drawer.Field("spring.bottomOutForceCoefficient");

            drawer.EndSubsection();

            drawer.BeginSubsection("Damper");
            drawer.Field("damper.bumpForce");
            drawer.Field("damper.reboundForce");
            drawer.Field("damper.curve");
            drawer.Info("X: Spring velocity (normalized) [m/s], Y: Force coefficient (normalized)");
            drawer.Info(
                "Since NVP2 damper curve is using normalized values. Make sure to adjust bump and rebound forces if you are updating from an older version.");
            drawer.EndSubsection();

            drawer.BeginSubsection("Geometry");
            drawer.Field("wheel.camberAtTop", true, "deg");
            drawer.Field("wheel.camberAtBottom", true, "deg");
            drawer.Field("squat");
            drawer.EndSubsection();

            drawer.BeginSubsection("Friction");
            drawer.Field("activeFrictionPreset");
            drawer.EmbeddedObjectEditor(((WheelController) target).activeFrictionPreset, drawer.positionRect);

            drawer.BeginSubsection("Longitudinal");
            drawer.Field("forwardFriction.slipCoefficient", true, "x100 %");
            drawer.Field("forwardFriction.forceCoefficient", true, "x100 %");
            drawer.EndSubsection();

            drawer.BeginSubsection("Lateral");
            drawer.Field("sideFriction.slipCoefficient", true, "x100 %");
            drawer.Field("sideFriction.forceCoefficient", true, "x100 %");
            drawer.EndSubsection();

            drawer.BeginSubsection("Load and Forces");
            drawer.Field("loadGripCurve");
            drawer.Info("X: Tire Load [N], Y: Max Tire Friction Force [N]");
            drawer.Field("maximumTireLoad", true, "Nm");
            drawer.Field("maximumTireGripForce", true, "Nm");
            drawer.EndSubsection();

            drawer.EndSubsection();

            drawer.BeginSubsection("Ground Detection");
            if (!drawer.Field("singleRay").boolValue)
            {
                SerializedProperty longScanRes =
                    drawer.Field("longitudinalScanResolution", !Application.isPlaying);
                SerializedProperty latScanRes =
                    drawer.Field("lateralScanResolution", !Application.isPlaying);
                if (longScanRes.intValue < 5)
                {
                    longScanRes.intValue = 5;
                }

                if (latScanRes.intValue < 1)
                {
                    latScanRes.intValue = 1;
                }

                int rayCount = longScanRes.intValue * latScanRes.intValue;
                drawer.Info($"Ray count: {rayCount}");
            }

            drawer.Field("applyForceToOthers");

            if (!drawer.Field("autoSetupLayerMask").boolValue)
            {
                drawer.Field("layerMask");
                drawer.Info(
                    "Make sure that vehicle's collider layers are unselected in the layerMask, as well as Physics.IgnoreRaycast layer. If not, " +
                    "wheels will collide with vehicle itself sand result in it behaving unpredictably.");
            }

            drawer.EndSubsection();

            drawer.BeginSubsection("Debug Values");
            drawer.Field("forwardFriction.slip", false, null, "Longitudinal Slip");
            drawer.Field("sideFriction.slip", false, null, "Lateral Slip");
            drawer.Field("suspensionForceMagnitude", false);
            drawer.Field("spring.bottomedOut", false);
            drawer.Field("wheel.motorTorque", false);
            drawer.Field("wheel.brakeTorque", false);

            drawer.Space();
            drawer.Field("debug");
            drawer.EndSubsection();

            drawer.EndEditor(this);
            return true;
        }

        public override bool UseDefaultMargins()
        {
            return false;
        }
    }
}