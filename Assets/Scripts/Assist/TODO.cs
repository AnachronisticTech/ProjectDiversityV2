using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     List of To-Do in this project.
/// </summary>
[System.Obsolete("Depricated TODO. Use TODOV2 instead.")]
public sealed class TODO : MonoBehaviour
{
    public enum TaskState
    {
        Pending,
        InDevelopment,
        Improvements,
        BugFixing,
        Completed,
    }

    [System.Serializable]
    public class Task
    {
        public string task = "New task";
        public string description = "Optional task description";
        public TaskState state = TaskState.Pending;

        public bool _isTaskVisibleInInspector = true;
    }

    [HideInInspector]
    public List<Task> tasks = new();

    public void AddNewTask()
    {
        tasks.Add(new());
    }
    public void RemoveTask(int taskIndex)
    {
        tasks.RemoveAt(taskIndex);
    }
}
