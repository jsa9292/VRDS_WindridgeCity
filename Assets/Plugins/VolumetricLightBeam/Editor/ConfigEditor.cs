#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace VLB
{
    [CustomEditor(typeof(Config))]
    public class ConfigEditor : EditorCommon
    {
        SerializedProperty geometryOverrideLayer, geometryLayerID, geometryTag, geometryRenderQueue, renderPipeline, renderingMode;
        SerializedProperty beamShader1Pass, beamShader2Pass, beamShaderSRP;
        SerializedProperty sharedMeshSides, sharedMeshSegments;
        SerializedProperty globalNoiseScale, globalNoiseVelocity;
        SerializedProperty fadeOutCameraTag;
        SerializedProperty noise3DData, noise3DSize;
        SerializedProperty dustParticlesPrefab;
        bool isRenderQueueCustom = false;

        protected override void OnEnable()
        {
            base.OnEnable();

            geometryOverrideLayer = FindProperty((Config x) => x.geometryOverrideLayer);
            geometryLayerID = FindProperty((Config x) => x.geometryLayerID);
            geometryTag = FindProperty((Config x) => x.geometryTag);

            geometryRenderQueue = FindProperty((Config x) => x.geometryRenderQueue);
            renderPipeline = FindProperty((Config x) => x.renderPipeline);
            renderingMode = FindProperty((Config x) => x.renderingMode);

            beamShader1Pass = serializedObject.FindProperty("beamShader1Pass");
            beamShader2Pass = serializedObject.FindProperty("beamShader2Pass");
            beamShaderSRP   = serializedObject.FindProperty("beamShaderSRP");

            sharedMeshSides = FindProperty((Config x) => x.sharedMeshSides);
            sharedMeshSegments = FindProperty((Config x) => x.sharedMeshSegments);

            globalNoiseScale = FindProperty((Config x) => x.globalNoiseScale);
            globalNoiseVelocity = FindProperty((Config x) => x.globalNoiseVelocity);

            fadeOutCameraTag = FindProperty((Config x) => x.fadeOutCameraTag);

            noise3DData = FindProperty((Config x) => x.noise3DData);
            noise3DSize = FindProperty((Config x) => x.noise3DSize);

            dustParticlesPrefab = FindProperty((Config x) => x.dustParticlesPrefab);

            RenderQueueGUIInit();

            Noise3D.LoadIfNeeded(); // Try to load Noise3D, maybe for the 1st time
        }

        void RenderQueueGUIInit()
        {
            isRenderQueueCustom = true;
            foreach (RenderQueue rq in System.Enum.GetValues(typeof(RenderQueue)))
            {
                if (rq != RenderQueue.Custom && geometryRenderQueue.intValue == (int)rq)
                {
                    isRenderQueueCustom = false;
                    break;
                }
            }
        }

        void RenderQueueGUIDraw()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUI.BeginChangeCheck();
                RenderQueue rq = isRenderQueueCustom ? RenderQueue.Custom : (RenderQueue)geometryRenderQueue.intValue;
                rq = (RenderQueue)EditorGUILayout.EnumPopup(EditorStrings.Config.GeometryRenderQueue, rq);
                if (EditorGUI.EndChangeCheck())
                {
                    isRenderQueueCustom = (rq == RenderQueue.Custom);

                    if (!isRenderQueueCustom)
                        geometryRenderQueue.intValue = (int)rq;
                }

                EditorGUI.BeginDisabledGroup(!isRenderQueueCustom);
                {
                    geometryRenderQueue.intValue = EditorGUILayout.IntField(geometryRenderQueue.intValue, GUILayout.MaxWidth(65.0f));
                }
                EditorGUI.EndDisabledGroup();
            }
        }

        protected override void OnHeaderGUI()
        {
            GUILayout.BeginVertical("In BigTitle");
            EditorGUILayout.Separator();

            var title = string.Format("Volumetric Light Beam - Plugin Configuration ({0})", (IsOverriddenInstance() ? "Override" : "Default"));
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            EditorGUILayout.LabelField(string.Format("Current Version: {0}", Version.Current), EditorStyles.miniBoldLabel);
            EditorGUILayout.Separator();
		    GUILayout.EndVertical();
        }

        void ReimportShaders()
        {
            ReimportAsset(beamShader1Pass.objectReferenceValue);
            ReimportAsset(beamShader2Pass.objectReferenceValue);
        }

        void ReimportAsset(Object obj)
        {
            if (obj)
            {
                var path = AssetDatabase.GetAssetPath(beamShader1Pass.objectReferenceValue);
                AssetDatabase.ImportAsset(path, ImportAssetOptions.Default);
            }
        }

        protected virtual bool IsOverriddenInstance() { return false; }
        bool IsDisabled() { return !IsOverriddenInstance(); }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            bool reloadNoise = false;

            bool isUsedInstance = (Object)Config.Instance == this.target;

            if (!IsOverriddenInstance())
            {
                if (isUsedInstance)
                {
                    if (!Application.isPlaying)
                    {
                        if (GUILayout.Button(EditorStrings.Config.CreateOverrideAsset))
                            ConfigOverrideEditor.CreateAsset();
                    }
                }
                else
                {
                    ButtonOpenConfig(/*miniButton*/ false);
                }
                DrawLineSeparator();
            }

            if (IsOverriddenInstance() && !isUsedInstance)
            {
                EditorGUILayout.HelpBox(EditorStrings.Config.MultipleAssets, MessageType.Error);
                EditorGUILayout.Separator();
                ButtonOpenConfig();
            }
            else
            {
                EditorGUI.BeginDisabledGroup(IsDisabled());
                {
                    EditorGUI.BeginChangeCheck();
                    {
                        if (FoldableHeader.Begin(this, "Beam Geometry"))
                        {
                            using (new EditorGUILayout.HorizontalScope())
                            {
                                geometryOverrideLayer.boolValue = EditorGUILayout.Toggle(EditorStrings.Config.GeometryOverrideLayer, geometryOverrideLayer.boolValue);
                                using (new EditorGUI.DisabledGroupScope(!geometryOverrideLayer.boolValue))
                                {
                                    geometryLayerID.intValue = EditorGUILayout.LayerField(geometryLayerID.intValue);
                                }
                            }

                            geometryTag.stringValue = EditorGUILayout.TagField(EditorStrings.Config.GeometryTag, geometryTag.stringValue);
                        }
                        FoldableHeader.End();

                        if (FoldableHeader.Begin(this, "Rendering"))
                        {
                            RenderQueueGUIDraw();

                            if (BeamGeometry.isCustomRenderPipelineSupported)
                            {
                                EditorGUI.BeginChangeCheck();
                                {
                                    renderPipeline.CustomEnum<RenderPipeline>(EditorStrings.Config.GeometryRenderPipeline, EditorStrings.Config.GeometryRenderPipelineEnumDescriptions);
                                }
                                if (EditorGUI.EndChangeCheck())
                                {
                                    Config.OnRenderPipelineChanged((RenderPipeline)renderPipeline.enumValueIndex);
                                    VolumetricLightBeam._EditorSetAllBeamGeomDirty(); // need to fully reset the BeamGeom to update the shader
                                    ReimportShaders();
                                }
                            }

                            EditorGUI.BeginChangeCheck();
                            {
                                EditorGUILayout.PropertyField(renderingMode, EditorStrings.Config.GeometryRenderingMode);

                                if (renderPipeline.enumValueIndex == (int)RenderPipeline.SRP_4_0_0_or_higher && renderingMode.enumValueIndex == (int)RenderingMode.MultiPass)
                                    EditorGUILayout.HelpBox(EditorStrings.Config.SrpAndMultiPassNoCompatible, MessageType.Error);

#pragma warning disable 0162 // warning CS0162: Unreachable code detected
                                if (renderingMode.enumValueIndex == (int)RenderingMode.GPUInstancing && !GpuInstancing.isSupported)
                                    EditorGUILayout.HelpBox(EditorStrings.Config.GeometryGpuInstancingNotSupported, MessageType.Warning);
#pragma warning restore 0162
                            }
                            if (EditorGUI.EndChangeCheck())
                            {
                                VolumetricLightBeam._EditorSetAllBeamGeomDirty(); // need to fully reset the BeamGeom to update the shader
                                GlobalMesh.Destroy();
                                ReimportShaders();
                            }
                        }
                        FoldableHeader.End();
                    }
                    if (EditorGUI.EndChangeCheck())
                    {
                        VolumetricLightBeam._EditorSetAllMeshesDirty();
                    }

                    if (FoldableHeader.Begin(this, "Shared Mesh"))
                    {
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(sharedMeshSides, EditorStrings.Config.SharedMeshSides);
                        EditorGUILayout.PropertyField(sharedMeshSegments, EditorStrings.Config.SharedMeshSegments);
                        if (EditorGUI.EndChangeCheck())
                        {
                            GlobalMesh.Destroy();
                            VolumetricLightBeam._EditorSetAllMeshesDirty();
                        }

                        var meshInfo = "These properties will change the mesh tessellation of each Volumetric Light Beam with 'Shared' MeshType.\nAdjust them carefully since they could impact performance.";
                        meshInfo += string.Format("\nShared Mesh stats: {0} vertices, {1} triangles", MeshGenerator.GetSharedMeshVertexCount(), MeshGenerator.GetSharedMeshIndicesCount() / 3);
                        EditorGUILayout.HelpBox(meshInfo, MessageType.Info);
                    }
                    FoldableHeader.End();

                    if (FoldableHeader.Begin(this, "Global 3D Noise"))
                    {
                        EditorGUILayout.PropertyField(globalNoiseScale, EditorStrings.Config.GlobalNoiseScale);
                        EditorGUILayout.PropertyField(globalNoiseVelocity, EditorStrings.Config.GlobalNoiseVelocity);
                    }
                    FoldableHeader.End();

                    if (FoldableHeader.Begin(this, "Camera to compute Fade Out"))
                    {
                        EditorGUI.BeginChangeCheck();
                        fadeOutCameraTag.stringValue = EditorGUILayout.TagField(EditorStrings.Config.FadeOutCameraTag, fadeOutCameraTag.stringValue);
                        if (EditorGUI.EndChangeCheck() && Application.isPlaying)
                            (target as Config).ForceUpdateFadeOutCamera();
                    }
                    FoldableHeader.End();

                    if (FoldableHeader.Begin(this, "Internal Data (do not change)"))
                    {
                        EditorGUILayout.PropertyField(beamShader1Pass, EditorStrings.Config.BeamShader1Pass);
                        EditorGUILayout.PropertyField(beamShader2Pass, EditorStrings.Config.BeamShader2Pass);
                        if (BeamGeometry.isCustomRenderPipelineSupported)
                            EditorGUILayout.PropertyField(beamShaderSRP,   EditorStrings.Config.BeamShaderSRP);
                        EditorGUILayout.PropertyField(dustParticlesPrefab, EditorStrings.Config.DustParticlesPrefab);

                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(noise3DData, EditorStrings.Config.Noise3DData);
                        EditorGUILayout.PropertyField(noise3DSize, EditorStrings.Config.Noise3DSize);
                        if (EditorGUI.EndChangeCheck())
                            reloadNoise = true;

                        if (Noise3D.isSupported && !Noise3D.isProperlyLoaded)
                            EditorGUILayout.HelpBox(EditorStrings.Common.HelpNoiseLoadingFailed, MessageType.Error);
                    }
                    FoldableHeader.End();

                    if (GUILayout.Button(EditorStrings.Config.OpenDocumentation, EditorStyles.miniButton))
                    {
                        UnityEditor.Help.BrowseURL(Consts.HelpUrlConfig);
                    }

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button(EditorStrings.Config.ResetToDefaultButton, EditorStyles.miniButton))
                        {
                            UnityEditor.Undo.RecordObject(target, "Reset Config Properties");
                            (target as Config).Reset();
                            EditorUtility.SetDirty(target);
                        }

                        if (GUILayout.Button(EditorStrings.Config.ResetInternalDataButton, EditorStyles.miniButton))
                        {
                            UnityEditor.Undo.RecordObject(target, "Reset Internal Data");
                            (target as Config).ResetInternalData();
                            EditorUtility.SetDirty(target);
                        }
                    }
                }
                EditorGUI.EndDisabledGroup();
            }

            serializedObject.ApplyModifiedProperties();

            if (reloadNoise)
                Noise3D._EditorForceReloadData(); // Should be called AFTER ApplyModifiedProperties so the Config instance has the proper values when reloading data
        }
    }
}
#endif
