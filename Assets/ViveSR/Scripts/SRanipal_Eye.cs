//========= Copyright 2018, HTC Corporation. All rights reserved. ===========
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ViveSR
{
    namespace anipal
    {
        namespace Eye
        {
            public enum EyeIndex { LEFT, RIGHT, }
            public enum GazeIndex { LEFT, RIGHT, COMBINE }

            public enum EyeShape
            {
                None            = -1,
                Eye_Left_Blink  = 0,
                Eye_Left_Wide,
                Eye_Left_Right,
                Eye_Left_Left,
                Eye_Left_Up,
                Eye_Left_Down,
                Eye_Right_Blink = 6,
                Eye_Right_Wide,
                Eye_Right_Right,
                Eye_Right_Left,
                Eye_Right_Up,
                Eye_Right_Down,
                Eye_Frown       = 12,
                Max             = 13,
            }

            [Serializable]
            public class EyeShapeTable
            {
                public SkinnedMeshRenderer skinnedMeshRenderer;
                public EyeShape[] eyeShapes;
            }

            public struct FocusInfo
            {
                public Vector3 point;
                public Vector3 normal;
                public float distance;
                public Collider collider;
                public Rigidbody rigidbody;
                public Transform transform;
            }

            public enum CalibrationResult
            {
                SUCCESS,
                FAIL,
                BUSY,
            }

            public static class SRanipal_Eye
            {
                public const int ANIPAL_TYPE_EYE = 0;

                /// <summary>
                /// Check HMD device is ViveProEye or not.
                /// </summary>
                /// <returns>true : ViveProEye, false : other HMD.</returns>
                [DllImport("SRanipal")]
                public static extern bool IsViveProEye();

                /// <summary>
                /// Gets data from anipal's Eye module.
                /// </summary>
                /// <param name="data">ViveSR.anipal.Eye.EyeData</param>
                /// <returns>Indicates the resulting ViveSR.Error status of this method.</returns>
                [DllImport("SRanipal")]
                public static extern Error GetEyeData(ref EyeData data);

                /// <summary>
                /// Sets the parameter of anipal's Eye module.
                /// </summary>
                /// <param name="parameter">ViveSR.anipal.Eye.EyeParameter</param>
                /// <returns>Indicates the resulting ViveSR.Error status of this method.</returns>
                [DllImport("SRanipal")]
                public static extern Error SetEyeParameter(EyeParameter parameter);

                /// <summary>
                /// Gets the parameter of anipal's Eye module.
                /// </summary>
                /// <param name="parameter">ViveSR.anipal.Eye.EyeParameter</param>
                /// <returns>Indicates the resulting ViveSR.Error status of this method.</returns>
                [DllImport("SRanipal")]
                public static extern Error GetEyeParameter(ref EyeParameter parameter);

                /// <summary>
                /// Indicate if user need to do eye calibration now.
                /// </summary>
                /// <param name="need">If need calibration, it will be true, otherwise it will be false.</param>
                /// <returns>Indicates the resulting ViveSR.Error status of this method.</returns>
                [DllImport("SRanipal")]
                public static extern int IsUserNeedCalibration(ref bool need);

                /// <summary>
                /// Launches anipal's Eye Calibration tool (an overlay program).
                /// </summary>
                /// <param name="callback">(Upcoming feature) A callback method invoked at the end of the calibration process.</param>
                /// <returns>Indicates the resulting ViveSR.Error status of this method.</returns>
                [DllImport("SRanipal")]
                private static extern int LaunchEyeCalibration(IntPtr callback);

                public const int WeightingCount = (int)EyeShape.Max;
                private static EyeData EyeData_;
                private static int LastUpdateFrame = -1;
                private static Error LastUpdateResult = Error.FAILED;
                private static Dictionary<EyeShape, float> Weightings;

                static SRanipal_Eye()
                {
                    Weightings = new Dictionary<EyeShape, float>();
                    for (int i = 0; i < WeightingCount; ++i) Weightings.Add((EyeShape)i, 0.0f);
                }

                private static bool UpdateData()
                {
                    if (Time.frameCount == LastUpdateFrame) return LastUpdateResult == Error.WORK;
                    else LastUpdateFrame = Time.frameCount;
                    LastUpdateResult = GetEyeData(ref EyeData_);
                    return LastUpdateResult == Error.WORK;
                }

                /// <summary>
                /// Gets the VerboseData of anipal's Eye module.
                /// </summary>
                /// <param name="data">ViveSR.anipal.Eye.VerboseData</param>
                /// <returns>Indicates whether the data received is new.</returns>
                public static bool GetVerboseData(out VerboseData data)
                {
                    bool update = UpdateData();
                    data = EyeData_.verbose_data;
                    return update;
                }

                /// <summary>
                /// Gets the openness value of an eye.
                /// </summary>
                /// <param name="eye">The index of an eye.</param>
                /// <param name="openness">The openness value of an eye, clamped between 0 (fully closed) and 1 (fully open). </param>
                /// <returns>Indicates whether the openness value received is valid.</returns>
                public static bool GetEyeOpenness(EyeIndex eye, out float openness)
                {
                    if (SRanipal_Eye_Framework.Status == SRanipal_Eye_Framework.FrameworkStatus.WORKING)
                    {
                        UpdateData();
                        SingleEyeData eyeData = eye == EyeIndex.LEFT ? EyeData_.verbose_data.left : EyeData_.verbose_data.right;
                        bool valid = eyeData.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_EYE_OPENNESS_VALIDITY);
                        openness = valid ? eyeData.eye_openness : 0; 
                    }
                    else
                    {
                        // If not support eye tracking, set default to open.
                        openness = 1;
                    }
                    return true;
                }

                /// <summary>
                /// Gets weighting values from anipal's Eye module.
                /// </summary>
                /// <param name="shapes">Weighting values obtained from anipal's Eye module.</param>
                /// <returns>Indicates whether the values received are new.</returns>
                public static bool GetEyeWeightings(out Dictionary<EyeShape, float> shapes)
                {
                    bool update = UpdateData();
                    float[] openness = new float[2];
                    bool[] valid = new bool[2];
                    Vector2[] pupilPosition = new Vector2[2];

                    foreach (EyeIndex index in (EyeIndex[])Enum.GetValues(typeof(EyeIndex)))
                    {
                        GetEyeOpenness(index, out openness[(int)index]);
                        valid[(int)index] = GetPupilPosition(index, out pupilPosition[(int)index]);
                    }

                    float[] weighting_up = new float[3] { Mathf.Max(pupilPosition[(int)GazeIndex.LEFT].y, 0f), Mathf.Max(pupilPosition[(int)GazeIndex.RIGHT].y, 0f), 0 };
                    float[] weighting_down = new float[3] { Mathf.Max(-pupilPosition[(int)GazeIndex.LEFT].y, 0f), Mathf.Max(-pupilPosition[(int)GazeIndex.RIGHT].y, 0f), 0 };
                    float[] weighting_left = new float[3] { Mathf.Max(-pupilPosition[(int)GazeIndex.LEFT].x, 0f), Mathf.Max(-pupilPosition[(int)GazeIndex.RIGHT].x, 0f), 0 };
                    float[] weighting_right = new float[3] { Mathf.Max(pupilPosition[(int)GazeIndex.LEFT].x, 0f), Mathf.Max(pupilPosition[(int)GazeIndex.RIGHT].x, 0f), 0 };
                    weighting_up[(int)GazeIndex.COMBINE] = (weighting_up[(int)GazeIndex.LEFT] + weighting_up[(int)GazeIndex.RIGHT]) / 2;
                    weighting_down[(int)GazeIndex.COMBINE] = (weighting_down[(int)GazeIndex.LEFT] + weighting_down[(int)GazeIndex.RIGHT]) / 2;
                    weighting_left[(int)GazeIndex.COMBINE] = (weighting_left[(int)GazeIndex.LEFT] + weighting_left[(int)GazeIndex.RIGHT]) / 2;
                    weighting_right[(int)GazeIndex.COMBINE] = (weighting_right[(int)GazeIndex.LEFT] + weighting_right[(int)GazeIndex.RIGHT]) / 2;

                    foreach (EyeShape index in (EyeShape[])Enum.GetValues(typeof(EyeShape)))
                    {
                        Weightings[index] = 0;
                    }
                    Weightings[EyeShape.Eye_Left_Blink] = 1 - openness[(int)EyeIndex.LEFT];
                    Weightings[EyeShape.Eye_Right_Blink] = 1 - openness[(int)EyeIndex.RIGHT];

                    if (valid[(int)EyeIndex.LEFT] && valid[(int)EyeIndex.RIGHT])
                    {
                        Ray gaze_ray = new Ray();
                        GetGazeRay(GazeIndex.COMBINE, out gaze_ray);
                        Vector3 gaze_direction = gaze_ray.direction - gaze_ray.origin;
                        gaze_direction.x = 0.0f;
                        Vector3 gaze_direction_normalized = gaze_direction.normalized;
                        Vector3 gaze_axis_z = Vector3.forward;
                        float y_weight = Mathf.Acos(Vector3.Dot(gaze_direction_normalized, gaze_axis_z));

                        Weightings[EyeShape.Eye_Left_Up] = gaze_direction_normalized.y < 0 ? 0 : y_weight;
                        Weightings[EyeShape.Eye_Left_Down] = gaze_direction_normalized.y < 0 ? y_weight : 0;
                        Weightings[EyeShape.Eye_Left_Left] = weighting_left[(int)GazeIndex.COMBINE];
                        Weightings[EyeShape.Eye_Left_Right] = weighting_right[(int)GazeIndex.COMBINE];
                        Weightings[EyeShape.Eye_Left_Wide] = Weightings[EyeShape.Eye_Left_Up];

                        Weightings[EyeShape.Eye_Right_Up] = gaze_direction_normalized.y < 0 ? 0 : y_weight;
                        Weightings[EyeShape.Eye_Right_Down] = gaze_direction_normalized.y < 0 ? y_weight : 0;
                        Weightings[EyeShape.Eye_Right_Left] = weighting_left[(int)GazeIndex.COMBINE];
                        Weightings[EyeShape.Eye_Right_Right] = weighting_right[(int)GazeIndex.COMBINE];
                        Weightings[EyeShape.Eye_Right_Wide] = Weightings[EyeShape.Eye_Right_Up];
                    }
                    else if (valid[(int)EyeIndex.LEFT])
                    {
                        Ray gaze_ray = new Ray();
                        GetGazeRay(GazeIndex.LEFT, out gaze_ray);
                        Vector3 gaze_direction = gaze_ray.direction - gaze_ray.origin;
                        gaze_direction.x = 0.0f;
                        Vector3 gaze_direction_normalized = gaze_direction.normalized;
                        Vector3 gaze_axis_z = Vector3.forward;
                        float y_weight = Mathf.Acos(Vector3.Dot(gaze_direction_normalized, gaze_axis_z));

                        Weightings[EyeShape.Eye_Left_Up] = gaze_direction_normalized.y < 0 ? 0 : y_weight;
                        Weightings[EyeShape.Eye_Left_Down] = gaze_direction_normalized.y < 0 ? y_weight : 0; 
                        Weightings[EyeShape.Eye_Left_Left] = weighting_left[(int)GazeIndex.LEFT];
                        Weightings[EyeShape.Eye_Left_Right] = weighting_right[(int)GazeIndex.LEFT];
                        Weightings[EyeShape.Eye_Left_Wide] = Weightings[EyeShape.Eye_Left_Up];
                    }
                    else if (valid[(int)EyeIndex.RIGHT])
                    {
                        Ray gaze_ray = new Ray();
                        GetGazeRay(GazeIndex.RIGHT, out gaze_ray);
                        Vector3 gaze_direction = gaze_ray.direction - gaze_ray.origin;
                        gaze_direction.x = 0.0f;
                        Vector3 gaze_direction_normalized = gaze_direction.normalized;
                        Vector3 gaze_axis_z = Vector3.forward;
                        float y_weight = Mathf.Acos(Vector3.Dot(gaze_direction_normalized, gaze_axis_z));

                        Weightings[EyeShape.Eye_Right_Up] = gaze_direction_normalized.y < 0 ? 0 : y_weight;
                        Weightings[EyeShape.Eye_Right_Down] = gaze_direction_normalized.y < 0 ? y_weight : 0;
                        Weightings[EyeShape.Eye_Right_Left] = weighting_left[(int)GazeIndex.RIGHT];
                        Weightings[EyeShape.Eye_Right_Right] = weighting_right[(int)GazeIndex.RIGHT];
                        Weightings[EyeShape.Eye_Right_Wide] = Weightings[EyeShape.Eye_Right_Up];
                    }
                    shapes = Weightings;
                    return update;
                }

                /// <summary>
                /// Tests eye gaze data.
                /// </summary>
                /// <param name="validity">A type of eye gaze data to test with.</param>
                /// <param name="gazeIndex">The index of a source of eye gaze data.</param>
                /// <returns>Indicates whether a source of eye gaze data is found.</returns>
                public static bool TryGaze(SingleEyeDataValidity validity, out GazeIndex gazeIndex)
                {
                    UpdateData();
                    bool[] valid = new bool[(int)GazeIndex.COMBINE + 1] { EyeData_.verbose_data.left.GetValidity(validity),
                                                                          EyeData_.verbose_data.right.GetValidity(validity),
                                                                          EyeData_.verbose_data.combined.eye_data.GetValidity(validity)};
                    gazeIndex = GazeIndex.COMBINE;
                    for (int i = (int)GazeIndex.COMBINE; i >= 0; --i)
                    {
                        if (valid[i])
                        {
                            gazeIndex = (GazeIndex)i;
                            return true;
                        }
                    }
                    return false;
                }

                /// <summary>
                /// Gets the gaze ray of a source of eye gaze data.
                /// </summary>
                /// <param name="gazeIndex">The index of a source of eye gaze data.</param>
                /// <param name="origin">The starting point of the ray in local coordinates.</param>
                /// <param name="direction">Tthe direction of the ray.</param>
                /// <returns>Indicates whether the eye gaze data received is valid.</returns>
                public static bool GetGazeRay(GazeIndex gazeIndex, out Vector3 origin, out Vector3 direction)
                {
                    bool valid = false;
                    origin = Vector3.zero;
                    direction = Vector3.forward;
                    if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING)
                    {
                        origin = Camera.main.transform.position;
                        valid = true;
                    }
                    else{
                        UpdateData();
                        SingleEyeData[] eyesData = new SingleEyeData[(int)GazeIndex.COMBINE + 1];
                        eyesData[(int)GazeIndex.LEFT] = EyeData_.verbose_data.left;
                        eyesData[(int)GazeIndex.RIGHT] = EyeData_.verbose_data.right;
                        eyesData[(int)GazeIndex.COMBINE] = EyeData_.verbose_data.combined.eye_data;

                        if (gazeIndex == GazeIndex.COMBINE)
                        {
                            valid = eyesData[(int)GazeIndex.COMBINE].GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_GAZE_DIRECTION_VALIDITY);
                            if (valid)
                            {
                                origin = eyesData[(int)GazeIndex.COMBINE].gaze_origin_mm * 0.001f;
                                direction = eyesData[(int)GazeIndex.COMBINE].gaze_direction_normalized;
                                direction.x *= -1;
                            }
                        }
                        else if (gazeIndex == GazeIndex.LEFT || gazeIndex == GazeIndex.RIGHT)
                        {
                            valid = eyesData[(int)gazeIndex].GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_GAZE_DIRECTION_VALIDITY);
                            if (valid)
                            {
                                origin = eyesData[(int)gazeIndex].gaze_origin_mm * 0.001f;
                                direction = eyesData[(int)gazeIndex].gaze_direction_normalized;
                                direction.x *= -1;
                            }
                        }
                    }
                    return valid;
                }

                /// <summary>
                /// Gets the gaze ray data of a source eye gaze data.
                /// </summary>
                /// <param name="gazeIndex">The index of a source of eye gaze data.</param>
                /// <param name="ray">The starting point and direction of the ray.</param>
                /// <returns>Indicates whether the gaze ray data received is valid.</returns>
                public static bool GetGazeRay(GazeIndex gazeIndex, out Ray ray)
                {
                    Vector3 origin = Vector3.zero;
                    Vector3 direction = Vector3.forward;
                    bool valid = false;
                    if (SRanipal_Eye_Framework.Status == SRanipal_Eye_Framework.FrameworkStatus.WORKING)
                    {
                        valid = GetGazeRay(gazeIndex, out origin, out direction);                   
                    }
                    else if (SRanipal_Eye_Framework.Status == SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT)
                    {
                        origin = Camera.main.transform.position;
                        valid = true;
                    }
                    ray = new Ray(origin, direction);
                    return valid;
                }

                /// <summary>
                /// Casts a ray against all colliders.
                /// </summary>
                /// <param name="index">A source of eye gaze data.</param>
                /// <param name="ray">The starting point and direction of the ray.</param>
                /// <param name="focusInfo">Information about where the ray focused on.</param>
                /// <param name="radius">The radius of the gaze ray</param>
                /// <param name="maxDistance">The max length of the ray.</param>
                /// <returns>Indicates whether the ray hits a collider.</returns>
                public static bool Focus(GazeIndex index, out Ray ray, out FocusInfo focusInfo, float radius, float maxDistance)
                {
                    UpdateData();
                    bool valid = GetGazeRay(index, out ray);
                    if (valid)
                    {
                        Ray rayGlobal = new Ray(Camera.main.transform.position, Camera.main.transform.TransformDirection(ray.direction));
                        RaycastHit hit;
                        if (radius == 0) valid = Physics.Raycast(rayGlobal, out hit, maxDistance, -1);
                        else             valid = Physics.SphereCast(rayGlobal, radius, out hit, maxDistance, -1);
                        focusInfo = new FocusInfo
                        {
                            point = hit.point,
                            normal = hit.normal,
                            distance = hit.distance,
                            collider = hit.collider,
                            rigidbody = hit.rigidbody,
                            transform = hit.transform
                        };
                    }
                    else
                    {
                        focusInfo = new FocusInfo();
                    }
                    return valid;
                }

                /// <summary>
                /// Casts a ray against all colliders.
                /// </summary>
                /// <param name="index">A source of eye gaze data.</param>
                /// <param name="ray">The starting point and direction of the ray.</param>
                /// <param name="focusInfo">Information about where the ray focused on.</param>
                /// <param name="maxDistance">The max length of the ray.</param>
                /// <returns>Indicates whether the ray hits a collider.</returns>
                public static bool Focus(GazeIndex index, out Ray ray, out FocusInfo focusInfo, float maxDistance)
                {
                    return Focus(index, out ray, out focusInfo, 0, float.MaxValue);
                }

                /// <summary>
                /// Casts a ray against all colliders.
                /// </summary>
                /// <param name="index">A source of eye gaze data.</param>
                /// <param name="ray">The starting point and direction of the ray.</param>
                /// <param name="focusInfo">Information about where the ray focused on.</param>
                /// <returns>Indicates whether the ray hits a collider.</returns>
                public static bool Focus(GazeIndex index, out Ray ray, out FocusInfo focusInfo)
                {
                    return Focus(index, out ray, out focusInfo, 0, float.MaxValue);
                }

                /// <summary>
                /// Gets the 2D position of a selected pupil.
                /// </summary>
                /// <param name="eye">The index of an eye.</param>
                /// <param name="postion">The 2D position of a selected pupil clamped between -1 and 1.
                /// Position (0, 0) indicates that the pupil is looking forward;
                /// position (1, 1) up-rightward; and
                /// position (-1, -1) left-downward.</param>
                /// <returns></returns>
                public static bool GetPupilPosition(EyeIndex eye, out Vector2 postion)
                {
                    bool valid = false;
                    if (SRanipal_Eye_Framework.Status == SRanipal_Eye_Framework.FrameworkStatus.WORKING)
                    {
                        UpdateData();
                        SingleEyeData eyeData = eye == EyeIndex.LEFT ? EyeData_.verbose_data.left : EyeData_.verbose_data.right;
                        valid = eyeData.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_PUPIL_POSITION_IN_SENSOR_AREA_VALIDITY);
                        postion = valid ? postion = new Vector2(eyeData.pupil_position_in_sensor_area.x * 2 - 1,
                                                                eyeData.pupil_position_in_sensor_area.y * -2 + 1) : Vector2.zero;

                    }
                    else
                    {
                        // If not support eye tracking, set default in middle.
                        postion = new Vector2(0.5f, 0.5f);
                        valid = true;
                    }
                    return valid;
                }

                /// <summary>
                /// Launches anipal's Eye Calibration feature (an overlay program).
                /// </summary>
                /// <returns>Indicates the resulting ViveSR.Error status of this method.</returns>
                public static bool LaunchEyeCalibration()
                {
                    int result = LaunchEyeCalibration(IntPtr.Zero);
                    return result == (int)Error.WORK;
                }
            }
        }
    }
}