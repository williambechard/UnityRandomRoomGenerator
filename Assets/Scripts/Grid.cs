using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Grid : MonoBehaviour
{

    public Room RoomPrefab;
    public List<Room> AllRooms = new();
    public List<Vector2> PossibleRooms = new();
    public List<Vector2> PossibleRoomsTop = new();
    public List<Vector2> PossibleRoomsBottom = new();
    public List<Vector2> PossibleRoomsLeft = new();
    public List<Vector2> PossibleRoomsRight = new();
    public List<Room> openDoors = new();


    public float timeToWait;
    public int targetNumberOfRooms;

    // or do some other code looking at all the paths

    [SerializeField]
    int directionWalkLimit;

    [SerializeField]
    private float roomWidth;
    [SerializeField]
    private float roomHeight;
    // Start is called before the first frame update
    void Start()
    {
        //determine roomWidth and height based on the prefab
        Room room = Instantiate<Room>(RoomPrefab);
        roomWidth = room.floor.transform.localScale.x;
        roomHeight = room.floor.transform.localScale.y;

        Destroy(room.gameObject, 0);

        StartCoroutine(BuildRandomMap());
        //StartCoroutine(BuildOrderlyMap());
    }

    /*
    IEnumerator BuildOrderlyMap()
    {
        //create initial room 
        spawnRoom(new Vector2(0, 0));

        int directionWalk = (int)UnityEngine.Random.Range(0, 4);
        int directionWalkTimer = 0;

        //and initial layout
        while (AllRooms.Count < targetNumberOfRooms && PossibleRooms.Count > 0)
        {
            if (directionWalkTimer >= directionWalkLimit)
            {
                directionWalkTimer = 0;
                directionWalk = (int)UnityEngine.Random.Range(0, 4);
            }

            Vector2 newRoomPosition = new Vector2(0, 0);
            int index;
            //determine which direction based on the direction that is the
            switch (directionWalk)
            {
                case 0:
                    index = UnityEngine.Random.Range(0, PossibleRoomsTop.Count - 1);
                    newRoomPosition = PossibleRoomsTop[index];
                    PossibleRoomsTop.RemoveAt(index);
                    break;
                case 1:
                    index = UnityEngine.Random.Range(0, PossibleRoomsBottom.Count - 1);
                    newRoomPosition = PossibleRoomsBottom[index];
                    PossibleRoomsBottom.RemoveAt(index);
                    break;
                case 2:
                    index = UnityEngine.Random.Range(0, PossibleRoomsLeft.Count - 1);
                    newRoomPosition = PossibleRoomsLeft[index];
                    PossibleRoomsLeft.RemoveAt(index);
                    break;
                case 3:
                    index = UnityEngine.Random.Range(0, PossibleRoomsRight.Count - 1);
                    newRoomPosition = PossibleRoomsRight[index];
                    PossibleRoomsRight.RemoveAt(index);
                    break;
            }

            directionWalkTimer++;

            spawnRoom(newRoomPosition);

            yield return new WaitForSeconds(timeToWait);
        }

        closeOffDoors();
    }
    */

    IEnumerator BuildRandomMap()
    {
        //create initial room 
        spawnRoom(new Vector2(0, 0));
        //and initial layout
        while (AllRooms.Count < targetNumberOfRooms)
        {
            if (PossibleRooms.Count > 0)
            {
                int index = Random.Range(0, PossibleRooms.Count);
                Vector2 newRoomPosition = PossibleRooms[index];
                PossibleRooms.RemoveAt(index);
                spawnRoom(newRoomPosition);

                yield return new WaitForSeconds(timeToWait);
            }
            else
            {
                Debug.LogError("Possible rooms ran out");
            }
        }

        closeOffDoors();
        closeExtra();
    }

    bool attemptCloseDoor(Room room, Vector2 targetDirection, GameObject door)
    {
        int targetIndex;
        targetIndex = AllRooms.FindIndex(i => i.x == targetDirection.x && i.y == targetDirection.y);
        if (targetIndex < 0)
        {
            door.SetActive(true);
            room.openDoors.Remove(door);
            return false;
        }
        else return true;
    }

    void closeOffDoors()
    {
        //now close off all doors that do not load to anything
        foreach (Room room in AllRooms)
        {
            Vector2 topDirection = new Vector2(room.x, room.y) + new Vector2(0, +1);
            Vector2 bottomDirection = new Vector2(room.x, room.y) + new Vector2(0, -1);
            Vector2 leftDirection = new Vector2(room.x, room.y) + new Vector2(-1, 0);
            Vector2 rightDirection = new Vector2(room.x, room.y) + new Vector2(1, 0);

            int openings = 0;

            if (attemptCloseDoor(room, topDirection, room.topDoor)) openings++;
            if (attemptCloseDoor(room, bottomDirection, room.bottomDoor)) openings++;
            if (attemptCloseDoor(room, leftDirection, room.leftDoor)) openings++;
            if (attemptCloseDoor(room, rightDirection, room.rightDoor)) openings++;

            if (openings >= 3) openDoors.Add(room);

        }
    }


    bool DFS2(Room targetRoom, GameObject doorToClose)
    {

        Stack<Room> currentPath = new Stack<Room>();
        GameObject otherDoor = null;
        Room otherRoom = null;
        Room r = targetRoom;

        currentPath.Push(r);

        //r.SetState(NodeState.Start);
        bool found = false;

        int escape = 0;
        //close door
        targetRoom.openDoors.Remove(doorToClose);
        doorToClose.SetActive(true);

        Vector2 topDirection = new Vector2(targetRoom.x, targetRoom.y) + new Vector2(0, 1);
        Vector2 bottomDirection = new Vector2(targetRoom.x, targetRoom.y) + new Vector2(0, -1);
        Vector2 leftDirection = new Vector2(targetRoom.x, targetRoom.y) + new Vector2(-1, 0);
        Vector2 rightDirection = new Vector2(targetRoom.x, targetRoom.y) + new Vector2(1, 0);

        int index = 0;
        switch (doorToClose.name)
        {
            case "Top":
                index = AllRooms.FindIndex(i => i.x == topDirection.x && i.y == topDirection.y);
                if (index >= 0)
                {
                    AllRooms[index].openDoors.Remove(AllRooms[index].bottomDoor);
                    otherDoor = AllRooms[index].bottomDoor;
                    otherRoom = AllRooms[index];
                    AllRooms[index].bottomDoor.SetActive(true);
                }
                break;
            case "Bottom":
                index = AllRooms.FindIndex(i => i.x == bottomDirection.x && i.y == bottomDirection.y);
                if (index >= 0)
                {
                    AllRooms[index].openDoors.Remove(AllRooms[index].topDoor);
                    otherDoor = AllRooms[index].topDoor;
                    otherRoom = AllRooms[index];
                    AllRooms[index].topDoor.SetActive(true);
                }
                break;
            case "Left":
                index = AllRooms.FindIndex(i => i.x == leftDirection.x && i.y == leftDirection.y);
                if (index >= 0)
                {
                    AllRooms[index].openDoors.Remove(AllRooms[index].rightDoor);
                    otherDoor = AllRooms[index].rightDoor;
                    otherRoom = AllRooms[index];
                    AllRooms[index].rightDoor.SetActive(true);
                }
                break;
            case "Right":
                index = AllRooms.FindIndex(i => i.x == rightDirection.x && i.y == rightDirection.y);
                if (index >= 0)
                {
                    AllRooms[index].openDoors.Remove(AllRooms[index].leftDoor);
                    otherDoor = AllRooms[index].leftDoor;
                    otherRoom = AllRooms[index];
                    AllRooms[index].leftDoor.SetActive(true);
                }
                break;
        }

        int NumberOfRooms = 0;
        int tIndex = -1;
        List<Room> DiscoveredRoom = new List<Room>();
        DiscoveredRoom.Add(targetRoom);
        while (currentPath.Count > 0)
        {
            escape++;
            if (escape >= 1000) break;

            Room tRoom = currentPath.Pop();


            Vector2 tDirection = new Vector2(tRoom.x, tRoom.y) + new Vector2(0, 1);
            Vector2 bDirection = new Vector2(tRoom.x, tRoom.y) + new Vector2(0, -1);
            Vector2 lDirection = new Vector2(tRoom.x, tRoom.y) + new Vector2(-1, 0);
            Vector2 rDirection = new Vector2(tRoom.x, tRoom.y) + new Vector2(1, 0);



            tIndex = AllRooms.FindIndex(i => i.x == tDirection.x && i.y == tDirection.y);
            if (tIndex >= 0 && tRoom.openDoors.Contains(tRoom.topDoor) && !DiscoveredRoom.Contains(AllRooms[tIndex]))
            {
                DiscoveredRoom.Add(AllRooms[tIndex]);
                currentPath.Push(AllRooms[tIndex].GetComponent<Room>());

            }

            tIndex = AllRooms.FindIndex(i => i.x == bDirection.x && i.y == bDirection.y);
            if (tIndex >= 0 && tRoom.openDoors.Contains(tRoom.bottomDoor) && !DiscoveredRoom.Contains(AllRooms[tIndex]))
            {
                DiscoveredRoom.Add(AllRooms[tIndex]);
                currentPath.Push(AllRooms[tIndex].GetComponent<Room>());

            }


            tIndex = AllRooms.FindIndex(i => i.x == rDirection.x && i.y == rDirection.y);
            if (tIndex >= 0 && tRoom.openDoors.Contains(tRoom.rightDoor) && !DiscoveredRoom.Contains(AllRooms[tIndex]))
            {
                DiscoveredRoom.Add(AllRooms[tIndex]);
                currentPath.Push(AllRooms[tIndex].GetComponent<Room>());

            }


            tIndex = AllRooms.FindIndex(i => i.x == lDirection.x && i.y == lDirection.y);
            if (tIndex >= 0 && tRoom.openDoors.Contains(tRoom.leftDoor) && !DiscoveredRoom.Contains(AllRooms[tIndex]))
            {
                DiscoveredRoom.Add(AllRooms[tIndex]);
                currentPath.Push(AllRooms[tIndex].GetComponent<Room>());

            }

        }


        if (DiscoveredRoom.Count == AllRooms.Count) found = true;

        if (!found)
        {
            //restore
            otherDoor.SetActive(false);
            otherRoom.openDoors.Add(otherDoor);
            targetRoom.openDoors.Add(doorToClose);
            doorToClose.SetActive(false);
        }

        return found;
    }



    void closeExtra()
    {
        while (openDoors.Count > 0)
        {

            Room room = openDoors[(int)Random.Range(0, openDoors.Count)]; //(int)Random.Range(0, openDoors.Count)

            Vector2 topDirection = new Vector2(room.x, room.y) + new Vector2(0, 1);
            Vector2 bottomDirection = new Vector2(room.x, room.y) + new Vector2(0, -1);
            Vector2 leftDirection = new Vector2(room.x, room.y) + new Vector2(-1, 0);
            Vector2 rightDirection = new Vector2(room.x, room.y) + new Vector2(1, 0);
            int index = 0;
            //determine what doors are open and randomly close one
            if (room.openDoors.Count > 1)
            {
                GameObject doorToClose = room.openDoors[Random.Range(0, room.openDoors.Count)];

                DFS2(room, doorToClose);
                //use path finding to ensure path to start room is accessible
                //double check that room that this connects too has another opening

            }

            //remove from open rooms
            openDoors.Remove(room);

        }

    }

    void spawnRoom(Vector2 pos)
    {
        Room r = Instantiate<Room>(RoomPrefab);
        r.transform.position = pos * new Vector2(roomWidth, roomHeight);
        r.pos = r.transform.position;
        r.transform.parent = this.transform;
        r.x = (int)pos.x;
        r.y = (int)pos.y;
        r.name = "[" + r.x + "," + r.y + "]";
        AllRooms.Add(r);

        Vector2 topPosition = new Vector2(r.x, r.y) + new Vector2(0, 1);
        Vector2 bottomPosition = new Vector2(r.x, r.y) + new Vector2(0, -1);
        Vector2 leftPosition = new Vector2(r.x, r.y) + new Vector2(-1, 0);
        Vector2 rightPosition = new Vector2(r.x, r.y) + new Vector2(1, 0);

        //determine possible other spawn locations and
        // add them to our list of possible rooms
        if (AllRooms.FindIndex(i => i.x == topPosition.x && i.y == topPosition.y) < 0 && !PossibleRooms.Contains(topPosition))
        {
            PossibleRooms.Add(topPosition);
            //PossibleRoomsTop.Add(new Vector2(pos.x, pos.y + 1));
        }
        if (AllRooms.FindIndex(i => i.x == bottomPosition.x && i.y == bottomPosition.y) < 0 && !PossibleRooms.Contains(bottomPosition))
        {
            PossibleRooms.Add(bottomPosition);
            //PossibleRoomsBottom.Add(new Vector2(pos.x, pos.y - 1));
        }
        if (AllRooms.FindIndex(i => i.x == rightPosition.x && i.y == rightPosition.y) < 0 && !PossibleRooms.Contains(rightPosition))
        {
            PossibleRooms.Add(rightPosition);
            //PossibleRoomsRight.Add(new Vector2(pos.x + 1, pos.y));
        }
        if (AllRooms.FindIndex(i => i.x == leftPosition.x && i.y == leftPosition.y) < 0 && !PossibleRooms.Contains(leftPosition))
        {
            PossibleRooms.Add(leftPosition);
            //PossibleRoomsLeft.Add(new Vector2(pos.x - 1, pos.y));
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
