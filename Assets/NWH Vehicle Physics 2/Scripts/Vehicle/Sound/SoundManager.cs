using System;
using System.Collections.Generic;
using NWH.VehiclePhysics2.Sound.SoundComponents;
using UnityEngine;
using UnityEngine.Audio;

#if UNITY_EDITOR
#endif


namespace NWH.VehiclePhysics2.Sound
{
    /// <summary>
    ///     Main class that manages all the sound aspects of the vehicle.
    /// </summary>
    [Serializable]
    public class SoundManager : VehicleComponent
    {
        /// <summary>
        ///     Tick-tock sound of a working blinker. First clip is played when blinker is turning on and second clip is played
        ///     when blinker is turning off.
        /// </summary>
        [Tooltip(
            "Tick-tock sound of a working blinker. First clip is played when blinker is turning on and second clip is played when blinker is turning off.")]
        public BlinkerComponent blinkerComponent = new BlinkerComponent();

        /// <summary>
        ///     Sound of air brakes releasing air. Supports multiple clips.
        /// </summary>
        [Tooltip("    Sound of air brakes releasing air. Supports multiple clips.")]
        public BrakeHissComponent brakeHissComponent = new BrakeHissComponent();

        /// <summary>
        ///     List of all SoundComponents.
        ///     Empty before the sound manager is initialized.
        ///     If using external sound components add them to this list so they get updated.
        /// </summary>
        [Tooltip(
            "List of all SoundComponents.\r\nEmpty before the sound manager is initialized.\r\nIf using external sound components add them to this list so they get updated.")]
        public List<SoundComponent> components = new List<SoundComponent>();

        /// <summary>
        ///     Sound of vehicle hitting other objects. Supports multiple clips.
        /// </summary>
        [Tooltip("    Sound of vehicle hitting other objects. Supports multiple clips.")]
        public CrashComponent crashComponent = new CrashComponent();

        /// <summary>
        ///     Mixer group for crash sound effects.
        /// </summary>
        [Tooltip("    Mixer group for crash sound effects.")]
        public AudioMixerGroup crashMixerGroup;

        /// <summary>
        ///     GameObject containing all the crash audio sources.
        /// </summary>
        [Tooltip("    GameObject containing all the crash audio sources.")]
        public GameObject crashSourceGO;

        public EngineFanComponent engineFanComponent = new EngineFanComponent();

        public AudioMixerGroup engineMixerGroup;

        /// <summary>
        ///     Sound of engine idling.
        /// </summary>
        [Tooltip("    Sound of engine idling.")]
        public EngineRunningComponent engineRunningComponent = new EngineRunningComponent();

        /// <summary>
        ///     GameObject containing all the engine audio sources.
        /// </summary>
        [Tooltip("    GameObject containing all the engine audio sources.")]
        public GameObject engineSourceGO;

        /// <summary>
        ///     Engine start / stop component. First clip is for starting and second one is for stopping.
        /// </summary>
        [Tooltip("    Engine start / stop component. First clip is for starting and second one is for stopping.")]
        public EngineStartComponent engineStartComponent = new EngineStartComponent();

        /// <summary>
        ///     GameObject containing all the exhaust audio sources.
        /// </summary>
        [Tooltip("    GameObject containing all the exhaust audio sources.")]
        public GameObject exhaustSourceGO;

        /// <summary>
        ///     Sound from changing gears. Supports multiple clips.
        /// </summary>
        [Tooltip("    Sound from changing gears. Supports multiple clips.")]
        public GearChangeComponent gearChangeComponent = new GearChangeComponent();

        [Tooltip("Horn sound.")]
        public HornComponent hornComponent = new HornComponent();

        /// <summary>
        ///     Set to true if listener inside vehicle. Mixer must be set up.
        /// </summary>
        [Tooltip("    Set to true if listener inside vehicle. Mixer must be set up.")]
        public bool insideVehicle;

        /// <summary>
        ///     Sound attenuation inside vehicle.
        /// </summary>
        [Tooltip("    Sound attenuation inside vehicle.")]
        public float interiorAttenuation = -7f;

        public float lowPassFrequency = 1600f;

        [Range(0.01f, 10f)]
        public float lowPassQ = 1f;

        public AudioMixerGroup masterGroup;

        /// <summary>
        ///     Master volume of a vehicle. To adjust volume of all vehicles or their components check audio mixer.
        /// </summary>
        [Range(0, 2)]
        [Tooltip(
            "    Master volume of a vehicle. To adjust volume of all vehicles or their components check audio mixer.")]
        public float masterVolume = 1f;

        /// <summary>
        ///     Optional custom mixer. If left empty default will be used (VehicleAudioMixer in Resources folder).
        /// </summary>
        [Tooltip(
            "    Optional custom mixer. If left empty default will be used (VehicleAudioMixer in Resources folder).")]
        public AudioMixer mixer;

        public AudioMixerGroup otherMixerGroup;

        /// <summary>
        ///     GameObject containing all other audio sources.
        /// </summary>
        [Tooltip("    GameObject containing all other audio sources.")]
        public GameObject otherSourceGO;

        public ReverseBeepComponent reverseBeepComponent = new ReverseBeepComponent();

        /// <summary>
        ///     Spatial blend of all audio sources. Can not be changed at runtime.
        /// </summary>
        [Range(0, 1)]
        [Tooltip("    Spatial blend of all audio sources. Can not be changed at runtime.")]
        public float spatialBlend = 0.9f;

        public AudioMixerGroup surfaceNoiseMixerGroup;

        /// <summary>
        ///     Sound from wheels hitting ground and/or obstracles. Supports multiple clips.
        /// </summary>
        [Tooltip("    Sound from wheels hitting ground and/or obstracles. Supports multiple clips.")]
        public SuspensionBumpComponent suspensionBumpComponent = new SuspensionBumpComponent();

        public AudioMixerGroup suspensionMixerGroup;

        public AudioMixerGroup transmissionMixerGroup;

        /// <summary>
        ///     GameObject containing all transmission audio sources.
        /// </summary>
        [Tooltip("    GameObject containing all transmission audio sources.")]
        public GameObject transmissionSourceGO;

        /// <summary>
        ///     Transmission whine from straight cut gears or just a noisy gearbox.
        /// </summary>
        [Tooltip("    Transmission whine from straight cut gears or just a noisy gearbox.")]
        public TransmissionWhineComponent transmissionWhineComponent = new TransmissionWhineComponent();

        /// <summary>
        ///     Sound of turbo's wastegate. Supports multiple clips.
        /// </summary>
        [Tooltip("    Sound of turbo's wastegate. Supports multiple clips.")]
        public TurboFlutterComponent turboFlutterComponent = new TurboFlutterComponent();

        public AudioMixerGroup turboMixerGroup;

        /// <summary>
        ///     Forced induction whistle component. Can be used for air intake noise or supercharger if spool up time is set to 0
        ///     under engine settings.
        /// </summary>
        [Tooltip(
            "Forced induction whistle component. Can be used for air intake noise or supercharger if spool up time is set to 0 under engine settings.")]
        public TurboWhistleComponent turboWhistleComponent = new TurboWhistleComponent();

        /// <summary>
        ///     Sound produced by wheel skidding over a surface. Tire squeal.
        /// </summary>
        [Tooltip("    Sound produced by wheel skidding over a surface. Tire squeal.")]
        public WheelSkidComponent wheelSkidComponent = new WheelSkidComponent();

        /// <summary>
        ///     Sound produced by wheel rolling over a surface. Tire hum.
        /// </summary>
        [Tooltip("    Sound produced by wheel rolling over a surface. Tire hum.")]
        public WheelTireNoiseComponent wheelTireNoiseComponent = new WheelTireNoiseComponent();

        private float originalAttenuation;

        private bool wasInsideVehicle;

        public override void Initialize()
        {
            if (mixer == null)
            {
                mixer = Resources.Load("Sound/VehicleAudioMixer") as AudioMixer;
            }

            if (mixer != null)
            {
                masterGroup = mixer.FindMatchingGroups("Master")[0];
                engineMixerGroup = mixer.FindMatchingGroups("Engine")[0];
                transmissionMixerGroup = mixer.FindMatchingGroups("Transmission")[0];
                surfaceNoiseMixerGroup = mixer.FindMatchingGroups("SurfaceNoise")[0];
                turboMixerGroup = mixer.FindMatchingGroups("Turbo")[0];
                suspensionMixerGroup = mixer.FindMatchingGroups("Suspension")[0];
                crashMixerGroup = mixer.FindMatchingGroups("Crash")[0];
                otherMixerGroup = mixer.FindMatchingGroups("Other")[0];
            }

            // Remember initial states
            mixer.GetFloat("attenuation", out originalAttenuation);

            // Initialize individual sound components
            engineStartComponent.audioMixerGroup = engineMixerGroup;
            engineStartComponent.container = engineSourceGO;
            engineStartComponent.Initialize();

            engineRunningComponent.audioMixerGroup = engineMixerGroup;
            engineRunningComponent.container = engineSourceGO;
            engineRunningComponent.Initialize();

            engineFanComponent.audioMixerGroup = engineMixerGroup;
            engineFanComponent.container = engineSourceGO;
            engineFanComponent.Initialize();

            turboWhistleComponent.audioMixerGroup = turboMixerGroup;
            turboWhistleComponent.container = engineSourceGO;
            turboWhistleComponent.Initialize();

            turboFlutterComponent.audioMixerGroup = turboMixerGroup;
            turboFlutterComponent.container = engineSourceGO;
            turboFlutterComponent.Initialize();

            transmissionWhineComponent.audioMixerGroup = transmissionMixerGroup;
            transmissionWhineComponent.container = transmissionSourceGO;
            transmissionWhineComponent.Initialize();

            gearChangeComponent.audioMixerGroup = transmissionMixerGroup;
            gearChangeComponent.container = transmissionSourceGO;
            gearChangeComponent.Initialize();

            brakeHissComponent.audioMixerGroup = otherMixerGroup;
            brakeHissComponent.container = otherSourceGO;
            brakeHissComponent.Initialize();

            blinkerComponent.audioMixerGroup = otherMixerGroup;
            blinkerComponent.container = otherSourceGO;
            blinkerComponent.Initialize();

            hornComponent.audioMixerGroup = otherMixerGroup;
            hornComponent.container = otherSourceGO;
            hornComponent.Initialize();

            wheelSkidComponent.audioMixerGroup = surfaceNoiseMixerGroup;
            wheelSkidComponent.container = otherSourceGO;
            wheelSkidComponent.Initialize();

            wheelTireNoiseComponent.audioMixerGroup = surfaceNoiseMixerGroup;
            wheelTireNoiseComponent.container = otherSourceGO;
            wheelTireNoiseComponent.Initialize();

            crashComponent.audioMixerGroup = crashMixerGroup;
            crashComponent.container = crashSourceGO;
            crashComponent.Initialize();

            suspensionBumpComponent.audioMixerGroup = suspensionMixerGroup;
            suspensionBumpComponent.container = otherSourceGO;
            suspensionBumpComponent.Initialize();

            reverseBeepComponent.audioMixerGroup = otherMixerGroup;
            reverseBeepComponent.container = otherSourceGO;
            reverseBeepComponent.Initialize();

            initialized = true;
        }

        public override void Awake(VehicleController vc)
        {
            base.Awake(vc);

            GetComponentsList(ref components);
            foreach (SoundComponent component in components)
            {
                component.Awake(vc);
            }

            // Create container game objects for positional audio
            CreateSourceGO("EngineAudioSources", vc.enginePosition, vc.transform, ref engineSourceGO);
            CreateSourceGO("TransmissionAudioSources", vc.transmissionPosition, vc.transform, ref transmissionSourceGO);
            CreateSourceGO("ExhaustAudioSources", vc.exhaustPosition, vc.transform, ref exhaustSourceGO);
            CreateSourceGO("CrashAudioSources", Vector3.zero, vc.transform, ref crashSourceGO);
            CreateSourceGO("OtherAudioSources", new Vector3(0, 0.2f, 0), vc.transform, ref otherSourceGO);
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

            // Adjust sound if inside vehicle.
            if (!wasInsideVehicle && insideVehicle)
            {
                mixer.SetFloat("attenuation", interiorAttenuation);
                mixer.SetFloat("lowPassFrequency", lowPassFrequency);
                mixer.SetFloat("lowPassQ", lowPassQ);
            }
            else if (wasInsideVehicle && !insideVehicle)
            {
                mixer.SetFloat("attenuation", originalAttenuation);
                mixer.SetFloat("lowPassFrequency", 22000f);
                mixer.SetFloat("lowPassQ", 1f);
            }

            wasInsideVehicle = insideVehicle;

            foreach (SoundComponent sc in components)
            {
                sc.Update();
            }
        }


        public override void OnDrawGizmosSelected(VehicleController vc)
        {
            base.OnDrawGizmosSelected(vc);

            Gizmos.color = Color.white;

            if (components == null || components.Count == 0)
            {
                GetComponentsList(ref components);
            }

            foreach (SoundComponent component in components)
            {
                component.OnDrawGizmosSelected(vc);
            }
        }

        public void CreateSourceGO(string name, Vector3 localPosition, Transform parent, ref GameObject sourceGO)
        {
            sourceGO = new GameObject();
            sourceGO.name = name;
            sourceGO.transform.SetParent(parent);
            sourceGO.transform.localPosition = localPosition;
        }

        public override void CheckState(int lodIndex)
        {
            base.CheckState(lodIndex);

            foreach (VehicleComponent component in components)
            {
                component.CheckState(lodIndex);
            }
        }

        /// <summary>
        ///     Initializes audio source to it's starting values.
        /// </summary>
        /// <param name="audioSource">AudioSource in question.</param>
        /// <param name="play">Play on awake?</param>
        /// <param name="loop">Should clip be looped?</param>
        /// <param name="volume">Volume of the audio source.</param>
        /// <param name="clip">Clip that will be set at the start.</param>
        public void SetAudioSourceDefaults(AudioSource audioSource, bool play = false, bool loop = false,
            float volume = 0f, AudioClip clip = null)
        {
            if (audioSource != null)
            {
                audioSource.spatialBlend = spatialBlend;
                audioSource.playOnAwake = play;
                audioSource.loop = loop;
                audioSource.volume = volume * vc.soundManager.masterVolume;
                audioSource.clip = clip;
                audioSource.priority = 200;

                if (play)
                {
                    if (!audioSource.isPlaying)
                    {
                        audioSource.Play();
                    }
                }
                else
                {
                    if (audioSource.isPlaying)
                    {
                        audioSource.Stop();
                    }
                }
            }
            else
            {
                Debug.LogWarning("AudioSource is null. Defaults cannot be set.");
            }
        }

        /// <summary>
        ///     Sets defaults to all the basic sound components when script is first added or reset is called.
        /// </summary>
        public override void SetDefaults(VehicleController vc)
        {
            base.SetDefaults(vc);

            if (mixer == null)
            {
                mixer = Resources.Load<AudioMixer>(VehicleController.DEFAULT_RESOURCES_PATH +
                                                   "Sound/VehicleAudioMixer");
                if (mixer == null)
                {
                    Debug.LogWarning("VehicleAudioMixer resource could not be loaded from resources.");
                }
            }

            GetComponentsList(ref components);
            foreach (SoundComponent soundComponent in components)
            {
                soundComponent.SetDefaults(vc);
            }
        }

        public override void Validate(VehicleController vc)
        {
            if (mixer == null)
            {
                Debug.LogError("Audio mixer of 'SoundManager' is not assigned.");
            }
        }

        private void GetComponentsList(ref List<SoundComponent> components)
        {
            components = new List<SoundComponent>
            {
                engineStartComponent,
                engineRunningComponent,
                engineFanComponent,
                turboWhistleComponent,
                turboFlutterComponent,
                transmissionWhineComponent,
                gearChangeComponent,
                brakeHissComponent,
                blinkerComponent,
                hornComponent,
                wheelSkidComponent,
                wheelTireNoiseComponent,
                crashComponent,
                suspensionBumpComponent,
                reverseBeepComponent
            };
        }
    }
}