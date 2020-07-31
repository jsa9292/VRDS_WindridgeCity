using System;
using NWH.VehiclePhysics2.Powertrain;
using UnityEngine;
using UnityEngine.Rendering;

namespace NWH.VehiclePhysics2.Effects
{
    /// <summary>
    ///     Generates skidmark meshes.
    /// </summary>
    [Serializable]
    public class SkidmarkGenerator
    {
        private float _albedoIntensity;
        private Bounds _bounds = new Bounds(Vector3.zero, Vector3.one * 10000);
        private Color _color = new Color32(0, 0, 0, 0);
        private Color[] _colors;
        private int _commonIndex;
        private SkidmarkRect _currentRect;
        private Vector3 _direction, _xDirection;
        private bool _fadeOverDistance = true;
        private float _groundOffset = 0.014f;
        private int _head;
        private float _intensity;
        private float _intensityVelocity;
        private bool _isGrounded;
        private bool _isInitial = true;
        private float _lowerIntensityThreshold = 0.01f;
        private float _markWidth = -1f;
        private int _maxMarks = 512;
        private int _maxTris;
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private float _minSqrDistance;
        private float _normalIntensity;
        private Vector3[] _normals;
        private int[] _outTriArray;
        private bool _persistent;
        private float _persistentDistance;
        private Color _prevColor = new Color(0, 0, 0, 0);
        private float _prevIntensity;
        private SkidmarkRect _previousRect;
        private int _prevSurfaceMapIndex;
        private int _sectionCount;
        private GameObject _skidmarkContainer;
        private SkidmarkDestroy _skidmarkDestroy;
        private Mesh _skidmarkMesh;
        private GameObject _skidObject;
        private float _smoothing = 0.5f;
        private int _surfaceMapCount;
        private int _surfaceMapIndex = -1;
        private int _tail;
        private Vector4[] _tangents;
        private WheelComponent _targetWheelComponent;
        private int[] _triangles;
        private Vector2[] _uvs;
        private Vector2 _vector00 = new Vector2(0, 0);
        private Vector2 _vector01 = new Vector2(0, 1);
        private Vector2 _vector10 = new Vector2(1, 0);
        private Vector2 _vector11 = new Vector2(1, 1);

        private Vector3[] _vertices;
        private bool _wasGroundedFlag;
        private Material _fallbackMaterial;


        public bool Initialize(WheelComponent wheelComponent, GameObject skidmarkContainer,
            float minDistance,
            int surfaceMapCount, int maxMarks, bool persistent, float persistentDistance,
            float groundOffset, float smoothing, float lowerIntensityThreshold, bool fadeOverDistance, Material fallbackMaterial)
        {
            _targetWheelComponent = wheelComponent;
            _minSqrDistance = minDistance * minDistance;
            _maxMarks = maxMarks;
            _surfaceMapCount = surfaceMapCount;
            _persistent = persistent;
            _persistentDistance = persistentDistance;
            _fadeOverDistance = !persistent && fadeOverDistance;
            _groundOffset = groundOffset;
            _smoothing = smoothing;
            _lowerIntensityThreshold = lowerIntensityThreshold;
            _skidmarkContainer = skidmarkContainer;
            _fallbackMaterial = fallbackMaterial;

            _maxTris = maxMarks * 6;
            _outTriArray = new int[_maxTris];
            _markWidth = wheelComponent.Width;

            _isInitial = true;
            _prevSurfaceMapIndex = _surfaceMapIndex;

            GenerateNewSection();
            return true;
        }

        public void Update(int surfaceMapIndex, float newIntensity, float albedoIntensity, float normalIntensity,
            Vector3 velocity, float dt)
        {
            _isGrounded = _targetWheelComponent.IsGrounded;
            _prevIntensity = _intensity;

            _surfaceMapIndex = surfaceMapIndex;
            _albedoIntensity = albedoIntensity;
            _normalIntensity = normalIntensity;

            if (newIntensity < _lowerIntensityThreshold || !_isGrounded)
            {
                _intensity = 0;
                _wasGroundedFlag = false;
                return;
            }

            _intensity = Mathf.SmoothDamp(_intensity, newIntensity, ref _intensityVelocity, _smoothing);

            // Calculate skidmark intensity on hard surfaces (asphalt, concrete, etc.)
            if (surfaceMapIndex >= 0)
            {
                // Get current position
                Vector3 currentPosition =
                    _skidObject.transform.InverseTransformPoint(
                        _targetWheelComponent.wheelController.wheelHit.groundPoint);
                currentPosition += _targetWheelComponent.wheelController.wheelHit.normal * _groundOffset;
                currentPosition += velocity * (dt * 0.5f);

                // Check distance
                float sqrDistance = (currentPosition - _previousRect.position).sqrMagnitude;
                if (sqrDistance < _minSqrDistance)
                {
                    return;
                }


                bool startAnew = _isGrounded && !_wasGroundedFlag || _intensity > 0 && _prevIntensity <= 0 ||
                                 surfaceMapIndex != _prevSurfaceMapIndex;

                // Initial triangle, can not use data from previous
                if (_isInitial || startAnew)
                {
                    Transform controllerTransform = _targetWheelComponent.ControllerTransform;
                    _currentRect.position = currentPosition;
                    _currentRect.normal = _targetWheelComponent.wheelController.wheelHit.normal;
                    Vector3 right = controllerTransform.right;
                    _currentRect.positionLeft =
                        currentPosition - right * (_markWidth * 0.5f);
                    _currentRect.positionRight =
                        currentPosition + right * (_markWidth * 0.5f);

                    _direction = controllerTransform.forward;
                    _xDirection = -right;
                    _currentRect.tangent = new Vector4(_xDirection.x, _xDirection.y, _xDirection.y, 1f);

                    _previousRect = _currentRect;
                    _wasGroundedFlag = true;

                    _isInitial = false;
                }
                // Use data from previous triangle to calculate geometry
                else
                {
                    _currentRect.position = currentPosition;
                    _currentRect.normal = _targetWheelComponent.wheelController.wheelHit.normal;
                    _direction = _currentRect.position - _previousRect.position;
                    _xDirection = Vector3.Cross(_direction, _targetWheelComponent.wheelController.wheelHit.normal)
                        .normalized;

                    _color.a = _intensity;
                    _currentRect.positionLeft = currentPosition + _xDirection * (_markWidth * 0.5f);
                    _currentRect.positionRight = currentPosition - _xDirection * (_markWidth * 0.5f);
                    _currentRect.tangent = new Vector4(_xDirection.x, _xDirection.y, _xDirection.z, 1f);
                }

                GenerateRectGeometry();

                _previousRect = _currentRect;
            }

            _prevSurfaceMapIndex = surfaceMapIndex;
        }

        public void DoubleSubArray(ref int[] data, ref int[] outArray, int index1, int index2, int length1, int length2)
        {
            Array.Copy(data, index1, outArray, 0, length1);
            Array.Copy(data, index2, outArray, length1, length2);
        }

        public void FadeOut(int startIndex, int endIndex, float targetAlpha)
        {
            if (startIndex > endIndex || startIndex >= _colors.Length || endIndex >= _colors.Length)
            {
                return;
            }

            float initAlpha = _colors[startIndex].a;
            float range = endIndex - startIndex;
            for (int i = startIndex; i < endIndex; i++)
            {
                _colors[i].a = Mathf.Lerp(initAlpha, targetAlpha, (i - startIndex) / range);
            }
        }

        public void GenerateNewSection()
        {
            _maxTris = _maxMarks * 6;
            _commonIndex = 0;
            _head = 0;
            _tail = 0;

            // Add skid object
            _skidObject = new GameObject($"SkidMesh_{_targetWheelComponent.wheelController.Parent.name}_" +
                                         $"{_targetWheelComponent.wheelController.name}_{_sectionCount}");
            _skidObject.transform.parent = _skidmarkContainer.transform;
            _skidObject.transform.position = _targetWheelComponent.wheelController.transform.position;
            _skidObject.isStatic = true;

            if (_persistent)
            {
                if (_skidmarkDestroy != null)
                {
                    _skidmarkDestroy.skidmarkIsBeingUsed = false; // Mark old section as not being used any more.
                }

                // Setup skidmark auto-destroy
                _skidmarkDestroy = _skidObject.AddComponent<SkidmarkDestroy>();
                _skidmarkDestroy.targetTransform = _targetWheelComponent.wheelController.transform;
                _skidmarkDestroy.distanceThreshold = _persistentDistance;
                _skidmarkDestroy.skidmarkIsBeingUsed = true;
            }


            // Setup mesh renderer
            if (!_skidObject.GetComponent<MeshRenderer>())
            {
                _meshRenderer = _skidObject.AddComponent<MeshRenderer>();
                if (_targetWheelComponent.surfacePreset != null)
                {
                    _meshRenderer.material = _targetWheelComponent.surfacePreset.skidmarkMaterial;
                }
                else
                {
                    _meshRenderer.material = _fallbackMaterial;
                }
                
                _meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
                _meshRenderer.lightProbeUsage = LightProbeUsage.Off;
            }

            // Add mesh filter
            _meshFilter = _skidObject.AddComponent<MeshFilter>();

            // Intiialize mesh arrays
            _vertices = new Vector3[_maxMarks * 4 * _surfaceMapCount];
            _normals = new Vector3[_maxMarks * 4 * _surfaceMapCount];
            _tangents = new Vector4[_maxMarks * 4 * _surfaceMapCount];
            _colors = new Color[_maxMarks * 4 * _surfaceMapCount];
            _uvs = new Vector2[_maxMarks * 4 * _surfaceMapCount];
            _triangles = new int[_maxMarks * 9];

            // Create new mesh
            _skidmarkMesh = new Mesh();
            _skidmarkMesh.bounds = _bounds;
            _skidmarkMesh.MarkDynamic();
            _skidmarkMesh.name = "SkidmarkMesh";
            _skidmarkMesh.subMeshCount = _surfaceMapCount;
            _meshFilter.mesh = _skidmarkMesh;

            _isInitial = true;

            _sectionCount++;
        }

        public void SubArray(ref int[] data, ref int[] outArray, int index, int length)
        {
            Array.Copy(data, index, outArray, 0, length);
        }

        private void GenerateRectGeometry()
        {
            int indexOffset = _commonIndex * 4;

            // Generate geometry.
            _vertices[indexOffset + 0] = _previousRect.positionLeft;
            _vertices[indexOffset + 1] = _previousRect.positionRight;
            _vertices[indexOffset + 2] = _currentRect.positionLeft;
            _vertices[indexOffset + 3] = _currentRect.positionRight;

            _normals[indexOffset + 0] = _previousRect.normal;
            _normals[indexOffset + 1] = _previousRect.normal;
            _normals[indexOffset + 2] = _currentRect.normal;
            _normals[indexOffset + 3] = _currentRect.normal;

            _tangents[indexOffset + 0] = _previousRect.tangent;
            _tangents[indexOffset + 1] = _previousRect.tangent;
            _tangents[indexOffset + 2] = _currentRect.tangent;
            _tangents[indexOffset + 3] = _currentRect.tangent;

            _colors[indexOffset + 0] = _prevColor;
            _colors[indexOffset + 1] = _prevColor;

            _color.r = _albedoIntensity;
            _color.g = _normalIntensity;
            _color.a = _intensity;

            _colors[indexOffset + 2] = _color;
            _colors[indexOffset + 3] = _color;

            // Fade
            if (_fadeOverDistance)
            {
                float fadeIntensity = 1f - 2f / _maxMarks;
                int n = _colors.Length;
                for (int i = 0; i < n; i++)
                {
                    _colors[i].a *= fadeIntensity;
                }
            }

            _prevColor = _color;

            _uvs[indexOffset + 0] = _vector00;
            _uvs[indexOffset + 1] = _vector10;
            _uvs[indexOffset + 2] = _vector01;
            _uvs[indexOffset + 3] = _vector11;

            int startTriIndex = _head;

            _triangles[startTriIndex + 0] = _commonIndex * 4 + 0;
            _triangles[startTriIndex + 2] = _commonIndex * 4 + 1;
            _triangles[startTriIndex + 1] = _commonIndex * 4 + 2;

            _triangles[startTriIndex + 3] = _commonIndex * 4 + 2;
            _triangles[startTriIndex + 5] = _commonIndex * 4 + 1;
            _triangles[startTriIndex + 4] = _commonIndex * 4 + 3;

            // Reassign the mesh
            _skidmarkMesh.vertices = _vertices;
            _skidmarkMesh.normals = _normals;
            _skidmarkMesh.tangents = _tangents;

            _head += 6;
            if (_head >= _maxMarks * 9)
            {
                _head = _head - _maxMarks * 9;
            }

            // Tail is chasing head
            if (_head > _tail)
            {
                int length = _head - _tail;
                SubArray(ref _triangles, ref _outTriArray, _head - length, length);
                _skidmarkMesh.SetTriangles(_outTriArray, 0);
            }
            // Head is chasing tail
            else if (_head < _tail)
            {
                int index1 = _tail;
                int length1 = _maxMarks * 9 - _tail;

                int index2 = 0;
                int length2 = _head;

                DoubleSubArray(ref _triangles, ref _outTriArray, index1, index2, length1, length2);
                _skidmarkMesh.SetTriangles(_outTriArray, 0);
            }

            // Assign to mesh
            _skidmarkMesh.colors = _colors;
            _skidmarkMesh.uv = _uvs;
            _skidmarkMesh.bounds = _bounds;
            _meshFilter.mesh = _skidmarkMesh;


            int triCount = GetTriangleCount();
            bool triangleOverflow = triCount >= _maxTris - 1;

            if (triangleOverflow && _persistent)
            {
                GenerateNewSection();
                return;
            }

            if (triangleOverflow)
            {
                // Move tail to keep the number of triangles at max value
                int prevTail = _tail;

                _tail += 6;

                // Wrap tail around if needed
                if (_tail >= _maxMarks * 9)
                {
                    _tail = _tail - _maxMarks * 9;
                }

                if (prevTail < _head && _tail > _head)
                {
                    _tail = _head;
                }
            }

            // Update common index (surface independent)
            _commonIndex++;
            if (_commonIndex >= _maxMarks * _surfaceMapCount)
            {
                _commonIndex = 0;
            }
        }

        private int GetTriangleCount()
        {
            if (_head == _tail)
            {
                return 0;
            }

            if (_head > _tail)
            {
                return _head - _tail;
            }

            if (_head < _tail)
            {
                return _maxMarks * 9 - _tail + _head;
            }

            return 0;
        }

        public struct EndIndex
        {
            public int surfaceMapIndex;
            public int triIndex;

            public EndIndex(int surfaceMapIndex, int triIndex)
            {
                this.surfaceMapIndex = surfaceMapIndex;
                this.triIndex = triIndex;
            }
        }
    }
}