using System;
using System.Collections.Generic;
using NWH.VehiclePhysics2.Demo;
using NWH.VehiclePhysics2.Utility;
using UnityEngine;

namespace NWH.VehiclePhysics2.Powertrain
{
    [Serializable]
    public class PowertrainComponent
    {
        /// <summary>
        ///     Angular velocity of the component.
        /// </summary>
        [Tooltip("    Angular velocity of the component.")]
        public float angularVelocity;

        /// <summary>
        ///     Angular inertia of the component. Higher inertia value will result in a powertrain that is slower to spin up, but
        ///     also slower to spin down. Too high values will result in (apparent) sluggish response while too low values will
        ///     result in vehicle being easy to stall.
        /// </summary>
        [Range(0.01f, 1f)]
        [Tooltip(
            "Angular inertia of the component. Higher inertia value will result in a powertrain that is slower to spin up, but\r\nalso slower to spin down. Too high values will result in (apparent) sluggish response while too low values will\r\nresult in vehicle being easy to stall.")]
        public float inertia = 0.02f;

        /// <summary>
        ///     Input component. Set automatically.
        /// </summary>
        [Tooltip("    Input component. Set automatically.")]
        public PowertrainComponent input;

        /// <summary>
        ///     Name of the component. Only unique names should be used on the same vehicle.
        /// </summary>
        [Tooltip("    Name of the component. Only unique names should be used on the same vehicle.")]
        public string name;

        /// <summary>
        ///     Output component.
        /// </summary>
        [Tooltip("    Output component.")]
        public PowertrainComponent outputA;

        protected bool _inputIsNull;

        protected float _lowerAngularVelocityLimit = -Mathf.Infinity;
        protected bool _outputAIsNull;
        protected float _upperAngularVelocityLimit = Mathf.Infinity;

        [SerializeField]
        protected OutputSelector outputASelector = new OutputSelector();

        /// <summary>
        ///     Damage in range of 0 to 1 that the component has received.
        /// </summary>
        private float _componentDamage;

        public PowertrainComponent()
        {
        }

        public PowertrainComponent(float inertia, string name)
        {
            this.name = name;
            this.inertia = inertia;
        }

        /// <summary>
        ///     Returns current component damage.
        /// </summary>
        public float ComponentDamage
        {
            get { return _componentDamage; }
            set { _componentDamage = value > 1 ? 1 : value < 0 ? 0 : value; }
        }

        /// <summary>
        ///     Minimum angular velocity a component can physically achieve.
        /// </summary>
        public float LowerAngularVelocityLimit
        {
            get { return _lowerAngularVelocityLimit; }
            set { _lowerAngularVelocityLimit = value; }
        }

        /// <summary>
        ///     RPM of component.
        /// </summary>
        [ShowInTelemetry]
        public float RPM
        {
            get { return UnitConverter.AngularVelocityToRPM(angularVelocity); }
        }

        /// <summary>
        ///     Maximum angular velocity a component can physically achieve.
        /// </summary>
        public float UpperAngularVelocityLimit
        {
            get { return _upperAngularVelocityLimit; }
            set { _upperAngularVelocityLimit = value; }
        }

        /// <summary>
        ///     Initializes PowertrainComponent.
        /// </summary>
        public virtual void Initialize()
        {
            if (inertia < 0.01f)
            {
                inertia = 0.01f;
            }
        }

        public virtual void OnEnable()
        {
            
        }

        public virtual void OnDisable()
        {
            
        }

        /// <summary>
        ///     Finds which powertrain component has its output set to this component.
        /// </summary>
        public virtual void FindInput(Solver solver)
        {
            List<PowertrainComponent> outputs = new List<PowertrainComponent>();
            foreach (PowertrainComponent component in solver.Components)
            {
                component.GetAllOutputs(ref outputs);
                foreach (PowertrainComponent output in outputs)
                {
                    if (output != null && output == this)
                    {
                        input = component;
                        _inputIsNull = false;
                        return;
                    }
                }
            }

            input = null;
            _inputIsNull = true;
        }

        /// <summary>
        ///     Retrieves and sets output powertrain components.
        /// </summary>
        /// <param name="solver"></param>
        public virtual void FindOutputs(Solver solver)
        {
            if (string.IsNullOrEmpty(outputASelector.name))
            {
                return;
            }

            PowertrainComponent output = solver.GetComponent(outputASelector.name);
            if (output == null)
            {
                Debug.LogError($"Unknown component '{outputASelector.name}'");
                return;
            }

            outputA = output;
        }

        /// <summary>
        ///     Retruns a list of PowertrainComponents that this component outputs to.
        /// </summary>
        public virtual void GetAllOutputs(ref List<PowertrainComponent> outputs)
        {
            outputs.Clear();
            outputs.Add(outputA);
        }

        public virtual void Integrate(float dt, int iterationCounter)
        {
        }

        /// <summary>
        ///     Gets called after solver has finished.
        /// </summary>
        public virtual void OnPostSolve()
        {
            angularVelocity = angularVelocity < _lowerAngularVelocityLimit
                ? _lowerAngularVelocityLimit
                : angularVelocity;
            angularVelocity = angularVelocity > _upperAngularVelocityLimit
                ? _upperAngularVelocityLimit
                : angularVelocity;
        }

        /// <summary>
        ///     Gets called before solver.
        /// </summary>
        public virtual void OnPreSolve()
        {
            _inputIsNull = input == null;
            _outputAIsNull = outputA == null;
        }

        public virtual float QueryAngularVelocity(float inputAngularVelocity, float dt)
        {
            angularVelocity = inputAngularVelocity;
            if (_outputAIsNull)
            {
                return 0;
            }

            float Wa = outputA.QueryAngularVelocity(inputAngularVelocity, dt);
            return Wa;
        }

        public virtual float QueryInertia()
        {
            if (_outputAIsNull)
            {
                return inertia;
            }

            float Ii = inertia;
            float Ia = outputA.QueryInertia();
            float I = Ii + Ia;
            return I;
        }

        public virtual float SendTorque(float torque, float inertiaSum, float dt)
        {
            if (_outputAIsNull)
            {
                return torque;
            }

            float T = outputA.SendTorque(torque, inertiaSum + inertia, dt);
            return T;
        }

        public void SetOutput(PowertrainComponent outputComponent)
        {
            if (string.IsNullOrEmpty(outputComponent.name))
            {
                Debug.LogWarning("Trying to set powertrain component output to a nameless component. " +
                                 "Output will be set to [none]");
            }

            SetOutput(outputComponent.name);
        }

        public void SetOutput(string outputName)
        {
            if (string.IsNullOrEmpty(outputName))
            {
                outputASelector.name = "[none]";
            }
            else
            {
                outputASelector.name = outputName;
            }
        }

        public virtual void Validate(VehicleController vc)
        {
            if (inertia < 0.01f)
            {
                inertia = 0.01f;
                Debug.LogWarning($"{name}: Inertia must be larger than 0.01f. Setting to 0.01f.");
            }
        }
    }
}