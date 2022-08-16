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
        Done,
        Live
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

        public bool isVisibleInInspector = true;

        public FeatureProperties(string name = defaultFeatureName, string desc = defaultEmptyString, FeatureState state = FeatureState.Pending)
        {
            this.name = name;
            this.desc = desc;
            this.state = state;
        }
    }

    [System.Serializable]
    public class Build
    {
        public string name;
        public List<FeatureProperties> changeLog;
        public BuildState state;

        public bool isVisibleInInspector = true;

        public Build(string name = defaultBuildName, BuildState state = BuildState.Dev)
        {
            this.name = name;
            changeLog = new List<FeatureProperties>();
            this.state = state;
        }
    }

    private const string defaultBuildName = "New Build";
    private const string defaultFeatureName = "New Feature";
    private const string defaultEmptyString = "";

    public List<Build> builds = new();
}