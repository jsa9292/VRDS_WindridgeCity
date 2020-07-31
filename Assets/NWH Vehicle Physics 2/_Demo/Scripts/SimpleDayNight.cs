using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SimpleDayNight : MonoBehaviour
{
    [Range(0, 1)]
    public float timeOfDay = 0.5f;
    public Light lightSource;
    public float maxLightIntensity = 1.1f;
    public Material skysphereMat;
    public float maxSkysphereExposure = 0.5f;
    public AnimationCurve lightIntensityCurve;
    public AnimationCurve ambientIntensityCurve;
    public ReflectionProbe reflectionProbe;
    public float sunYRotation = 0f;

    private Vector3 _lightEulerAngles;
    private float _prevTimeOfDay;

    private void Start()
    {
        _lightEulerAngles = lightSource.transform.eulerAngles;
    }

    void Update()
    {
        if (_prevTimeOfDay != timeOfDay)
        {
            _lightEulerAngles.x = timeOfDay * 180f - 90f;
            _lightEulerAngles.y = sunYRotation;
            lightSource.transform.eulerAngles = _lightEulerAngles;
            float intensity = lightIntensityCurve.Evaluate(timeOfDay);
            lightSource.intensity = intensity * maxLightIntensity;

            float ambIntensity = ambientIntensityCurve.Evaluate(timeOfDay);
            skysphereMat.SetFloat("_Exposure", ambIntensity * maxSkysphereExposure);
            RenderSettings.ambientIntensity = ambientIntensityCurve.Evaluate(ambIntensity);

            reflectionProbe.RenderProbe();
        }
        
        _prevTimeOfDay = timeOfDay;
    }
}
