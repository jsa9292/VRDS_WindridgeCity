using UnityEngine;
using System.Collections;
using UnityEditor;

namespace PedestrianSystem{

	//This is Custom Script. Don't make any changes to it.

	[CustomEditor(typeof(PedestrianSystemManager))]
	public class ObjectBuilderEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			PedestrianSystemManager myScript = (PedestrianSystemManager)target;

			if(GUILayout.Button("Get Waypoints"))
			{
				myScript.GetWaypoints();
			}
				
			if(GUILayout.Button("Add Waypoint"))
			{
				myScript.AddWaypoint();
			}

			if(GUILayout.Button("Remove Waypoint"))
			{
				myScript.RemoveLastWaypoint();
			}
		}
	}
}