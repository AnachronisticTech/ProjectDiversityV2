using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using HelperNamespace;

/// <summary>
///     The way entities move in scene.
/// </summary>
public sealed class NPCNavigationControllerV3 : MonoBehaviour
{
    [Header("Waypoint data")]
    public Waypoint currentWaypoint;
    [Range(0.0f, 0.99f)]
    public float chanceOfFlippingDirection = 0.25f;
    //private bool isMovingClockwise = false;

    private void Start()
    {
        Debug.Log("Spawned: " + name);
    }

    private void Update()
    {

    }
}
