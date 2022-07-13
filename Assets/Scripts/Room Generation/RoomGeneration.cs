using System.Collections.Generic;
using UnityEngine;

public class RoomGeneration 
{
    private int numRooms;
    private CellGeneration world;

    public Room startRoom {get; private set;}
    public Vector2Int startRoomCell {get; private set;}

    private Dictionary<Vector2Int, Room> roomMap;
    public HashSet<Room> rooms {get; private set;}

    public RoomGeneration(int numRooms, CellGeneration world) {
        this.numRooms = numRooms;
        this.world = world;

        roomMap = new Dictionary<Vector2Int, Room>();
        rooms = new HashSet<Room>();
    }

    public void GenerateRooms() {
        InitializeRooms();
        while (rooms.Count > numRooms)
        {
            Room room = GenerationUtils.GetRandom(rooms);
            HashSet<Room> adjacentRooms = GetAdjacentRooms(room);
            if (adjacentRooms.Count > 0)
            {
                Room otherRoom = GenerationUtils.GetRandom(adjacentRooms);
                room.MergeWithRoom(roomMap, otherRoom);
                rooms.Remove(otherRoom);
            }
        }
    }

    private HashSet<Room> GetAdjacentRooms(Room room)
    {
        HashSet<Room> adjacent = new HashSet<Room>();
        foreach (Vector2Int v in room)
            foreach (Vector2Int adj in world.GetAdjacent(v))
                if (roomMap.ContainsKey(adj) && roomMap[adj] != room)
                    adjacent.Add(roomMap[adj]);
        return adjacent;
    }

    private void InitializeRooms()
    {
        CalculateStartRoom();
        HashSet<Vector2Int> cells = new HashSet<Vector2Int>(world.Cells);
        cells.Remove(startRoomCell);
        foreach (Vector2Int v in cells)
        {
            Room room = new Room();
            room.AddCell(v);
            roomMap[v] = room;
            rooms.Add(room);
        }
    }

    private void CalculateStartRoom()
    {
        startRoomCell = Vector2Int.zero;
        foreach (Vector2Int v in world.Cells)
            if (v.y == startRoomCell.y && v.x < startRoomCell.x)
                startRoomCell = v;
            else if (v.y > startRoomCell.y)
                startRoomCell = v;
       startRoom = new Room();
       startRoom.AddCell(startRoomCell);
    }
}