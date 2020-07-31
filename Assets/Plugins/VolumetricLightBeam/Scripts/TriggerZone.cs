using UnityEngine;

namespace VLB
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(VolumetricLightBeam))]
    [HelpURL(Consts.HelpUrlTriggerZone)]
    public class TriggerZone : MonoBehaviour
    {
        /// <summary>
        /// Define if the Collider will be created as a convex trigger (not physical, most common behavior) or as a regular collider (physical).
        /// </summary>
        public bool setIsTrigger = true;

        /// <summary>
        /// Change the length of the Collider. For example, set 2.0 to make the Collider 2x longer than the beam. Default value is 1.0.
        /// </summary>
        public float rangeMultiplier = 1.0f;

        const int kMeshColliderNumSides = 8;
        Mesh m_Mesh = null;

        void Update()
        {
            var beam = GetComponent<VolumetricLightBeam>();

            if (beam)
            {
                var meshCollider = gameObject.GetOrAddComponent<MeshCollider>();
                Debug.Assert(meshCollider);

                var rangeEnd = beam.fallOffEnd * rangeMultiplier;
                var lerpedRadiusEnd = Mathf.LerpUnclamped(beam.coneRadiusStart, beam.coneRadiusEnd, rangeMultiplier);

                m_Mesh = MeshGenerator.GenerateConeZ_Radius(rangeEnd, beam.coneRadiusStart, lerpedRadiusEnd, kMeshColliderNumSides, 0, false, false);
                m_Mesh.hideFlags = Consts.ProceduralObjectsHideFlags;
                meshCollider.sharedMesh = m_Mesh;

                if (setIsTrigger)
                {
                    meshCollider.convex = true;
                    meshCollider.isTrigger = true;
                }

                GameObject.Destroy(this);
            }
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            rangeMultiplier = Mathf.Max(rangeMultiplier, 0.001f);
        }
#endif
    }
}
