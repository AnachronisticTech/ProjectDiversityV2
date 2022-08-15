using UnityEngine;
using UnityEditor;

namespace CoreAttributes
{
    [CustomPropertyDrawer(typeof(NamedArrayAttribute))]
    public sealed class NamedArrayPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            try
            {
                int pos = int.Parse(property.propertyPath.Split('[', ']')[1]);
                EditorGUI.ObjectField(rect, property, new GUIContent(((NamedArrayAttribute)attribute).names[pos]));
            }
            catch
            {
                EditorGUI.ObjectField(rect, property, label);
            }
        }
    }
}
