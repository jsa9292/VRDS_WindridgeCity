using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NWH.VehiclePhysics2.Effects
{
    /// <summary>
    ///     Controls exhaust flashes. These are achieved through changing the color of the flame textures.
    /// </summary>
    [Serializable]
    public class ExhaustFlash : Effect
    {
        public bool flash;

        public List<Light> flashLights = new List<Light>();
        public bool flashOnRevLimiter = true;
        public bool flashOnShift = true;

        /// <summary>
        ///     Textures representing exhaust flash. If multiple are assigned a random texture will be chosen for each flash.
        /// </summary>
        [Tooltip(
            "Textures representing exhaust flash. If multiple are assigned a random texture will be chosen for each flash.")]
        public List<Texture2D> flashTextures = new List<Texture2D>();

        
        /// <summary>
        ///     Mesh renderer(s) for the exhaust flash meshes. Materials used should have '_TintColor' property.
        /// </summary>
        [UnityEngine.Tooltip("    Mesh renderer(s) for the exhaust flash meshes. Materials used should have '_TintColor' property.")]
        public List<MeshRenderer> meshRenderers = new List<MeshRenderer>();

        private bool _hasFlashed;

        public override void Initialize()
        {
            initialized = true;
        }

        public override void Awake(VehicleController vc)
        {
            base.Awake(vc);

            DisableEffects();
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

            if (_hasFlashed)
            {
                DisableEffects();
            }

            flash |= vc.powertrain.engine.revLimiterActive && flashOnRevLimiter;
            flash |= vc.powertrain.transmission.IsShifting && flashOnShift && vc.powertrain.engine.RPMPercent > 0.6f;

            if (flash)
            {
                EnableEffects();
            }
        }

        public override void Enable()
        {
            base.Enable();

            EnableEffects();
        }

        private void EnableEffects()
        {
            if (_hasFlashed)
            {
                return;
            }

            int textureCount = flashTextures.Count;
            foreach (MeshRenderer renderer in meshRenderers)
            {
                renderer.material.SetTexture("_MainTex", flashTextures[Random.Range(0, textureCount)]);
                float r = Random.Range(0.2f, 0.6f);
                renderer.transform.localScale = new Vector3(r, r, r);
                renderer.enabled = true;
            }

            foreach (Light light in flashLights)
            {
                light.enabled = true;
            }

            flash = false;
            _hasFlashed = true;
        }

        public override void Disable()
        {
            base.Disable();

            DisableEffects();
        }

        private void DisableEffects()
        {
            foreach (MeshRenderer renderer in meshRenderers)
            {
                renderer.enabled = false;
            }

            foreach (Light light in flashLights)
            {
                light.enabled = false;
            }

            _hasFlashed = false;
        }

        public void Flash()
        {
            flash = true;
        }
    }
}