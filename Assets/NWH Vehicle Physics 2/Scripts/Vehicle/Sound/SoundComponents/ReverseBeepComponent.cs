using System;
using UnityEngine;

namespace NWH.VehiclePhysics2.Sound.SoundComponents
{
    [Serializable]
    public class ReverseBeepComponent : SoundComponent
    {
        public bool beepOnNegativeVelocity = true;
        public bool beepOnReverseGear = true;

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

            int gear = vc.powertrain.transmission.Gear;
            if (beepOnReverseGear && gear < 0 ||
                beepOnNegativeVelocity && vc.ForwardVelocity < -0.2f && gear <= 0)
            {
                if (!Source.isPlaying)
                {
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


        public override void SetDefaults(VehicleController vc)
        {
            base.SetDefaults(vc);

            if (Clip == null)
            {
                Clip = Resources.Load(VehicleController.DEFAULT_RESOURCES_PATH + "Sound/ReverseBeep") as AudioClip;
                if (Clip == null)
                {
                    Debug.LogWarning(
                        $"Audio Clip for sound component {GetType().Name} could not be loaded from resources. Source will not play.");
                }
            }
        }
    }
}