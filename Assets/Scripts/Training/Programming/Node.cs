using UnityEngine;

/// <summary>
///     [What does this Node do]
/// </summary>
public sealed class Node
{
    public Vector3 WorldPosition { get; set; }
    public bool IsWalkable { get; set; }

    public int GCost { get; set; }
    public int HCost { get; set; }
    public int FCost
    {
        get 
        { 
            return GCost + HCost; 
        }
    }

    public Vector3Int GridPosition { get; private set; }

    public Node(Vector3 worldPosition, bool isWalkable, Vector3Int gridPosition)
    {
        WorldPosition = worldPosition;
        IsWalkable = isWalkable;
        GridPosition = gridPosition;
    }
}
