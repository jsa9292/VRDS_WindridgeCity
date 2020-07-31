using System;
using UnityEngine;

namespace NWH.VehiclePhysics2.Effects
{
    /// <summary>
    ///     One section (rectangle) of the skidmark.
    /// </summary>
    [Serializable]
    public struct SkidmarkRect
    {
        public Vector3 normal;
        public Vector3 position;
        public Vector3 positionLeft;
        public Vector3 positionRight;
        public Vector4 tangent;

        public static void LocalToWorldRect(ref SkidmarkRect localRect, ref SkidmarkRect worldRect, Transform t)
        {
            worldRect.position = t.TransformPoint(localRect.position);
            worldRect.normal = t.TransformDirection(localRect.normal);
            worldRect.tangent = t.TransformDirection(localRect.tangent);
            worldRect.positionLeft = t.TransformPoint(localRect.positionLeft);
            worldRect.positionRight = t.TransformPoint(localRect.positionRight);
        }

        public static void WorldToLocalRect(ref SkidmarkRect worldRect, ref SkidmarkRect localRect, Transform t)
        {
            localRect.position = t.InverseTransformPoint(worldRect.position);
            localRect.normal = t.InverseTransformDirection(worldRect.normal);
            localRect.tangent = t.InverseTransformDirection(worldRect.tangent);
            localRect.positionLeft = t.InverseTransformPoint(worldRect.positionLeft);
            localRect.positionRight = t.InverseTransformPoint(worldRect.positionRight);
        }
    }
}