using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace NWH.VehiclePhysics2
{
    /// <summary>
    ///     ScriptableObject that contains a list of all the initial states for all the available VehicleComponents.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "NWH Vehicle Physics", menuName = "NWH Vehicle Physics/State Settings", order = 1)]
    public class StateSettings : ScriptableObject
    {
        public List<StateDefinition> definitions = new List<StateDefinition>();
        public List<LOD> LODs = new List<LOD>();

        public StateDefinition GetDefinition(string fullComponentTypeName)
        {
            return definitions.Find(d => d.fullName == fullComponentTypeName);
        }

        public void Reload()
        {
            List<string> fullNames = Assembly.GetAssembly(typeof(VehicleComponent)).GetTypes()
                .Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(VehicleComponent))).Select(t => t.FullName).ToList();

            foreach (string fullName in fullNames)
            {
                if (GetDefinition(fullName) == null)
                {
                    definitions.Add(new StateDefinition(fullName, true, true, -1));
                }
            }

            definitions.RemoveAll(d => fullNames.All(n => n != d.fullName));

            definitions = definitions.OrderBy(d => d.fullName).ToList();
        }
    }
}