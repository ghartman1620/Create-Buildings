using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A room in our building. 
// the building is made up of an nxn grid, with rooms filling some of this space. Buildings will be 
// irregularly shaped and can fill some or all of the grid.

// Each room shall know what other rooms it is connected to by a passageway. This shall be known as the room's
// adjacency list, not to be confused with the list of rooms that border a particular room, which is not 
// stored in any fashion but must be found by iterating over the room list that constructs a building.

// Each room shall also know what space on the nxn grid it occupies. This is a list of integer x, y pairs.

public class Room
{
    public static readonly float INF = -1;
    
    // The list of rooms connceted to this room by a passageway.
    private HashSet<Room> adj;

    // The space this room occupies as a list of coordinates in the building nxn grid.
    private HashSet<Vector2> space;

    private int id;
    private float weight;
    
    // For use in CreateRooms.Dijkstra
    private float distance;
    private bool visited;
    private Room parent;

    private bool onMainPath;

    public Room()
    {
        adj = new HashSet<Room>();
        space = new HashSet<Vector2>();
        parent = null;
        visited = false;
        distance = INF;
    }
    public HashSet<Room> Adj
    {
        get { return adj; }
    }
    public HashSet<Vector2> Space
    {
        get { return space; }
    }
    public int Id
    {
        get { return id; }
        set { id = value; }
    }
    public float Weight
    {
        get { return weight; }
        set { weight = value; }
    }
    public float Distance
    {
        get { return distance; }
        set { distance = value; }
    }
    public bool Visited
    {
        get { return visited;  }
        set { visited = value; }
    }
    public Room Parent
    {
        get { return parent; }
        set { parent = value; }
    }
    public bool OnMainPath
    {
        get { return onMainPath; }
        set { onMainPath = value; }
    }
    public string toString()
    {
        string s = "";
        foreach(Vector2 point in space)
        {
            s = s + ("(" + point.x + "," + point.y + ")");
        }
        return "Room occupying points at " + s;
    }
    public override bool Equals(System.Object other)
    {
        if(other.GetType() == typeof(Room))
        {
            return ((Room)other).id == this.id;
        }
        return false;
    }
    public override int GetHashCode() 
    {
        return this.id;
    }
}
