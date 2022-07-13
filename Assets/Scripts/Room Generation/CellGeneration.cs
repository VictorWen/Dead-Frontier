using System.Collections.Generic;
using UnityEngine;

public class CellGeneration
{
    public HashSet<Vector2Int> Cells { get { return closed; } }

    private int numCells;

    private HashSet<Vector2Int> open;
    private HashSet<Vector2Int> closed;

    public CellGeneration(int numCells) {
        this.numCells = numCells;
        open = new HashSet<Vector2Int>();
        closed = new HashSet<Vector2Int>();
    }

    public void GenerateCells() {
        open.Clear();
        closed.Clear();

        open.Add(Vector2Int.zero);
        for (int i = 0; i < numCells; i++)
        {
            Vector2Int nextCell = GenerationUtils.GetRandom(open);

            closed.Add(nextCell);
            open.Remove(nextCell);
            OpenAdjacentCells(nextCell);
        }
    }

    private void OpenAdjacentCells(Vector2Int cell)
    {
        foreach (Vector2Int v in GetAdjacent(cell))
            if (!closed.Contains(v))
                open.Add(v);
    }

    public List<Vector2Int> GetAdjacent(Vector2Int cell)
    {
        List<Vector2Int> adjacent = new List<Vector2Int>();
        adjacent.Add(cell + new Vector2Int(1, 0));
        adjacent.Add(cell + new Vector2Int(0, 1));
        adjacent.Add(cell - new Vector2Int(1, 0));
        adjacent.Add(cell - new Vector2Int(0, 1));
        return adjacent;
    }
}