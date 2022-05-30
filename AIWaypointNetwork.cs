using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum PathDisplayMode { None, Paths, Connections, ConnectionPaths}
public enum GizmoShape { None, Cube, Sphere, WireCube, WireSphere, Texture }

[ExecuteInEditMode]
public class AIWaypointNetwork : MonoBehaviour
{
    [HideInInspector] public PathDisplayMode DisplayMode = PathDisplayMode.Connections;
    [HideInInspector] public GizmoShape shapes;
    [HideInInspector] public int UIStartPoint = 0;
    [HideInInspector] public int UIEndPoint = 0;
    [HideInInspector] public Texture2D gizmoTexture;
    [HideInInspector] public float gizmoSize;
    
    [HideInInspector] public Color waypointsConnectionPointColor = new Color();
    [HideInInspector] public Color waypointsPathsColor = new Color();
    [HideInInspector] public Color waypointsConnectionsColor = new Color();
    [HideInInspector] public Color waypointsConnectionPathsColor = new Color();
    
    [HideInInspector] public List<Transform> Waypoints = new List<Transform>();

    private void OnDrawGizmos()
    {
        foreach (var t in Waypoints.Where(t => t != null))
        {
            Gizmos.color = waypointsConnectionPointColor;
            switch (shapes)
            {
                case GizmoShape.Cube:
                    Gizmos.DrawCube(t.position, new Vector3(gizmoSize, gizmoSize, gizmoSize));
                    break;
                case GizmoShape.Sphere:
                    Gizmos.DrawSphere(t.position, gizmoSize);
                    break;
                case GizmoShape.Texture:
                    if (gizmoTexture != null)
                        Gizmos.DrawGUITexture(
                            new Rect(t.position.x, t.position.y, (float) gizmoSize,
                                (float) gizmoSize), gizmoTexture);
                    break;
                case GizmoShape.WireCube:
                    Gizmos.DrawWireCube(t.position, new Vector3(gizmoSize, gizmoSize, gizmoSize));
                    break;
                case GizmoShape.WireSphere:
                    Gizmos.DrawWireSphere(t.position, gizmoSize);
                    break;
                case GizmoShape.None:
                    break;
            }
        }
    }
}