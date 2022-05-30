using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using HelperNamespace;

/// <summary>
///     [What does this PlayerControllerInspector do]
/// </summary>
[CustomEditor(typeof(PlayerController)), CanEditMultipleObjects]
public sealed class PlayerControllerInspector : Editor
{
    PlayerController root;

    SerializedProperty secondsPerStatsUpdate;
    SerializedProperty playerTarget;

    bool showPlayerControllerDebugData = false;

    private readonly LabelData playerInputsTitleLabel = new("Player inputs");
    private readonly LabelData playerInputsEnabledStateLabel = new("enabled"  , textColor: Color.green, fontStyle: FontStyle.Bold);
    private readonly LabelData playerInputsDisabledStateLabel = new("disabled", textColor: Color.red  , fontStyle: FontStyle.Bold);

    private void OnEnable()
    {
        root = (PlayerController)target;

        secondsPerStatsUpdate = serializedObject.FindProperty(nameof(root.secondsPerStatsUpdate));
        playerTarget = serializedObject.FindProperty(nameof(root.target));
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        base.OnInspectorGUI();

        EditorTools.Line();

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        showPlayerControllerDebugData = GUILayout.Toggle(showPlayerControllerDebugData, new GUIContent("Show Player controller debug data"));
        if (showPlayerControllerDebugData)
        {
            EditorTools.Line();

            EditorGUILayout.PropertyField(secondsPerStatsUpdate);

            EditorTools.Line();

            EditorGUILayout.PropertyField(playerTarget);
            
            EditorTools.Line();
        
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            if (root.DisableMovement)
            {
                EditorTools.Label(5.0f, 5.0f, playerInputsTitleLabel, playerInputsDisabledStateLabel);
                GUI.enabled = false;
            }
            else
            {
                EditorTools.Label(5.0f, 5.0f, playerInputsTitleLabel, playerInputsEnabledStateLabel);
                GUI.enabled = true;
            }

            EditorTools.Line();

            EditorGUILayout.Vector2Field("Inputs", root.GetXZ);
            EditorGUILayout.Vector3Field("Move forces", root.GetMove);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Player state");
            string playerState = "";
            if (root.GetIfCrouching)
            {
                playerState += "Crouching and ";
            }
            else
            {
                playerState += "Standing and ";
            }
            if (root.GetIfIsLongJumping)
            {
                playerState += "jumping";
            }
            else
            {
                playerState += "not jumping";
            }
            EditorGUILayout.LabelField(playerState);
            EditorGUILayout.EndHorizontal();
                
            GUI.enabled = true;
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}
