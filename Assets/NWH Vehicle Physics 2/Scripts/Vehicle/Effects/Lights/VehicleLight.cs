using System;
using System.Collections.Generic;
using UnityEngine;

namespace NWH.VehiclePhysics2.Effects
{
    /// <summary>
    ///     Single vehicle light.
    /// </summary>
    [Serializable]
    public class VehicleLight
    {
        /// <summary>
        ///     All the light sources representing the vehicle light.
        ///     E.g. low beam can be represented by a directional light to represent light beam and
        ///     and emissive mesh to represent light optics.
        /// </summary>
        [Tooltip(
            "    All the light sources representing the vehicle light.\r\n    E.g. low beam can be represented by a directional light to represent light beam and\r\n    and emissive mesh to represent light optics.")]
        public List<LightSource> lightSources = new List<LightSource>();

        protected bool isOn;

        /// <summary>
        ///     State of the light.
        /// </summary>
        public bool On
        {
            get { return isOn; }
            set { isOn = value; }
        }

        public void SetState(bool state)
        {
            if (state && !isOn)
            {
                TurnOn();
            }
            else if (!state && isOn)
            {
                TurnOff();
            }
        }

        public void Toggle()
        {
            if (isOn)
            {
                TurnOff();
            }
            else
            {
                TurnOn();
            }
        }

        /// <summary>
        ///     Turns off the light source or disables emission on the mesh. Mesh is required to have standard shader.
        /// </summary>
        public void TurnOff()
        {
            if (!isOn)
            {
                return;
            }

            foreach (LightSource source in lightSources)
            {
                source.TurnOff();
            }

            isOn = false;
        }

        /// <summary>
        ///     Turns on the light source or enables emission on the mesh. Mesh is required to have standard shader.
        /// </summary>
        public void TurnOn()
        {
            if (isOn)
            {
                return;
            }

            foreach (LightSource source in lightSources)
            {
                source.TurnOn();
            }

            isOn = true;
        }
    }
}