using UnityEngine;

/// <summary>
///     [What does this Node do]
/// </summary>
[System.Serializable]
public sealed class Node
{
    public Vector3 WorldPosition { get; set; }
    public bool IsWalkable { get; set; }

    public int gCost;
    public int hCost;
    public int fCost
    {
        get 
        { 
            return gCost + hCost; 
        }
    }

    public Node parent;

    public Vector3Int GridPosition { get; private set; }

    public Node(Vector3 worldPosition, bool isWalkable, Vector3Int gridPosition)
    {
        WorldPosition = worldPosition;
        IsWalkable = isWalkable;
        GridPosition = gridPosition;
    }
}
