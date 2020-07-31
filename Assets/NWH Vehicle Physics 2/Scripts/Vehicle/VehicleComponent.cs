using System;
using UnityEngine;

namespace NWH.VehiclePhysics2
{
    /// <summary>
    ///     Base class for all VehicleComponents.
    /// </summary>
    [Serializable]
    public abstract class VehicleComponent
    {
        /// <summary>
        ///     Contains info about component's state.
        /// </summary>
        [Tooltip("    Contains info about component's state.")]
        public StateDefinition state = new StateDefinition();

        protected string fullTypeName;
        protected bool initialized;

        protected VehicleController vc;
        protected bool wasEnabled;

        /// <summary>
        ///     True if component is active.
        ///     Component will be active if it is on, enabled and initialized.
        ///     Use this to check if the component should be updated.
        /// </summary>
        public bool Active
        {
            get { return state.isOn && state.isEnabled && initialized; }
        }

        /// <summary>
        ///     True if the component has been initialized.
        /// </summary>
        public bool Initialized
        {
            get { return initialized; }
            set { initialized = value; }
        }

        /// <summary>
        ///     True if the component is enabled.
        /// </summary>
        public bool IsEnabled
        {
            get { return state.isEnabled; }
            private set { state.isEnabled = value; }
        }

        /// <summary>
        ///     True if the component is on.
        /// </summary>
        public bool IsOn
        {
            get { return state.isOn; }
            set { state.isOn = value; }
        }

        /// <summary>
        ///     Index of LOD up to which the component will be active (inclusive).
        ///     ExampLe: 1 = component is active at LODs 0 and 1.
        ///     Set to -1 to disable LOD checking.
        /// </summary>
        public int LodIndex
        {
            get { return state.lodIndex; }
            set { state.lodIndex = value; }
        }

        /// <summary>
        ///     Reference to the parent VehicleController.
        ///     Null before Awake() is called.
        /// </summary>
        public VehicleController VehicleController
        {
            get { return vc; }
            set { vc = value; }
        }

        /// <summary>
        ///     Called after Awake() initialize the vehicle.
        ///     Similar to MonoBehavior's Start() but called only when the vehicle is Enabled.
        ///     Disabled components will not be initialized until they are Enabled.
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        ///     First function that gets called on all components.
        ///     VehicleController reference will be null before this function is called.
        /// </summary>
        public virtual void Awake(VehicleController vc)
        {
            this.vc = vc;
            initialized = false;
            wasEnabled = false;
            fullTypeName = GetType().FullName;

            if (state == null)
            {
                state = new StateDefinition();
            }

            state.fullName = fullTypeName;
            LoadStateFromDefinitionsFile(fullTypeName, ref state);
        }

        /// <summary>
        ///     Equivalent to MonoBehavior's FixedUpdate().
        /// </summary>
        public abstract void FixedUpdate();

        /// <summary>
        ///     Equivalent to MonoBehaviour's Update() function.
        /// </summary>
        public abstract void Update();

        /// <summary>
        ///     Call to enable the vehicle component.
        ///     Not called outside of play mode.
        /// </summary>
        /// <returns>True if successful.</returns>
        public virtual void Enable()
        {
            if (!initialized && Application.isPlaying)
            {
                Initialize();
            }

            state.isEnabled = true;
            wasEnabled = true;
        }

        /// <summary>
        ///     Call to disable the vehicle component.
        ///     Will get called when component's Enabled/Disabled toggle button gets clicked in editor.
        /// </summary>
        /// <returns>True if successful.</returns>
        public virtual void Disable()
        {
            state.isEnabled = false;
            wasEnabled = false;
        }

        /// <summary>
        ///     Equivalent to MonoBehavior's OnDrawGizmosSelected().
        /// </summary>
        public virtual void OnDrawGizmosSelected(VehicleController vc)
        {
        }

        /// <summary>
        ///     Loads state settings from StateSettings ScriptableObject.
        /// </summary>
        public void LoadStateFromDefinitionsFile(string fullTypeName, ref StateDefinition state)
        {
            if (vc.stateSettings == null)
            {
                return;
            }

            StateDefinition loadedState = vc.stateSettings.GetDefinition(fullTypeName);
            if (loadedState != null)
            {
                state.isOn = loadedState.isOn;
                state.isEnabled = loadedState.isEnabled;
                state.lodIndex = loadedState.lodIndex;
                state.fullName = fullTypeName;
            }
            else
            {
                Debug.LogWarning(
                    $"State definition {fullTypeName} could not be loaded. Click on 'Refresh' button under Settings > State Settings.");
            }
        }

        /// <summary>
        ///     Checks the current state and enables or disables the component if needed.
        ///     Also handles LOD checking.
        /// </summary>
        public virtual void CheckState(int lodIndex)
        {
            if (!initialized)
            {
                Initialize();
            }

            if (!state.isOn)
            {
                if (state.isEnabled)
                {
                    Disable();
                }

                return;
            }

            if (state.lodIndex >= 0) // Use LOD
            {
                if (state.lodIndex >= lodIndex) // Inside LOD
                {
                    if (!state.isEnabled)
                    {
                        Enable();
                    }
                }
                else // Out of LOD
                {
                    if (state.isEnabled)
                    {
                        Disable();
                    }
                }
            }
            else // Do not use LOD
            {
                if (state.isEnabled && !wasEnabled)
                {
                    Enable();
                }
                else if (!state.isEnabled && wasEnabled)
                {
                    Disable();
                }
            }

            wasEnabled = state.isEnabled;
        }

        /// <summary>
        ///     Resets component's values to defaults. Also called when Reset is called from inspector.
        /// </summary>
        public virtual void SetDefaults(VehicleController vc)
        {
        }

        /// <summary>
        ///     If enabled disables the component and vice versa.
        /// </summary>
        public void ToggleState()
        {
            if (IsEnabled)
            {
                Disable();
            }
            else
            {
                Enable();
            }
        }

        /// <summary>
        ///     Ran when VehicleController.Validate is called.
        ///     Checks if the component setup is valid and alerts the developer if there are any issues.
        /// </summary>
        public virtual void Validate(VehicleController vc)
        {
        }
    }
}