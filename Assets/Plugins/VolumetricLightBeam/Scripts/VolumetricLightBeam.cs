//#define DEBUG_SHOW_APEX

using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;

namespace VLB
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [SelectionBase]
    [HelpURL(Consts.HelpUrlBeam)]
    public partial class VolumetricLightBeam : MonoBehaviour
    {
        /// <summary>
        /// Get the color value from the light (when attached to a Spotlight) or not
        /// </summary>
        public bool colorFromLight = true;

        /// <summary>
        /// Apply a flat/plain/single color, or a gradient
        /// </summary>
        public ColorMode colorMode = Consts.ColorModeDefault;

        /// <summary>
        /// RGBA plain color, if colorMode is Flat (takes account of the alpha value).
        /// </summary>
#if UNITY_2018_1_OR_NEWER
        [ColorUsageAttribute(false, true)]
#else
        [ColorUsageAttribute(false, true, 0f, 8f, 0.125f, 3f)]
#endif
        [FormerlySerializedAs("colorValue")]
        public Color color = Consts.FlatColor;

        /// <summary>
        /// Gradient color applied along the beam, if colorMode is Gradient (takes account of the color and alpha variations).
        /// </summary>
        public Gradient colorGradient;

        /// <summary>
        /// Get the intensity value from the light (when attached to a Spotlight) or not
        /// </summary>
        public bool intensityFromLight = true;

        /// <summary>
        /// Disabled: the inside and outside intensity values are the same and controlled by intensityGlobal property
        /// Enabled: the inside and outside intensity values are distinct (intensityInside and intensityOutside)
        /// </summary>
        public bool intensityModeAdvanced = false;

        /// <summary>
        /// Beam inside intensity (when looking at the beam from the inside directly at the source).
        /// </summary>
        [FormerlySerializedAs("alphaInside")]
        [Range(Consts.IntensityMin, Consts.IntensityMax)] public float intensityInside = Consts.IntensityDefault;

        [System.Obsolete("Use 'intensityGlobal' or 'intensityInside' instead")]
        public float alphaInside { get { return intensityInside; } set { intensityInside = value; } }

        /// <summary>
        /// Beam outside intensity (when looking at the beam from behind).
        /// </summary>
        [FormerlySerializedAs("alphaOutside"), FormerlySerializedAs("alpha")]
        [Range(Consts.IntensityMin, Consts.IntensityMax)] public float intensityOutside = Consts.IntensityDefault;

        [System.Obsolete("Use 'intensityGlobal' or 'intensityOutside' instead")]
        public float alphaOutside { get { return intensityOutside; } set { intensityOutside = value; } }

        /// <summary>
        /// Global beam intensity, to use only when intensityModeAdvanced is false
        /// Otherwise use intensityOutside and intensityInside independently
        /// </summary>  
        public float intensityGlobal { get { return intensityOutside; } set { intensityInside = value; intensityOutside = value; } }

        /// <summary>
        /// Change how the light beam colors will be mixed with the scene
        /// </summary>
        public BlendingMode blendingMode = Consts.BlendingModeDefault;

        /// <summary>
        /// Get the spotAngle value from the light (when attached to a Spotlight) or not
        /// </summary>
        [FormerlySerializedAs("angleFromLight")]
        public bool spotAngleFromLight = true;

        /// <summary>
        /// Spot Angle (in degrees). This doesn't take account of the radiusStart, and is not necessarily the same than the cone angle.
        /// </summary>
        [Range(Consts.SpotAngleMin, Consts.SpotAngleMax)] public float spotAngle = Consts.SpotAngleDefault;

        /// <summary>
        /// Cone Angle (in degrees). This takes account of the radiusStart, and is not necessarily the same than the spot angle.
        /// </summary>
        public float coneAngle { get { return Mathf.Atan2(coneRadiusEnd - coneRadiusStart, fallOffEnd) * Mathf.Rad2Deg * 2f; } }

        /// <summary>
        /// Start radius of the cone geometry.
        /// 0 will generate a perfect cone geometry. Higher values will generate truncated cones.
        /// </summary>
        [FormerlySerializedAs("radiusStart")]
        public float coneRadiusStart = Consts.ConeRadiusStart;

        /// <summary>
        /// End radius of the cone geometry
        /// </summary>
        public float coneRadiusEnd { get { return Utils.ComputeConeRadiusEnd(fallOffEnd, spotAngle); } }

        /// <summary>
        /// Volume (in unit^3) of the cone (from the base to fallOffEnd)
        /// </summary>
        public float coneVolume { get { float r1 = coneRadiusStart, r2 = coneRadiusEnd; return (Mathf.PI / 3) * (r1 * r1 + r1 * r2 + r2 * r2) * fallOffEnd; } }

        /// <summary>
        /// Apex distance of the truncated radius
        /// If coneRadiusStart = 0, the apex is the at the truncated radius, so coneApexOffsetZ = 0
        /// Otherwise, coneApexOffsetZ > 0 and represents the local position Z offset
        /// </summary>
        public float coneApexOffsetZ {
            get { // simple intercept
                float ratioRadius = coneRadiusStart / coneRadiusEnd;
                return ratioRadius == 1f ? float.MaxValue : ((fallOffEnd * ratioRadius) / (1 - ratioRadius));
            }
        }

        /// <summary>
        /// Shared: this beam will use the global shared mesh (recommended setting, since it will save a lot on memory).
        /// Custom: this beam will use a custom mesh instead. Check the following properties to control how the mesh will be generated.
        /// </summary>
        public MeshType geomMeshType = Consts.GeomMeshType;

        /// <summary>
        /// Set a custom number of Sides for the cone geometry.
        /// Higher values give better looking results, but require more memory and graphic performance.
        /// This value is only used when geomMeshType is Custom.
        /// </summary>
        [FormerlySerializedAs("geomSides")]
        public int geomCustomSides = Consts.GeomSidesDefault;

        /// <summary>
        /// Returns the effective number of Sides used by this beam.
        /// Could come from the shared mesh, or the custom mesh
        /// </summary>
        public int geomSides
        {
            get { return geomMeshType == MeshType.Custom ? geomCustomSides : Config.Instance.sharedMeshSides; }
            set { geomCustomSides = value; Debug.LogWarning("The setter VLB.VolumetricLightBeam.geomSides is OBSOLETE and has been renamed to geomCustomSides."); }
        }

        /// <summary>
        /// Set a custom Segments for the cone geometry.
        /// Higher values give better looking results, but require more memory and graphic performance.
        /// This value is only used when geomMeshType is Custom.
        /// </summary>
        public int geomCustomSegments = Consts.GeomSegmentsDefault;

        /// <summary>
        /// Returns the effective number of Segments used by this beam.
        /// Could come from the shared mesh, or the custom mesh
        /// </summary>
        public int geomSegments
        {
            get { return geomMeshType == MeshType.Custom ? geomCustomSegments : Config.Instance.sharedMeshSegments; }
            set { geomCustomSegments = value; Debug.LogWarning("The setter VLB.VolumetricLightBeam.geomSegments is OBSOLETE and has been renamed to geomCustomSegments."); }
        }


        /// <summary>
        /// Show the cone cap (only visible from inside)
        /// </summary>
        public bool geomCap = Consts.GeomCap;

        /// <summary>
        /// Get the fallOffEnd value from the light (when attached to a Spotlight) or not
        /// </summary>
        [FormerlySerializedAs("fadeEndFromLight")]
        public bool fallOffEndFromLight = true;

        [System.Obsolete("Use 'fallOffEndFromLight' instead")]
        public bool fadeEndFromLight { get { return fallOffEndFromLight; } set { fallOffEndFromLight = value; } }

        /// <summary>
        /// Light attenuation formula used to compute fading between 'fallOffStart' and 'fallOffEnd'
        /// </summary>
        public AttenuationEquation attenuationEquation = Consts.AttenuationEquationDefault;

        /// <summary>
        /// Custom blending mix between linear and quadratic attenuation formulas.
        /// Only used if attenuationEquation is set to AttenuationEquation.Blend.
        /// 0.0 = 100% Linear
        /// 0.5 = Mix between 50% Linear and 50% Quadratic
        /// 1.0 = 100% Quadratic
        /// </summary>
        [Range(0f, 1f)] public float attenuationCustomBlending = Consts.AttenuationCustomBlending;

        /// <summary>
        /// Proper lerp value between linear and quadratic attenuation, used by the shader.
        /// </summary>
        public float attenuationLerpLinearQuad {
            get {
                if (attenuationEquation == AttenuationEquation.Linear) return 0f;
                else if (attenuationEquation == AttenuationEquation.Quadratic) return 1f;
                return attenuationCustomBlending;
            }
        }

        /// <summary>
        /// Distance from the light source (in units) the beam will start to fade out.
        /// </summary>
        /// 
        [FormerlySerializedAs("fadeStart")]
        public float fallOffStart = Consts.FallOffStart;

        [System.Obsolete("Use 'fallOffStart' instead")]
        public float fadeStart { get { return fallOffStart; } set { fallOffStart = value; } }

        /// <summary>
        /// Distance from the light source (in units) the beam is entirely faded out.
        /// </summary>
        [FormerlySerializedAs("fadeEnd")]
        public float fallOffEnd = Consts.FallOffEnd;

        [System.Obsolete("Use 'fallOffEnd' instead")]
        public float fadeEnd { get { return fallOffEnd; } set { fallOffEnd = value; } }

        /// <summary>
        /// Distance from the world geometry the beam will fade.
        /// 0 = hard intersection
        /// Higher values produce soft intersection when the beam intersects other opaque geometry.
        /// </summary>
        public float depthBlendDistance = Consts.DepthBlendDistance;

        /// <summary>
        /// Distance from the camera the beam will fade.
        /// 0 = hard intersection
        /// Higher values produce soft intersection when the camera is near the cone triangles.
        /// </summary>
        public float cameraClippingDistance = Consts.CameraClippingDistance;

        /// <summary>
        /// Boost intensity factor when looking at the beam from the inside directly at the source.
        /// </summary>
        [Range(0f, 1f)]
        public float glareFrontal = Consts.GlareFrontal;

        /// <summary>
        /// Boost intensity factor when looking at the beam from behind.
        /// </summary>
        [Range(0f, 1f)]
        public float glareBehind = Consts.GlareBehind;

        /// <summary>
        /// Modulate the thickness of the beam when looking at it from the side.
        /// Higher values produce thinner beam with softer transition at beam edges.
        /// </summary>
        [FormerlySerializedAs("fresnelPowOutside")]
        public float fresnelPow = Consts.FresnelPow;

        /// <summary>
        /// Enable 3D Noise effect and choose the mode
        /// </summary>
        public NoiseMode noiseMode = Consts.NoiseModeDefault;

        public bool isNoiseEnabled { get { return noiseMode != NoiseMode.Disabled; } }

        [System.Obsolete("Use 'noiseMode' instead")]
        public bool noiseEnabled { get { return isNoiseEnabled; } set { noiseMode = value ? NoiseMode.WorldSpace : NoiseMode.Disabled; } }

        [FormerlySerializedAs("noiseEnabled")]
        [SerializeField] bool _DEPRECATED_NoiseEnabled = false;

        /// <summary>
        /// Contribution factor of the 3D Noise (when enabled).
        /// Higher intensity means the noise contribution is stronger and more visible.
        /// </summary>
        [Range(Consts.NoiseIntensityMin, Consts.NoiseIntensityMax)] public float noiseIntensity = Consts.NoiseIntensityDefault;

        /// <summary>
        /// Get the noiseScale value from the Global 3D Noise configuration
        /// </summary>
        public bool noiseScaleUseGlobal = true;

        /// <summary>
        /// 3D Noise texture scaling: higher scale make the noise more visible, but potentially less realistic.
        /// </summary>
        [Range(Consts.NoiseScaleMin, Consts.NoiseScaleMax)] public float noiseScaleLocal = Consts.NoiseScaleDefault;

        /// <summary>
        /// Get the noiseVelocity value from the Global 3D Noise configuration
        /// </summary>
        public bool noiseVelocityUseGlobal = true;

        /// <summary>
        /// World Space direction and speed of the 3D Noise scrolling, simulating the fog/smoke movement.
        /// </summary>
        public Vector3 noiseVelocityLocal = Consts.NoiseVelocityDefault;

        /// <summary>
        /// Fade out starting distance.
        /// Beyond this distance, the beam intensity will start to be dimmed.
        /// </summary>
        public float fadeOutBegin
        {
            get { return _FadeOutBegin; }
            set { SetFadeOutValue(ref _FadeOutBegin, value); }
        }

        /// <summary>
        /// Fade out ending distance.
        /// Beyond this distance, the beam will be culled off to save on performance.
        /// </summary>
        public float fadeOutEnd
        {
            get { return _FadeOutEnd; }
            set { SetFadeOutValue(ref _FadeOutEnd, value); }
        }

        /// <summary>
        /// Is Fade Out feature enabled or not?
        /// </summary>
        public bool isFadeOutEnabled { get { return _FadeOutBegin >= 0 && _FadeOutEnd >= 0; } }


        /// <summary>
        /// Unique ID of the beam's sorting layer.
        /// </summary>
        public int sortingLayerID
        {
            get { return _SortingLayerID; }
            set {
                _SortingLayerID = value;
                if (m_BeamGeom) m_BeamGeom.sortingLayerID = value;
            }
        }

        /// <summary>
        /// Name of the beam's sorting layer.
        /// </summary>
        public string sortingLayerName
        {
            get { return SortingLayer.IDToName(sortingLayerID); }
            set { sortingLayerID = SortingLayer.NameToID(value); }
        }

        /// <summary>
        /// The overlay priority within its layer.
        /// Lower numbers are rendered first and subsequent numbers overlay those below.
        /// </summary>
        public int sortingOrder
        {
            get { return _SortingOrder; }
            set
            {
                _SortingOrder = value;
                if (m_BeamGeom) m_BeamGeom.sortingOrder = value;
            }
        }

        /// <summary>
        /// If true, the light beam will keep track of the changes of its own properties and the spotlight attached to it (if any) during playtime.
        /// This would allow you to modify the light beam in realtime from Script, Animator and/or Timeline.
        /// Enabling this feature is at very minor performance cost. So keep it disabled if you don't plan to modify this light beam during playtime.
        /// </summary>
        public bool trackChangesDuringPlaytime
        {
            get { return _TrackChangesDuringPlaytime; }
            set { _TrackChangesDuringPlaytime = value; StartPlaytimeUpdateIfNeeded(); }
        }

        /// <summary> Is the beam currently tracking property changes? </summary>
        public bool isCurrentlyTrackingChanges { get { return m_CoPlaytimeUpdate != null; } }

        /// <summary> Has the geometry already been generated? </summary>
        public bool hasGeometry { get { return m_BeamGeom != null; } }

        /// <summary> Bounds of the geometry's mesh (if the geometry exists) </summary>
        public Bounds bounds { get { return m_BeamGeom != null ? m_BeamGeom.meshRenderer.bounds : new Bounds(Vector3.zero, Vector3.zero); } }

        public int blendingModeAsInt { get { return Mathf.Clamp((int)blendingMode, 0, System.Enum.GetValues(typeof(BlendingMode)).Length); } }



        // INTERNAL
        public MaterialManager.DynamicOcclusion _INTERNAL_DynamicOcclusionMode { get; set; }
        public MaterialManager.DynamicOcclusion _INTERNAL_EnabledDynamicOcclusionMode { get { return m_IsDynamicOcclusionEnabled ? _INTERNAL_DynamicOcclusionMode : MaterialManager.DynamicOcclusion.Off; } }
        bool m_IsDynamicOcclusionEnabled = false;

        public void _INTERNAL_SetDynamicOcclusionCallback(string shaderKeyword, MaterialModifier.Callback cb)
        {
            m_IsDynamicOcclusionEnabled = cb != null;

            if (m_BeamGeom)
                m_BeamGeom.SetDynamicOcclusionCallback(shaderKeyword, cb);
        }

        public delegate void OnWillCameraRenderCB(Camera cam);
        public event OnWillCameraRenderCB onWillCameraRenderThisBeam;

        public void _INTERNAL_OnWillCameraRenderThisBeam(Camera cam)
        {
            if (onWillCameraRenderThisBeam != null)
                onWillCameraRenderThisBeam(cam);
        }

        public delegate void OnBeamGeometryInitialized();
        private OnBeamGeometryInitialized m_OnBeamGeometryInitialized;

        public void RegisterOnBeamGeometryInitializedCallback(OnBeamGeometryInitialized cb)
        {
            m_OnBeamGeometryInitialized += cb;

            if(m_BeamGeom)
            {
                CallOnBeamGeometryInitializedCallback();
            }
        }

        void CallOnBeamGeometryInitializedCallback()
        {
            if (m_OnBeamGeometryInitialized != null)
            {
                m_OnBeamGeometryInitialized();
                m_OnBeamGeometryInitialized = null;
            }
        }

#pragma warning disable 0414
        [SerializeField] int pluginVersion = -1;
#pragma warning restore 0414

        [FormerlySerializedAs("trackChangesDuringPlaytime")]
        [SerializeField] bool _TrackChangesDuringPlaytime = false;

        [SerializeField] int _SortingLayerID = 0;
        [SerializeField] int _SortingOrder = 0;

        [FormerlySerializedAs("fadeOutBegin")]
        [SerializeField] float _FadeOutBegin = Consts.FadeOutBeginDefault;
        [FormerlySerializedAs("fadeOutEnd")]
        [SerializeField] float _FadeOutEnd = Consts.FadeOutEndDefault;

        void SetFadeOutValue(ref float propToChange, float value)
        {
            bool wasEnabled = isFadeOutEnabled;
            propToChange = value;

#if UNITY_EDITOR
            if (Application.isPlaying)
#endif
            {
                if (isFadeOutEnabled != wasEnabled)
                    OnFadeOutStateChanged();
            }
        }

        void OnFadeOutStateChanged()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
#endif
            {
                // Restart only when fadeout is enabled: on disable, the coroutine will kill itself automatically
                if (isFadeOutEnabled && m_BeamGeom) m_BeamGeom.RestartFadeOutCoroutine();
            }
        }

        /// Internal property used for QA testing purpose, do not change
        public uint _INTERNAL_InstancedMaterialGroupID { get; protected set; }

        BeamGeometry m_BeamGeom = null;
        Coroutine m_CoPlaytimeUpdate = null;

#if UNITY_EDITOR
        static VolumetricLightBeam[] _EditorFindAllInstances()
        {
            return Resources.FindObjectsOfTypeAll<VolumetricLightBeam>();
        }

        public static void _EditorSetAllMeshesDirty()
        {
            foreach (var instance in _EditorFindAllInstances())
                instance._EditorSetMeshDirty();
        }

        public static void _EditorSetAllBeamGeomDirty()
        {
            foreach (var instance in _EditorFindAllInstances())
                instance.m_EditorDirtyFlags |= EditorDirtyFlags.FullBeamGeomGAO;
        }

        public void _EditorSetMeshDirty() { m_EditorDirtyFlags |= EditorDirtyFlags.Mesh; }
        public void _EditorSetBeamGeomDirty() { m_EditorDirtyFlags |= EditorDirtyFlags.FullBeamGeomGAO; }

        [System.Flags]
        enum EditorDirtyFlags
        {
            Clean = 0,
            Props = 1 << 1,
            Mesh = 1 << 2,
            BeamGeomGAO = 1 << 3,
            FullBeamGeomGAO = Mesh | BeamGeomGAO,
            Everything = Props | Mesh | BeamGeomGAO,
        }
        EditorDirtyFlags m_EditorDirtyFlags;
        CachedLightProperties m_PrevCachedLightProperties;

        public UnityEditor.StaticEditorFlags GetStaticEditorFlagsForSubObjects()
        {
            // Apply the same static flags to the BeamGeometry and DustParticles than the VLB GAO
            var flags = UnityEditor.GameObjectUtility.GetStaticEditorFlags(gameObject);
            flags &= ~(
                // remove the Lightmap static flag since it will generate error messages when selecting the BeamGeometry GAO in the editor
#if UNITY_2019_2_OR_NEWER
                UnityEditor.StaticEditorFlags.ContributeGI
#else
                UnityEditor.StaticEditorFlags.LightmapStatic
#endif
                | UnityEditor.StaticEditorFlags.NavigationStatic
                | UnityEditor.StaticEditorFlags.OffMeshLinkGeneration
                | UnityEditor.StaticEditorFlags.OccluderStatic
                );
            return flags;
        }
#endif

        public string meshStats
        {
            get
            {
                Mesh mesh = m_BeamGeom ? m_BeamGeom.coneMesh : null;
                if (mesh) return string.Format("Cone angle: {0:0.0} degrees\nMesh: {1} vertices, {2} triangles", coneAngle, mesh.vertexCount, mesh.triangles.Length / 3);
                else return "no mesh available";
            }
        }

        public int meshVerticesCount { get { return (m_BeamGeom && m_BeamGeom.coneMesh) ? m_BeamGeom.coneMesh.vertexCount : 0; } }
        public int meshTrianglesCount { get { return (m_BeamGeom && m_BeamGeom.coneMesh) ? m_BeamGeom.coneMesh.triangles.Length / 3 : 0; } }

        Light _CachedLight = null;
        Light lightSpotAttached
        {
            get
            {
                if(_CachedLight == null) _CachedLight = GetComponent<Light>();
                if (_CachedLight && _CachedLight.type == LightType.Spot) return _CachedLight;
                return null;
            }
        }

        /// <summary>
        /// Returns a value indicating if the world position passed in argument is inside the light beam or not.
        /// This functions treats the beam like infinite (like the beam had an infinite length and never fell off)
        /// </summary>
        /// <param name="posWS">World position</param>
        /// <returns>
        /// < 0 position is out
        /// = 0 position is exactly on the beam geometry 
        /// > 0 position is inside the cone
        /// </returns>
        public float GetInsideBeamFactor(Vector3 posWS) { return GetInsideBeamFactorFromObjectSpacePos(transform.InverseTransformPoint(posWS)); }

        public float GetInsideBeamFactorFromObjectSpacePos(Vector3 posOS)
        {
            if (posOS.z < 0f) return -1f;

            // Compute a factor to know how far inside the beam cone the camera is
            var triangle2D = new Vector2(posOS.xy().magnitude, posOS.z + coneApexOffsetZ).normalized;
            const float maxRadiansDiff = 0.1f;
            float slopeRad = (coneAngle * Mathf.Deg2Rad) / 2;

            return Mathf.Clamp((Mathf.Abs(Mathf.Sin(slopeRad)) - Mathf.Abs(triangle2D.x)) / maxRadiansDiff, -1, 1);
        }

        [System.Obsolete("Use 'GenerateGeometry()' instead")]
        public void Generate() { GenerateGeometry(); }

        /// <summary>
        /// Regenerate the beam mesh (and also the material).
        /// This can be slow (it recreates a mesh from scratch), so don't call this function during playtime.
        /// You would need to call this function only if you want to change the properties 'geomSides' and 'geomCap' during playtime.
        /// Otherwise, for the other properties, just enable 'trackChangesDuringPlaytime', or manually call 'UpdateAfterManualPropertyChange()'
        /// </summary>
        public virtual void GenerateGeometry()
        {
            HandleBackwardCompatibility(pluginVersion, Version.Current);
            pluginVersion = Version.Current;

            ValidateProperties();

            if (m_BeamGeom == null)
            {
                m_BeamGeom = Utils.NewWithComponent<BeamGeometry>("Beam Geometry");
                m_BeamGeom.Initialize(this);
                CallOnBeamGeometryInitializedCallback();
            }

            m_BeamGeom.RegenerateMesh();
            m_BeamGeom.visible = enabled;
        }

        /// <summary>
        /// Update the beam material and its bounds.
        /// Calling manually this function is useless if your beam has its property 'trackChangesDuringPlaytime' enabled
        /// (because then this function is automatically called each frame).
        /// However, if 'trackChangesDuringPlaytime' is disabled, and you change a property via Script for example,
        /// you need to call this function to take the property change into account.
        /// All properties changes are took into account, expect 'geomSides' and 'geomCap' which require to regenerate the geometry via 'GenerateGeometry()'
        /// </summary>
        public virtual void UpdateAfterManualPropertyChange()
        {
            ValidateProperties();
            if (m_BeamGeom) m_BeamGeom.UpdateMaterialAndBounds();
        }

#if !UNITY_EDITOR
        void Start()
        {
            // In standalone builds, simply generate the geometry once in Start
            GenerateGeometry();
        }
#else
        void Start()
        {
            if (Application.isPlaying)
            {
                GenerateGeometry();
                m_EditorDirtyFlags = EditorDirtyFlags.Clean;
            }
            else
            {
                // In Editor, creating geometry from Start and/or OnValidate generates warning in Unity 2017.
                // So we do it from Update
                m_EditorDirtyFlags = EditorDirtyFlags.Everything;
            }

            StartPlaytimeUpdateIfNeeded();
        }

        void OnValidate()
        {
            m_EditorDirtyFlags |= EditorDirtyFlags.Props; // Props have been modified from Editor
        }

        void Update() // EDITOR ONLY
        {
            // Handle edition of light properties in Editor
            if (!Application.isPlaying)
            {
                var newProps = new CachedLightProperties(lightSpotAttached);
                if(!newProps.Equals(m_PrevCachedLightProperties))
                    m_EditorDirtyFlags |= EditorDirtyFlags.Props;
                m_PrevCachedLightProperties = newProps;
            }

            if (m_EditorDirtyFlags == EditorDirtyFlags.Clean)
            {
                if (Application.isPlaying)
                {
                    if (!trackChangesDuringPlaytime) // during Playtime, realtime changes are handled by CoUpdateDuringPlaytime
                        return;
                }
            }
            else
            {
                if (m_EditorDirtyFlags.HasFlag(EditorDirtyFlags.Mesh))
                {
                    if (m_EditorDirtyFlags.HasFlag(EditorDirtyFlags.BeamGeomGAO))
                        DestroyBeam();

                    GenerateGeometry(); // regenerate everything
                }
                else if (m_EditorDirtyFlags.HasFlag(EditorDirtyFlags.Props))
                {
                    ValidateProperties();
                }
            }

            // If we modify the attached Spotlight properties, or if we animate the beam via Unity 2017's timeline,
            // we are not notified of properties changes. So we update the material anyway.
            UpdateAfterManualPropertyChange();

            m_EditorDirtyFlags = EditorDirtyFlags.Clean;
        }

        public void Reset()
        {
            colorMode = Consts.ColorModeDefault;
            color = Consts.FlatColor;
            colorFromLight = true;

            intensityFromLight = true;
            intensityModeAdvanced = false;
            intensityInside = Consts.IntensityDefault;
            intensityOutside = Consts.IntensityDefault;

            blendingMode = Consts.BlendingModeDefault;

            spotAngleFromLight = true;
            spotAngle = Consts.SpotAngleDefault;

            coneRadiusStart = Consts.ConeRadiusStart;
            geomMeshType = Consts.GeomMeshType;
            geomCustomSides = Consts.GeomSidesDefault;
            geomCustomSegments = Consts.GeomSegmentsDefault;
            geomCap = Consts.GeomCap;

            attenuationEquation = Consts.AttenuationEquationDefault;
            attenuationCustomBlending = Consts.AttenuationCustomBlending;

            fallOffEndFromLight = true;
            fallOffStart = Consts.FallOffStart;
            fallOffEnd = Consts.FallOffEnd;

            depthBlendDistance = Consts.DepthBlendDistance;
            cameraClippingDistance = Consts.CameraClippingDistance;

            glareFrontal = Consts.GlareFrontal;
            glareBehind = Consts.GlareBehind;

            fresnelPow = Consts.FresnelPow;

            noiseMode = Consts.NoiseModeDefault;
            noiseIntensity = Consts.NoiseIntensityDefault;
            noiseScaleUseGlobal = true;
            noiseScaleLocal = Consts.NoiseScaleDefault;
            noiseVelocityUseGlobal = true;
            noiseVelocityLocal = Consts.NoiseVelocityDefault;

            sortingLayerID = 0;
            sortingOrder = 0;

            fadeOutBegin = Consts.FadeOutBeginDefault;
            fadeOutEnd = Consts.FadeOutEndDefault;

            trackChangesDuringPlaytime = false;

            m_EditorDirtyFlags = EditorDirtyFlags.Everything;
        }
#endif

        void OnEnable()
        {
            if (m_BeamGeom) m_BeamGeom.visible = true;
            StartPlaytimeUpdateIfNeeded();
        }

        void OnDisable()
        {
            if (m_BeamGeom) m_BeamGeom.visible = false;
            m_CoPlaytimeUpdate = null;
        }

        void StartPlaytimeUpdateIfNeeded()
        {
            if (Application.isPlaying && trackChangesDuringPlaytime && m_CoPlaytimeUpdate == null)
            {
                m_CoPlaytimeUpdate = StartCoroutine(CoPlaytimeUpdate());
            }
        }

        IEnumerator CoPlaytimeUpdate()
        {
            while (trackChangesDuringPlaytime && enabled)
            {
                UpdateAfterManualPropertyChange();
                yield return null;
            }
            m_CoPlaytimeUpdate = null;
        }

        void OnDestroy()
        {
            DestroyBeam();
        }

        void DestroyBeam()
        {
            if (m_BeamGeom) DestroyImmediate(m_BeamGeom.gameObject); // Make sure to delete the GAO
            m_BeamGeom = null;
        }

        void AssignPropertiesFromSpotLight(Light lightSpot)
        {
            if (lightSpot && lightSpot.type == LightType.Spot)
            {
                if (intensityFromLight) { intensityModeAdvanced = false; intensityGlobal = lightSpot.intensity; }
                if (fallOffEndFromLight) fallOffEnd = lightSpot.range;
                if (spotAngleFromLight) spotAngle = lightSpot.spotAngle;
                if (colorFromLight)
                {
                    colorMode = ColorMode.Flat;
                    color = lightSpot.color;
                }
            }
        }

        void ClampProperties()
        {
            intensityInside = Mathf.Clamp(intensityInside, Consts.IntensityMin, Consts.IntensityMax);
            intensityOutside = Mathf.Clamp(intensityOutside, Consts.IntensityMin, Consts.IntensityMax);

            attenuationCustomBlending = Mathf.Clamp01(attenuationCustomBlending);

            fallOffEnd = Mathf.Max(Consts.FallOffDistancesMinThreshold, fallOffEnd);
            fallOffStart = Mathf.Clamp(fallOffStart, 0f, fallOffEnd - Consts.FallOffDistancesMinThreshold);

            spotAngle = Mathf.Clamp(spotAngle, Consts.SpotAngleMin, Consts.SpotAngleMax);
            coneRadiusStart = Mathf.Max(coneRadiusStart, 0f);

            depthBlendDistance = Mathf.Max(depthBlendDistance, 0f);
            cameraClippingDistance = Mathf.Max(cameraClippingDistance, 0f);

            geomCustomSides = Mathf.Clamp(geomCustomSides, Consts.GeomSidesMin, Consts.GeomSidesMax);
            geomCustomSegments = Mathf.Clamp(geomCustomSegments, Consts.GeomSegmentsMin, Consts.GeomSegmentsMax);

            fresnelPow = Mathf.Max(0f, fresnelPow);

            glareBehind = Mathf.Clamp01(glareBehind);
            glareFrontal = Mathf.Clamp01(glareFrontal);

            noiseIntensity = Mathf.Clamp(noiseIntensity, Consts.NoiseIntensityMin, Consts.NoiseIntensityMax);
        }

        void ValidateProperties()
        {
            AssignPropertiesFromSpotLight(lightSpotAttached);
            ClampProperties();
        }

        void HandleBackwardCompatibility(int serializedVersion, int newVersion)
        {
            if (serializedVersion == -1) return;            // freshly new spawned entity: nothing to do
            if (serializedVersion == newVersion) return;    // same version: nothing to do

            if (serializedVersion < 1301)
            {
                // quadratic attenuation is a new feature of 1.3
                attenuationEquation = AttenuationEquation.Linear;
            }

            if (serializedVersion < 1501)
            {
                // custom mesh is a new feature of 1.5
                geomMeshType = MeshType.Custom;
                geomCustomSegments = 5;
            }

            if (serializedVersion < 1610)
            {
                // intensity global/advanced mode is a feature of 1.61
                intensityFromLight = false;
                intensityModeAdvanced = !Mathf.Approximately(intensityInside, intensityOutside);
            }

            if (serializedVersion < 1710)
            {
                // noiseMode is a feature of 1.71
                noiseMode = _DEPRECATED_NoiseEnabled ? NoiseMode.WorldSpace : NoiseMode.Disabled;
            }

            Utils.MarkCurrentSceneDirty();
        }

#if UNITY_EDITOR && DEBUG_SHOW_APEX
        void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(new Vector3(0, 0, -coneApexOffsetZ), 0.025f);
        }
#endif
    }
}
