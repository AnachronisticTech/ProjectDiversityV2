using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HelperNamespace;
using System;

/// <summary>
///     [What does this AStar do]
/// </summary>
public sealed class Grid : MonoBehaviour
{
    private enum GridGizmo
    {
        None,
        Walkable,
        NonWalkable,
        Both
    }

    [SerializeField]
    private Vector3 gridWorldSize = Vector3.one;
    [SerializeField]
    private Vector3 gridOffset = Vector3.zero;
    [SerializeField]
    private float nodeRadius = 1.0f;
    [SerializeField]
    private LayerMask obstacleLayerMask;

    private PathFinding pathFinding;
    private Node[,,] grid;
    private float nodeDiameter;
    private Vector3Int gridSize;

    [Header("Debugging")]
    [SerializeField]
    private GridGizmo gridGizmoDrawMode = GridGizmo.None;
    [SerializeField, ConditionalHide(nameof(gridGizmoDrawMode), true)]
    private Color walkableGizmoColor = new(0.0f, 1.0f, 0.0f, 0.25f);
    [SerializeField, ConditionalHide(nameof(gridGizmoDrawMode), true)]
    private Color nonWalkableGizmoColor = new(1.0f, 0.0f, 0.0f, 0.25f);
    [SerializeField, ConditionalHide(nameof(gridGizmoDrawMode), true)]
    private Color seekerGizmoColor = Color.blue;
    [SerializeField, ConditionalHide(nameof(gridGizmoDrawMode), true)]
    private Color targetGizmoColor = Color.cyan;
    [SerializeField, ConditionalHide(nameof(gridGizmoDrawMode), true)]
    private Color pathNodeGizmoColor = Color.yellow;
    [SerializeField]
    private Vector3 gridWorldBottomLeft;

    private void Awake()
    {
        pathFinding = GetComponent<PathFinding>();
    }

    private void Start()
    {
        InitializeGrid(() =>
        {
            StartCoroutine(CreateGrid(() =>
            {
                Debug.Log("Finished creating grid.");
            }));
        });
    }
    private void InitializeGrid(Action onComplete = null)
    {
        Debug.Log("Initializing grid...");

        nodeDiameter = nodeRadius * 2.0f;
        gridSize = new Vector3Int(Mathf.RoundToInt(gridWorldSize.x / nodeDiameter),
                                  Mathf.RoundToInt(gridWorldSize.y / nodeDiameter),
                                  Mathf.RoundToInt(gridWorldSize.z / nodeDiameter));

        onComplete?.Invoke();
    }
    private IEnumerator CreateGrid(Action onComplete = null)
    {
        Debug.Log("Creating grid...");

        gridWorldBottomLeft = transform.position - Vector3.right * ((gridWorldSize.x / 2.0f) - gridOffset.x) -
                                                   Vector3.up * ((gridWorldSize.y / 2.0f) - gridOffset.y) -
                                                   Vector3.forward * ((gridWorldSize.z / 2.0f) - gridOffset.z);

        grid = new Node[gridSize.x, gridSize.y, gridSize.z];
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int z = 0; z < gridSize.z; z++)
                {
                    Vector3 worldPoint = gridWorldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) +
                                                               Vector3.up * (y * nodeDiameter + nodeRadius) +
                                                               Vector3.forward * (z * nodeDiameter + nodeRadius);

                    bool walkable = Physics.CheckSphere(worldPoint, nodeRadius, obstacleLayerMask);

                    grid[x, y, z] = new Node(worldPoint, walkable, new Vector3Int(x,y,z));
                }
            }
        }

        onComplete?.Invoke();

        yield return null;
    }

    public Node GetNodeForWorldPosition(Vector3 worldPosition)
    {
        Vector3 percent = new(Mathf.Clamp01((worldPosition.x - gridOffset.x + gridWorldSize.x / 2.0f) / gridWorldSize.x),
                              Mathf.Clamp01((worldPosition.y - gridOffset.y + gridWorldSize.y / 2.0f) / gridWorldSize.y),
                              Mathf.Clamp01((worldPosition.z - gridOffset.z + gridWorldSize.z / 2.0f) / gridWorldSize.z));
        
        // get the xyz indecies of the grid array
        Vector3Int indecies = new(Mathf.RoundToInt((gridSize.x - 1) * percent.x),
                                  Mathf.RoundToInt((gridSize.y - 1) * percent.y),
                                  Mathf.RoundToInt((gridSize.z - 1) * percent.z));

        return grid[indecies.x, indecies.y, indecies.z];
    }
    
    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    // this will skip this iteration of node because is the current node of the agent
                    if (x == 0 && y == 0 && z == 0)
                        continue;
                
                    Vector3Int check = new(node.GridPosition.x + x, 
                                           node.GridPosition.y + y, 
                                           node.GridPosition.z + z);

                    if (check.x >= 0 && check.x < gridSize.x && check.y >= 0 && check.y < gridSize.y && check.z >= 0 && check.z < gridSize.z)
                    {
                        neighbours.Add(grid[check.x, check.y, check.z]);
                    }
                }
            }
        }

        return neighbours;
    }

    public List<Node> path;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position + gridOffset, gridWorldSize);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(gridWorldBottomLeft, 0.1f);

        if (grid == null)
            return;

        Node seekerNode = GetNodeForWorldPosition(pathFinding.seeker.position);
        Node targetNode = GetNodeForWorldPosition(pathFinding.target.position);
        foreach (Node node in grid)
        {
            if (seekerNode == node)
                Gizmos.color = seekerGizmoColor;
            else if (targetNode == node)
                Gizmos.color = targetGizmoColor;
            else
                Gizmos.color = node.IsWalkable ? walkableGizmoColor : nonWalkableGizmoColor;

            if (path != null && path.Contains(node))
                Gizmos.color = pathNodeGizmoColor;

            switch (gridGizmoDrawMode)
            {
                case GridGizmo.None:
                    break;
                case GridGizmo.Walkable:
                    if (node.IsWalkable)
                        DrawGizmoNode(node);
                    break;
                case GridGizmo.NonWalkable:
                    if (!node.IsWalkable)
                        DrawGizmoNode(node);
                    break;
                case GridGizmo.Both:
                    DrawGizmoNode(node);
                    break;
            }
        }
    }
    private void DrawGizmoNode(Node node)
    {
        Gizmos.DrawCube(node.WorldPosition, Vector3.one * (nodeDiameter - 0.1f));
    }
}
