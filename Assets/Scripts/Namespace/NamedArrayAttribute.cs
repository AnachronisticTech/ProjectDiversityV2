using UnityEngine;

namespace CoreAttributes
{
    public sealed class NamedArrayAttribute : PropertyAttribute
    {
        public readonly string[] names;
        public NamedArrayAttribute(string[] names) { this.names = names; }
    }
}
