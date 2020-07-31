using System;
using NWH.VehiclePhysics2.GroundDetection;
using NWH.VehiclePhysics2.Powertrain;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NWH.VehiclePhysics2.Sound.SoundComponents
{
    /// <summary>
    ///     Sound produced by tire skidding over surface.
    /// </summary>
    [Serializable]
    public class WheelSkidComponent : SoundComponent
    {
        private float[] _prevPitch;
        private float[] _prevVolume;
        private int _wheelCount;

        public override void Initialize()
        {
            _wheelCount = vc.Wheels.Count;
            for (int index = 0; index < _wheelCount; index++)
            {
                WheelComponent wheel = vc.Wheels[index];
                AudioSource a = wheel.ControllerGO.AddComponent<AudioSource>();
                vc.soundManager.SetAudioSourceDefaults(a, true, true);
                Sources.Add(a);
            }

            AddSourcesToMixer();

            _prevVolume = new float[_wheelCount];
            _prevPitch = new float[_wheelCount];

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

            float newVolume = 0;
            float newPitch = basePitch;

            if (vc.groundDetection != null)
            {
                for (int i = 0; i < _wheelCount; i++)
                {
                    WheelComponent wheelComponent = vc.Wheels[i];
                    SurfacePreset surfacePreset = wheelComponent.surfacePreset;

                    bool hasSlip = wheelComponent.HasLateralSlip || wheelComponent.HasLongitudinalSlip;
                    if (wheelComponent.IsGrounded && surfacePreset != null && surfacePreset.playSkidSounds && hasSlip)
                    {
                        float slipPercent = wheelComponent.NormalizedLateralSlip +
                                            wheelComponent.NormalizedLongitudinalSlip;
                        slipPercent = slipPercent < 0 ? 0 : slipPercent > 1 ? 1 : slipPercent;

                        if (surfacePreset.skidSoundClip != null)
                        {
                            AudioSource source = Sources[i];

                            if (!source.isPlaying)
                            {
                                source.Play();
                            }

                            if (source.clip != surfacePreset.skidSoundClip)
                            {
                                // Change skid clip
                                source.clip = surfacePreset.skidSoundClip;
                                source.time = Random.Range(0f, surfacePreset.skidSoundClip.length);
                                source.time = Random.Range(0f, source.clip.length);
                            }

                            float absAngVel = wheelComponent.angularVelocity < 0
                                ? -wheelComponent.angularVelocity
                                : wheelComponent.angularVelocity;
                            float speedCoeff = vc.Speed / 3f + absAngVel / 20f;
                            speedCoeff = speedCoeff > 1f ? 1f : speedCoeff;
                            newVolume = slipPercent * surfacePreset.skidSoundVolume * speedCoeff;
                            newVolume = Mathf.Lerp(_prevVolume[i], newVolume, vc.deltaTime * 12f);

                            float loadCoeff = wheelComponent.wheelController.wheel.load /
                                              wheelComponent.wheelController.maximumTireLoad;
                            loadCoeff = loadCoeff > 1f ? 1f : loadCoeff;
                            newPitch = surfacePreset.skidSoundPitch + loadCoeff * 0.3f;
                            newPitch = Mathf.Lerp(_prevPitch[i], newPitch, vc.deltaTime * 18f);
                        }
                    }
                    else
                    {
                        newVolume = 0;
                        newPitch = basePitch;
                    }

                    SetVolume(newVolume, i);
                    SetPitch(newPitch, i);

                    _prevVolume[i] = newVolume;
                    _prevPitch[i] = newPitch;
                }
            }
        }


        public override void SetDefaults(VehicleController vc)
        {
            base.SetDefaults(vc);

            baseVolume = 0.5f;
            basePitch = 1.2f;

            // Clip set throught surface map system
        }
    }
}