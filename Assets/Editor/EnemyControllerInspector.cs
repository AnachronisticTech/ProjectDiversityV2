using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using HelperNamespace;

/// <summary>
///     [What does this EnemyControllerInspector do]
/// </summary>
[CustomEditor(typeof(EnemyController)), CanEditMultipleObjects]
public sealed class EnemyControllerInspector : Editor
{
    EnemyController root;

    SerializedProperty enemyTarget;
    SerializedProperty playerController;
    SerializedProperty enemyController;
    SerializedProperty currentAttackTimer;
    SerializedProperty attackTypeChance;

    bool showEnemyControllerDebugData = false;

    private void OnEnable()
    {
        root = (EnemyController)target;

        enemyTarget = serializedObject.FindProperty(nameof(root.target));
        playerController = serializedObject.FindProperty(nameof(root.playerController));
        enemyController = serializedObject.FindProperty(nameof(root.enemyController));

        currentAttackTimer = serializedObject.FindProperty(nameof(root.currentAttackTimer));
        attackTypeChance = serializedObject.FindProperty(nameof(root.attackTypeChance));
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        base.OnInspectorGUI();

        EditorTools.Line();

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        showEnemyControllerDebugData = GUILayout.Toggle(showEnemyControllerDebugData, new GUIContent("Show " + root.name + " controller debug data"));
        if (showEnemyControllerDebugData)
        {
            EditorTools.Line();

            EditorGUILayout.PropertyField(enemyTarget);
            if (root.enemyController == null && root.playerController != null)
            {
                EditorGUILayout.PropertyField(playerController);
            }
            else if (root.enemyController != null && root.playerController == null)
            {
                EditorGUILayout.PropertyField(enemyController);
            }
            else
            {
                EditorTools.Label("Valid controller NOT found on Target.    ", textColor: Color.red, labelAlignment: TextAnchor.MiddleRight);
            }

            EditorTools.Line();

            EditorGUILayout.PropertyField(currentAttackTimer);
            EditorGUILayout.PropertyField(attackTypeChance);
        }
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}
