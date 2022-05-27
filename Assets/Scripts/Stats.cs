using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEditor;

using HelperNamespace;

public sealed class Stats : MonoBehaviour
{
    [SerializeField]
    private StatsScriptableObject statsSO;

    private Dictionary<string, Stat> _statsDict;
    public Dictionary<string, Stat> GetStatsDict { get => _statsDict; }

    private void Start()
    {
        _statsDict = new Dictionary<string, Stat>();

        foreach (Stat stat in statsSO.statsList)
        {
            _statsDict.Add(stat.GetName(), new Stat(stat.GetName(), stat.GetValue(), stat.GetMinValue(), stat.GetMaxValue()));
        }
    }
}
