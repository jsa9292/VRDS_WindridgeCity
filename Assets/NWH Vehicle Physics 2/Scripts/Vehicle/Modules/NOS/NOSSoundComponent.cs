using System;
using NWH.VehiclePhysics2.Sound.SoundComponents;
using UnityEngine;

namespace NWH.VehiclePhysics2.Modules.NOS
{
    /// <summary>
    ///     Sound component producing the distinct 'hiss' sound of active NOS.
    /// </summary>
    [Serializable]
    public class NOSSoundComponent : SoundComponent
    {
        [NonSerialized]
        public NOSModule nosModule;

        private bool wasActive;

        public override void Initialize()
        {
            // Initialize engine sound
            if (Clip != null)
            {
                Source = container.AddComponent<AudioSource>();
                vc.soundManager.SetAudioSourceDefaults(Source, false, true, 0f, Clip);
                AddSourcesToMixer();
                Stop();
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

            if (Clip != null && Sources != null)
            {
                if (nosModule.active && !wasActive)
                {
                    SetVolume(baseVolume);
                    SetPitch(basePitch);
                    Play();
                }
                else if (!nosModule.active && wasActive)
                {
                    Stop();
                }
            }

            wasActive = nosModule.active;
        }


        public override void SetDefaults(VehicleController vc)
        {
            base.SetDefaults(vc);

            baseVolume = 0.2f;

            if (Clip == null)
            {
                Clip = Resources.Load(VehicleController.DEFAULT_RESOURCES_PATH + "Sound/NOS") as AudioClip;
                if (Clip == null)
                {
                    Debug.LogWarning(
                        $"Audio Clip for sound component {GetType().Name}  from resources. Source will not play.");
                }
            }
        }
    }
}