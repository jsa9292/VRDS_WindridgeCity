// UNITY_SHADER_NO_UPGRADE

#ifndef _VLB_SHADER_PROPERTIES_INCLUDED_
#define _VLB_SHADER_PROPERTIES_INCLUDED_

#include "ShaderPropertySystem.cginc"

/// ****************************************
/// PROPERTIES DECLARATION
/// ****************************************
VLB_DEFINE_PROP_START

#if VLB_CUSTOM_INSTANCED_OBJECT_MATRICES
    VLB_DEFINE_PROP(float4x4, _LocalToWorldMatrix)
    VLB_DEFINE_PROP(float4x4, _WorldToLocalMatrix)
#endif

#if VLB_COLOR_GRADIENT_MATRIX_HIGH || VLB_COLOR_GRADIENT_MATRIX_LOW
    VLB_DEFINE_PROP(float4x4, _ColorGradientMatrix)
#else
    VLB_DEFINE_PROP(float4, _ColorFlat)
#endif

    VLB_DEFINE_PROP(half, _AlphaInside)
    VLB_DEFINE_PROP(half, _AlphaOutside)
    VLB_DEFINE_PROP(float2, _ConeSlopeCosSin)   // between -1 and +1
    VLB_DEFINE_PROP(float2, _ConeRadius)        // x = start radius ; y = end radius
    VLB_DEFINE_PROP(float, _ConeApexOffsetZ)    // > 0
    VLB_DEFINE_PROP(float, _AttenuationLerpLinearQuad)
    VLB_DEFINE_PROP(float, _DistanceFadeStart)
    VLB_DEFINE_PROP(float, _DistanceFadeEnd)
    VLB_DEFINE_PROP(float, _DistanceCamClipping)
    VLB_DEFINE_PROP(float, _FadeOutFactor)
    VLB_DEFINE_PROP(float, _FresnelPow)             // must be != 0 to avoid infinite fresnel
    VLB_DEFINE_PROP(float, _GlareFrontal)
    VLB_DEFINE_PROP(float, _GlareBehind)
    VLB_DEFINE_PROP(float, _DrawCap)
    VLB_DEFINE_PROP(float4, _CameraParams)          // xyz: object space forward vector ; w: cameraIsInsideBeamFactor (-1 : +1)

#if VLB_OCCLUSION_CLIPPING_PLANE
    VLB_DEFINE_PROP(float4, _ClippingPlaneWS)
    VLB_DEFINE_PROP(float,  _ClippingPlaneProps)
#elif VLB_OCCLUSION_DEPTH_TEXTURE
    VLB_DEFINE_PROP(float,     _DynamicOcclusionDepthProps)
#endif

#if VLB_DEPTH_BLEND
    VLB_DEFINE_PROP(float, _DepthBlendDistance)
#endif

#if VLB_NOISE_3D
    VLB_DEFINE_PROP(float4, _NoiseLocal)
    VLB_DEFINE_PROP(float4, _NoiseParam)
#endif

#ifdef VLB_SRP_API
    VLB_DEFINE_PROP(float2, _CameraBufferSizeSRP)
#endif

VLB_DEFINE_PROP_END

#if VLB_OCCLUSION_DEPTH_TEXTURE
// Setting a Texture property to a GPU instanced material is not supported, so keep it as regular property
uniform sampler2D _DynamicOcclusionDepthTexture;
#endif
/// ****************************************

#endif
