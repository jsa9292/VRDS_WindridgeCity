using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VLB
{
    [HelpURL(Consts.HelpUrlConfig)]
    public class Config : ScriptableObject
    {
        /// <summary>
        /// Override the layer on which the procedural geometry is created or not
        /// </summary>
        public bool geometryOverrideLayer = Consts.ConfigGeometryOverrideLayerDefault;

        /// <summary>
        /// The layer the procedural geometry gameObject is in (only if geometryOverrideLayer is enabled)
        /// </summary>
        public int geometryLayerID = Consts.ConfigGeometryLayerIDDefault;

        /// <summary>
        /// The tag applied on the procedural geometry gameObject
        /// </summary>
        public string geometryTag = Consts.ConfigGeometryTagDefault;

        /// <summary>
        /// Determine in which order beams are rendered compared to other objects.
        /// This way for example transparent objects are rendered after opaque objects, and so on.
        /// </summary>
        public int geometryRenderQueue = (int)Consts.ConfigGeometryRenderQueueDefault;

        /// <summary>
        /// Select the Render Pipeline (Built-In or SRP) in use.
        /// </summary>
        public RenderPipeline renderPipeline = Consts.ConfigGeometryRenderPipelineDefault;

        [System.Obsolete("Use 'renderingMode' instead")]
        public bool forceSinglePass = false;

        /// <summary>
        /// MultiPass: Use the 2 pass shader. Will generate 2 drawcalls per beam.
        /// SinglePass: Use the 1 pass shader. Will generate 1 drawcall per beam. Mandatory when using Render Pipeline such as HDRP, URP and LWRP.
        /// GPUInstancing: Dynamically batch multiple beams to combine and reduce draw calls (Feature only supported in Unity 5.6 or above). More info: https://docs.unity3d.com/Manual/GPUInstancing.html
        /// </summary>
        public RenderingMode renderingMode = Consts.ConfigGeometryRenderingModeDefault;

        /// <summary>
        /// Actual Rendering Mode used on the current platform
        /// </summary>
        public RenderingMode actualRenderingMode { get { return (renderingMode == RenderingMode.GPUInstancing && !GpuInstancing.isSupported) ? RenderingMode.SinglePass : renderingMode; } }

        /// <summary>
        /// Depending on the actual Rendering Mode used, returns true if the single pass shader will be used, false otherwise.
        /// </summary>
        public bool useSinglePassShader { get { return actualRenderingMode != RenderingMode.MultiPass; } }

        /// <summary>
        /// Main shaders applied to the cone beam geometry
        /// </summary>
        [SerializeField, HighlightNull] Shader beamShader1Pass = null;
   
        [FormerlySerializedAs("BeamShader"), FormerlySerializedAs("beamShader")]
        [SerializeField, HighlightNull] Shader beamShader2Pass = null;

        [SerializeField, HighlightNull] Shader beamShaderSRP = null;

        public Shader beamShader
        {
            get
            {
                if (ShouldEnableSrpApi(renderPipeline)) return beamShaderSRP;
                else return useSinglePassShader ? beamShader1Pass : beamShader2Pass;
            }
        }

        /// <summary>
        /// Number of Sides of the shared cone mesh
        /// </summary>
        public int sharedMeshSides = Consts.ConfigSharedMeshSides;

        /// <summary>
        /// Number of Segments of the shared cone mesh
        /// </summary>
        public int sharedMeshSegments = Consts.ConfigSharedMeshSegments;

        /// <summary>
        /// Global 3D Noise texture scaling: higher scale make the noise more visible, but potentially less realistic.
        /// </summary>
        [Range(Consts.NoiseScaleMin, Consts.NoiseScaleMax)]
        public float globalNoiseScale = Consts.NoiseScaleDefault;

        /// <summary>
        /// Global World Space direction and speed of the noise scrolling, simulating the fog/smoke movement
        /// </summary>
        public Vector3 globalNoiseVelocity = Consts.NoiseVelocityDefault;

        /// <summary>
        /// 3D Noise param sent to the shader
        /// </summary>
        public Vector4 globalNoiseParam { get { return new Vector4(globalNoiseVelocity.x, globalNoiseVelocity.y, globalNoiseVelocity.z, globalNoiseScale); } }

        /// <summary>
        /// Tag used to retrieve the camera used to compute the fade out factor on beams
        /// </summary>
        public string fadeOutCameraTag = Consts.ConfigFadeOutCameraTagDefault;

        public Transform fadeOutCameraTransform
        {
            get
            {
                if (m_CachedFadeOutCamera == null)
                {
                    ForceUpdateFadeOutCamera();
                }

                return m_CachedFadeOutCamera;
            }
        }

        /// <summary>
        /// Call this function if you want to manually change the fadeOutCameraTag property at runtime
        /// </summary>
        public void ForceUpdateFadeOutCamera()
        {
            var gao = GameObject.FindGameObjectWithTag(fadeOutCameraTag);
            if (gao)
                m_CachedFadeOutCamera = gao.transform;
        }

        /// <summary>
        /// Binary file holding the 3D Noise texture data (a 3D array). Must be exactly Size * Size * Size bytes long.
        /// </summary>
        [HighlightNull]
        public TextAsset noise3DData = null;

        /// <summary>
        /// Size (of one dimension) of the 3D Noise data. Must be power of 2. So if the binary file holds a 32x32x32 texture, this value must be 32.
        /// </summary>
        public int noise3DSize = Consts.ConfigNoise3DSizeDefault;

        /// <summary>
        /// ParticleSystem prefab instantiated for the Volumetric Dust Particles feature (Unity 5.5 or above)
        /// </summary>
        [HighlightNull]
        public ParticleSystem dustParticlesPrefab = null;

        // INTERNAL
#pragma warning disable 0414
        [SerializeField] int pluginVersion = -1;
#pragma warning restore 0414

        Transform m_CachedFadeOutCamera = null;

        [RuntimeInitializeOnLoadMethod]
        static void OnStartup()
        {
            Instance.m_CachedFadeOutCamera = null;
            OnRenderPipelineChanged(Instance.renderPipeline);
        }

        static bool ShouldEnableSrpApi(RenderPipeline pipeline)
        {
            return BeamGeometry.isCustomRenderPipelineSupported && pipeline == RenderPipeline.SRP_4_0_0_or_higher;
        }

        public static void OnRenderPipelineChanged(RenderPipeline pipeline)
        {
            bool enableSrpApi = ShouldEnableSrpApi(pipeline);
            Utils.SetShaderKeywordEnabled(MaterialKeywordSRP.kKeyword, enableSrpApi);

#if UNITY_EDITOR
            if(enableSrpApi)    MaterialKeywordSRP.Create(Instance.beamShaderSRP);
            else                MaterialKeywordSRP.Delete(Instance.beamShaderSRP);
#endif
        }

        public void Reset()
        {
            ResetInternalData();

            geometryOverrideLayer = Consts.ConfigGeometryOverrideLayerDefault;
            geometryLayerID = Consts.ConfigGeometryLayerIDDefault;
            geometryTag = Consts.ConfigGeometryTagDefault;
            geometryRenderQueue = (int)Consts.ConfigGeometryRenderQueueDefault;

            sharedMeshSides = Consts.ConfigSharedMeshSides;
            sharedMeshSegments = Consts.ConfigSharedMeshSegments;

            globalNoiseScale = Consts.NoiseScaleDefault;
            globalNoiseVelocity = Consts.NoiseVelocityDefault;

            renderPipeline = Consts.ConfigGeometryRenderPipelineDefault;
            renderingMode = Consts.ConfigGeometryRenderingModeDefault;

#if UNITY_EDITOR
            GlobalMesh.Destroy();
            VolumetricLightBeam._EditorSetAllMeshesDirty();
            OnRenderPipelineChanged(renderPipeline);
#endif
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            noise3DSize = Mathf.Max(2, Mathf.NextPowerOfTwo(noise3DSize));

            sharedMeshSides = Mathf.Clamp(sharedMeshSides, Consts.GeomSidesMin, Consts.GeomSidesMax);
            sharedMeshSegments = Mathf.Clamp(sharedMeshSegments, Consts.GeomSegmentsMin, Consts.GeomSegmentsMax);
        }
#endif

        public void ResetInternalData()
        {
            beamShader1Pass = Shader.Find("Hidden/VolumetricLightBeam1Pass");
            beamShader2Pass = Shader.Find("Hidden/VolumetricLightBeam2Pass");
            if (BeamGeometry.isCustomRenderPipelineSupported)
                beamShaderSRP = Shader.Find("Hidden/VolumetricLightBeamSRP");

            noise3DData = Resources.Load("Noise3D_64x64x64") as TextAsset;
            noise3DSize = Consts.ConfigNoise3DSizeDefault;

            dustParticlesPrefab = Resources.Load("DustParticles", typeof(ParticleSystem)) as ParticleSystem;
        }

        public ParticleSystem NewVolumetricDustParticles()
        {
            if (!dustParticlesPrefab)
            {
                if (Application.isPlaying)
                {
                    Debug.LogError("Failed to instantiate VolumetricDustParticles prefab.");
                }
                return null;
            }

            var instance = Instantiate(dustParticlesPrefab);
#if UNITY_5_4_OR_NEWER
            instance.useAutoRandomSeed = false;
#endif
            instance.name = "Dust Particles";
            instance.gameObject.hideFlags = Consts.ProceduralObjectsHideFlags;
            instance.gameObject.SetActive(true);
            return instance;
        }

#if UNITY_EDITOR
        public static void EditorSelectInstance()
        {
            Selection.activeObject = Config.Instance;
            if(Selection.activeObject == null)
                Debug.LogErrorFormat("Cannot find any Config resource");
        }
#endif

        void OnEnable()
        {
            HandleBackwardCompatibility(pluginVersion, Version.Current);
            pluginVersion = Version.Current;

#if UNITY_EDITOR
            var instanceNoAssert = GetInstance(false);
            if (instanceNoAssert != null)
                OnRenderPipelineChanged(instanceNoAssert.renderPipeline);
#endif
        }

        void HandleBackwardCompatibility(int serializedVersion, int newVersion)
        {
            if (serializedVersion == newVersion) return;    // same version: nothing to do

            if (serializedVersion < 1600)
            {
#pragma warning disable 0618
                renderingMode = forceSinglePass ? RenderingMode.SinglePass : RenderingMode.MultiPass;
#pragma warning restore 0618
            }

            if (serializedVersion < 1760)
            {
                beamShaderSRP = Shader.Find("Hidden/VolumetricLightBeamSRP");
            }

            Utils.MarkObjectDirty(this);
        }

        const string kAssetName = "Config";

        // Singleton management
        static Config m_Instance = null;
        public static Config Instance { get { return GetInstance(true); } }

        private static Config GetInstance(bool assertIfNotFound)
        {
#if UNITY_EDITOR
            // Do not cache the instance during editing in order to handle new asset created or moved.
            if (!Application.isPlaying || m_Instance == null)
#else
                if (m_Instance == null)
#endif
            {
                var foundOverride = Resources.Load<ConfigOverride>(ConfigOverride.kAssetName);
                if (foundOverride)
                {
                    m_Instance = foundOverride;
                }
                else
                {
                    var found = Resources.Load<Config>(Config.kAssetName);
                    Debug.Assert(!assertIfNotFound || found != null, string.Format("Can't find any resource of type '{0}'. Make sure you have a ScriptableObject of this type in a 'Resources' folder.", typeof(Config)));
                    m_Instance = found;
                }
            }

            return m_Instance;
        }
    }
}
