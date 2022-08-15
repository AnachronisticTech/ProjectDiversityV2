using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TODOV2)), CanEditMultipleObjects]
public sealed class TODOV2Inspector : Editor
{
    private TODOV2 root;

    private const float buttonWidth = 32.0f;

    private string buildName;

    private void OnEnable()
    {
        root = (TODOV2)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawButtonProperties addButton = new()
        {
            label = "+",
            undoRecordDesc = "Added new Build index",
            execute = () => root.AddNewBuildToList(),
            width = buttonWidth
        };
        DrawButtonProperties removeButton = new()
        {
            label = "-",
            undoRecordDesc = "Removed Build last index",
            execute = () => root.RemoveBuildIndex(),
            width = buttonWidth
        };
        DrawListCountManipulation("Builds", addButton, removeButton);

        for (int i = 0; i < root.GetBuildCount; i++)
        {
            EditorGUI.BeginChangeCheck();
            buildName = EditorGUILayout.TextField(new GUIContent("Build name"), root.GetBuildNameAtIndex(i));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(root, "Renamed build name");
                root.SetBuildNameAtIndex(buildName, i);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    private struct DrawButtonProperties
    {
        public string label;
        public string undoRecordDesc;
        public System.Action execute;
        public float width;
    }
    private void DrawListCountManipulation(string buildName, DrawButtonProperties leftButton, DrawButtonProperties rightButton)
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(new GUIContent(buildName));

        DrawButton(leftButton);

        GUI.enabled = root.GetBuildCount > 0;
        DrawButton(rightButton);
        GUI.enabled = true;

        EditorGUILayout.EndHorizontal();
    }
    private void DrawButton(DrawButtonProperties buttonProperties)
    {
        if (GUILayout.Button(buttonProperties.label, GUILayout.Width(buttonProperties.width)))
        {
            Undo.RecordObject(root, buttonProperties.undoRecordDesc);
            buttonProperties.execute.Invoke();
        }
    }
}
