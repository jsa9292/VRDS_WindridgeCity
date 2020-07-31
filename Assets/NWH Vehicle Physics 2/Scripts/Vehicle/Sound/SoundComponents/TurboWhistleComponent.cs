using System;
using UnityEngine;

namespace NWH.VehiclePhysics2.Sound.SoundComponents
{
    /// <summary>
    ///     Sound of turbocharger or supercharger.
    /// </summary>
    [Serializable]
    public class TurboWhistleComponent : SoundComponent
    {
        /// <summary>
        ///     Pitch range that will be added to the base pitch depending on turbos's RPM.
        /// </summary>
        [Range(0, 5)]
        [Tooltip("    Pitch range that will be added to the base pitch depending on turbos's RPM.")]
        public float pitchRange = 0.9f;

        public override void Initialize()
        {
            if (Clip != null)
            {
                Source = container.AddComponent<AudioSource>();
                vc.soundManager.SetAudioSourceDefaults(Source, true, true, 0, Clip);
                AddSourcesToMixer();
            }

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

            if (Clip != null && vc.powertrain.engine.IsRunning &&
                vc.powertrain.engine.forcedInduction.useForcedInduction)
            {
                SetVolume(Mathf.Clamp01(baseVolume * Mathf.Pow(vc.powertrain.engine.forcedInduction.boost, 1.5f)) *
                          vc.soundManager.masterVolume);
                SetPitch(basePitch + pitchRange * vc.powertrain.engine.forcedInduction.boost);
                Play();
            }
            else
            {
                if (Source != null)
                {
                    SetVolume(0);
                    Stop();
                }
            }
        }


        public override void SetDefaults(VehicleController vc)
        {
            base.SetDefaults(vc);

            baseVolume = 0.12f;
            basePitch = 0f;
            pitchRange = 0.8f;

            if (Clip == null)
            {
                Clip = Resources.Load(VehicleController.DEFAULT_RESOURCES_PATH + "Sound/TurboWhistle") as AudioClip;
                if (Clip == null)
                {
                    Debug.LogWarning(
                        $"Audio Clip for sound component {GetType().Name} could not be loaded from resources. Source will not play.");
                }
            }
        }
    }
}