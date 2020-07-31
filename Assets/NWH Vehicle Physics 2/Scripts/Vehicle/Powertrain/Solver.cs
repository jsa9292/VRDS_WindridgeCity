using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NWH.VehiclePhysics2.Powertrain
{
    [Serializable]
    public class Solver
    {
        public enum PhysicsQuality
        {
            Low = 8,
            Medium = 12,
            High = 16,
            VeryHigh = 24,
            Ultra = 32,
            Overkill = 48
        }

        /// <summary>
        ///     Number of iterations that will be run in one FixedUpdate.
        ///     Higher number of iterations will equal higher powertrain physics quality, but will also result in linear
        ///     decrease of performance. Values less than 8 (Low) are not recommended and might result in calculation instability.
        /// </summary>
        [Tooltip(
            "Number of iterations that will be run in one FixedUpdate.\r\nHigher number of iterations will equal higher powertrain physics quality, but will also result in linear\r\ndecrease of performance. Values less than 8 (Low) are not recommended and might result in calculation instability.")]
        public PhysicsQuality physicsQuality = PhysicsQuality.High;

        [SerializeField]
        private List<PowertrainComponent> _components = new List<PowertrainComponent>();

        /// <summary>
        ///     Time between solver iterations. Equals iterations/Time.fixedDeltaTime.
        /// </summary>
        private float _dt;

        /// <summary>
        ///     Number of iterations that will be ran each FixedUpdate.
        /// </summary>
        private float _iterations = 16;

        public List<PowertrainComponent> Components
        {
            get { return _components; }
        }

        public void Initialize(List<PowertrainComponent> powertrainComponents)
        {
            _components = powertrainComponents;

            foreach (PowertrainComponent component in _components)
            {
                component.Initialize();
            }
        }

        public void AddComponent(PowertrainComponent i)
        {
            _components.Add(i);
        }

        public PowertrainComponent GetComponent(int index)
        {
            if (index < 0 || index >= _components.Count)
            {
                Debug.LogError("Component index out of bounds.");
                return null;
            }

            return _components[index];
        }

        public PowertrainComponent GetComponent(string name)
        {
            return _components.FirstOrDefault(c => c.name == name);
        }

        public List<string> GetComponentNames()
        {
            return _components.Select(c => c.name).ToList();
        }

        public void RemoveComponent(PowertrainComponent i)
        {
            _components.Remove(i);
        }

        public void Solve()
        {
            int componentCount = _components.Count;
            if (componentCount == 0)
            {
                return;
            }

            _iterations = (int) physicsQuality;
            _dt = Time.fixedDeltaTime * (1f / _iterations);

            // OnPreSolve
            for (int i = 0; i < componentCount; i++)
            {
                _components[i].OnPreSolve();
            }

            // Integrate
            EngineComponent engine = _components[0] as EngineComponent;
            ClutchComponent clutch = _components[1] as ClutchComponent;
            for (int i = 0; i < _iterations; i++)
            {
                engine.clutchEnagagement = clutch.clutchEngagement;
                engine.Integrate(_dt, i);
            }

            // OnPostSolve
            for (int i = 0; i < componentCount; i++)
            {
                _components[i].OnPostSolve();
            }
        }
    }
}