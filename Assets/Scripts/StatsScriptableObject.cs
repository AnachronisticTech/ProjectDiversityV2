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
    [SerializeField]
    private List<Stat> stats = new List<Stat>();

    public Dictionary<string, Stat> statsDict = new Dictionary<string, Stat>();

    private void OnEnable()
    {
        foreach (Stat stat in stats)
        {
            statsDict.Add(stat.GetName(), stat);
        }
    }
}
