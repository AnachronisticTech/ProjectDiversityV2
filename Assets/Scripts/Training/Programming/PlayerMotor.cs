using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using HelperNamespace;
using System.Linq;

/// <summary>
///     [What does this PlayerMotor do]
/// </summary>
public sealed class PlayerMotor : MonoBehaviour
{
    private NavMeshAgent player;
    private LineRenderer lineRenderer;

    [SerializeField]
    private float pathVisualYOffset = 0;

    private void Start()
    {
        player = GetComponent<NavMeshAgent>();
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.startWidth = 0.15f;
        lineRenderer.endWidth = 0.15f;
        lineRenderer.positionCount = 0;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 50.0f))
                player.SetDestination(hit.point);
        }

        if (player.hasPath)
        {
            DrawPath();
        }
    }

    private void DrawPath()
    {
        lineRenderer.positionCount = player.path.corners.Length;
        lineRenderer.SetPosition(0, player.transform.position);

        if (player.path.corners.Length < 2)
            return;

        for (int i = 1; i < player.path.corners.Length; i++)
        {
            Vector3 point = new(player.path.corners[i].x,
                                player.path.corners[i].y + pathVisualYOffset,
                                player.path.corners[i].z);

            lineRenderer.SetPosition(i, point);
        }
    }
}
