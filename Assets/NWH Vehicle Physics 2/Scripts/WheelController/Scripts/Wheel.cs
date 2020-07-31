using System;
using System.Collections.Generic;
using NWH.VehiclePhysics2.Demo;
using UnityEngine;

namespace NWH.WheelController3D
{
    /// <summary>
    ///     Contains everything wheel related, including rim and tire.
    /// </summary>
    [Serializable]
    public class Wheel
    {
        /// <summary>
        ///     Current angular velocity of the wheel in rad/s.
        /// </summary>
        [ShowInTelemetry]
        [Tooltip("    Current angular velocity of the wheel in rad/s.")]
        public float angularVelocity;

        /// <summary>
        ///     Brake torque applied to the wheel in Nm.
        /// </summary>
        [ShowInTelemetry]
        [Tooltip("    Brake torque applied to the wheel in Nm.")]
        public float brakeTorque;

        /// <summary>
        ///     Current camber angle.
        /// </summary>
        [ShowInTelemetry]
        [Tooltip("    Current camber angle.")]
        public float camberAngle;

        /// <summary>
        ///     Camber angle at the bottom of suspension travel (fully extended).
        /// </summary>
        [Tooltip("    Camber angle at the bottom of suspension travel (fully extended).")]
        public float camberAtBottom;

        /// <summary>
        ///     Camber angle at the top of suspension travel (fully compressed).
        /// </summary>
        [Tooltip("    Camber angle at the top of suspension travel (fully compressed).")]
        public float camberAtTop;

        /// <summary>
        ///     Forward vector of the wheel in world coordinates.
        /// </summary>
        [Tooltip("    Forward vector of the wheel in world coordinates.")]
        public Vector3 forward;

        /// <summary>
        ///     Inertia of the wheel.
        /// </summary>
        [Tooltip("    Inertia of the wheel.")]
        public float inertia;

        /// <summary>
        ///     Vector in world coordinates pointing towards the inside of the wheel.
        /// </summary>
        [Tooltip("    Vector in world coordinates pointing towards the inside of the wheel.")]
        public Vector3 inside;

        /// <summary>
        ///     Tire load in Nm.
        /// </summary>
        [ShowInTelemetry]
        [Tooltip("    Tire load in Nm.")]
        public float load;

        /// <summary>
        ///     Mass of the wheel. Inertia is calculated from this.
        /// </summary>
        [Tooltip("    Mass of the wheel. Inertia is calculated from this.")]
        public float mass = 20.0f;

        /// <summary>
        ///     Motor torque applied to the wheel. Since NWH Vehicle Physics 2 the value is readonly and setting it will have no
        ///     effect
        ///     since torque calculation is done inside powertrain solver.
        /// </summary>
        [ShowInTelemetry]
        [Tooltip(
            "Motor torque applied to the wheel. Since NWH Vehicle Physics 2 the value is readonly and setting it will have no effect\r\nsince torque calculation is done inside powertrain solver.")]
        public float motorTorque;

        /// <summary>
        ///     Position offset of the non-rotating part.
        /// </summary>
        [Tooltip("    Position offset of the non-rotating part.")]
        public Vector3 nonRotatingPositionOffset;

        public bool nonRotatingVisualIsNull;

        public float prevAngularVelocity;

        /// <summary>
        ///     Total radius of the tire in [m].
        /// </summary>
        [Tooltip("    Total radius of the tire in [m].")]
        public float radius = 0.35f;

        /// <summary>
        ///     Vector in world coordinates pointing to the right of the wheel.
        /// </summary>
        [Tooltip("    Vector in world coordinates pointing to the right of the wheel.")]
        public Vector3 right;

        /// <summary>
        ///     GameObject containing the rim MeshCollider. This is used to prevent objects from penetrating into the wheel from
        ///     sides
        ///     or top,
        ///     where the ground detection does not work.
        /// </summary>
        [Tooltip(
            "GameObject containing the rim MeshCollider. This is used to prevent objects from penetrating into the wheel from sides\r\nor top,\r\nwhere the ground detection does not work.")]
        public GameObject rimColliderGO;

        /// <summary>
        ///     Offset of the rim from the center of steering rotation.
        /// </summary>
        [Tooltip("    Offset of the rim from the center of steering rotation.")]
        public float rimOffset;

        /// <summary>
        ///     Current rotation angle of the wheel visual in regards to it's X axis vector.
        /// </summary>
        [Tooltip("    Current rotation angle of the wheel visual in regards to it's X axis vector.")]
        public float rotationAngle;

        /// <summary>
        ///     Current wheel RPM.
        /// </summary>
        [Tooltip("    Current wheel RPM.")]
        public float RPM;

        /// <summary>
        ///     Current steer angle of the wheel.
        /// </summary>
        [ShowInTelemetry]
        [Tooltip("    Current steer angle of the wheel.")]
        public float steerAngle;

        /// <summary>
        ///     Wheel's up vector in world coordinates.
        /// </summary>
        [Tooltip("    Wheel's up vector in world coordinates.")]
        public Vector3 up;

        public bool visualIsNull;

        /// <summary>
        ///     In cases where wheel visual's model might have wrong pivot point this field can
        ///     be used to center the wheel or move it in/out. It is always preferable to
        ///     fix the model in modelling software or by parenting it to another, empty, transform
        ///     and resetting the pivot that way.
        ///     https://docs.unity3d.com/Manual/HOWTO-FixZAxisIsUp.html
        /// </summary>
        [Tooltip(
            "In cases where wheel visual's model might have wrong pivot point this field can\r\nbe used to center the wheel or move it in/out. It is always preferable to\r\nfix the model in modelling software or by parenting it to another, empty, transform\r\nand resetting the pivot that way.\r\nhttps://docs.unity3d.com/Manual/HOWTO-FixZAxisIsUp.html")]
        public Vector3 visualPositionOffset = Vector3.zero;

        /// <summary>
        ///     Use if wheel visual's model has wrong rotation or if you want to make the wheel appear to wobble (adjust Z axis to
        ///     get
        ///     the wobble).
        ///     It is always preferable to fix the model in modelling software or by parenting it to another, empty, transform
        ///     and resetting the pivot that way.
        ///     https://docs.unity3d.com/Manual/HOWTO-FixZAxisIsUp.html
        /// </summary>
        [Tooltip(
            "Use if wheel visual's model has wrong rotation or if you want to make the wheel appear to wobble (adjust Z axis to get the wobble).\r\nIt is always preferable to\r\nfix the model in modelling software or by parenting it to another, empty, transform\r\nand resetting the pivot that way.\r\nhttps://docs.unity3d.com/Manual/HOWTO-FixZAxisIsUp.html")]
        public Vector3 visualRotationOffset = Vector3.zero;

        /// <summary>
        ///     Width of the tyre.
        /// </summary>
        [Tooltip("    Width of the tyre.")]
        public float width = 0.25f;

        /// <summary>
        ///     Position of the wheel in world coordinates.
        /// </summary>
        [Tooltip("    Position of the wheel in world coordinates.")]
        public Vector3 worldPosition;

        /// <summary>
        ///     Rotation of the wheel in world coordinates.
        /// </summary>
        [Tooltip("    Rotation of the wheel in world coordinates.")]
        public Quaternion worldRotation;

        /// <summary>
        ///     Object representing non-rotating part of the wheel. This could be things such as brake calipers, external fenders,
        ///     etc.
        /// </summary>
        [Tooltip(
            "Object representing non-rotating part of the wheel. This could be things such as brake calipers, external fenders, etc.")]
        [SerializeField]
        private GameObject nonRotatingVisual;

        /// <summary>
        ///     GameObject representing the visual aspect of the wheel / wheel mesh.
        ///     Should not have any physics colliders attached to it.
        /// </summary>
        [Tooltip(
            "GameObject representing the visual aspect of the wheel / wheel mesh.\r\nShould not have any physics colliders attached to it.")]
        [SerializeField]
        private GameObject visual;

        public GameObject Visual
        {
            get { return visual; }
            set
            {
                visual = value;
                visualIsNull = visual == null;
            }
        }

        public GameObject NonRotatingVisual
        {
            get { return nonRotatingVisual; }
            set
            {
                nonRotatingVisual = value;
                nonRotatingVisualIsNull = nonRotatingVisual == null;
            }
        }

        /// <summary>
        ///     Calculation of static parameters and creation of rim collider.
        /// </summary>
        public void Initialize(WheelController wc)
        {
            visualIsNull = visual == null;
            nonRotatingVisualIsNull = nonRotatingVisual == null;

            // Precalculate wheel variables
            inertia = 0.5f * mass * (radius * radius + radius * radius);

            if (rimColliderGO != null || !wc.useRimCollider || visual == null)
            {
                return;
            }

            // Instantiate rim (prevent ground passing through the side of the wheel)
            rimColliderGO = new GameObject();
            rimColliderGO.name = "RimCollider";
            rimColliderGO.transform.position =
                wc.transform.position + wc.transform.right * (rimOffset * (int) wc.vehicleSide);
            rimColliderGO.transform.parent = wc.transform;
            rimColliderGO.layer = LayerMask.NameToLayer("Ignore Raycast");

            MeshFilter mf = rimColliderGO.AddComponent<MeshFilter>();
            mf.name = "Rim Mesh Filter";
            mf.mesh = GenerateRimColliderMesh(visual.transform);
            mf.mesh.name = "Rim Mesh";

            MeshCollider mc = rimColliderGO.AddComponent<MeshCollider>();
            mc.name = "Rim MeshCollider";
            mc.convex = true;

            PhysicMaterial material = new PhysicMaterial();
            material.staticFriction = 0f;
            material.dynamicFriction = 0f;
            material.bounciness = 0.3f;
            mc.material = material;
        }

        private Mesh GenerateRimColliderMesh(Transform rt)
        {
            Mesh mesh = new Mesh();
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            float halfWidth = width / 1.5f;
            float theta = 0.0f;
            float startAngleOffset = Mathf.PI / 18.0f;
            float x = radius * 0.5f * Mathf.Cos(theta);
            float y = radius * 0.5f * Mathf.Sin(theta);
            Vector3 pos = rt.InverseTransformPoint(worldPosition + up * y + forward * x);
            Vector3 newPos = pos;

            int vertexIndex = 0;
            for (theta = startAngleOffset; theta <= Mathf.PI * 2 + startAngleOffset; theta += Mathf.PI / 12.0f)
            {
                if (theta <= Mathf.PI - startAngleOffset)
                {
                    x = radius * 1.06f * Mathf.Cos(theta);
                    y = radius * 1.06f * Mathf.Sin(theta);
                }
                else
                {
                    x = radius * 0.05f * Mathf.Cos(theta);
                    y = radius * 0.05f * Mathf.Sin(theta);
                }

                newPos = rt.InverseTransformPoint(worldPosition + up * y + forward * x);

                // Left Side
                Vector3 p0 = pos - rt.InverseTransformDirection(right) * halfWidth;
                Vector3 p1 = newPos - rt.InverseTransformDirection(right) * halfWidth;

                // Right side
                Vector3 p2 = pos + rt.InverseTransformDirection(right) * halfWidth;
                Vector3 p3 = newPos + rt.InverseTransformDirection(right) * halfWidth;

                vertices.Add(p0);
                vertices.Add(p1);
                vertices.Add(p2);
                vertices.Add(p3);

                // Triangles (double sided)
                // 013
                triangles.Add(vertexIndex + 3);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 0);

                // 023
                triangles.Add(vertexIndex + 0);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 3);

                pos = newPos;
                vertexIndex += 4;
            }

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            return mesh;
        }
    }
}