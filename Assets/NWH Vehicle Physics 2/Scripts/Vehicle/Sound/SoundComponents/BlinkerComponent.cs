using System;
using NWH.VehiclePhysics2.Effects;
using UnityEngine;

namespace NWH.VehiclePhysics2.Sound.SoundComponents
{
    /// <summary>
    ///     Click-clack of the working blinker.
    ///     Accepts two clips, first is for the blinker turning on and the second is for blinker turning off.
    /// </summary>
    [Serializable]
    public class BlinkerComponent : SoundComponent
    {
        private bool _onFlag;
        private bool _offFlag;

        public override void Initialize()
        {
            if (Clip != null)
            {
                Source = container.AddComponent<AudioSource>();
                vc.soundManager.SetAudioSourceDefaults(Source, false, false, baseVolume);
                AddSourcesToMixer();

                Source.dopplerLevel = 0;
            }

            initialized = true;
            
            foreach (LightSource ls in vc.effectsManager.lightsManager.leftBlinkers.lightSources)
            {
                ls.onLightTurnedOn.AddListener(() => { _onFlag = true; });
                ls.onLightTurnedOff.AddListener(() => { _offFlag = true; });
            }
            
            foreach (LightSource ls in vc.effectsManager.lightsManager.rightBlinkers.lightSources)
            {
                ls.onLightTurnedOn.AddListener(() => { _onFlag = true; });
                ls.onLightTurnedOff.AddListener(() => { _offFlag = true; });
            }
        }
        
        public override void FixedUpdate()
        {
        }

        public override void Update()
        {
            if (!Active || Clips.Count == 0)
            {
                return;
            }

            Source.volume = baseVolume;
            Source.pitch = basePitch;

            if (_onFlag)
            {
                Source.clip = Clips[0];
                Play();
            }
            
            if (_offFlag)
            {
                // Play off clip if available or play the same on clip if not.
                if (Clips.Count == 2)
                {
                    Source.clip = Clips[1];
                }
                else
                {
                    Source.clip = Clips[0];
                }

                Play();
            }

            _onFlag = false;
            _offFlag = false;
        }


        public override void SetDefaults(VehicleController vc)
        {
            base.SetDefaults(vc);

            baseVolume = 0.8f;
            basePitch = 1f;

            if (Clip == null || Clips.Count == 0)
            {
                AudioClip blinkerOn =
                    Resources.Load(VehicleController.DEFAULT_RESOURCES_PATH + "Sound/BlinkerOn") as AudioClip;
                if (blinkerOn == null)
                {
                    Debug.LogWarning(
                        $"Audio Clip for sound component {GetType().Name} could not be loaded from resources. Source will not play.");
                }
                else
                {
                    Clips.Add(blinkerOn);
                }

                AudioClip blinkerOff =
                    Resources.Load(VehicleController.DEFAULT_RESOURCES_PATH + "Sound/BlinkerOff") as AudioClip;
                if (blinkerOff == null)
                {
                    Debug.LogWarning(
                        $"Audio Clip for sound component {GetType().Name} could not be loaded from resources. Source will not play.");
                }
                else
                {
                    Clips.Add(blinkerOff);
                }
            }
        }
    }
}