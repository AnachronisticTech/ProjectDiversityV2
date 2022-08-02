using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using HelperNamespace;

/// <summary>
///     [What does this POVInspector do]
/// </summary>
[CustomEditor(typeof(POVEvent)), CanEditMultipleObjects]
public sealed class POVEventInspector : Editor
{
    POVEvent root;

    //SerializedProperty scansPerSeconds;
    //SerializedProperty inSightObjects;
    //SerializedProperty focusedObject;
    //SerializedProperty focusAreaGizmoColor;
    //SerializedProperty unfocusAreaGizmoColor;
    //SerializedProperty visionAreaSegments;

    //bool showVisibilityDebug = false;

    private void OnEnable()
    {
        root = (POVEvent)target;

        //scansPerSeconds = serializedObject.FindProperty(nameof(root.scansPerSeconds));
        //inSightObjects = serializedObject.FindProperty(nameof(root.inSightObjects));
        //focusedObject = serializedObject.FindProperty(nameof(root.focusedObject));
        //focusAreaGizmoColor = serializedObject.FindProperty(nameof(root.focusAreaGizmoColor));
        //unfocusAreaGizmoColor = serializedObject.FindProperty(nameof(root.unfocusAreaGizmoColor));
        //visionAreaSegments = serializedObject.FindProperty(nameof(root.visionAreaSegments));
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        /**
        serializedObject.Update();
        
        EditorTools.Label(StringRepo.Physics.VisibilityLabel, 15, EditorStyles.boldLabel, Color.white, topSpace: 10);
        
        EditorTools.Line();

        if (root.forgetFocusedObjectRange <= root.GetViewDistance + 0.5f)
        {
            root.forgetFocusedObjectRange = root.GetViewDistance + 0.5f;
        }

        base.OnInspectorGUI();

        EditorTools.Line();

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        showVisibilityDebug = GUILayout.Toggle(showVisibilityDebug, new GUIContent(StringRepo.POV.VisibilityDataDebugLabel));
        if (showVisibilityDebug)
        {
            EditorTools.Line();

            EditorGUILayout.PropertyField(scansPerSeconds);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.PropertyField(inSightObjects);
            EditorGUILayout.EndVertical();

            EditorGUILayout.PropertyField(focusedObject);

            GUILayout.Space(5.0f);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            root.showVisibilityGizmos = GUILayout.Toggle(root.showVisibilityGizmos, new GUIContent(StringRepo.POV.ShowVisibilityGizmosLabel));
            GUI.enabled = root.showVisibilityGizmos;
            EditorTools.Line();
            EditorGUILayout.PropertyField(focusAreaGizmoColor);
            EditorGUILayout.PropertyField(unfocusAreaGizmoColor);
            EditorGUILayout.PropertyField(visionAreaSegments);
            GUI.enabled = true;
            EditorGUILayout.EndVertical();

            GUILayout.Space(5.0f);

            if (GUILayout.Button(StringRepo.Controllers.ClearStoredNavDataLabel))
            {
                root.ClearStoredNavigationData();
            }
        }
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
        **/
    }
}
