using System;
using System.Collections.Generic;
using UnityEngine;

namespace NWH.VehiclePhysics2.GroundDetection
{
    /// <summary>
    ///     Maps SurfacePreset to the terrain texture indices and object tags.
    /// </summary>
    [Serializable]
    public class SurfaceMap
    {
        /// <summary>
        ///     Name of the surface map. For display purposes only.
        /// </summary>
        [Tooltip("    Name of the surface map. For display purposes only.")]
        public string name;

        public SurfacePreset surfacePreset;

        /// <summary>
        ///     Objects with tags in this list will be recognized as this type of surface.
        /// </summary>
        [Tooltip("    Objects with tags in this list will be recognized as this type of surface.")]
        public List<string> tags = new List<string>();

        /// <summary>
        ///     Indices of terrain textures that represent this type of surface. Starts with 0 with the first texture being in the
        ///     top left corner
        ///     under terrain settings - Paint Texture.
        /// </summary>
        [Tooltip(
            "Indices of terrain textures that represent this type of surface. Starts with 0 with the first texture being in the top left corner " +
            "under terrain settings - Paint Texture.")]
        public List<int> terrainTextureIndices = new List<int>();
    }
}