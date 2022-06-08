using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HelperNamespace;

/// <summary>
///     [What does this TODO do]
/// </summary>
public sealed class TODO : MonoBehaviour
{
    [System.Serializable]
    private class Task
    {
        public string task = "New task";
        public string description = "Optional task description";
        public bool isComplete = false;
    }

    [SerializeField]
    private List<Task> tasks = new List<Task>();
}
