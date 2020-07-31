#if UNITY_2018_1_OR_NEWER
#define VLB_SRP_SUPPORT // Comment this to disable SRP support
#endif

#if VLB_SRP_SUPPORT
#if UNITY_2019_1_OR_NEWER
using AliasCurrentPipeline = UnityEngine.Rendering.RenderPipelineManager;
using AliasCameraEvents = UnityEngine.Rendering.RenderPipelineManager;
using CallbackType = System.Action<UnityEngine.Rendering.ScriptableRenderContext, UnityEngine.Camera>;
#else
using AliasCurrentPipeline = UnityEngine.Experimental.Rendering.RenderPipelineManager;
using AliasCameraEvents = UnityEngine.Experimental.Rendering.RenderPipeline;
using CallbackType = System.Action<UnityEngine.Camera>;
#endif // UNITY_2019_1_OR_NEWER
#endif // VLB_SRP_SUPPORT

public static class SRPHelper
{
#if VLB_SRP_SUPPORT
    public static bool IsUsingCustomRenderPipeline()
    {
        return AliasCurrentPipeline.currentPipeline != null || UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset != null;
    }

    public static void RegisterOnBeginCameraRendering(CallbackType cb)
    {
        if (IsUsingCustomRenderPipeline())
        {
            AliasCameraEvents.beginCameraRendering -= cb;
            AliasCameraEvents.beginCameraRendering += cb;
        }
    }

    public static void UnregisterOnBeginCameraRendering(CallbackType cb)
    {
        if (IsUsingCustomRenderPipeline())
        {
            AliasCameraEvents.beginCameraRendering -= cb;
        }
    }
#else
    public static bool IsUsingCustomRenderPipeline() { return false; }
#endif
}
