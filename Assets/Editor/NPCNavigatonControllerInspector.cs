using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using HelperNamespace;

/// <summary>
///     Custom editor for NPCNavigationControllerV2.
/// </summary>
[System.Obsolete("Old custom editor for NPCNavigationControllerV2.")]
[CustomEditor(typeof(NPCNavigationControllerV2)), CanEditMultipleObjects]
public sealed class NPCNavigatonControllerInspector : Editor
{
    NPCNavigationControllerV2 root;

    #region Variable declaration

    SerializedProperty behaviorMode;

    SerializedProperty viewDistance;
    SerializedProperty forgetFocusedObjectRange;
    SerializedProperty viewAngle;
    SerializedProperty viewHeight;
    SerializedProperty interestLayerMask;
    SerializedProperty sightBlockLayerMask;
    SerializedProperty scansPerSecond;
    SerializedProperty focusedObject;
    
    SerializedProperty showVisibilityGizmos;
    SerializedProperty focusAreaGizmoColor;
    SerializedProperty unfocusAreaGizmoColor;
    SerializedProperty visionAreaSegments;
    
    SerializedProperty currentWaypoint;
    SerializedProperty chanceOfFlippingDirection;
    
    SerializedProperty stopDistance;
    SerializedProperty rotationSpeed;
    SerializedProperty movementSpeed;
    
    SerializedProperty jumpCheck;
    SerializedProperty jumpLayerMask;
    SerializedProperty maxUnitsJump;

    bool showVisibilityDebug = false;
    bool showWaypointDebug = false;
    bool showNavigationDebug = false;

    #endregion

    private void OnEnable()
    {
        root = (NPCNavigationControllerV2)target;

        #region Variable initialization

        behaviorMode = serializedObject.FindProperty(nameof(root.behaviorMode));

        viewDistance = serializedObject.FindProperty(nameof(root.viewDistance));
        forgetFocusedObjectRange = serializedObject.FindProperty(nameof(root.forgetFocusedObjectRange));
        viewAngle = serializedObject.FindProperty(nameof(root.viewAngle));
        viewHeight = serializedObject.FindProperty(nameof(root.viewHeight));
        interestLayerMask = serializedObject.FindProperty(nameof(root.interestLayerMask));
        sightBlockLayerMask = serializedObject.FindProperty(nameof(root.sightBlockLayerMask));
        scansPerSecond = serializedObject.FindProperty(nameof(root.scansPerSecond));
        focusedObject = serializedObject.FindProperty(nameof(root.focusedObject));

        showVisibilityGizmos = serializedObject.FindProperty(nameof(root.showVisibilityGizmos));
        focusAreaGizmoColor = serializedObject.FindProperty(nameof(root.focusAreaGizmoColor));
        unfocusAreaGizmoColor = serializedObject.FindProperty(nameof(root.unfocusAreaGizmoColor));
        visionAreaSegments = serializedObject.FindProperty(nameof(root.visionAreaSegments));

        currentWaypoint = serializedObject.FindProperty(nameof(root.currentWaypoint));
        chanceOfFlippingDirection = serializedObject.FindProperty(nameof(root.chanceOfFlippingDirection));

        stopDistance = serializedObject.FindProperty(nameof(root.stopDistance));
        rotationSpeed = serializedObject.FindProperty(nameof(root.rotationSpeed));
        movementSpeed = serializedObject.FindProperty(nameof(root.movementSpeed));

        jumpCheck = serializedObject.FindProperty(nameof(root.jumpCheck));
        jumpLayerMask = serializedObject.FindProperty(nameof(root.jumpLayerMask));
        maxUnitsJump = serializedObject.FindProperty(nameof(root.maxUnitsJump));
        
        #endregion
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(behaviorMode);

        EditorTools.Line();

        #region Visibility data GUI

        EditorTools.Label(StringRepo.Physics.VisibilityLabel, 15, EditorStyles.boldLabel, Color.white, topSpace: 10);

        EditorGUILayout.PropertyField(viewDistance);
        EditorGUILayout.PropertyField(forgetFocusedObjectRange);
        EditorGUILayout.PropertyField(viewAngle);
        EditorGUILayout.PropertyField(viewHeight);
        EditorGUILayout.PropertyField(interestLayerMask);
        EditorGUILayout.PropertyField(sightBlockLayerMask);
        EditorGUILayout.PropertyField(scansPerSecond);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        showVisibilityDebug = GUILayout.Toggle(showVisibilityDebug, new GUIContent(StringRepo.POV.VisibilityDataDebugLabel));
        if (showVisibilityDebug)
        {
            GUILayout.Space(5.0f);

            EditorGUILayout.PropertyField(showVisibilityGizmos);

            GUI.enabled = root.showVisibilityGizmos;

            EditorGUILayout.PropertyField(focusAreaGizmoColor);
            EditorGUILayout.PropertyField(unfocusAreaGizmoColor);
            EditorGUILayout.PropertyField(visionAreaSegments);

            GUI.enabled = true;
            
            EditorGUILayout.PropertyField(focusedObject);
            
            GUILayout.Space(5.0f);

            if (GUILayout.Button(StringRepo.Controllers.ClearStoredNavDataLabel))
            {
                root.ClearStoredNavigationData();
            }

            GUILayout.Space(5.0f);
        }
        EditorGUILayout.EndVertical();
        
        #endregion

        EditorTools.Line(topSpace: 10.0f);

        if (root.behaviorMode == NPCNavigationControllerV2.BehaviourMode.partol)
        {
            #region Waypoint data GUI

        EditorTools.Label(StringRepo.Waypoint.WaypointLabel, 15, EditorStyles.boldLabel, Color.white, topSpace: 10);

        EditorGUILayout.PropertyField(currentWaypoint);
        EditorGUILayout.PropertyField(chanceOfFlippingDirection);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        showWaypointDebug = GUILayout.Toggle(showWaypointDebug, new GUIContent(StringRepo.Waypoint.WaypointDebugLabel));
        if (showWaypointDebug)
        {
            GUILayout.Space(5.0f);

            EditorTools.Label(StringRepo.Waypoint.WaypointDestinationString + root.GetDestinationString);

            GUILayout.Space(5.0f);
        }
        EditorGUILayout.EndVertical();

        #endregion
        }
        else if (root.behaviorMode == NPCNavigationControllerV2.BehaviourMode.focused)
        {

        }

        EditorTools.Line(topSpace: 10.0f);

        #region Navigation data GUI

        EditorTools.Label(StringRepo.Controllers.NavigationLabel, 15, EditorStyles.boldLabel, Color.white, topSpace: 10);

        EditorGUILayout.PropertyField(stopDistance);
        EditorGUILayout.PropertyField(rotationSpeed);
        EditorGUILayout.PropertyField(movementSpeed);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        showNavigationDebug = GUILayout.Toggle(showNavigationDebug, new GUIContent(StringRepo.Controllers.NavigationDebugLabel));
        if (showNavigationDebug)
        {
            GUILayout.Space(5.0f);

            EditorTools.Label(StringRepo.Controllers.CharacterVelocityString + root.GetVelocityString);

            GUILayout.Space(5.0f);
        }
        EditorGUILayout.EndVertical();

        #endregion

        EditorTools.Line(topSpace: 10.0f);

        #region Jump data GUI

        EditorTools.Label(StringRepo.Controllers.JumpingLabel, 15, EditorStyles.boldLabel, Color.white, topSpace: 10);

        EditorGUILayout.PropertyField(jumpCheck);
        EditorGUILayout.PropertyField(jumpLayerMask);
        EditorGUILayout.PropertyField(maxUnitsJump);

        #endregion

        serializedObject.ApplyModifiedProperties();
    }
}
