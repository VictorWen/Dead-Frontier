using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pair<T> 
{
    public T first {get; private set;}
    public T second {get; private set;}

    public Pair(T a, T b) {
        this.first = a;
        this.second = b;
    }

    public override bool Equals(object obj)
    {   
        if (obj == null || GetType() != obj.GetType())
            return false;
        Pair<T> other = (Pair<T>) obj;
        return (first.Equals(other.first) && second.Equals(other.second)) ||
            (first.Equals(other.second) && (second.Equals(other.first)));
    }
    
    // override object.GetHashCode
    public override int GetHashCode()
    {
        int firstHash = first == null ? 0 : first.GetHashCode();
        int secondHash = second == null ? 0 : second.GetHashCode();
        return firstHash ^ secondHash;
    }
}

public class RoomGenScript : MonoBehaviour
{
    [SerializeField] private int numCells = 25;
    [SerializeField] private int numRooms = 5;
    [SerializeField] private TileBase tile;

    private Tilemap tilemap;

    private HashSet<Vector2Int> open = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> closed = new HashSet<Vector2Int>();

    private Vector2Int startRoom;
    // TODO: refactor into new class
    private HashSet<HashSet<Vector2Int>> rooms = new HashSet<HashSet<Vector2Int>>();
    private Dictionary<Vector2Int, HashSet<Vector2Int>> roomIndices = new Dictionary<Vector2Int, HashSet<Vector2Int>>();
    private HashSet<Vector3Int> walls = new HashSet<Vector3Int>();
    private HashSet<Pair<Vector2Int>> hallways = new HashSet<Pair<Vector2Int>>();

    private void Start()
    {
        tilemap = GetComponent<Tilemap>();
        PlaceCells();
        PlaceRooms();
        
        BuildRooms(); // TODO: Maybe extract to another class
        BuildHallways();
    }

    private void BuildHallways() {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>() { startRoom };
        HashSet<Pair<HashSet<Vector2Int>>> visitedRooms = new HashSet<Pair<HashSet<Vector2Int>>>();
        queue.Enqueue(startRoom);
        roomIndices[startRoom] = null;
        while (queue.Count > 0) {
            Vector2Int v = queue.Dequeue();
            foreach (Vector2Int a in GetAdjacent(v)) {
                if (!visited.Contains(a) && roomIndices.ContainsKey(a)) {
                    Pair<HashSet<Vector2Int>> roomPair = new Pair<HashSet<Vector2Int>>(roomIndices[a], roomIndices[v]);
                    if (roomIndices[a] != roomIndices[v] && !visitedRooms.Contains(roomPair)) {
                        hallways.Add(new Pair<Vector2Int>(v, a));
                        visitedRooms.Add(roomPair);
                    }
                    queue.Enqueue(a);
                    visited.Add(a);
                }
            }
        }
        foreach (Pair<Vector2Int> hallway in hallways) {
            BuildHallway(hallway.first, hallway.second);
        }
    }

    private void BuildHallway(Vector2Int a, Vector2Int b) {
        Vector2Int c = (a + b) * 9;
        for (int i = 0; i < 9; i++) {
            for (int j = 0; j < 9; j++) {
                tilemap.SetTile(new Vector3Int(c.x + i, c.y + j, 0), tile);
            }
        }
    }

    private void BuildRooms()
    {
        BuildFloors();
        BuildWalls();
    }

    private void BuildWalls() {
        foreach (Vector3Int w in walls) {
            tilemap.SetTile(w, tile);
            tilemap.SetTileFlags(w, TileFlags.None);
            tilemap.SetColor(w, Color.gray);
        }
    }

    private void BuildFloors()
    {
        BuildRoomCellFloor(2 * startRoom, Color.black);
        foreach (HashSet<Vector2Int> room in rooms)
            BuildRoomFloor(room);
    }

    private void BuildRoomFloor(HashSet<Vector2Int> room)
    {
        Color color = GetRandomColor();
        foreach (Vector2Int v in room)
        {
            BuildRoomCellFloor(2 * v, color);
            FillRoomCellFloorGaps(room, v, color);
        }
    }

    private void FillRoomCellFloorGaps(HashSet<Vector2Int> room, Vector2Int cell, Color color) 
    {
        foreach (Vector2Int v in GetAdjacent(cell))
            if (room.Contains(v))
                BuildRoomCellFloor((v + cell) , color); // TODO: Do this better
    }

    private void BuildRoomCellFloor(Vector2Int cell, Color color)
    {
        foreach (Vector3Int pos in GetSquare(cell * 9, 9))
        {
            tilemap.SetTile(pos, tile);
            tilemap.SetTileFlags(pos, TileFlags.None);
            tilemap.SetColor(pos, color);
            foreach (Vector2Int v in GetAdjacentPlus(new Vector2Int(pos.x, pos.y))) {
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

    private void PlaceRooms()
    {
        InitializeRooms();
        Debug.Log(rooms.Count);
        while (rooms.Count > numRooms)
        {
            HashSet<Vector2Int> room = GetRandom(rooms);
            HashSet<HashSet<Vector2Int>> adjacentRooms = GetAdjacentRooms(room);
            if (adjacentRooms.Count > 0)
            {
                HashSet<Vector2Int> otherRoom = GetRandom(adjacentRooms);
                MergeRooms(room, otherRoom);
            }
            Debug.Log(rooms.Count);
        }
    }

    private void MergeRooms(HashSet<Vector2Int> roomA, HashSet<Vector2Int> roomB)
    {
        foreach (Vector2Int v in roomB)
        {
            roomA.Add(v);
            roomIndices[v] = roomA;
        }
        rooms.Remove(roomB);
    }

    private void InitializeRooms()
    {
        startRoom = GetStartRoom();
        tilemap.SetColor(new Vector3Int(startRoom.x, startRoom.y, 0), Color.black);
        closed.Remove(startRoom);
        foreach (Vector2Int v in closed)
        {
            HashSet<Vector2Int> room = new HashSet<Vector2Int>() { v };
            roomIndices[v] = room;
            rooms.Add(room);
        }
    }

    private Vector2Int GetStartRoom()
    {
        Vector2Int startRoom = Vector2Int.zero;
        foreach (Vector2Int v in closed)
            if (v.y == startRoom.y && v.x < startRoom.x)
                startRoom = v;
            else if (v.y > startRoom.y)
                startRoom = v;
        return startRoom;
    }

    private void PlaceCells()
    {
        open.Add(Vector2Int.zero);
        for (int i = 0; i < numCells; i++)
        {
            Vector2Int nextCell = GetRandom(open);

            closed.Add(nextCell);
            open.Remove(nextCell);
            OpenAdjacentCells(nextCell);
        }
    }

    private Vector2Int GetRandomOpenCell()
    {
        int id = Random.Range(0, open.Count - 1);
        int count = 0;
        foreach (Vector2Int v in open)
        {
            if (count == id)
                return v;
            count++;
        }
        throw new UnityException("This should never happen");
    }

    private void OpenAdjacentCells(Vector2Int cell)
    {
        foreach (Vector2Int v in GetAdjacent(cell))
            if (!closed.Contains(v))
                open.Add(v);
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

    private HashSet<HashSet<Vector2Int>> GetAdjacentRooms(HashSet<Vector2Int> room)
    {
        HashSet<HashSet<Vector2Int>> adjacent = new HashSet<HashSet<Vector2Int>>();
        foreach (Vector2Int v in room)
            foreach (Vector2Int adj in GetAdjacent(v))
                if (roomIndices.ContainsKey(adj) && roomIndices[adj] != room)
                    adjacent.Add(roomIndices[adj]);
        return adjacent;
    }

    private T GetRandom<T>(HashSet<T> set)
    {
        int id = Random.Range(0, set.Count - 1);
        int count = 0;
        foreach (T v in set)
        {
            if (count == id)
                return v;
            count++;
        }
        Debug.Log(set.Count + " " + id + " " + count);
        throw new UnityException("This should never happen");
    }

    private Color GetRandomColor()
    {
        float r = (float)Random.Range(0, numCells) / numCells;
        float g = (float)Random.Range(0, numCells) / numCells;
        float b = (float)Random.Range(0, numCells) / numCells;

        return new Color(r, g, b);
    }
}
