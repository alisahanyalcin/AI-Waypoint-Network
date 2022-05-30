using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[CustomEditor(typeof(AIWaypointNetwork))]
public class AIWaypointNetworkEditor : Editor
{
	private bool _error = false;
	
	public override void OnInspectorGUI()
	{
		AIWaypointNetwork network = (AIWaypointNetwork) target;

		network.shapes = (GizmoShape) EditorGUILayout.EnumPopup("Connection Point", network.shapes);
		network.DisplayMode = (PathDisplayMode) EditorGUILayout.EnumPopup("Mode", network.DisplayMode);
		
		network.gizmoSize = EditorGUILayout.Slider("Gizmo Size", network.gizmoSize, 0.1f, 10f);

		network.waypointsConnectionPointColor = EditorGUILayout.ColorField("Connection Point Color", network.waypointsConnectionPointColor);

		if (network.shapes == GizmoShape.Texture)
			network.gizmoTexture = (Texture2D) EditorGUILayout.ObjectField("Gizmo Texture", network.gizmoTexture, typeof(Texture2D), false);
		
		switch (network.DisplayMode)
		{
			case PathDisplayMode.Paths:
				network.waypointsPathsColor = EditorGUILayout.ColorField("Paths Mode Color", network.waypointsPathsColor);
				network.UIStartPoint =
					EditorGUILayout.IntSlider("Waypoint Start", network.UIStartPoint, 0, network.Waypoints.Count - 1);
				network.UIEndPoint =
					EditorGUILayout.IntSlider("Waypoint End", network.UIEndPoint, 0, network.Waypoints.Count - 1);
				break;
			case PathDisplayMode.Connections:
				network.waypointsConnectionsColor = EditorGUILayout.ColorField("Connections Mode Color", network.waypointsConnectionsColor);
				break;
			case PathDisplayMode.ConnectionPaths:
				network.waypointsConnectionPathsColor = EditorGUILayout.ColorField("Connection Paths Mode Color", network.waypointsConnectionPathsColor);
				break;
			case PathDisplayMode.None:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
		EditorGUILayout.PropertyField(new SerializedObject(network).FindProperty("Waypoints"), true);

		if (_error)
		{
			EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).GetStyle("HelpBox");
			EditorGUILayout.HelpBox("Start Point and End Point not cannot be equal!", MessageType.Warning);
			if (GUILayout.Button("Fix it!"))
			{
				Debug.Log("Fixed!");
				network.UIStartPoint = Random.Range(0, network.Waypoints.Count - 1);
				network.UIEndPoint = Random.Range(1, network.Waypoints.Count - 2);

				if (network.UIStartPoint == network.UIEndPoint)
					network.UIEndPoint = network.UIEndPoint + 1;
			}
		}
	}
	
	private void OnSceneGUI()
	{
		AIWaypointNetwork network = (AIWaypointNetwork) target;

		for (int i = 0; i < network.Waypoints.Count; i++)
			if (network.Waypoints[i] != null)
				Handles.Label(network.Waypoints[i].position, "Waypoint " + /*(i + 1)*/i);
		
		switch (network.DisplayMode)
		{
			case PathDisplayMode.Connections:
			{
				Vector3[] linePoints = new Vector3[network.Waypoints.Count + 1];

				for (int i = 0; i <= network.Waypoints.Count; i++)
				{
					int waypointIndex = i != network.Waypoints.Count ? i : 0;

					if (network.Waypoints[waypointIndex] != null)
						linePoints[i] = network.Waypoints[waypointIndex].position;
				}

				Handles.color = network.waypointsConnectionsColor;
				Handles.DrawPolyLine(linePoints);
				break;
			}
			case PathDisplayMode.Paths:
			{
				NavMeshPath path = new NavMeshPath();

				if (network.Waypoints[network.UIStartPoint] != null && network.Waypoints[network.UIEndPoint] != null)
				{
					Vector3 from = network.Waypoints[network.UIStartPoint].position;
					Vector3 to = network.Waypoints[network.UIEndPoint].position;

					if (network.UIStartPoint == network.UIEndPoint)
					{
						_error = true;
						return;
					}
					else
						_error = false;

					NavMesh.CalculatePath(from, to, NavMesh.AllAreas, path);

					Handles.color = network.waypointsPathsColor;
					Handles.DrawPolyLine(path.corners);
				}

				break;
			}
			case PathDisplayMode.ConnectionPaths:
			{
				NavMeshPath path = new NavMeshPath();
			
				for (int i = 0; i < network.Waypoints.Count; i++)
				{
					if (network.Waypoints[i] != null)
					{
						Vector3 from = network.Waypoints[i].position;

						for (int j = 0; j < network.Waypoints.Count; j++)
						{
							if (network.Waypoints[j] != null)
							{
								Vector3 to = network.Waypoints[j].position;

								if (j - i == 1 || i == (network.Waypoints.Count - 1) && j == 0)
								{
									NavMesh.CalculatePath(from, to, NavMesh.AllAreas, path);
								
								}
								Handles.color = network.waypointsConnectionPathsColor;
								Handles.DrawPolyLine(path.corners);
							}
						}
					}
				}

				break;
			}
		}
	}
}