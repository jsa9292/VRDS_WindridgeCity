using System;
using System.Collections.Generic;
using NWH.VehiclePhysics2.GroundDetection;
using NWH.VehiclePhysics2.Powertrain;
using UnityEngine;

namespace NWH.VehiclePhysics2.Effects
{
    [Serializable]
    public class SkidmarkManager : Effect
    {
        /// <summary>
        ///     Should skidmarks fade as they get nearer to getting destroyed. This results in a soft alpha transition rather than
        ///     hard
        ///     cut
        ///     at the end of the skidmark. Ignored when persistent skidmarks are used.
        /// </summary>
        [Tooltip(
            "Should skidmarks fade as they get nearer to getting destroyed. This results in a soft alpha transition rather than hard\r\ncut\r\nat the end of the skidmark. Ignored when persistent skidmarks are used.")]
        public bool fadeOverDistance = true;

        /// <summary>
        ///     Higher value will give darker skidmarks for the same slip. Check corresponding SurfacePreset (GroundDetection ->
        ///     Presets)
        ///     for per-surface settings.
        /// </summary>
        [Range(0, 1)]
        [Tooltip(
            "Higher value will give darker skidmarks for the same slip. Check corresponding SurfacePreset (GroundDetection -> Presets)\r\nfor per-surface settings.")]
        public float globalSkidmarkIntensity = 0.6f;

        /// <summary>
        ///     Height above ground at which skidmarks will be drawn. If too low clipping between skidmark and ground surface will
        ///     occur.
        /// </summary>
        [Tooltip(
            "Height above ground at which skidmarks will be drawn. If too low clipping between skidmark and ground surface will\r\noccur.")]
        public float groundOffset = 0.025f;

        /// <summary>
        ///     When skidmark alpha value is below this value skidmark mesh will not be generated.
        /// </summary>
        [Tooltip("    When skidmark alpha value is below this value skidmark mesh will not be generated.")]
        public float lowerIntensityThreshold = 0.05f;

        /// <summary>
        ///     Number of skidmarks that will be drawn per one section, before mesh is saved and new one is generated.
        /// </summary>
        [Tooltip(
            "Number of skidmarks that will be drawn per one section, before mesh is saved and new one is generated.")]
        public int maxMarksPerSection = 180;

        /// <summary>
        ///     Max skidmark texture alpha.
        /// </summary>
        [Range(0, 1)]
        [Tooltip("    Max skidmark texture alpha.")]
        public float maxSkidmarkAlpha = 0.6f;

        /// <summary>
        ///     Distance from the last skidmark section needed to generate a new one.
        /// </summary>
        [Tooltip("    Distance from the last skidmark section needed to generate a new one.")]
        public float minDistance = 0.12f;

        /// <summary>
        ///     Persistent skidmarks get deleted when distance from the parent vehicle is higher than this.
        /// </summary>
        [Tooltip("    Persistent skidmarks get deleted when distance from the parent vehicle is higher than this.")]
        public float persistentSkidmarkDestroyDistance = 100f;

        /// <summary>
        ///     If enabled skidmarks will stay on the ground until distance from the vehicle becomes greater than
        ///     persistentSkidmarkDistance.
        ///     If disabled skidmarks will stay on the ground until maxMarksPerSection is reached and then will start getting
        ///     deleted from the end.
        /// </summary>
        [Tooltip(
            "If enabled skidmarks will stay on the ground until distance from the vehicle becomes greater than persistentSkidmarkDistance. " +
            "If disabled skidmarks will stay on the ground until maxMarksPerSection is reached and then will start getting deleted from the oldest skidmark.")]
        public bool persistentSkidmarks;

        /// <summary>
        ///     Smoothing between skidmark triangles. Value represents time required for alpha to go from 0 to 1.
        /// </summary>
        [Range(0.01f, 0.1f)]
        [Tooltip(
            "    Smoothing between skidmark triangles. Value represents time required for alpha to go from 0 to 1.")]
        public float smoothing = 0.07f;

        private int prevWheelCount;
        private GameObject skidmarkContainer;

        private List<SkidmarkGenerator> skidmarkGenerators = new List<SkidmarkGenerator>();

        public override void Initialize()
        {
            if (vc.groundDetection.groundDetectionPreset == null)
            {
                return;
            }

            skidmarkContainer = GameObject.Find("SkidContainer");
            if (skidmarkContainer == null)
            {
                skidmarkContainer = new GameObject("SkidContainer");
                skidmarkContainer.isStatic = true;
            }

            foreach (WheelComponent wheelComponent in vc.powertrain.wheels)
            {
                SkidmarkGenerator skidmarkGenerator = new SkidmarkGenerator();
                skidmarkGenerator.Initialize(wheelComponent, skidmarkContainer, minDistance,
                    vc.groundDetection.groundDetectionPreset.surfaceMaps.Count, maxMarksPerSection,
                    persistentSkidmarks, persistentSkidmarkDestroyDistance, groundOffset, smoothing,
                    lowerIntensityThreshold, fadeOverDistance, vc.groundDetection.groundDetectionPreset.fallbackSurfacePreset.skidmarkMaterial);
                skidmarkGenerators.Add(skidmarkGenerator);
            }

            float minPersistentDistance = maxMarksPerSection * minDistance * 1.5f;
            if (persistentSkidmarkDestroyDistance < minPersistentDistance)
            {
                persistentSkidmarkDestroyDistance = minPersistentDistance;
            }

            prevWheelCount = vc.Wheels.Count;

            initialized = true;
        }

        public override void FixedUpdate()
        {
        }

        public override void Update()
        {
            if (!Active)
            {
                return;
            }

            // Check if can be updated
            if (vc.groundDetection == null || !vc.groundDetection.IsEnabled)
            {
                return;
            }

            // Check for added/removed wheels and re-init if needed
            int wheelCount = vc.powertrain.wheels.Count;
            if (prevWheelCount != wheelCount)
            {
                initialized = false;
                Initialize();
            }

            prevWheelCount = wheelCount;

            // Update skidmarks
            Debug.Assert(skidmarkGenerators.Count == vc.powertrain.wheels.Count,
                "Skidmark generator count must equal wheel count");

            int n = skidmarkGenerators.Count;
            for (int i = 0; i < n; i++)
            {
                WheelComponent wheelComponent = vc.powertrain.wheels[i];
                SurfacePreset surfacePreset = wheelComponent.surfacePreset;
                if (surfacePreset == null || !surfacePreset.drawSkidmarks)
                {
                    continue;
                }
                
                bool surfaceMapIsNull = surfacePreset == null;

                int surfaceMapIndex = -1;
                if (!surfaceMapIsNull)
                {
                    surfaceMapIndex = wheelComponent.surfaceMapIndex;
                }

                float intensity = 1f;
                if (surfaceMapIndex >= 0)
                {
                    float latFactor = wheelComponent.NormalizedLateralSlip;
                    latFactor = latFactor < vc.lateralSlipThreshold ? 0 : latFactor - vc.lateralSlipThreshold;
                    float lonFactor = wheelComponent.NormalizedLongitudinalSlip;
                    lonFactor = lonFactor < vc.lateralSlipThreshold ? 0 : lonFactor - vc.lateralSlipThreshold;

                    float slipIntensity = latFactor + lonFactor;
                    float weightCoeff = wheelComponent.wheelController.wheel.load * 2f /
                                        wheelComponent.wheelController.maximumTireLoad;
                    weightCoeff = weightCoeff < 0 ? 0f : weightCoeff > 1f ? 1f : weightCoeff;
                    slipIntensity *= wheelComponent.surfacePreset.slipFactor * weightCoeff;

                    intensity = wheelComponent.surfacePreset.skidmarkBaseIntensity + slipIntensity;
                    intensity = intensity > 1f ? 1f : intensity < 0f ? 0f : intensity;
                }

                intensity *= globalSkidmarkIntensity;
                intensity = intensity < 0f ? 0f : intensity > maxSkidmarkAlpha ? maxSkidmarkAlpha : intensity;

                float albedoIntensity = 0f;
                float normalIntensity = 0f;
                
                // TODO - check that fallback surface preset is not null

                skidmarkGenerators[i].Update(surfaceMapIndex, intensity, albedoIntensity, normalIntensity,
                    wheelComponent.wheelController.pointVelocity, vc.fixedDeltaTime);
            }
        }
    }
}