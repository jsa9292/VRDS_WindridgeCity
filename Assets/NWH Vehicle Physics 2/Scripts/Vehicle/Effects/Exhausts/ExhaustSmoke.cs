using System;
using System.Collections.Generic;
using UnityEngine;

namespace NWH.VehiclePhysics2.Effects
{
    /// <summary>
    ///     Controls particle emitters that represent exhaust smoke based on engine state.
    /// </summary>
    [Serializable]
    public class ExhaustSmoke : Effect
    {
        /// <summary>
        /// Maximum distance an exhaust particle can live from the point of emission.
        /// </summary>
        [Range(0,1)] public float lifetimeDistance = 0.4f;
        
        /// <summary>
        /// How much soot is emitted when throttle is pressed.
        /// </summary>
        [Range(0,1)] public float sootIntensity = 0.4f;
        
        /// <summary>
        /// Particle start speed is multiplied by this value based on engine RPM. 
        /// </summary>
        [Range(1, 5)] public float maxSpeedMultiplier = 1.4f;
        
        /// <summary>
        /// Particle start size is multiplied by this value based on engine RPM. 
        /// </summary>
        [Range(1, 5)] public float maxSizeMultiplier = 1.2f;
        
        /// <summary>
        /// Normal particle start color. Used when there is no throttle - engine is under no load.
        /// </summary>
        [UnityEngine.Tooltip("Normal particle start color. Used when there is no throttle - engine is under no load.")]
        public Color normalColor = new Color(0.6f, 0.6f, 0.6f, 0.3f);
        
        /// <summary>
        /// Soot particle start color. Used under heavy throttle - engine is under load.
        /// </summary>
        [UnityEngine.Tooltip("Soot particle start color. Used under heavy throttle - engine is under load.")]
        public Color sootColor = new Color(0.1f, 0.1f, 0.8f);
        
        /// <summary>
        /// List of exhaust particle systems.
        /// </summary>
        [UnityEngine.Tooltip("List of exhaust particle systems.")]
        public List<ParticleSystem> particleSystems = new List<ParticleSystem>();

        private float _initLifetime;
        private float _initStartSpeedMin;
        private float _initStartSpeedMax;
        private float _initStartSizeMin;
        private float _initStartSizeMax;
        private float _sootAmount;
        private ParticleSystem.EmissionModule _emissionModule;
        private ParticleSystem.MainModule _mainModule;
        private ParticleSystem.MinMaxCurve _minMaxCurve;
        private float _vehicleSpeed;
        private float _absVehicleSpeed;

        public override void Initialize()
        {
            initialized = true;

            foreach (ParticleSystem ps in particleSystems)
            {
                if (ps == null)
                {
                    Debug.LogError($"One or more of the exhaust ParticleSystems on the vehicle {vc.name} is null.");
                }
            }

            if (particleSystems == null || particleSystems.Count == 0)
            {
                state.isEnabled = false;
            }
            else
            {
                _emissionModule = particleSystems[0].emission;
                _mainModule = particleSystems[0].main;

                _initLifetime = _mainModule.startLifetime.constant;
                _initStartSpeedMin = _mainModule.startSpeed.constantMin;
                _initStartSpeedMax = _mainModule.startSpeed.constantMax;
                _initStartSizeMin = _mainModule.startSize.constantMin;
                _initStartSizeMax = _mainModule.startSize.constantMax;
            }

            maxSizeMultiplier = Mathf.Clamp(maxSizeMultiplier, 1f, Mathf.Infinity);
            maxSpeedMultiplier = Mathf.Clamp(maxSpeedMultiplier, 1f, Mathf.Infinity);
        }

        public override void FixedUpdate()
        {
        }

        public override void Update()
        {
            if (!Active)
            {
                return;
            }

            if (vc.powertrain.Active && vc.powertrain.engine.IsRunning)
            {
                _vehicleSpeed = vc.Speed;
                _absVehicleSpeed = _vehicleSpeed < 0 ? -_vehicleSpeed : _vehicleSpeed;
                
                foreach (ParticleSystem ps in particleSystems)
                {
                    if (!ps.isPlaying)
                    {
                        ps.Play();
                    }

                    _emissionModule = ps.emission;
                    _mainModule = ps.main;

                    float engineLoad = vc.powertrain.engine.GetLoad();
                    float rpmPercent = vc.powertrain.engine.RPMPercent;

                    if (!_emissionModule.enabled)
                    {
                        _emissionModule.enabled = true;
                    }

                    // Always emit for particle to only reach lifetimeDistance
                    float speedLifetime = _vehicleSpeed < 0.2f && _vehicleSpeed > -0.2f ? 0 : lifetimeDistance / _vehicleSpeed;
                    float lifetime = Mathf.Lerp(_initLifetime, speedLifetime, _absVehicleSpeed * 0.35f);
                    _mainModule.startLifetime = lifetime;

                    // Color
                    _sootAmount = vc.powertrain.engine.throttlePosition * sootIntensity;
                    _mainModule.startColor = Color.Lerp(
                        _mainModule.startColor.color, 
                        Color.Lerp(normalColor, sootColor, _sootAmount),
                        Time.deltaTime * 7f);
                    
                    // Speed
                    float speedMultiplier = maxSpeedMultiplier - 1f;
                    _minMaxCurve = _mainModule.startSpeed;
                    _minMaxCurve.constantMin = _initStartSpeedMin + rpmPercent * speedMultiplier;
                    _minMaxCurve.constantMax = _initStartSpeedMax + rpmPercent * speedMultiplier;
                    _mainModule.startSpeed = _minMaxCurve;
                    
                    // Size
                    float sizeMultiplier = maxSizeMultiplier - 1f;
                    _minMaxCurve = _mainModule.startSize;
                    _minMaxCurve.constantMin = _initStartSizeMin + rpmPercent * sizeMultiplier;
                    _minMaxCurve.constantMax = _initStartSizeMax + rpmPercent * sizeMultiplier;
                    _mainModule.startSize = _minMaxCurve;

                    if (vc.damageHandler.IsEnabled)
                    {
                        _sootAmount += vc.damageHandler.Damage;
                    }
                }
            }
            else
            {
                foreach (ParticleSystem ps in particleSystems)
                {
                    if (ps.isPlaying) ps.Stop();
                    ParticleSystem.EmissionModule emission = ps.emission;
                    emission.enabled = false;
                }
            }
        }

        public override void Enable()
        {
            base.Enable();

            foreach (ParticleSystem ps in particleSystems)
            {
                ParticleSystem.EmissionModule emission = ps.emission;
                ps.Play();
            }
        }

        public override void Disable()
        {
            base.Disable();
            
            foreach (ParticleSystem ps in particleSystems)
            {
                ParticleSystem.EmissionModule emission = ps.emission;
                ps.Stop();
            }
        }
    }
}