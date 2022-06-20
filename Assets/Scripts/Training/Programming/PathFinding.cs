using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HelperNamespace;

/// <summary>
///     [What does this PathFinding do]
/// </summary>
public sealed class PathFinding : MonoBehaviour
{
    Grid grid;

    private void Awake()
    {
        grid = GetComponent<Grid>();
    }

    private void FindPath(Vector3 startPosition, Vector3 targetPosition)
    {
        Node startNode = grid.GetNodeForWorldPosition(startPosition);
        Node targetNode = grid.GetNodeForWorldPosition(targetPosition);

        List<Node> openSet = new();
        HashSet<Node> closeSet = new();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                //TODO: optimize this
                if (openSet[i].FCost < currentNode.FCost || (openSet[i].FCost == currentNode.FCost && openSet[i].HCost < currentNode.HCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closeSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return;
            }

            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                if (!neighbour.IsWalkable || closeSet.Contains(neighbour))
                    continue;
            }
        }
    }

    private int GetDistance(Node A, Node B)
    {
        return 0;
    }
}
