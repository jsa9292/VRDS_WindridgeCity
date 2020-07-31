using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NWH.VehiclePhysics2.Sound.SoundComponents
{
    /// <summary>
    ///     Shifter sound played when changing gears.
    ///     Supports multiple audio clips of which one is chosen at random each time this effect is played.
    /// </summary>
    [Serializable]
    public class GearChangeComponent : SoundComponent
    {
        /// <summary>
        ///     Determines how much pitch of the gear shift sound can vary from one shift to another.
        ///     Final pitch is calculated as base pitch +- randomPitchRange.
        /// </summary>
        [Range(0, 0.5f)]
        [Tooltip(
            "Determines how much pitch of the gear shift sound can vary from one shift to another.\r\nFinal pitch is calculated as base pitch +- randomPitchRange.")]
        public float randomPitchRange = 0.2f;

        /// <summary>
        ///     Determines how much volume of the gear shift sound car vary.
        ///     Final volume is caulculated as base volume +- randomVolumeRange.
        /// </summary>
        [Range(0, 0.5f)]
        [Tooltip(
            "Determines how much volume of the gear shift sound car vary.\r\nFinal volume is caulculated as base volume +- randomVolumeRange.")]
        public float randomVolumeRange = 0.1f;

        private int previousGear;

        public override void Initialize()
        {
            if (Clips.Count != 0)
            {
                // Initialize gear shift sound
                Source = container.AddComponent<AudioSource>();
                vc.soundManager.SetAudioSourceDefaults(Source, false, false, baseVolume, RandomClip);
                AddSourcesToMixer();

                Source.dopplerLevel = 0;
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

            if (Clips.Count != 0)
            {
                if (previousGear != vc.powertrain.transmission.Gear && !Source.isPlaying)
                {
                    Source.clip = RandomClip;
                    SetVolume(baseVolume + baseVolume * Random.Range(-randomVolumeRange, randomVolumeRange));
                    SetPitch(basePitch + basePitch * Random.Range(-randomPitchRange, randomPitchRange));
                    if (Source.enabled)
                    {
                        Play();
                    }
                }

                previousGear = vc.powertrain.transmission.Gear;
            }
        }


        public override void SetDefaults(VehicleController vc)
        {
            base.SetDefaults(vc);

            baseVolume = 0.16f;
            basePitch = 0.8f;

            if (Clip == null)
            {
                Clip = Resources.Load(VehicleController.DEFAULT_RESOURCES_PATH + "Sound/GearChange") as AudioClip;
                if (Clip == null)
                {
                    Debug.LogWarning(
                        $"Audio Clip for sound component {GetType().Name} could not be loaded from resources. Source will not play.");
                }
            }
        }
    }
}