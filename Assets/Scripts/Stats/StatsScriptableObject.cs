using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     [What does this StatsScriptableObject do]
/// </summary>
[CreateAssetMenu(fileName = "[EntityName]Stats", menuName = "Stats Scriptable Object")]
public sealed class StatsScriptableObject : ScriptableObject
{
    [HideInInspector]
    public List<Stat> statsList = new();

    //private void OnValidate()
    //{
    //    foreach (Stat stat in statsList)
    //    {
    //        stat.SetValue(Mathf.Clamp(stat.GetValue, stat.GetMinValue, stat.GetMaxValue));
    //    }
    //}
}
