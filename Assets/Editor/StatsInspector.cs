using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using HelperNamespace;

/// <summary>
///     [What does this StatsInspector do]
/// </summary>
[CustomEditor(typeof(Stats)), CanEditMultipleObjects]
public sealed class StatsInspector : Editor
{
    Stats root;

    private void OnEnable()
    {
        root = (Stats)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorTools.Line();

        EditorTools.ShowStatDictionaryInInspector(root.statsDict);
    }
}
