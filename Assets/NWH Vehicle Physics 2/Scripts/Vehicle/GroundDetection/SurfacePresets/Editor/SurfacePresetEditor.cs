using NWH.NUI;
using UnityEditor;

namespace NWH.VehiclePhysics2.GroundDetection
{
    /// <summary>
    ///     Editor for SurfacePreset.
    /// </summary>
    [CustomEditor(typeof(SurfacePreset))]
    [CanEditMultipleObjects]
    public class SurfacePresetEditor : NUIEditor
    {
        private SurfacePreset _surfacePreset;

        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
            {
                return false;
            }

            _surfacePreset = (SurfacePreset) target;

            drawer.BeginSubsection("General");
            drawer.Field("name");
            drawer.EndSubsection();

            drawer.BeginSubsection("Friction Settings");
            drawer.Field("frictionPreset");
            drawer.EndSubsection();

            drawer.BeginSubsection("Skidmark Settings");
            if (drawer.Field("drawSkidmarks").boolValue)
            {
                drawer.Field("skidmarkMaterial");
                drawer.Field("slipFactor");
            }
            drawer.EndSubsection();

            drawer.BeginSubsection("Dust/Smoke Particle Settings");
            if (drawer.Field("emitParticles").boolValue)
            {
                drawer.Field("particleType");
                drawer.Field("particleSize");
                drawer.Field("particleColor");
                drawer.Field("particleMaxAlpha");
                drawer.Field("maxParticleEmissionRateOverDistance");
                drawer.Field("particleLifeDistance");
                drawer.Field("maxParticleLifetime");
            }

            drawer.EndSubsection();
            
            drawer.BeginSubsection("Chunk Particle Settings");
            if (drawer.Field("emitChunks").boolValue)
            {
                drawer.Field("maxChunkEmissionRateOverDistance");
                drawer.Field("chunkLifeDistance");
                drawer.Field("maxChunkLifetime");
            }
            drawer.EndSubsection();

            drawer.BeginSubsection("Sound Settings");
            drawer.BeginSubsection("Skid Sounds");
            if (drawer.Field("playSkidSounds").boolValue)
            {
                drawer.Field("skidSoundVolume");
                drawer.Field("skidSoundPitch");
                drawer.Field("skidSoundClip");
            }
            drawer.EndSubsection();
            drawer.BeginSubsection("Surface Sounds");
            if (drawer.Field("playSurfaceSounds").boolValue)
            {
                drawer.Field("slipSensitiveSurfaceSound");
                drawer.Field("surfaceSoundVolume");
                drawer.Field("surfaceSoundPitch");
                drawer.Field("surfaceSoundClip");
            }

            drawer.EndSubsection();
            drawer.EndSubsection();

            drawer.EndEditor(this);
            return true;
        }
    }
}