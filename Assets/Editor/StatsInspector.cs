using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using HelperNamespace;

/// <summary>
///     [What does this StatsInspector do]
/// </summary>
[CustomEditor(typeof(StatsList)), CanEditMultipleObjects]
public sealed class StatsInspector : Editor
{
    StatsList root;

    private void OnEnable()
    {
        root = (StatsList)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorTools.Line();

        EditorTools.ShowStatDictionaryInInspector(root.statsDict);
    }
}
