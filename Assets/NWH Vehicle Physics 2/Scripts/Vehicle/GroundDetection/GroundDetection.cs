using System;
using NWH.VehiclePhysics2.Powertrain;
using NWH.WheelController3D;
using UnityEngine;
using WheelHit = NWH.WheelController3D.WheelHit;

namespace NWH.VehiclePhysics2.GroundDetection
{
    /// <summary>
    ///     Handles surface/ground detection for the vehicle.
    /// </summary>
    [Serializable]
    public class GroundDetection : VehicleComponent
    {
        public GroundDetectionPreset groundDetectionPreset;
        private Terrain activeTerrain;

        private int currentIndex;
        private Transform hitTransform;
        private float[] mix;

        private float[,,] splatmapData;
        private TerrainData terrainData;
        private Vector3 terrainPos;

        public override void Initialize()
        {
            initialized = true;
        }

        public override void FixedUpdate()
        {
        }

        public override void Update()
        {
        }

        /// <summary>
        ///     Gets the surface map the wheel is currently on.
        /// </summary>
        public bool GetCurrentSurfaceMap(WheelController wheelController, ref int surfaceIndex,
            ref SurfacePreset outSurfacePreset)
        {
            surfaceIndex = -1;
            outSurfacePreset = null;
            
            if (!IsEnabled)
            {
                return false;
            }

            if (groundDetectionPreset == null)
            {
                Debug.LogError("GroundDetectionPreset is required but is null. Go to VehicleController > FX > Grnd. Det. and " +
                               "assign a GroundDetectionPreset.");
                return false;
            }

            hitTransform = wheelController.wheelHit?.raycastHit.transform;
            if (wheelController.isGrounded && hitTransform != null)
            {
                wheelController.GetGroundHit(out WheelHit hit);

                // Check for tags
                int mapCount = groundDetectionPreset.surfaceMaps.Count;
                for (int e = 0; e < mapCount; e++)
                {
                    SurfaceMap map = groundDetectionPreset.surfaceMaps[e];
                    int tagCount = map.tags.Count;

                    for (int i = 0; i < tagCount; i++)
                    {
                        if (hitTransform.CompareTag(map.tags[i]))
                        {
                            outSurfacePreset = map.surfacePreset;
                            surfaceIndex = e;
                            return true;
                        }
                    }
                }

                // Find active terrain
                activeTerrain = hitTransform.GetComponent<Terrain>();

                if (activeTerrain)
                {
                    // Check for terrain textures
                    int dominantTerrainIndex = GetDominantTerrainTexture(hit.point, activeTerrain);
                    if (dominantTerrainIndex != -1)
                    {
                        for (int e = 0; e < groundDetectionPreset.surfaceMaps.Count; e++)
                        {
                            SurfaceMap map = groundDetectionPreset.surfaceMaps[e];

                            int n = map.terrainTextureIndices.Count;
                            for (int i = 0; i < n; i++)
                            {
                                if (map.terrainTextureIndices[i] == dominantTerrainIndex)
                                {
                                    outSurfacePreset = map.surfacePreset;
                                    surfaceIndex = e;
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            if (groundDetectionPreset.fallbackSurfacePreset != null)
            {
                outSurfacePreset = groundDetectionPreset.fallbackSurfacePreset;
                surfaceIndex = -1;
                return true;
            }

            Debug.LogError(
                $"Fallback surface map of ground detection preset {groundDetectionPreset.name} not assigned.");
            outSurfacePreset = null;
            surfaceIndex = -1;
            return false;
        }

        /// <summary>
        ///     Returns most prominent texture at the point in a terrain.
        /// </summary>
        public int GetDominantTerrainTexture(Vector3 worldPos, Terrain terrain)
        {
            // returns the zero-based surfaceIndex of the most dominant texture
            // on the main terrain at this world position.
            GetTerrainTextureComposition(worldPos, terrain, ref mix);
            if (mix != null)
            {
                float maxMix = 0;
                int maxIndex = 0;
                // loop through each mix value and find the maximum
                for (int n = 0; n < mix.Length; ++n)
                {
                    if (mix[n] > maxMix)
                    {
                        maxIndex = n;
                        maxMix = mix[n];
                    }
                }

                return maxIndex;
            }

            return -1;
        }

        public void GetTerrainTextureComposition(Vector3 worldPos, Terrain terrain, ref float[] cellMix)
        {
            terrainData = terrain.terrainData;
            terrainPos = terrain.transform.position;
            // Calculate which splat map cell the worldPos falls within (ignoring y)
            int mapX = (int) ((worldPos.x - terrainPos.x) / terrainData.size.x * terrainData.alphamapWidth);
            int mapZ = (int) ((worldPos.z - terrainPos.z) / terrainData.size.z * terrainData.alphamapHeight);
            // Get the splat data for this cell as a 1x1xN 3d array (where N = number of textures)
            splatmapData = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);
            // Extract the 3D array data to a 1D array:
            cellMix = new float[splatmapData.GetUpperBound(2) + 1];
            for (int n = 0; n < cellMix.Length; ++n)
            {
                cellMix[n] = splatmapData[0, 0, n];
            }
        }


        public override void SetDefaults(VehicleController vc)
        {
            base.SetDefaults(vc);

            if (groundDetectionPreset == null)
            {
                groundDetectionPreset =
                    Resources.Load(VehicleController.DEFAULT_RESOURCES_PATH + "DefaultGroundDetectionPreset")
                        as GroundDetectionPreset;
            }
        }

        public override void Validate(VehicleController vc)
        {
            base.Validate(vc);
            
            Debug.Assert(groundDetectionPreset != null, "GroundDetectionPreset is required but is null. " +
                                                        "Go to VehicleController > FX > Grnd. Det. and " +
                                                        "assign a GroundDetectionPreset.");

            if (groundDetectionPreset != null)
            {
                Debug.Assert(groundDetectionPreset.fallbackSurfacePreset != null, $"Fallback Surface Preset is not assigned " +
                                                                                  $"for {groundDetectionPreset.name}. Fallback Surface Preset is the only required" +
                                                                                  $" SurfacePreset. Go to VehicleController > FX > Grnd. Det. and " +
                                                                                  "assign a Fallback Surface Preset.");

                // Check if surface map tags exist in the scene
                for (int i = 0; i < groundDetectionPreset.surfaceMaps.Count; i++)
                {
                    for (int j = 0; j < groundDetectionPreset.surfaceMaps[i].tags.Count; j++)
                    {
                        string tag = groundDetectionPreset.surfaceMaps[i].tags[j];
                        try
                        {
                            vc.transform.CompareTag(tag);
                        }
                        catch
                        {
                            Debug.LogWarning($"Tag '{tag}' does not exist in the scene yet the SurfaceMap {groundDetectionPreset.surfaceMaps[i].name}" +
                                             $" uses it. Make sure to add the missing tag or to remove it from the surface map if not needed." +
                                             $" This could happen if you are using default/demo GroundDetectionPreset in a project where these tags are not defined.");
                            throw;
                        }
                    }
                }
            }
        }
    }
}