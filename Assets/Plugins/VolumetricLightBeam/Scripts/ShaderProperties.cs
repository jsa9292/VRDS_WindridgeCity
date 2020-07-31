using UnityEngine;

namespace VLB
{
    public static class ShaderProperties
    {
        public static readonly int FadeOutFactor                = Shader.PropertyToID("_FadeOutFactor");
        public static readonly int ClippingPlaneWS              = Shader.PropertyToID("_ClippingPlaneWS");
        public static readonly int ClippingPlaneProps           = Shader.PropertyToID("_ClippingPlaneProps");
        public static readonly int ConeSlopeCosSin              = Shader.PropertyToID("_ConeSlopeCosSin");
        public static readonly int ConeRadius                   = Shader.PropertyToID("_ConeRadius");
        public static readonly int ConeApexOffsetZ              = Shader.PropertyToID("_ConeApexOffsetZ");
        public static readonly int ColorFlat                    = Shader.PropertyToID("_ColorFlat");
        public static readonly int AlphaInside                  = Shader.PropertyToID("_AlphaInside");
        public static readonly int AlphaOutside                 = Shader.PropertyToID("_AlphaOutside");
        public static readonly int AttenuationLerpLinearQuad    = Shader.PropertyToID("_AttenuationLerpLinearQuad");
        public static readonly int DistanceFadeStart            = Shader.PropertyToID("_DistanceFadeStart");
        public static readonly int DistanceFadeEnd              = Shader.PropertyToID("_DistanceFadeEnd");
        public static readonly int DistanceCamClipping          = Shader.PropertyToID("_DistanceCamClipping");
        public static readonly int FresnelPow                   = Shader.PropertyToID("_FresnelPow");
        public static readonly int GlareBehind                  = Shader.PropertyToID("_GlareBehind");
        public static readonly int GlareFrontal                 = Shader.PropertyToID("_GlareFrontal");
        public static readonly int DrawCap                      = Shader.PropertyToID("_DrawCap");
        public static readonly int DepthBlendDistance           = Shader.PropertyToID("_DepthBlendDistance");
        public static readonly int NoiseLocal                   = Shader.PropertyToID("_NoiseLocal");
        public static readonly int NoiseParam                   = Shader.PropertyToID("_NoiseParam");
        public static readonly int CameraParams                 = Shader.PropertyToID("_CameraParams");
        public static readonly int CameraBufferSizeSRP          = Shader.PropertyToID("_CameraBufferSizeSRP");
        public static readonly int ColorGradientMatrix          = Shader.PropertyToID("_ColorGradientMatrix");
        public static readonly int LocalToWorldMatrix           = Shader.PropertyToID("_LocalToWorldMatrix");
        public static readonly int WorldToLocalMatrix           = Shader.PropertyToID("_WorldToLocalMatrix");
        public static readonly int BlendSrcFactor               = Shader.PropertyToID("_BlendSrcFactor");
        public static readonly int BlendDstFactor               = Shader.PropertyToID("_BlendDstFactor");
        public static readonly int DynamicOcclusionDepthTexture = Shader.PropertyToID("_DynamicOcclusionDepthTexture");
        public static readonly int DynamicOcclusionDepthProps   = Shader.PropertyToID("_DynamicOcclusionDepthProps");
    }
}

