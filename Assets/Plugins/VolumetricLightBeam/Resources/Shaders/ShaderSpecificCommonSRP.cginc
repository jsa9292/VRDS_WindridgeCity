// The following comment prevents Unity from auto upgrading the shader. Please keep it to keep backward compatibility.
// UNITY_SHADER_NO_UPGRADE

#ifndef _VLB_SHADER_SPECIFIC_COMMON_SRP_INCLUDED_
#define _VLB_SHADER_SPECIFIC_COMMON_SRP_INCLUDED_

// Redefine missing functions from legacy pipeline

inline float4 ComputeNonStereoScreenPos(float4 pos)
{
    float4 o = pos * 0.5f;
    o.xy = float2(o.x, o.y * _ProjectionParams.x) + o.w;
    o.zw = pos.zw;
    return o;
}

#define TransformStereoScreenSpaceTex(uv, w) uv

inline float4 ComputeScreenPos(float4 pos)
{
    float4 o = ComputeNonStereoScreenPos(pos);
#if defined(UNITY_SINGLE_PASS_STEREO)
    o.xy = TransformStereoScreenSpaceTex(o.xy, pos.w);
#endif
    return o;
}

#endif
