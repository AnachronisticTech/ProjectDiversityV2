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
    SerializedProperty currentAttackTimer;
    SerializedProperty attackTypeChance;

    bool showEnemyControllerDebugData = false;

    private void OnEnable()
    {
        root = (EnemyController)target;

        enemyTarget = serializedObject.FindProperty(nameof(root.target));
        currentAttackTimer = serializedObject.FindProperty(nameof(currentAttackTimer));
        attackTypeChance = serializedObject.FindProperty(nameof(attackTypeChance));
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

            EditorTools.Line();

            EditorGUILayout.PropertyField(currentAttackTimer);
            EditorGUILayout.PropertyField(attackTypeChance);
        }
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}
