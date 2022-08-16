using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TODOV2)), CanEditMultipleObjects]
public sealed class TODOV2Inspector : Editor
{
    private TODOV2 root;

    private const float buttonWidth = 32.0f;

    private void OnEnable()
    {
        root = (TODOV2)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Latest Dev build: " + GetLatestVersionName(TODOV2.BuildState.Dev, TODOV2.FeatureState.Live, true));
        EditorGUILayout.LabelField("Latest Live build: " + GetLatestVersionName(TODOV2.BuildState.Live, TODOV2.FeatureState.Live));
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("Builds"));
        DrawButton(new GUIContent("+"), "Added new build to list", () => { root.builds.Add(new TODOV2.Build()); });
        DrawButton(new GUIContent("-"), "Removed last build from list", () => { root.builds.RemoveAt(root.builds.Count - 1); }, root.builds.Count > 0);
        EditorGUILayout.EndHorizontal();
        
        GuiLine();

        for (int b = 0; b < root.builds.Count; b++)
        {
            EditorGUI.BeginChangeCheck();
            bool showBuild = EditorGUILayout.BeginFoldoutHeaderGroup(root.builds[b].isVisibleInInspector, new GUIContent(root.builds[b].name));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(root, "Toggle visibility for build " + GetVersionName(b));
                root.builds[b].isVisibleInInspector = showBuild;
                EditorUtility.SetDirty(root);
            }
            if (root.builds[b].isVisibleInInspector)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.BeginHorizontal();
                DrawTextField(new GUIContent("Build name"), root.builds[b].name, "Renamed " + GetVersionName(b), (string newTextField) => { root.builds[b].name = newTextField; });
                DrawButton(new GUIContent("X"), "Removed build " + GetVersionName(b), () => { root.builds.RemoveAt(b); });
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Features"));
                DrawButton(new GUIContent("+"), "Added new feature to list", () => { root.builds[b].changeLog.Add(new TODOV2.FeatureProperties()); });
                DrawButton(new GUIContent("-"), "Removed last build from list", () => { root.builds[b].changeLog.RemoveAt(root.builds[b].changeLog.Count - 1); }, root.builds[b].changeLog.Count > 0);
                EditorGUILayout.EndHorizontal();
                
                GuiLine();
                
                for (int f = 0; f < root.builds[b].changeLog.Count; f++)
                {
                    EditorGUI.BeginChangeCheck();
                    bool showFeature = EditorGUILayout.Foldout(root.builds[b].changeLog[f].isVisibleInInspector, new GUIContent(root.builds[b].changeLog[f].name), true);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(root, "Toggle visibility for feature " + GetVersionName(b, f));
                        root.builds[b].changeLog[f].isVisibleInInspector = showFeature;
                        EditorUtility.SetDirty(root);
                    }

                    if (root.builds[b].changeLog[f].isVisibleInInspector)
                    {
                        EditorGUILayout.BeginHorizontal();
                        DrawTextField(new GUIContent("Feature name"), root.builds[b].changeLog[f].name, "Renamed " + GetVersionName(b, f), (string newTextField) => { root.builds[b].changeLog[f].name = newTextField; });
                        DrawButton(new GUIContent("X"), "Removed feature " + GetVersionName(b, f), () => { root.builds[b].changeLog.RemoveAt(f); });
                        EditorGUILayout.EndHorizontal();
                        DrawTextField(new GUIContent("Feature description"), root.builds[b].changeLog[f].desc, "Changed description for " + GetVersionName(b, f), (string newTextField) => { root.builds[b].changeLog[f].desc = newTextField; });

                        EditorGUI.BeginChangeCheck();
                        TODOV2.FeatureState newFeatureState = (TODOV2.FeatureState)EditorGUILayout.EnumPopup("Feature state", root.builds[b].changeLog[f].state);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(root, "Changed state of feature " + GetVersionName(b, f));
                            root.builds[b].changeLog[f].state = newFeatureState;
                            EditorUtility.SetDirty(root);
                        }

                        GuiLine();
                    }
                }

                EditorGUI.BeginChangeCheck();
                TODOV2.BuildState newBuildState = (TODOV2.BuildState)EditorGUILayout.EnumPopup("Build state", root.builds[b].state);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(root, "Changed state of build " + GetVersionName(b));
                    root.builds[b].state = newBuildState;
                    EditorUtility.SetDirty(root);
                }

                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            GuiLine();
        }
    }

    private string GetVersionName(int buildIndex, int featureIndex = -1)
    {
        return buildIndex.ToString() + "." + (featureIndex > -1 ? featureIndex : 0);
    }
    private string GetLatestVersionName(TODOV2.BuildState buildState, TODOV2.FeatureState featureState, bool otherThanFeatureState = false)
    {
        string latestVersion = "None";
        for (int b = 0; b < root.builds.Count; b++)
        {
            if (root.builds[b].state == buildState)
            {
                for (int f = 0; f < root.builds[b].changeLog.Count; f++)
                {
                    if (otherThanFeatureState)
                    {
                        if (root.builds[b].changeLog[f].state != featureState)
                            latestVersion = b + "." + f + " @ " + root.builds[b].name + " > " + root.builds[b].changeLog[f].name;
                    }
                    else
                    {
                        if (root.builds[b].changeLog[f].state == featureState)
                            latestVersion = b + "." + f + " @ " + root.builds[b].name + " > " + root.builds[b].changeLog[f].name;
                    }
                }
            }
        }
        return latestVersion;
    }

    private void DrawButton(GUIContent buttonGUIContent, string buttonUndoRecord, System.Action onClick, bool visibilityCondition = true, float buttonWidth = buttonWidth)
    {
        GUI.enabled = visibilityCondition;
        if (GUILayout.Button(buttonGUIContent, GUILayout.Width(buttonWidth)))
        {
            Undo.RecordObject(root, buttonUndoRecord);
            onClick.Invoke();
            EditorUtility.SetDirty(root);
        }
        GUI.enabled = true;
    }
    private void DrawTextField(GUIContent guiContent, string textFieldContent, string textFieldUndoRecord, System.Action<string> onChange, bool visibilityCondition = true)
    {
        GUI.enabled = visibilityCondition;
        EditorGUI.BeginChangeCheck();
        string newTextFieldContent = EditorGUILayout.TextField(guiContent, textFieldContent);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(root, textFieldUndoRecord);
            onChange.Invoke(newTextFieldContent);
            EditorUtility.SetDirty(root);
        }
        GUI.enabled = true;
    }
    private void GuiLine(int i_height = 1)
    {
        Rect rect = EditorGUILayout.GetControlRect(false, i_height);
        rect.height = i_height;
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
    }
}
