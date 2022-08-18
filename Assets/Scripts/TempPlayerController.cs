/*
 * Script developed by Andreas Monoyios
 * GitHub: https://github.com/AMonoyios?tab=repositories
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
///     Temporary code to move player to the world space
/// </summary>
/// <remarks>
///     Code courtesy of PekkeDev on Youtube
///     source: https://www.youtube.com/watch?v=y_zdW5LIX5o&ab_channel=PekkeDev
/// </remarks>
[RequireComponent(typeof(NavMeshAgent))]
public sealed class TempPlayerController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5.0f;
    [SerializeField]
    private LayerMask groundLayerMask;

    private bool isMoving = false;
    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 200.0f))
            {
                agent.SetDestination(hit.point);
            }
        }

        if (!isMoving)
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 200.0f, groundLayerMask))
            {
                transform.LookAt(hit.point + Vector3.up * transform.position.y);
            }
        }
    }

    private void FixedUpdate()
    {
        isMoving = agent.remainingDistance > agent.stoppingDistance;
    }
}
