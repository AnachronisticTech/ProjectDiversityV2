using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

using CoreAttributes;

/// <summary>
///     [What does this TODOV2 do]
/// </summary>
public sealed class TODOV2 : MonoBehaviour
{
    [System.Serializable]
    public enum FeatureState
    {
        Pending,
        InDevelopment,
        Testing,
        BugFixing,
        Done
    }

    [System.Serializable]
    public enum BuildState
    {
        [InspectorName("Development")]
        Dev,
        Live
    }

    [System.Serializable]
    public class FeatureProperties
    {
        public string name;
        public string desc;
        public FeatureState state;
    }

    [System.Serializable]
    public class Build
    {
        public string BuildIndex;
        public List<FeatureProperties> changeLog;
        public BuildState buildState;
    }

    public List<Build> builds;
}