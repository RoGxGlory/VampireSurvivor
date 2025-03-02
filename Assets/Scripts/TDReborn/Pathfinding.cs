using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public GridManager gridManager;

    public List<GridCell> GetPath()
    {
        List<GridCell> path = new List<GridCell>();

        if (gridManager.startCell == null || gridManager.endCell == null)
        {
            Debug.LogError("Start or End Cell not assigned!");
            return path;
        }

        GridCell current = gridManager.startCell;
        path.Add(current);

        while (current != gridManager.endCell)
        {
            GridCell next = GetNextPathCell(current, path);
            if (next == null)
            {
                Debug.LogError("Path is broken! No valid path to end.");
                break;
            }

            path.Add(next);
            current = next;
        }

        return path;
    }

    private GridCell GetNextPathCell(GridCell current, List<GridCell> path)
    {
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (var dir in directions)
        {
            int newX = current.gridX + dir.x;
            int newY = current.gridY + dir.y;

            if (gridManager.IsInBounds(newX, newY))
            {
                GridCell neighbor = gridManager.GetCell(newX, newY);
                if (neighbor != null && neighbor.isPath && !path.Contains(neighbor))
                {
                    return neighbor;
                }
            }
        }

        return null; // No valid next cell found
    }
}