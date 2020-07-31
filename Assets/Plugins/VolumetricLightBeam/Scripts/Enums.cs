namespace VLB
{
    public enum ColorMode
    {
        Flat,       // Apply a flat/plain/single color
        Gradient    // Apply a gradient
    }

    public enum AttenuationEquation
    {
        Linear = 0,     // Simple linear attenuation.
        Quadratic = 1,  // Quadratic attenuation, which usually gives more realistic results.
        Blend = 2       // Custom blending mix between linear and quadratic attenuation formulas. Use attenuationEquation property to tweak the mix.
    }

    public enum BlendingMode
    {
        Additive,
        SoftAdditive,
        TraditionalTransparency,
    }

    public enum NoiseMode
    {
        /// <summary> 3D Noise is disabled </summary>
        Disabled,
        /// <summary> 3D Noise is enabled: noise will look static compared to the world </summary>
        WorldSpace,
        /// <summary> 3D Noise is enabled: noise will look static compared to the beam position </summary>
        LocalSpace,
    }

    public enum MeshType
    {
        Shared, // Use the global shared mesh (recommended setting, since it will save a lot on memory). Will use the geometry properties set on Config.
        Custom, // Use a custom mesh instead. Will use the geometry properties set on the beam.
    }

    public enum RenderPipeline
    {
        /// <summary> Unity's built-in Render Pipeline. </summary>
        BuiltIn,
        /// <summary> Use a Scriptable Render Pipeline (HDRP, URP, LWRP or a custom one) with Core RP 4.0.0 or higher. </summary>
        SRP_4_0_0_or_higher,
    }

    public enum RenderingMode
    {
        /// <summary> Use the 2 pass shader. Will generate 2 drawcalls per beam (Not compatible with custom Render Pipeline such as HDRP and LWRP).</summary>
        MultiPass,
        /// <summary> Use the 1 pass shader. Will generate 1 drawcall per beam. </summary>
        SinglePass,
        /// <summary> Dynamically batch multiple beams to combine and reduce draw calls. </summary>
        GPUInstancing,
    }

    public enum RenderQueue
    {
        /// Specify a custom render queue.
        Custom = 0,

        /// This render queue is rendered before any others.
        Background = 1000,

        /// Opaque geometry uses this queue.
        Geometry = 2000,

        /// Alpha tested geometry uses this queue.
        AlphaTest = 2450,

        /// Last render queue that is considered "opaque".
        GeometryLast = 2500,

        /// This render queue is rendered after Geometry and AlphaTest, in back-to-front order.
        Transparent = 3000,

        /// This render queue is meant for overlay effects.
        Overlay = 4000,
    }

    public enum OccluderDimensions
    {
        /// <summary> the beam will react against 3D Occluders. </summary>
        Occluders3D,

        /// <summary> the beam will react against 2D Occluders. This is useful when using the beams with 2D objects (such as 2D Sprites). </summary>
        Occluders2D
    }

    public enum PlaneAlignment
    {
        /// <summary>Align the plane to the surface normal which blocks the beam. Works better for large occluders such as floors and walls.</summary>
        Surface,
        /// <summary>Keep the plane aligned with the beam direction. Works better with more complex occluders or with corners.</summary>
        Beam
    }

    [System.Flags]
    public enum DynamicOcclusionUpdateRate
    {
        Never = 1 << 0,
        OnEnable = 1 << 1,
        OnBeamMove = 1 << 2,
        EveryXFrames = 1 << 3,
        OnBeamMoveAndEveryXFrames = OnBeamMove | EveryXFrames,
    }
}
