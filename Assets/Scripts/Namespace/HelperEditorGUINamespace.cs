using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
///     [What does this HelperEditorGUINamespace do]
/// </summary>
namespace HelperEditorGUINamespace
{
    public class CustomEditorGUI : Editor
    {
        private const float constWidth = 32.0f;

        public static void DrawButton<T>(T target, GUIContent buttonGUIContent, string buttonUndoRecord, System.Action onClick, bool visibilityCondition = true, float buttonWidth = constWidth) where T : Object
        {
            GUI.enabled = visibilityCondition;
            if (GUILayout.Button(buttonGUIContent, GUILayout.Width(buttonWidth)))
            {
                Undo.RecordObject(target, buttonUndoRecord);
                onClick.Invoke();
                EditorUtility.SetDirty(target);
            }
            GUI.enabled = true;
        }
        public static void DrawTextField<T>(T target, GUIContent guiContent, string textFieldContent, string textFieldUndoRecord, System.Action<string> onChange, bool visibilityCondition = true, float textFieldWidth = float.NaN) where T : Object
        {
            GUI.enabled = visibilityCondition;
            EditorGUI.BeginChangeCheck();
            string newTextFieldContent = textFieldWidth.Equals(float.NaN) ? EditorGUILayout.TextField(guiContent, textFieldContent) :
                                                                            EditorGUILayout.TextField(guiContent, textFieldContent, GUILayout.Width(textFieldWidth));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, textFieldUndoRecord);
                onChange.Invoke(newTextFieldContent);
                EditorUtility.SetDirty(target);
            }
            GUI.enabled = true;
        }
        public static void GuiLine(int i_height = 1)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, i_height);
            rect.height = i_height;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        }
    }
}
