using System;
using UnityEngine;

namespace NWH.VehiclePhysics2
{
    /// <summary>
    ///     ScriptableObject representing settings for a single LOD.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "NWH Vehicle Physics", menuName = "NWH Vehicle Physics/LOD",
        order = 1)]
    public class LOD : ScriptableObject
    {
        public float distance;
        public bool singleRayGroundDetection;
    }
}