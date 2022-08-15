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
    private enum FeatureState
    {
        Pending,
        InDevelopment,
        Testing,
        BugFixing,
        Done
    }

    [System.Serializable]
    private enum BuildState
    {
        [InspectorName("Development")]
        Dev,
        Live
    }

    [System.Serializable]
    private class FeatureProperties
    {
        public string name;
        public string desc;
        public FeatureState state;
    }

    [System.Serializable]
    private class Build
    {
        public string buildName;
        public List<FeatureProperties> changeLog;
        public BuildState buildState;

        public Build(string buildName = defaultBuildName)
        {
            this.buildName = buildName;
        }
    }
    #region Build functions

    public void AddNewBuildToList(string buildName = defaultBuildName) 
    { 
        builds.Add(new Build(buildName)); 
    }

    public void RemoveBuildIndex(int index = -1)
    {
        builds.RemoveAt(index <= -1 ? builds.Count - 1 : index);
    }

    public int GetBuildCount { get { return builds.Count; } }

    public string GetBuildNameAtIndex(int index)
    {
        return builds[index].buildName;
    }

    public void SetBuildNameAtIndex(string newBuildName, int index)
    {
        builds[index].buildName = newBuildName;
    }

    #endregion

    private const string defaultBuildName = "New Build";

    private List<Build> builds;
}