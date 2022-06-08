using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using HelperNamespace;

/// <summary>
///     Custom editor for TODO class.
/// </summary>
[CustomEditor(typeof(TODO)), CanEditMultipleObjects]
public sealed class TODOInspector : Editor
{
    TODO root;

    Color baseColor = new();
    Color developementColor = Color.yellow;
    Color improvementsColor = Color.cyan;
    Color bugFixingColor = Color.red;
    Color completedColor = Color.green;

    GUIStyle buttonStyle = new();
    GUIStyle descriptionStyle = new();

    const string deleteUnicode = "\u2718";

    private void OnEnable()
    {
        root = (TODO)target;

        InitializeStyles();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorTools.Line();

        baseColor = GUI.color;

        #region List count controls
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Tasks count");
        EditorGUILayout.LabelField(root.tasks.Count.ToString(), GUILayout.Width(32));
        GUI.color = Color.green;
        if (GUILayout.Button("+", buttonStyle))
        {
            Undo.RecordObject(root, "Added new To-Do task.");
            root.tasks.Add(new TODO.Task());
            EditorUtility.SetDirty(root);
        }
        GUI.color = Color.red;
        if (GUILayout.Button("-", buttonStyle))
        {
            Undo.RecordObject(root, "Removed last To-Do task.");
            root.tasks.RemoveAt(root.tasks.Count - 1);
            EditorUtility.SetDirty(root);
        }
        GUI.color = baseColor;
        EditorGUILayout.EndHorizontal();
        #endregion

        EditorTools.Line();

        for (int i = 0; i < root.tasks.Count; i++)
        {
            GUI.color = root.tasks[i].state switch
            {
                TODO.TaskState.Pending => baseColor,
                TODO.TaskState.InDevelopment => developementColor,
                TODO.TaskState.Improvements => improvementsColor,
                TODO.TaskState.BugFixing => bugFixingColor,
                TODO.TaskState.Completed => completedColor,
                _ => baseColor,
            };

            #region Task visibility
            EditorGUI.BeginChangeCheck();
            bool visibleTaskInInspector = EditorGUILayout.Foldout(root.tasks[i]._isTaskVisibleInInspector, root.tasks[i].task, true);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(root, "Toggle visibility for " + root.tasks[i].task);
                root.tasks[i]._isTaskVisibleInInspector = visibleTaskInInspector;
                EditorUtility.SetDirty(root);
            }
            #endregion

            if (root.tasks[i]._isTaskVisibleInInspector)
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.BeginVertical();

                #region Task name
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Task", GUILayout.Width(100.0f));
                EditorGUI.BeginChangeCheck();
                string taskName = EditorGUILayout.TextField(root.tasks[i].task);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(root, "Changed name of " + taskName);
                    root.tasks[i].task = taskName;
                    EditorUtility.SetDirty(root);
                }
                EditorGUILayout.EndHorizontal();
                #endregion

                #region Description area
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Description", GUILayout.Width(100.0f));
                EditorGUI.BeginChangeCheck();
                string taskDescription = EditorGUILayout.TextArea(root.tasks[i].description, descriptionStyle);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(root, "Changed description of " + taskName);
                    root.tasks[i].description = taskDescription;
                    EditorUtility.SetDirty(root);
                }
                EditorGUILayout.EndHorizontal();
                #endregion

                #region Task state
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("State", GUILayout.Width(100.0f));
                EditorGUI.BeginChangeCheck();
                TODO.TaskState taskState = (TODO.TaskState)EditorGUILayout.EnumPopup(root.tasks[i].state);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(root, "Changed state for " + taskName);
                    root.tasks[i].state = taskState;
                    EditorUtility.SetDirty(root);
                }
                EditorGUILayout.EndHorizontal();
                #endregion

                EditorGUILayout.EndVertical();

                #region Delete task button
                GUI.color = Color.red;
                if (GUILayout.Button(deleteUnicode, GUILayout.Width(32.0f), GUILayout.ExpandHeight(true)))
                {
                    Undo.RecordObject(root, "Deleted " + root.tasks[i].task);
                    root.tasks.RemoveAt(i);
                    EditorUtility.SetDirty(root);
                }
                GUI.color = baseColor;
                #endregion

                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(5);
        }
    }

    private void InitializeStyles()
    {
        buttonStyle = new GUIStyle(EditorStyles.miniButton)
        {
            fixedWidth = 32.0f
        };

        descriptionStyle = new GUIStyle(EditorStyles.textArea)
        {
            wordWrap = true,
            stretchHeight = true,
        };
    }
}
