/*
 * Script developed by Andreas Monoyios
 * GitHub: https://github.com/AMonoyios?tab=repositories
 * 
 * Reference: Sebastian Lague
 * Source: https://www.youtube.com/watch?v=73Dc5JTCmKI&ab_channel=SebastianLague
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
///     [What does this VisionInspector do]
/// </summary>
[CustomEditor(typeof(FieldOfView)), CanEditMultipleObjects]
public sealed class FieldOFViewInspector : Editor
{
    private FieldOfView root;

    private void OnEnable()
    {
        root = (FieldOfView)target;
    }

    private void OnSceneGUI()
    {
        Handles.color = Color.white;
        Handles.DrawWireArc(root.transform.position, Vector3.up, Vector3.forward, 360, root.GetViewRadius);

        Vector3 viewAngleA = root.DirFromAngle(-root.GetViewAngle / 2.0f);
        Vector3 viewAngleB = root.DirFromAngle(root.GetViewAngle / 2.0f);
        Handles.DrawLine(root.transform.position, root.transform.position + viewAngleA * root.GetViewRadius);
        Handles.DrawLine(root.transform.position, root.transform.position + viewAngleB * root.GetViewRadius);

        Handles.color = Color.red;
        foreach (Transform interactableInRange in root.GetTransformsOfInteractablesInViewRange())
        {
            Handles.DrawLine(root.transform.position, interactableInRange.position);
        }
    }
}
