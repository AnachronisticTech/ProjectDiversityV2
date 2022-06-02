using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using HelperNamespace;

/// <summary>
///     [What does this StatsListInspector do]
/// </summary>
[CustomEditor(typeof(StatsScriptableObject)), CanEditMultipleObjects]
public sealed class StatsScriptableObjectInspector : Editor
{
    StatsScriptableObject root;

    int listCount;

    GUIStyle smallButtonStyle = new();

    private void OnEnable()
    {
        root = (StatsScriptableObject)target;

        listCount = root.statsList.Count;

        InitializeStyles();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Color defaultColor = GUI.color;
        
        EditorTools.Label("Stats List", 25, new GUIStyle(EditorStyles.boldLabel), labelAlignment: TextAnchor.MiddleCenter);
        
        EditorTools.Line();

        EditorGUILayout.BeginHorizontal();
        listCount = EditorGUILayout.IntField(new GUIContent("Stats count"), root.statsList.Count);
        GUI.color = Color.green;
        if (GUILayout.Button("+", smallButtonStyle))
        {
            root.statsList.Add(new Stat(StatTypes.Unknown, 0));
        }
        GUI.color = Color.red;
        if (GUILayout.Button("-", smallButtonStyle))
        {
            root.statsList.RemoveAt(root.statsList.Count - 1);
        }
        GUI.color = defaultColor;
        EditorGUILayout.EndHorizontal();

        EditorTools.Line();

        for (int i = 0; i < root.statsList.Count; i++)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUILayout.BeginVertical();
                    if (i == 0)
                        GUI.enabled = false;
                    if (GUILayout.Button("Up", GUILayout.Width(64), GUILayout.Height(40)))
                    {
                        root.statsList = Values.SwapListElements(root.statsList, i, i - 1);
                    }
                    GUI.enabled = true;
                    if (i == root.statsList.Count - 1)
                        GUI.enabled = false;
                    if (GUILayout.Button("Down", GUILayout.Width(64), GUILayout.Height(40)))
                    {
                        root.statsList = Values.SwapListElements(root.statsList, i, i + 1);
                    }
                    GUI.enabled = true;
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                    root.statsList[i]._statName = (StatTypes)EditorGUILayout.EnumPopup(new GUIContent("Name"), root.statsList[i]._statName);

                    if (root.statsList[i]._hasMinValue && root.statsList[i]._hasMaxValue)
                    {
                        root.statsList[i]._value = EditorGUILayout.Slider(new GUIContent("Value"), root.statsList[i]._value, root.statsList[i]._minValue, root.statsList[i]._maxValue);
                    }
                    else
                    {
                        root.statsList[i]._value = EditorGUILayout.FloatField(new GUIContent("Value"), root.statsList[i]._value);
                    }

                    EditorGUILayout.BeginHorizontal();
                        root.statsList[i]._hasMinValue = EditorGUILayout.Toggle(new GUIContent("Has minimum value"), root.statsList[i]._hasMinValue);
                        if (root.statsList[i]._hasMinValue)
                        {
                            root.statsList[i]._minValue = EditorGUILayout.FloatField(root.statsList[i]._minValue);
                        }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                        root.statsList[i]._hasMaxValue = EditorGUILayout.Toggle(new GUIContent("Has maximum value"), root.statsList[i]._hasMaxValue);
                        if (root.statsList[i]._hasMaxValue)
                        {
                            root.statsList[i]._maxValue = EditorGUILayout.FloatField(root.statsList[i]._maxValue);
                        }
                    EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginHorizontal();
                    GUI.color = Color.red;
                    if (GUILayout.Button("X", GUILayout.Width(32), GUILayout.Height(80)))
                    {
                        root.statsList.Remove(root.statsList[i]);
                    }
                    GUI.color = defaultColor;
                EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void InitializeStyles()
    {
        smallButtonStyle = new GUIStyle(EditorStyles.miniButton)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            fixedWidth = 32,
        };
    }
}
