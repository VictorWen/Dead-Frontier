using System.Collections.Generic;
using UnityEngine;

public class Room 
{
    private HashSet<Vector2Int> cells;

    public Room() {
        cells = new HashSet<Vector2Int>();
    }

    public void AddCell(Vector2Int cell) {
        cells.Add(cell);
    }

    public void MergeWithRoom(Dictionary<Vector2Int, Room> roomMap, Room other) {
        foreach (Vector2Int v in other.cells) {
            cells.Add(v);
            roomMap[v] = this;
        }
        other.cells.Clear();
    }

    public IEnumerator<Vector2Int> GetEnumerator() {
        return cells.GetEnumerator();
    }

    public bool Contains(Vector2Int v) {
        return cells.Contains(v);
    }
}