using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using HelperEditorGUINamespace;

[CustomEditor(typeof(TODOV2)), CanEditMultipleObjects]
public sealed class TODOV2Inspector : Editor
{
    private TODOV2 root;
    private GUIStyle InsetFoldoutStyle => new(EditorStyles.foldout)
    {
        margin = new RectOffset(10, 0, 0, 0)
    };

    private static Color baseColor = new();
    private static Color PendingColor => Color.cyan;
    private static Color DevelopementColor => Color.yellow;
    private static Color TestingColor => Color.blue;
    private static Color BugFixingColor => Color.red;
    private static Color DoneColor => Color.magenta;
    private static Color LiveColor => Color.green;

    private void OnEnable()
    {
        root = (TODOV2)target;
        baseColor = GUI.color;
    }

    public override void OnInspectorGUI()
    {
        DrawVersionMonitor();

        DrawBuildControlPanel();

        CustomEditorGUI.GuiLine();

        for (int b = 0; b < root.builds.Count; b++)
        {
            EditorGUI.BeginChangeCheck();
            bool showBuild = EditorGUILayout.BeginFoldoutHeaderGroup(root.builds[b].isVisibleInInspector, new GUIContent(root.builds[b].name));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(root, "Toggle visibility for build " + root.GetVersionName(b));
                root.builds[b].isVisibleInInspector = showBuild;
                EditorUtility.SetDirty(root);
            }

            if (root.builds[b].isVisibleInInspector)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.BeginHorizontal();
                CustomEditorGUI.DrawTextField(root, new GUIContent("Build name"), root.builds[b].name, "Renamed " + root.GetVersionName(b), (string newTextField) => { root.builds[b].name = newTextField; });
                CustomEditorGUI.DrawButton(root, new GUIContent("X"), "Removed build " + root.GetVersionName(b), () => { root.builds.RemoveAt(b); });
                EditorGUILayout.EndHorizontal();

                // Bug fix: when removing the last index of the list and on it points to an out of range index
                if (b >= root.builds.Count)
                    continue;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Features"));
                CustomEditorGUI.DrawButton(root, new GUIContent("+"), "Added new feature to list", () => { root.builds[b].changeLog.Add(new TODOV2.FeatureProperties()); });
                CustomEditorGUI.DrawButton(root, new GUIContent("-"), "Removed last build from list", () => { root.builds[b].changeLog.RemoveAt(root.builds[b].changeLog.Count - 1); }, root.builds[b].changeLog.Count > 0);
                EditorGUILayout.EndHorizontal();

                CustomEditorGUI.GuiLine();

                for (int f = 0; f < root.builds[b].changeLog.Count; f++)
                {
                    switch (root.builds[b].changeLog[f].state)
                    {
                        case TODOV2.FeatureState.Pending:
                            GUI.color = PendingColor;
                            break;
                        case TODOV2.FeatureState.InDevelopment:
                            GUI.color = DevelopementColor;
                            break;
                        case TODOV2.FeatureState.Testing:
                            GUI.color = TestingColor;
                            break;
                        case TODOV2.FeatureState.BugFixing:
                            GUI.color = BugFixingColor;
                            break;
                        case TODOV2.FeatureState.Done:
                            GUI.color = DoneColor;
                            break;
                        case TODOV2.FeatureState.Live:
                            GUI.color = LiveColor;
                            break;
                    }

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUI.BeginChangeCheck();
                    bool showFeature = EditorGUILayout.Foldout(root.builds[b].changeLog[f].isVisibleInInspector, new GUIContent(root.builds[b].changeLog[f].name), true, InsetFoldoutStyle);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(root, "Toggle visibility for feature " + root.GetVersionName(b, f));
                        root.builds[b].changeLog[f].isVisibleInInspector = showFeature;
                        EditorUtility.SetDirty(root);
                    }

                    if (root.builds[b].changeLog[f].isVisibleInInspector)
                    {
                        EditorGUILayout.BeginHorizontal();
                        CustomEditorGUI.DrawTextField(root, new GUIContent("Feature name"), root.builds[b].changeLog[f].name, "Renamed " + root.GetVersionName(b, f), (string newTextField) => { root.builds[b].changeLog[f].name = newTextField; });
                        CustomEditorGUI.DrawButton(root, new GUIContent("X"), "Removed feature " + root.GetVersionName(b, f), () => { root.builds[b].changeLog.RemoveAt(f); });
                        EditorGUILayout.EndHorizontal();

                        // Bug fix: when removing the last index of the list and on it points to an out of range index
                        if (f >= root.builds[b].changeLog.Count)
                            continue;

                        CustomEditorGUI.DrawTextField(root, new GUIContent("Feature description"), root.builds[b].changeLog[f].desc, "Changed description for " + root.GetVersionName(b, f), (string newTextField) => { root.builds[b].changeLog[f].desc = newTextField; });

                        EditorGUI.BeginChangeCheck();
                        TODOV2.FeatureState newFeatureState = (TODOV2.FeatureState)EditorGUILayout.EnumPopup("Feature state", root.builds[b].changeLog[f].state);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(root, "Changed state of feature " + root.GetVersionName(b, f));
                            root.builds[b].changeLog[f].state = newFeatureState;
                            EditorUtility.SetDirty(root);
                        }

                        CustomEditorGUI.GuiLine();
                    }

                    EditorGUILayout.EndVertical();
                    GUI.color = baseColor;
                }

                EditorGUI.BeginChangeCheck();
                TODOV2.BuildState newBuildState = (TODOV2.BuildState)EditorGUILayout.EnumPopup("Build state", root.builds[b].state);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(root, "Changed state of build " + root.GetVersionName(b));
                    root.builds[b].state = newBuildState;
                    EditorUtility.SetDirty(root);
                }

                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            CustomEditorGUI.GuiLine();
        }
    }

    private void DrawVersionMonitor()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Latest Dev build: " + root.GetLatestVersionName(TODOV2.BuildState.Dev, TODOV2.FeatureState.Live, true));
        EditorGUILayout.LabelField("Latest Live build: " + root.GetLatestVersionName(TODOV2.BuildState.Live, TODOV2.FeatureState.Live));
        EditorGUILayout.EndVertical();
    }

    private void DrawBuildControlPanel()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("Builds"));
        CustomEditorGUI.DrawButton(root, new GUIContent("+"), "Added new build to list", () => { root.builds.Add(new TODOV2.Build()); });
        CustomEditorGUI.DrawButton(root, new GUIContent("-"), "Removed last build from list", () => { root.builds.RemoveAt(root.builds.Count - 1); }, root.builds.Count > 0);
        EditorGUILayout.EndHorizontal();
    }
}
