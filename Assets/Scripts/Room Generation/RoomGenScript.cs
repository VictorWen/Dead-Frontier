using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomGenScript : MonoBehaviour
{
    [SerializeField] private int numCells = 25;
    [SerializeField] private int numRooms = 5;
    [SerializeField] private TileBase tile;
    private Tilemap tilemap;


    private HashSet<Vector3Int> walls = new HashSet<Vector3Int>();
    private HashSet<Pair<Vector2Int>> hallways = new HashSet<Pair<Vector2Int>>();

    private CellGeneration cellGen;
    private RoomGeneration roomGen;

    private void Start()
    {
        tilemap = GetComponent<Tilemap>();
        
        cellGen = new CellGeneration(numCells);
        cellGen.GenerateCells();

        roomGen = new RoomGeneration(numRooms, cellGen);
        roomGen.GenerateRooms();

        BuildRooms(); // TODO: Maybe extract to another class
        //BuildHallways();
    }

    // private void BuildHallways()
    // {
    //     Queue<Vector2Int> queue = new Queue<Vector2Int>();
    //     HashSet<Vector2Int> visited = new HashSet<Vector2Int>() { startRoom };
    //     HashSet<Pair<HashSet<Vector2Int>>> visitedRooms = new HashSet<Pair<HashSet<Vector2Int>>>();
    //     queue.Enqueue(startRoom);
    //     roomIndices[startRoom] = null;
    //     while (queue.Count > 0)
    //     {
    //         Vector2Int v = queue.Dequeue();
    //         foreach (Vector2Int a in GetAdjacent(v))
    //         {
    //             if (!visited.Contains(a) && roomIndices.ContainsKey(a))
    //             {
    //                 Pair<HashSet<Vector2Int>> roomPair = new Pair<HashSet<Vector2Int>>(roomIndices[a], roomIndices[v]);
    //                 if (roomIndices[a] != roomIndices[v] && !visitedRooms.Contains(roomPair))
    //                 {
    //                     hallways.Add(new Pair<Vector2Int>(v, a));
    //                     visitedRooms.Add(roomPair);
    //                 }
    //                 queue.Enqueue(a);
    //                 visited.Add(a);
    //             }
    //         }
    //     }
    //     foreach (Pair<Vector2Int> hallway in hallways)
    //     {
    //         BuildHallway(hallway.first, hallway.second);
    //     }
    // }

    private void BuildHallway(Vector2Int a, Vector2Int b)
    {
        Vector2Int c = (a + b) * 9;
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                tilemap.SetTile(new Vector3Int(c.x + i, c.y + j, 0), tile);
            }
        }
    }

    private void BuildRooms()
    {
        BuildFloors();
        BuildWalls();
    }

    private void BuildFloors()
    {
        Vector2Int startRoom = roomGen.startRoomCell;
        BuildRoomCellFloor(2 * startRoom, Color.black);
        foreach (Room room in roomGen.rooms)
            BuildRoomFloor(room);
    }

    private void BuildWalls()
    {
        foreach (Vector3Int w in walls)
        {
            tilemap.SetTile(w, tile);
            tilemap.SetTileFlags(w, TileFlags.None);
            tilemap.SetColor(w, Color.gray);
        }
    }

    private void BuildRoomFloor(Room room)
    {
        Color color = GetRandomColor();
        foreach (Vector2Int v in room)
        {
            BuildRoomCellFloor(2 * v, color);
            FillRoomCellFloorGaps(room, v, color);
        }
    }

    private void FillRoomCellFloorGaps(Room room, Vector2Int cell, Color color)
    {
        foreach (Vector2Int v in GetAdjacent(cell))
            if (room.Contains(v))
                BuildRoomCellFloor((v + cell), color); // TODO: Do this better
    }

    private void BuildRoomCellFloor(Vector2Int cell, Color color)
    {
        foreach (Vector3Int pos in GetSquare(cell * 9, 9))
        {
            tilemap.SetTile(pos, tile);
            tilemap.SetTileFlags(pos, TileFlags.None);
            tilemap.SetColor(pos, color);
            foreach (Vector2Int v in GetAdjacentPlus(new Vector2Int(pos.x, pos.y)))
            {
                Vector3Int wallPos = new Vector3Int(v.x, v.y, 0);
                if (tilemap.GetTile(wallPos) == null)
                    walls.Add(wallPos);
            }
            walls.Remove(pos);
        }
    }

    private IEnumerable<Vector3Int> GetSquare(Vector2Int topLeft, int size)
    {
        Vector3Int tl = new Vector3Int(topLeft.x, topLeft.y, 0);
        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
                yield return tl + new Vector3Int(i, j, 0);
        yield break;
    }

    private List<Vector2Int> GetAdjacent(Vector2Int cell)
    {
        List<Vector2Int> adjacent = new List<Vector2Int>();
        adjacent.Add(cell + new Vector2Int(1, 0));
        adjacent.Add(cell + new Vector2Int(0, 1));
        adjacent.Add(cell - new Vector2Int(1, 0));
        adjacent.Add(cell - new Vector2Int(0, 1));
        return adjacent;
    }

    private List<Vector2Int> GetAdjacentPlus(Vector2Int cell)
    {
        List<Vector2Int> adjacent = new List<Vector2Int>();
        for (int i = -1; i <= 1; i++)
            for (int j = -1; j <= 1; j++)
                if (i != 0 || j != 0)
                    adjacent.Add(new Vector2Int(i, j) + cell);
        return adjacent;
    }

    private Color GetRandomColor()
    {
        float r = (float)Random.Range(0, numCells) / numCells;
        float g = (float)Random.Range(0, numCells) / numCells;
        float b = (float)Random.Range(0, numCells) / numCells;

        return new Color(r, g, b);
    }
}
