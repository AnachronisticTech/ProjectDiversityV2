using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HelperNamespace;

/// <summary>
///     [What does this StatsScriptableObject do]
/// </summary>
[CreateAssetMenu(fileName = "[EntityName]Stats", menuName = "Stats Scriptable Object")]
public sealed class StatsScriptableObject : ScriptableObject
{
    public List<Stat> statsList = new List<Stat>();
}
