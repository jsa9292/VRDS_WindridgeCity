using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace NWH.VehiclePhysics2.Effects
{
    /// <summary>
    ///     Single light source. Can be either an emissive mesh or a light.
    /// </summary>
    [Serializable]
    public class LightSource
    {
        public enum LightType
        {
            Light,
            Mesh
        }

        /// <summary>
        ///     Color of the emitted light.
        /// </summary>
        [ColorUsage(true, true)]
        [Tooltip("    Color of the emitted light.")]
        public Color emissionColor;

        /// <summary>
        ///     Light (point/spot/directional/etc.) representing the vehicle light. Will only be used if light type is set to
        ///     Light.
        /// </summary>
        [Tooltip(
            "Light (point/spot/directional/etc.) representing the vehicle light. Will only be used if light type is set to\r\nLight.")]
        public Light light;

        /// <summary>
        ///     Mesh renderer using standard shader. Emission on the material will be turned on or off depending on light state.
        ///     Will only be used if light type is set to Mesh.
        /// </summary>
        [Tooltip(
            "Mesh renderer using standard shader. Emission on the material will be turned on or off depending on light state.")]
        public MeshRenderer meshRenderer;

        /// <summary>
        ///     If your mesh has more than one material set this number to the index of required material.
        /// </summary>
        [Tooltip("    If your mesh has more than one material set this number to the index of required material.")]
        public int rendererMaterialIndex;

        /// <summary>
        ///     Type of the light.
        /// </summary>
        [Tooltip("    Type of the light.")]
        public LightType type;

        /// <summary>
        /// Called when the light is turned on.
        /// </summary>
        [NonSerialized] public UnityEvent onLightTurnedOn = new UnityEvent();
        
        /// <summary>
        /// Called when the light is turned off.
        /// </summary>
        [NonSerialized] public UnityEvent onLightTurnedOff = new UnityEvent();
        
        /// <summary>
        /// Is the light currently on?
        /// </summary>
        public bool IsOn { get; private set; }

        public virtual void TurnOff()
        {
            if (IsOn)
            {
                onLightTurnedOff.Invoke();   
            }

            if (type == LightType.Light && light != null)
            {
                light.enabled = false;
            }
            else if (Application.isPlaying)
            {
                if (meshRenderer == null || meshRenderer.material == null)
                {
                    return;
                }

                meshRenderer.materials[rendererMaterialIndex].DisableKeyword("_EMISSION");
            }

            IsOn = false;
        }

        public virtual void TurnOn()
        {
            if (!IsOn)
            {
                onLightTurnedOn.Invoke();
            }
            
            if (type == LightType.Light && light != null)
            {
                light.enabled = true;
            }
            else if (Application.isPlaying)
            {
                if (meshRenderer == null || meshRenderer.material == null)
                {
                    return;
                }

                Material mat = meshRenderer.materials[rendererMaterialIndex];
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", emissionColor);
            }

            IsOn = true;
        }
    }
}