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

    GUIStyle smallButtonStyle = new();
    GUIStyle statCountStyle = new();

    const string deleteUnicode = "\u2718";
    const string upUnicode = "\u25b2";
    const string downUnicode = "\u25bc";

    const float minMaxDiff = 0.2f;

    private void OnEnable()
    {
        root = (StatsScriptableObject)target;
    
        InitializeStyles();
    }

    public override void OnInspectorGUI()
    {
        Color defaultColor = GUI.color;
        
        EditorTools.Label("Stats List", 25, new GUIStyle(EditorStyles.boldLabel), labelAlignment: TextAnchor.MiddleCenter);
        
        EditorTools.Line();

        #region Stats list count control
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Stats count: " + root.statsList.Count.ToString(), statCountStyle);

        #region Add new stat
        GUI.color = Color.green;
        if (GUILayout.Button("+", smallButtonStyle))
        {
            Undo.RecordObject(root, "Added new stat on " + root.name);
            root.statsList.Add(new Stat(StatTypes.Unknown, 0, float.MinValue, float.MaxValue));
            EditorUtility.SetDirty(root);
        }
        #endregion

        #region Remove last stat
        GUI.color = Color.red;
        if (GUILayout.Button("-", smallButtonStyle))
        {
            Undo.RecordObject(root, "Removed " + root.statsList[^1].GetName + " from " + root.name);
            root.statsList.RemoveAt(root.statsList.Count - 1);
            EditorUtility.SetDirty(root);
        }
        #endregion
        
        GUI.color = defaultColor;
        
        EditorGUILayout.EndHorizontal();
        #endregion

        EditorTools.Line();

        for (int i = 0; i < root.statsList.Count; i++)
        {
            EditorGUI.BeginChangeCheck();

            #region Controls the visibility in editor for the specific stat
            bool _toggleStatVisibility = EditorGUILayout.Foldout(root.statsList[i]._showHideStat, root.statsList[i].GetName, true);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(root, "Toggle visibility for " + root.statsList[i].GetName + " on " + root.name);
                root.statsList[i]._showHideStat = _toggleStatVisibility;
                EditorUtility.SetDirty(root);
            }
            #endregion
            
            if (root.statsList[i]._showHideStat)
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

                #region Move element up/down buttons
                EditorGUILayout.BeginVertical(GUILayout.Width(64));
                
                if (i == 0)
                    GUI.enabled = false;
                if (GUILayout.Button(upUnicode, GUILayout.ExpandHeight(true)))
                {
                    Undo.RecordObject(root, "Moved " + root.statsList[i].GetName + " element Up one");
                    root.statsList = Values.SwapListElements(root.statsList, i, i - 1);
                    EditorUtility.SetDirty(root);
                }
                GUI.enabled = true;
                
                if (i == root.statsList.Count - 1)
                    GUI.enabled = false;
                if (GUILayout.Button(downUnicode, GUILayout.ExpandHeight(true)))
                {
                    Undo.RecordObject(root, "Moved " + root.statsList[i].GetName + " element Down one");
                    root.statsList = Values.SwapListElements(root.statsList, i, i + 1);
                    EditorUtility.SetDirty(root);
                }
                GUI.enabled = true;
                
                EditorGUILayout.EndVertical();
                #endregion

                #region Stats data fields
                EditorGUILayout.BeginVertical();

                #region Stat name field
                StatTypes _oldStatName = root.statsList[i]._statName;
                
                EditorGUI.BeginChangeCheck();
                StatTypes _newStatName = (StatTypes)EditorGUILayout.EnumPopup(new GUIContent("Name"), root.statsList[i]._statName);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(root, "Changed name from " + _oldStatName + " to " + _newStatName + " for " + root.name);
                    root.statsList[i]._statName = _newStatName;
                    EditorUtility.SetDirty(root);
                }
                #endregion

                #region Stat value field
                // shows slider field or float field depending on min/max of stat
                float _oldValue = root.statsList[i].GetValue;
                EditorGUI.BeginChangeCheck();
                float _newValue;
                if (root.statsList[i]._hasMinValue && root.statsList[i]._hasMaxValue)
                {
                    _newValue = EditorGUILayout.Slider(new GUIContent("Value"), root.statsList[i]._value, root.statsList[i]._minValue, root.statsList[i]._maxValue);
                }
                else
                {
                    _newValue = EditorGUILayout.FloatField(new GUIContent("Value"), root.statsList[i]._value);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(root, "Changed value for " + root.statsList[i].GetName + ": " + _oldValue + " on " + root.name);
                    root.statsList[i]._value = _newValue;
                    EditorUtility.SetDirty(root);
                }

                // ensures the value will be in bounds of min/max of stat
                root.statsList[i]._value = Values.EnsureInRange(root.statsList[i]._value, root.statsList[i]._minValue, root.statsList[i]._maxValue);
                #endregion

                #region Toggle min value
                EditorGUILayout.BeginHorizontal();
                
                // Toggle if the stat has a min value
                EditorGUI.BeginChangeCheck();
                bool _hasMinValue = EditorGUILayout.Toggle(new GUIContent("Has minimum value"), root.statsList[i]._hasMinValue);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(root, "Toggled " + root.statsList[i].GetName + " to have or not have a min value");
                    root.statsList[i]._hasMinValue = _hasMinValue;
                    EditorUtility.SetDirty(root);
                }

                // control the min/max value of the stat
                if (root.statsList[i]._hasMinValue)
                {
                    EditorGUI.BeginChangeCheck();
                    float _minValue = EditorGUILayout.FloatField(root.statsList[i]._minValue);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(root, "Changed min value for " + root.statsList[i].GetName + " on " + root.name);
                        root.statsList[i]._minValue = _minValue;
                        EditorUtility.SetDirty(root);
                    }
                }
                else
                {
                    if (Values.IsFloatLessThan(float.MinValue, root.statsList[i]._minValue))
                    {
                        root.statsList[i]._minValue = float.MinValue;
                        EditorUtility.SetDirty(root);
                    }
                }

                EditorGUILayout.EndHorizontal();
                #endregion

                #region Toggle max value
                EditorGUILayout.BeginHorizontal();
                
                // Toggle if the stat has a max value
                EditorGUI.BeginChangeCheck();
                bool _hasMaxValue = EditorGUILayout.Toggle(new GUIContent("Has maximum value"), root.statsList[i]._hasMaxValue);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(root, "Toggled " + root.statsList[i].GetName + " to have or not have a max value");
                    root.statsList[i]._hasMaxValue = _hasMaxValue;
                    EditorUtility.SetDirty(root);
                }

                // control the min/max value of the stat
                if (root.statsList[i]._hasMaxValue)
                {
                    EditorGUI.BeginChangeCheck();
                    float _maxValue = EditorGUILayout.FloatField(root.statsList[i]._maxValue);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(root, "Changed max value for " + root.statsList[i].GetName + " on " + root.name);
                        root.statsList[i]._maxValue = _maxValue;
                        EditorUtility.SetDirty(root);
                    }
                }
                else
                {
                    if (Values.IsFloatMoreThan(float.MaxValue, root.statsList[i]._maxValue))
                    {
                        root.statsList[i]._maxValue = float.MaxValue;
                        EditorUtility.SetDirty(root);
                    }
                }
                
                EditorGUILayout.EndHorizontal();
                #endregion

                #region Ensure correct difference between min and max bounds
                // ensures that the min value will always be lower than the max value
                if (root.statsList[i]._hasMinValue && root.statsList[i]._hasMaxValue)
                {
                    if (root.statsList[i]._minValue >= root.statsList[i]._maxValue)
                    {
                        root.statsList[i]._minValue = root.statsList[i]._maxValue - minMaxDiff;
                        EditorUtility.SetDirty(root);
                    }
                }
                #endregion

                EditorGUILayout.EndVertical();
                #endregion

                #region Delete element button
                GUI.color = Color.red;
                if (GUILayout.Button(deleteUnicode, GUILayout.Width(32), GUILayout.ExpandHeight(true)))
                {
                    Undo.RecordObject(root, "Removed stat " + root.statsList[i].GetName + " from " + root.name);
                    root.statsList.Remove(root.statsList[i]);
                    EditorUtility.SetDirty(root);
                }
                GUI.color = defaultColor;
                #endregion

                EditorGUILayout.EndHorizontal();
            }
        }
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
