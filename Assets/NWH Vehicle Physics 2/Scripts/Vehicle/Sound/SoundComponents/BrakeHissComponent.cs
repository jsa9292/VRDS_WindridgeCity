using System;
using UnityEngine;

namespace NWH.VehiclePhysics2.Sound.SoundComponents
{
    /// <summary>
    ///     Imitates brake hiss on vehicles with pneumatic brake systems such as trucks and buses.
    ///     Accepts multiple clips of which one will be chosen at random each time this effect is played.
    /// </summary>
    [Serializable]
    public class BrakeHissComponent : SoundComponent
    {
        private bool _prevActive;

        public override void Initialize()
        {
            if (Clip != null)
            {
                Source = container.AddComponent<AudioSource>();
                vc.soundManager.SetAudioSourceDefaults(Source, false, false, baseVolume);
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

            if (Clip != null)
            {
                if (_prevActive && !vc.brakes.Active && vc.Speed < 1f && !Source.isPlaying)
                {
                    Source.clip = RandomClip;
                    //SetVolume(Mathf.Clamp01(vc.brakes.airBrakePressure / 10f) * baseVolume);
                    Play();
                    //vc.brakes.airBrakePressure = 0;
                }

                _prevActive = vc.brakes.Active;
            }
        }


        public override void SetDefaults(VehicleController vc)
        {
            base.SetDefaults(vc);

            baseVolume = 0.1f;
            basePitch = 1f;

            if (Clip == null)
            {
                Clip = Resources.Load(VehicleController.DEFAULT_RESOURCES_PATH + "Sound/AirBrakes") as AudioClip;
                if (Clip == null)
                {
                    Debug.LogWarning(
                        $"Audio Clip for sound component {GetType().Name} could not be loaded from resources. Source will not play.");
                }
            }
        }
    }
}