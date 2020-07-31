using System;
using UnityEngine;

namespace NWH.VehiclePhysics2.Sound.SoundComponents
{
    /// <summary>
    ///     EngineFanComponent is used to imitate engine fan running, the sound especially prominent in commercial vehicles and
    ///     off-road vehicles with clutch driven fan.
    /// </summary>
    [Serializable]
    public class EngineFanComponent : SoundComponent
    {
        [Range(0, 1)]
        public float pitchRange = 0.5f;

        public override void Initialize()
        {
            if (Clips.Count != 0)
            {
                Source = container.AddComponent<AudioSource>();
                vc.soundManager.SetAudioSourceDefaults(Source, false, true, baseVolume, Clip);
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

            if (vc.powertrain.engine.IsRunning)
            {
                if (!Source.isPlaying)
                {
                    Play();
                }

                float rpmPercent = vc.powertrain.engine.RPMPercent;
                SetVolume(rpmPercent * rpmPercent * baseVolume);
                SetPitch(basePitch + pitchRange * rpmPercent);
            }
            else
            {
                if (Source.isPlaying)
                {
                    Stop();
                }
            }
        }


        public override void SetDefaults(VehicleController vc)
        {
            base.SetDefaults(vc);

            baseVolume = 0.05f;
            basePitch = 1f;

            if (Clip == null)
            {
                Clip = Resources.Load(VehicleController.DEFAULT_RESOURCES_PATH + "Sound/EngineFan") as AudioClip;
                if (Clip == null)
                {
                    Debug.LogWarning(
                        $"Audio Clip for sound component {GetType().Name} could not be loaded from resources. Source will not play.");
                }
            }
        }
    }
}