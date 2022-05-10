using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using HelperNamespace;

[InitializeOnLoad()]
public class WaypointEditor
{
    private static Color selectionSphereColor;
    private static Color widthLineColor;
    private static Color previousWaypointLineColor;
    private static Color nextWaypointLineColor;

    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
    public static void OnDrawSceneGizmo(Waypoint waypoint, GizmoType gizmoType)
    {
        if ((gizmoType & GizmoType.Selected) != 0)
        {
            selectionSphereColor = Color.yellow;
            widthLineColor = Color.white;
            previousWaypointLineColor = Color.red;
            nextWaypointLineColor = Color.green;
        }
        else
        {
            selectionSphereColor = Color.yellow * 0.5f;
            widthLineColor = Color.white * 0.5f;
            previousWaypointLineColor = Color.red * 0.4f;
            nextWaypointLineColor = Color.green * 0.4f;
        }
        Gizmos.color = selectionSphereColor;
        Gizmos.DrawSphere(waypoint.transform.position, 0.1f);

        Gizmos.color = widthLineColor;
        Gizmos.DrawLine(waypoint.transform.position + (waypoint.transform.right * waypoint.width / 2.0f),
                        waypoint.transform.position - (waypoint.transform.right * waypoint.width / 2.0f));

        if (waypoint.previousWaypoint != null)
        {
            Vector3 offset = waypoint.transform.right * waypoint.width / 2.0f;
            Vector3 offsetTo = waypoint.previousWaypoint.transform.right * waypoint.previousWaypoint.width / 2.0f;

            Gizmos.color = previousWaypointLineColor;
            EditorTools.DrawDestinationArrow(waypoint.transform.position + offset, waypoint.previousWaypoint.transform.position + offsetTo);
        }
        if (waypoint.nextWaypoint != null)
        {
            Vector3 offset = waypoint.transform.right * -waypoint.width / 2.0f;
            Vector3 offsetTo = waypoint.nextWaypoint.transform.right * -waypoint.nextWaypoint.width / 2.0f;

            Gizmos.color = nextWaypointLineColor;
            EditorTools.DrawDestinationArrow(waypoint.transform.position + offset, waypoint.nextWaypoint.transform.position + offsetTo);
        }
    }
}
