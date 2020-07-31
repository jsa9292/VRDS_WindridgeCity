using System;
using System.Linq;

namespace NWH.VehiclePhysics2
{
    /// <summary>
    ///     Class storing VehicleComponent state.
    /// </summary>
    [Serializable]
    public class StateDefinition
    {
        public string fullName;
        public bool isEnabled = true;
        public bool isOn = true;
        public int lodIndex = -1;
        public string name;

        public StateDefinition()
        {
        }

        public StateDefinition(string fullName, bool isOn, bool isEnabled, int lod)
        {
            this.fullName = fullName;
            name = fullName.Split('.').Last();
            this.isOn = isOn;
            this.isEnabled = isEnabled;
            lodIndex = lod;
        }
    }
}