using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace NWH.VehiclePhysics2.Sound.SoundComponents
{
    /// <summary>
    ///     Base class for all vehicle SoundComponents.
    ///     Inserts a layer above Unity's AudioSource(s) which insures that the values are set properly, master volume is used,
    ///     etc.
    ///     Supports multiple AudioSources/AudioClips per one SoundComponent for random clip switching.
    /// </summary>
    [Serializable]
    public abstract class SoundComponent : VehicleComponent
    {
        /// <summary>
        ///     AudioMixerGroup that this SoundComponent belongs to.
        /// </summary>
        [Tooltip("    AudioMixerGroup that this SoundComponent belongs to.")]
        public AudioMixerGroup audioMixerGroup;

        /// <summary>
        ///     Base pitch of the sound component.
        /// </summary>
        [FormerlySerializedAs("pitch")]
        [Range(0f, 2f)]
        [Tooltip("    Base pitch of the sound component.")]
        public float basePitch = 1f;

        /// <summary>
        ///     Base volume of the sound component.
        /// </summary>
        [FormerlySerializedAs("volume")]
        [Range(0f, 1f)]
        [Tooltip("    Base volume of the sound component.")]
        public float baseVolume = 0.1f;

        /// <summary>
        ///     List of audio clips this component can use. Some components can use multiple clips in which case they will be
        ///     chosen at random, and some components can use only one
        ///     in which case only the first clip will be selected. Check manual for more details.
        /// </summary>
        [Tooltip(
            "List of audio clips this component can use. Some components can use multiple clips in which case they will be chosen at random, and some components can use only one " +
            "in which case only the first clip will be selected. Check manual for more details.")]
        public List<AudioClip> clips = new List<AudioClip>();

        /// <summary>
        ///     Container to which this SoundComponent belongs to. All AudioSources will be attached to this object.
        /// </summary>
        [Tooltip(
            "Container to which this SoundComponent belongs to. All AudioSources will be attached to this object.")]
        public GameObject container;

        [NonSerialized]
        protected List<AudioSource> sources = new List<AudioSource>();

        /// <summary>
        ///     Gets or sets the first clip in the clips list.
        /// </summary>
        public AudioClip Clip
        {
            get { return clips.Count > 0 ? clips[0] : null; }
            set
            {
                if (clips.Count > 0)
                {
                    clips[0] = value;
                }
                else
                {
                    clips.Add(value);
                }
            }
        }

        /// <summary>
        ///     Gets or sets the whole clip list.
        /// </summary>
        public List<AudioClip> Clips
        {
            get { return clips; }
            set { clips = value; }
        }

        /// <summary>
        ///     Gets a random clip from clips list.
        /// </summary>
        public AudioClip RandomClip
        {
            get { return clips[Random.Range(0, clips.Count)]; }
        }

        /// <summary>
        ///     Gets or sets the first audio source in the sources list.
        /// </summary>
        public AudioSource Source
        {
            get
            {
                if (sources.Count > 0)
                {
                    return sources[0];
                }

                return null;
            }
            set
            {
                if (sources.Count > 0)
                {
                    sources[0] = value;
                }

                sources.Add(value);
            }
        }

        /// <summary>
        ///     AudioSources belonging to this SoundComponent.
        /// </summary>
        public List<AudioSource> Sources
        {
            get { return sources; }
            set { sources = value; }
        }

        /// <summary>
        ///     Enables all the AudioSources belonging to this SoundComponent.
        ///     Calls Play() on all the looping sources.
        /// </summary>
        public override void Enable()
        {
            base.Enable();

            foreach (AudioSource source in sources)
            {
                if (!source.enabled)
                {
                    source.enabled = true;
                    if (source.loop)
                    {
                        source.Play();
                    }
                }
            }
        }

        /// <summary>
        ///     Disables all the AudioSources belonging to this SoundComponent.
        ///     Will call Stop() as well as disable the source.
        /// </summary>
        public override void Disable()
        {
            base.Disable();

            foreach (AudioSource source in sources)
            {
                if (source.isPlaying)
                {
                    Stop();
                }

                if (source.enabled)
                {
                    source.enabled = false;
                }

                source.volume = 0;
            }
        }

        /// <summary>
        ///     Adds outputs of sources to the mixer.
        /// </summary>
        public void AddSourcesToMixer()
        {
            foreach (AudioSource source in sources)
            {
                source.outputAudioMixerGroup = audioMixerGroup;
            }
        }

        /// <summary>
        ///     Gets pitch of the Source. Equal to Source.pitch.
        /// </summary>
        public float GetPitch()
        {
            if (!Active)
            {
                return 0;
            }

            return Source.pitch;
        }

        /// <summary>
        ///     Gets volume of the Source. Equal to Source.volume.
        /// </summary>
        public float GetVolume()
        {
            if (!Active)
            {
                return 0;
            }

            return Source.volume;
        }

        /// <summary>
        ///     Plays the source if it is not already playing.
        /// </summary>
        public void Play()
        {
            if (!Active)
            {
                return;
            }

            if (Source.isPlaying)
            {
                return;
            }

            Source.Play();
        }

        /// <summary>
        ///     Plays the source at index if not already playing.
        /// </summary>
        /// <param name="index">Index of the source to play.</param>
        public void Play(int index)
        {
            if (!Active)
            {
                return;
            }

            AudioSource s = Sources[index];
            if (s.isPlaying)
            {
                return;
            }

            s.Play();
        }


        /// <summary>
        ///     Sets pitch for the [id]th source in sources list.
        /// </summary>
        /// <param name="pitch">Pitch to set.</param>
        /// <param name="index">Index of the source.</param>
        public void SetPitch(float pitch, int index)
        {
            if (!Active)
            {
                return;
            }

            pitch = pitch < 0 ? 0 : pitch > 5 ? 5 : pitch;
            sources[index].pitch = pitch;
        }


        /// <summary>
        ///     Sets pitch for the first source in sources list.
        /// </summary>
        /// <param name="pitch">Pitch to set.</param>
        public void SetPitch(float pitch)
        {
            if (!Active)
            {
                return;
            }

            pitch = pitch < 0 ? 0 : pitch > 5 ? 5 : pitch;
            Source.pitch = pitch;
        }


        /// <summary>
        ///     Sets volume for the [id]th source in sources list. Use instead of directly changing source volume as this takes
        ///     master volume into account.
        /// </summary>
        /// <param name="volume">Volume to set.</param>
        /// <param name="index">Index of the target source.</param>
        public void SetVolume(float volume, int index)
        {
            if (!Active)
            {
                return;
            }

            volume = volume < 0 ? 0 : volume > 2 ? 2 : volume;
            sources[index].volume = volume * vc.soundManager.masterVolume;
        }

        /// <summary>
        ///     Sets the volume of AudioSource. Takes master volume into account.
        /// </summary>
        /// <param name="volume">Volume to set.</param>
        /// <param name="source">Target AudioSource.</param>
        public void SetVolume(float volume, AudioSource source)
        {
            if (!Active)
            {
                return;
            }

            volume = volume < 0 ? 0 : volume > 2 ? 2 : volume;
            source.volume = volume * vc.soundManager.masterVolume;
        }

        /// <summary>
        ///     Sets volume for the first source in sources list. Use instead of directly changing source volume as this takes
        ///     master volume into account.
        /// </summary>
        /// <param name="volume">Volume to set.</param>
        public void SetVolume(float volume)
        {
            if (!Active)
            {
                return;
            }

            Source.volume = volume * vc.soundManager.masterVolume;
        }

        /// <summary>
        ///     Stops the AudioSource if already playing.
        /// </summary>
        public void Stop()
        {
            if (!Source.isPlaying)
            {
                return;
            }

            Source.Stop();
        }

        /// <summary>
        ///     Stops the AudioSource at index if already playing.
        /// </summary>
        /// <param name="index">Target AudioSource index.</param>
        public void Stop(int index)
        {
            AudioSource s = Sources[index];
            if (!s.isPlaying)
            {
                return;
            }

            s.Stop();
        }
    }
}