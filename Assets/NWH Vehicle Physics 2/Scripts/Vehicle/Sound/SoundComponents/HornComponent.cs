using System;
using UnityEngine;

namespace NWH.VehiclePhysics2.Sound.SoundComponents
{
    /// <summary>
    ///     Vehicle horn sound.
    /// </summary>
    [Serializable]
    public class HornComponent : SoundComponent
    {
        public override void Initialize()
        {
            if (Clips.Count != 0)
            {
                Source = container.AddComponent<AudioSource>();
                vc.soundManager.SetAudioSourceDefaults(Source, false, true, baseVolume, RandomClip);
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

            if (Source != null && Clip != null)
            {
                if (vc.input.Horn && !Source.isPlaying)
                {
                    SetPitch(basePitch);
                    SetVolume(baseVolume);
                    Play();
                }
                else if (!vc.input.Horn && Source.isPlaying)
                {
                    Stop();
                }
            }
        }

        public override void SetDefaults(VehicleController vc)
        {
            base.SetDefaults(vc);

            if (Clip == null)
            {
                Clip = Resources.Load(VehicleController.DEFAULT_RESOURCES_PATH + "Sound/Horn") as AudioClip;
                if (Clip == null)
                {
                    Debug.LogWarning(
                        $"Audio Clip for sound component {GetType().Name} could not be loaded from resources. Source will not play.");
                }
            }
        }
    }
}