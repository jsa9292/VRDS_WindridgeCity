#if UNITY_5_6_OR_NEWER
#define VLB_GPU_INSTANCING_SUPPORT
#endif

// Force isDepthBlendEnabled at true when GPU Instancing is enabled, to prevent from breaking the batch if 1 beam has it at 0 and 1 has it at > 0
#define FORCE_ENABLE_DEPTHBLEND_FOR_GPU_INSTANCING

using UnityEngine;

namespace VLB
{
    public static class GpuInstancing
    {
        /// <summary>
        /// Returns if GPU Instancing feature is supported on this Unity version or not.
        /// </summary>
#if VLB_GPU_INSTANCING_SUPPORT
        public const bool isSupported = true;
#else
        public static bool isSupported { get; private set; }
#endif

#if FORCE_ENABLE_DEPTHBLEND_FOR_GPU_INSTANCING
        public static bool forceEnableDepthBlend { get { return Config.Instance.actualRenderingMode == RenderingMode.GPUInstancing; } }
#else
        public const bool forceEnableDepthBlend = false;
#endif


        public static void SetMaterialProperties(Material material, bool enableInstancing)
        {
#if VLB_GPU_INSTANCING_SUPPORT
            Debug.Assert(material != null);
            material.enableInstancing = enableInstancing;
            material.SetKeywordEnabled("VLB_GPU_INSTANCING", enableInstancing);
#endif
        }

        public static bool CanBeBatched(VolumetricLightBeam beamA, VolumetricLightBeam beamB, ref string reasons)
        {
            bool ret = true;

            if (!CanBeBatched(beamA, ref reasons))
                ret = false;

            if (!CanBeBatched(beamB, ref reasons))
                ret = false;

            if ((beamA.GetComponent<DynamicOcclusionAbstractBase>() == null) != (beamB.GetComponent<DynamicOcclusionAbstractBase>() == null))
            {
                AppendErrorMessage(ref reasons, string.Format("{0}/{1}: dynamically occluded and non occluded beams cannot be batched together", beamA.name, beamB.name));
                ret = false;
            }

            if (beamA.colorMode != beamB.colorMode)
            {
                AppendErrorMessage(ref reasons, "Color Mode mismatch");
                ret = false;
            }

            if (beamA.blendingMode != beamB.blendingMode)
            {
                AppendErrorMessage(ref reasons, "Blending Mode mismatch");
                ret = false;
            }

            if (beamA.isNoiseEnabled != beamB.isNoiseEnabled)
            {
                AppendErrorMessage(ref reasons, "3D Noise enabled mismatch");
                ret = false;
            }

            if (!forceEnableDepthBlend)
            {
#pragma warning disable 0162
                if ((beamA.depthBlendDistance > 0) != (beamB.depthBlendDistance > 0))
                {
                    AppendErrorMessage(ref reasons, "Opaque Geometry Blending mismatch");
                    ret = false;
                }
#pragma warning restore 0162
            }

            return ret;
        }

        public static bool CanBeBatched(VolumetricLightBeam beam, ref string reasons)
        {
            bool ret = true;

            if (beam.geomMeshType != MeshType.Shared)
            {
                AppendErrorMessage(ref reasons, string.Format("{0} is not using shared mesh", beam.name));
                ret = false;
            }

            if (beam.GetComponent<DynamicOcclusionDepthBuffer>())
            {
                AppendErrorMessage(ref reasons, string.Format("{0} is using the DynamicOcclusion DepthBuffer feature", beam.name));
                ret = false;
            }
            return ret;
        }

        static void AppendErrorMessage(ref string message, string toAppend)
        {
            if (message != "") message += "\n";
            message += "- " + toAppend;
        }
    }
}
