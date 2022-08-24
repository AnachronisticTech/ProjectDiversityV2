using System;
using UnityEngine;

namespace CoreAttributes
{
    /// <summary>
    ///		This Attribute controls if the field is visible in the inspector or not.
    /// </summary>
    /// <remarks>
    ///     This works for Field/Property/Class/Struct.
    /// </remarks>
    [Serializable]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
    public sealed class HideConditionallyAttribute : PropertyAttribute
    {
        // The name of the bool field that will be in control
        private readonly string _conditionalSourceField;
        public string GetConditionalSourceField { get => _conditionalSourceField; }

        // If set to true then the field will be hidden otherwise it will be disabled but still visible in the inspector
        private readonly bool _hideInInspector;
        public bool IsHidingInInspector { get => _hideInInspector; }

        // If set to true then the condition will be reversed and then calculate the visibility in the inspector
        private readonly bool _reverseCondition;
        public bool ReverseCondition { get => _reverseCondition; }

        public HideConditionallyAttribute(string conditionalSourceField, bool hideInInspector = true, bool reverseCondition = false)
        {
            _conditionalSourceField = conditionalSourceField;
            _hideInInspector = hideInInspector;
            _reverseCondition = reverseCondition;
        }
    }
}
