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

    SerializedProperty statsList;

    GUIStyle smallButtonStyle = new();
    GUIStyle statCountStyle = new();

    private void OnEnable()
    {
        root = (StatsScriptableObject)target;

        // TODO: convert to statsList serializeProperty where possible
        // TODO: use root and SetDirty() to record and save all the remaining data changed

        statsList = serializedObject.FindProperty(nameof(root.statsList));

        InitializeStyles();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Color defaultColor = GUI.color;
        
        EditorTools.Label("Stats List", 25, new GUIStyle(EditorStyles.boldLabel), labelAlignment: TextAnchor.MiddleCenter);
        
        EditorTools.Line();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Stats count: " + statsList.arraySize.ToString(), statCountStyle);
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
            root.statsList[i]._showHideStat = EditorGUILayout.Foldout(root.statsList[i]._showHideStat, root.statsList[i].GetName, true);
            if (root.statsList[i]._showHideStat)
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                #region Move element up/down buttons
                EditorGUILayout.BeginVertical(GUILayout.Width(64));
                if (i == 0)
                    GUI.enabled = false;
                if (GUILayout.Button("Up", GUILayout.ExpandHeight(true)))
                {
                    root.statsList = Values.SwapListElements(root.statsList, i, i - 1);
                }
                GUI.enabled = true;
                if (i == root.statsList.Count - 1)
                    GUI.enabled = false;
                if (GUILayout.Button("Down", GUILayout.ExpandHeight(true)))
                {
                    root.statsList = Values.SwapListElements(root.statsList, i, i + 1);
                }
                GUI.enabled = true;
                EditorGUILayout.EndVertical();
                #endregion
                #region Stats data fields
                EditorGUILayout.BeginVertical();
                root.statsList[i]._statName = (StatTypes)EditorGUILayout.EnumPopup(new GUIContent("Name"), root.statsList[i]._statName);
                if (root.statsList[i]._hasMinValue && root.statsList[i]._hasMaxValue)
                {
                    root.statsList[i]._value = EditorGUILayout.Slider(new GUIContent("Value"), root.statsList[i]._value, root.statsList[i]._minValue, root.statsList[i]._maxValue);
                }
                else
                {
                    root.statsList[i]._value = EditorGUILayout.FloatField(new GUIContent("Value"), root.statsList[i]._value);
                
                    // TODO: ensure that the max value will always be higher than the lowest value
                }
                #region Toggle min value
                EditorGUILayout.BeginHorizontal();
                root.statsList[i]._hasMinValue = EditorGUILayout.Toggle(new GUIContent("Has minimum value"), root.statsList[i]._hasMinValue);
                if (root.statsList[i]._hasMinValue)
                {
                    root.statsList[i]._minValue = EditorGUILayout.FloatField(root.statsList[i]._minValue);
                }
                EditorGUILayout.EndHorizontal();
                #endregion
                #region Toggle max value
                EditorGUILayout.BeginHorizontal();
                root.statsList[i]._hasMaxValue = EditorGUILayout.Toggle(new GUIContent("Has maximum value"), root.statsList[i]._hasMaxValue);
                if (root.statsList[i]._hasMaxValue)
                {
                    root.statsList[i]._maxValue = EditorGUILayout.FloatField(root.statsList[i]._maxValue);
                }
                EditorGUILayout.EndHorizontal();
                #endregion
                EditorGUILayout.EndVertical();
                #endregion
                #region Delete element button
                GUI.color = Color.red;
                if (GUILayout.Button("X", GUILayout.Width(32), GUILayout.ExpandHeight(true)))
                {
                    root.statsList.Remove(root.statsList[i]);
                }
                GUI.color = defaultColor;
                #endregion
                EditorGUILayout.EndHorizontal();
            }
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

        statCountStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            alignment = TextAnchor.MiddleRight,
        };
    }
}
