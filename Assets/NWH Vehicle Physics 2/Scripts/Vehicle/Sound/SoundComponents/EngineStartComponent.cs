using System;
using UnityEngine;

namespace NWH.VehiclePhysics2.Sound.SoundComponents
{
    /// <summary>
    ///     Sound of an engine starting / stopping.
    ///     Plays while start is active.
    /// </summary>
    [Serializable]
    public class EngineStartComponent : SoundComponent
    {
        public override void Initialize()
        {
            // Initilize start/stop source
            if (Clips.Count > 0)
            {
                Source = container.AddComponent<AudioSource>();
                vc.soundManager.SetAudioSourceDefaults(Source, false, false, 0f, Clip);
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

            // Starting and stopping engine sound
            if (Source != null && Clips.Count > 0)
            {
                if (vc.powertrain.engine.StarterActive)
                {
                    if (!Source.isPlaying)
                    {
                        SetVolume(baseVolume);
                        Play();
                    }
                }
                else
                {
                    if (Source.isPlaying)
                    {
                        Stop();
                    }
                }
            }
        }


        public override void SetDefaults(VehicleController vc)
        {
            base.SetDefaults(vc);
            baseVolume = 0.2f;
            basePitch = 1f;

            if (Clip == null)
            {
                Clip = Resources.Load(VehicleController.DEFAULT_RESOURCES_PATH + "Sound/EngineStart") as AudioClip;
                if (Clip == null)
                {
                    Debug.LogWarning(
                        $"Audio Clip for sound component {GetType().Name} could not be loaded from resources. Source will not play.");
                }
            }
        }
    }
}