using UnityEngine;
using UnityEditor;

namespace CoreAttributes
{
    /// <summary>
    ///		Thus is the property drawer for the HideConditionallyAttribute.
    /// </summary>
    [CustomPropertyDrawer(typeof(HideConditionallyAttribute))]
    public sealed class HideConditionallyPropertyDrawer : PropertyDrawer
    {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			HideConditionallyAttribute conditionalHide = (HideConditionallyAttribute)attribute;
			bool enabled = GetConditionalHideAttributeResult(conditionalHide, property);

			bool wasEnabled = GUI.enabled;
			GUI.enabled = enabled;
			if (!conditionalHide.IsHidingInInspector || enabled)
			{
				EditorGUI.PropertyField(position, property, label, true);
			}
			GUI.enabled = wasEnabled;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			HideConditionallyAttribute conditionalHide = (HideConditionallyAttribute)attribute;
			bool enabled = GetConditionalHideAttributeResult(conditionalHide, property);

			if (!conditionalHide.IsHidingInInspector || enabled)
			{
				return EditorGUI.GetPropertyHeight(property, label);
			}
			else
			{
				return -EditorGUIUtility.standardVerticalSpacing;
			}
		}

		private bool GetConditionalHideAttributeResult(HideConditionallyAttribute condHAtt, SerializedProperty property)
		{
			bool enabled = true;
			string propertyPath = property.propertyPath; //returns the property path of the property we want to apply the attribute to
			string conditionPath = propertyPath.Replace(property.name, condHAtt.GetConditionalSourceField); //changes the path to the conditionalsource property path
			SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);

			if (sourcePropertyValue != null)
			{
				enabled = condHAtt.ReverseCondition ? !sourcePropertyValue.boolValue : sourcePropertyValue.boolValue;
			}
			else
			{
				Debug.LogWarning("Attempting to use a ConditionalHideAttribute but no matching SourcePropertyValue found in object: " + condHAtt.GetConditionalSourceField);
			}

			return enabled;
		}
	}
}
