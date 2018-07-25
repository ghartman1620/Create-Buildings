using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// This process is based on this presentation by a Path of Exile developer about 
// their level generation process: https://www.youtube.com/watch?v=GcM9Ynfzll0

public class CreateRooms : MonoBehaviour {
    public GameObject Floor;
    public GameObject Door;
    public Transform player;
    public int TileWidth;
    public int Size;
    public int Connections;
    public int MaxWeight;
   

    private Room[,] rooms;
    private List<GameObject> doors;
    private List<GameObject> floors;

	// Use this for initialization
	void Start () {
        Assert.IsTrue((Size * Size) - 1 > Connections);
        Assert.IsTrue(Connections >= 0);
        if(Size * Size - 1 > Connections && Connections >= 0)
        {
            this.doors = new List<GameObject>();
            this.floors = new List<GameObject>();
            rooms = new Room[Size, Size];
            int z = 0;
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    rooms[i, j] = new Room();
                    rooms[i, j].Id = z;
                    z++;
                }
            }
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    if (i - 1 >= 0) rooms[i, j].Adj.Add(rooms[i - 1, j]);
                    if (i + 1 < Size) rooms[i, j].Adj.Add(rooms[i + 1, j]);
                    if (j - 1 >= 0) rooms[i, j].Adj.Add(rooms[i, j - 1]);
                    if (j + 1 < Size) rooms[i, j].Adj.Add(rooms[i, j + 1]);

                    rooms[i, j].Space.Add(new Vector2(i, j));
                }
            }
            for (int i = 0; i < Connections; i++)
            {
                MergeRoom(rooms);
            }


            // Want a set of rooms so I can regardless of size have an 
            // equal chance of picking each.
            HashSet<Room> hs = new HashSet<Room>();
            for(int i = 0; i < Size; i++)
            {
                for(int j = 0; j < Size; j++)
                {
                    hs.Add(rooms[i, j]);
                }
            }
            Room[] uniqueRoomsArray = new Room[hs.Count];
            hs.CopyTo(uniqueRoomsArray);
            Room begin;
            Room end;
            do
            {
                begin = rooms[0, Random.Range(0, rooms.GetLength(1))];
                end = rooms[rooms.GetLength(0) - 1, Random.Range(0, rooms.GetLength(1))];
            } while (begin.Equals(end));

            Debug.Log("Begin is " + begin.Id);
            Debug.Log("End is " + end.Id);

            foreach(Room r in uniqueRoomsArray)
            {
                r.Weight = Random.Range(1, MaxWeight);
            }
            //DrawRooms(rooms, 0, 0, 10, Floor, Door);
            PrintRooms(rooms);
            Dijkstra(hs, begin);
            PrintRooms(rooms);
            //DrawRooms(rooms, 0, 0, 10, Floor, Door);


            Debug.Log("End to begin path is: ");
            HashSet<Room> path = new HashSet<Room>();
            for(Room r = end; r != begin; r = r.Parent)
            {
                Debug.Log(r.Id);
                Assert.IsNotNull(r.Parent);
                path.Add(r);
            }
            path.Add(begin);
            Debug.Log(begin.Id);
            DrawRooms(rooms, 0, 0, TileWidth, Floor, Door, path);

            //this is a bit silly - i'm not actually sure how to get just one thing from a hashset.
            // need to learn a bit more about c# iterators...
            foreach (Vector2 spot in begin.Space)
            {
                player.transform.Translate(new Vector3(spot.x * TileWidth, 0, spot.y * TileWidth));
                break;
            }

        }

        




    }

	
	// Update is called once per frame
	void Update () {
        //if (Input.GetButtonDown("Fire1"))
        //{
        //    foreach(GameObject t in doors)
        //    {
        //        Destroy(t);
        //    }
        //    mergeRoom(this.rooms);
        //    createRooms(this.rooms, 0, 0, 10, Floor, Door);
        //}
    }


    private void Dijkstra(HashSet<Room> rooms, Room source)
    {

        
        List<Room> unvisited = new List<Room>();
        foreach (Room r in rooms)
        {
            unvisited.Add(r);
            r.Distance = Room.INF;
            r.Visited = false;
        }
        
        source.Distance = 0;
        Room curr = source;
        IComparer<Room> comp = new ByDistance();
        unvisited.Sort(comp);

        Debug.Log("Printing unvisited");
        foreach (Room r in unvisited)
        {
            Debug.Log("Room " + r.Id + " at distance " + r.Distance);
        }
        while (!(unvisited.Count == 0))
        {
            
            Debug.Log("Visiting room " + curr.Id);
            foreach(Room visit in curr.Adj)
            {
                if(!visit.Visited && (visit.Distance == Room.INF || visit.Distance > curr.Distance + curr.Weight))
                {
                    Debug.Log("Relaxing distance from curr to " + visit.Id);
                    
                    visit.Distance = curr.Distance + curr.Weight;
                    visit.Parent = curr;

                    // So this bit of code makes this not really dijkstra, because dijkstra runs in VlogV.
                    // I have to use an O(n) operation to decrease the distance key more or less
                    // because the C# standard library is deficient any class implemented using a heap
                    // with an API appropriate for use as a priority queue
                    // And I was having issues importing a library called PowerCollections into unity.
                    // So this runs in O(V^2).

                    // This is what would be the operation DecreaseKey() in a proper MinHeap.

                    // So now I'm sad and Edsger Dijkstra hates UCSC a little bit more.

                    //remove the now unsorted visit from the list
                    unvisited.Remove(visit);

                    // insert it back into the unvisited list, now sorted
                    int i;
                    for (i = 0; i < unvisited.Count; i++)
                    {
                        if (comp.Compare(unvisited[i], visit) > 0)
                        {
                            unvisited.Insert(i, visit);
                            break;
                        }
                    }
                    // visit is the largest distance thing in unvisited
                    if(i == unvisited.Count)
                    {
                        unvisited.Add(visit);
                    }
                    Debug.Log("Printing unvisited");
                    foreach (Room r in unvisited)
                    {
                        Debug.Log("Room " + r.Id + " at distance " + r.Distance);
                    }

                }
            }
            foreach (Room r in unvisited)
            {
                Debug.Log("Room " + r.Id + " at distance " + r.Distance);
            }
            curr.Visited = true;
            unvisited.Remove(curr);
            if(unvisited.Count > 0)
            {
                curr = unvisited[0];
            }
            
        }

        Debug.Log("dijkstra finished");

    }

    // Creates the associated prefabs with this room. 
    // Creates a floor of the given Transform for each coordinate in this Room's space
    // list at position baseX + x*size
    // Also creates a door at every position appopriate to create a connection between adjacent
    // rooms.
    private void DrawRooms(Room[,] rooms, float baseX, float baseZ, float size, GameObject floor, GameObject door)
    {
        foreach (GameObject d in doors)
        {
            Destroy(d);
        }
        foreach (GameObject f in floors)
        {
            Destroy(f);
        }
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                
                CreateFloors(rooms[i, j], floor, baseX, baseZ, size);

                foreach (Room r in rooms[i,j].Adj)
                {
                    if(rooms[i,j] != null)
                    {
                        CreateDoor(rooms[i, j], r, door, baseX, baseZ, size);
                    }
                }
            }
        }
    }

    // Works as above, but only creates rooms in the included HashSet
    private void DrawRooms(Room[,] rooms, float baseX, float baseZ, float size, GameObject floor, GameObject door, HashSet<Room> inclusions)
    {
        foreach (GameObject d in doors)
        {
            Destroy(d);
        }
        foreach (GameObject f in floors)
        {
            Destroy(f);
        }
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                if(inclusions.Contains(rooms[i,j]))
                {
                    CreateFloors(rooms[i, j], floor, baseX, baseZ, size);
                    foreach (Room r in rooms[i, j].Adj)
                    {
                        if (rooms[i, j] != null && inclusions.Contains(r))
                        {
                            CreateDoor(rooms[i, j], r, door, baseX, baseZ, size);
                        }
                    }
                }
            }
        }
    }

    private void MergeRoom(Room[,] rooms)
    {
        int randX = Random.Range(0, Size);
        int randY = Random.Range(0, Size);
        Room r = rooms[randX, randY];
        // Ensure there actually are some rooms to merge into this one
        if (r.Adj.Count > 0)
        {
            Room[] roomArray = new Room[r.Adj.Count];
            r.Adj.CopyTo(roomArray);
            Room rand = roomArray[Random.Range(0, r.Adj.Count)];
            
            foreach (Vector2 pt in rand.Space)
            {
                r.Space.Add(pt);
                rooms[(int)pt.x, (int)pt.y] = r;

            }
            foreach (Room pointsToRemovedRoom in rand.Adj)
            {
                if(pointsToRemovedRoom != r)
                {
                    
                   
                    pointsToRemovedRoom.Adj.Remove(rand);
                    pointsToRemovedRoom.Adj.Add(r);
                    r.Adj.Add(pointsToRemovedRoom);
                }
            }
            r.Adj.Remove(rand);
            int removedId = rand.Id;
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    Assert.AreNotEqual(removedId, rooms[i, j].Id);
                    foreach(Room room in rooms[i,j].Adj)
                    {
                        if(removedId == room.Id)
                        {   
                            Assert.AreNotEqual(removedId, room.Id);

                        }
                    }
                }
            }
        }
        
    }
    private static void PrintRooms(Room[,] r)
    {
        for(int i = 0; i < r.GetLength(0); i++)
        {
            for(int j = 0; j < r.GetLength(1); j++)
            {
                string s = "";
                foreach(Room room in r[i,j].Adj)
                {
                    s += room.Id + ", ";
                }
                Debug.Log(i + ", " + j + ", id " + r[i,j].Id + " is adjacent to " + s);
                Debug.Log(i + ", " + j + ", id " + r[i,j].Id + " has parent " + (r[i, j].Parent == null ? "null" : r[i,j].Parent.Id.ToString()) + " and distance " + r[i, j].Distance);
            }
        }
    }



    private void CreateFloors(Room r, GameObject floor, float baseX, float baseZ, float size)
    {
        foreach (Vector2 point in r.Space)
        {
            float x = point.x * size + baseX;
            float y = 0;
            float z = point.y * size + baseZ;
            Instantiate(floor, new Vector3(x, y, z), Quaternion.identity);
            floors.Add(Instantiate(floor, new Vector3(x, y, z), Quaternion.identity));
        }
    }
    private void CreateDoor(Room r1, Room r2, GameObject door, float baseX, float baseZ, float size)
    {
        foreach(Vector2 pt1 in r1.Space)
        {
            foreach(Vector2 pt2 in r2.Space)
            {
                // Look for any pair of points in these rooms space that 
                // is adjacent. Make a door there.

                // The points are adjacent veritically (same horizontal position)
                if(pt1.x == pt2.x && Mathf.Abs(pt1.y-pt2.y) == 1)
                {
                    Vector2 absPoint1 = new Vector2(baseX + pt1.x * size, baseZ + pt1.y * size);
                    Vector2 absPoint2 = new Vector2(baseX + pt2.x * size, baseZ + pt2.y * size);
                    Vector3 doorPoint = new Vector2((absPoint1.x + absPoint2.x) / 2, (absPoint1.y + absPoint2.y) / 2);
                    doors.Add(Instantiate(door.GetComponent<Transform>(), new Vector3(doorPoint.x, 0, doorPoint.y), Quaternion.identity).gameObject);
                }
                else if (pt1.y == pt2.y && Mathf.Abs(pt1.x-pt2.x) == 1)
                {
                    Vector2 absPoint1 = new Vector2(baseX + pt1.x * size, baseZ + pt1.y * size);
                    Vector2 absPoint2 = new Vector2(baseX + pt2.x * size, baseZ + pt2.y * size);
                    Vector3 doorPoint = new Vector2((absPoint1.x + absPoint2.x) / 2, (absPoint1.y + absPoint2.y) / 2);
                    doors.Add(Instantiate(door.GetComponent<Transform>(), new Vector3(doorPoint.x, 0, doorPoint.y), Quaternion.AngleAxis(90, new Vector3(0, 1, 0))).gameObject);

                }
            }
        }
    }
}
