using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HelperNamespace;

/// <summary>
///     [What does this PathFinding do]
/// </summary>
public sealed class PathFinding : MonoBehaviour
{
    public Transform seeker;
    public Transform target;

    Grid grid;

    private void Awake()
    {
        grid = GetComponent<Grid>();
    }

    private void Update()
    {
        FindPath(seeker.position, target.position);
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
                if (openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                    currentNode = openSet[i];
            }

            openSet.Remove(currentNode);
            closeSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                if (!neighbour.IsWalkable || closeSet.Contains(neighbour))
                    continue;

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if  (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);

                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
    }

    private void RetracePath(Node startNode, Node targetNode)
    {
        List<Node> path = new();

        Node currentNode = targetNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();

        grid.path = path;
    }

    // TODO: Not working as intended.
    private int GetDistance(Node nodeA, Node nodeB)
    {
        int distanceX = Mathf.Abs(nodeA.GridPosition.x - nodeB.GridPosition.x);
        //int distanceY = Mathf.Abs(nodeA.GridPosition.y - nodeB.GridPosition.y);
        int distanceZ = Mathf.Abs(nodeA.GridPosition.z - nodeB.GridPosition.z);

        //if (distanceX >= distanceY)
        //{
        //    return 14 * distanceY + 10 * (distanceX - distanceY);
        //}
        //else if (distanceY >= distanceZ)
        //{
        //    return 14 * distanceZ + 10 * (distanceY - distanceZ);
        //}
        //else
        //{
        //    return 14 * distanceX + 10 * (distanceZ - distanceX);
        //}

        if (distanceX > distanceZ)
        {
            return 14 * distanceZ + 10 * (distanceX - distanceZ);
        }
        else
        {
            return 14 * distanceX + 10 * (distanceZ - distanceX);
        }
    }
}
