using System;
using UnityEngine;

namespace NWH.VehiclePhysics2.Sound.SoundComponents
{
    /// <summary>
    ///     Sound of an engine idling.
    /// </summary>
    [Serializable]
    public class EngineRunningComponent : SoundComponent
    {
        /// <summary>
        ///     Distortion at maximum engine load.
        /// </summary>
        [Range(0, 1)]
        [Tooltip("    Distortion at maximum engine load.")]
        public float maxDistortion = 0.4f;

        /// <summary>
        ///     Pitch added to the base engine pitch depending on engine RPM.
        /// </summary>
        [Range(0, 4)]
        [Tooltip("    Pitch added to the base engine pitch depending on engine RPM.")]
        public float pitchRange = 2.5f;

        /// <summary>
        ///     Smoothing of engine volume.
        /// </summary>
        [Range(0, 1)]
        [Tooltip("    Smoothing of engine volume.")]
        public float smoothing = 0.05f;

        /// <summary>
        ///     Volume added to the base engine volume depending on engine state.
        /// </summary>
        [Range(0, 1)]
        [Tooltip("    Volume added to the base engine volume depending on engine state.")]
        public float volumeRange = 0.1f;

        private float _volume;
        private float _volumeVelocity;

        public override void Initialize()
        {
            // Initialize engine sound
            if (Clip != null)
            {
                Source = container.AddComponent<AudioSource>();
                vc.soundManager.SetAudioSourceDefaults(Source, false, true, 0f, Clip);
                AddSourcesToMixer();
                Stop();
                SetVolume(0);
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

            // Engine sound
            if (Source != null && Clip != null)
            {
                if (vc.powertrain.engine.IsRunning || vc.powertrain.engine.starterActive)
                {
                    if (!Source.isPlaying && Source.enabled)
                    {
                        Play();
                    }

                    float newPitch = basePitch + vc.powertrain.engine.RPMPercent * pitchRange;
                    float throttleInput = vc.powertrain.engine.revLimiterActive
                        ? 1f
                        : vc.powertrain.engine.ThrottlePosition;
                    SetPitch(newPitch);

                    float newVolume = baseVolume + throttleInput * volumeRange;

                    audioMixerGroup.audioMixer.SetFloat("engineDistortion", throttleInput * maxDistortion);

                    if (vc.powertrain.engine.starterActive)
                    {
                        newVolume = baseVolume;
                    }

                    _volume = Mathf.SmoothDamp(_volume, newVolume, ref _volumeVelocity, smoothing);
                    SetVolume(_volume);
                }
                else
                {
                    if (Source.isPlaying)
                    {
                        Stop();
                    }

                    SetVolume(0);
                    SetPitch(0);
                }
            }
        }


        public override void SetDefaults(VehicleController vc)
        {
            base.SetDefaults(vc);

            baseVolume = 0.2f;
            basePitch = 0.4f;

            if (Clip == null)
            {
                Clip = Resources.Load(VehicleController.DEFAULT_RESOURCES_PATH + "Sound/EngineRunning") as AudioClip;
                if (Clip == null)
                {
                    Debug.LogWarning(
                        $"Audio Clip for sound component {GetType().Name} could not be loaded from resources. Source will not play.");
                }
            }
        }
    }
}