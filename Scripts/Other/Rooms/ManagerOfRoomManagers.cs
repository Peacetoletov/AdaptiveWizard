using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: possibly divide this into RoomManager and Room classes, with one (static?) RoomManager class containing multiple Room objects DONE
// TODO: come up for a solution to having multiple rooms (how does PositionInRoomToPositionInWorld() work with many rooms?) - maybe
// add variable roomID to each Room object, and keep a table mapping roomIDs to world coordinate offsets?
// Also add this roomID to an enemy when it spawns
// TODO: add a method that converts an object's (enemy's) position to a roomID. I can use the fact that two rooms' bounding boxes will
// never overlap
// TODO: connect two rooms together (done) and change MinigameManager so that it spawns enemies in player's current room
public class ManagerOfRoomManagers : MonoBehaviour
{
    /*
    // public GameObjects used for instantiating
    public GameObject wallObj;
    public GameObject floorObj;
    public GameObject enemy1Obj;        // only for testing
    public GameObject enemy2Obj;        // only for testing
    public GameObject enemyGatlingObj;        // only for testing
    

    private string[] roomVisual;        // visual representation of the room
    private Pathfinding.Node[,] roomNodes;          // 2D array of floor nodes of the room, used for pathfinding. 2D array allows for easy heuristic calculation
    */

    // public GameObjects used for instantiating
    public GameObject roomManagerObj;


    private List<RoomManager> roomManagers;

    // player position on map
    private int playerCurRoomID;
    private int playerLastRoomID;


    private void Start() {
        Restart();
    }

    public void Restart() {
        GenerateRoomManagers();
        InitPlayerRoomID();
    }

    private void InitPlayerRoomID() {
        this.playerCurRoomID = RoomIDOfObject(MainGameManager.GetPlayer());
        this.playerLastRoomID = playerCurRoomID;
    }

    private void Update() {
        if (MainGameManager.IsGameActive()) {
            // update the ID of the room that the player is currently in
            UpdatePlayerRoomID();

            RoomManager playerRoom = roomManagers[playerCurRoomID];
            if (playerRoom.IsCombatRoom() && !playerRoom.GetCombatManager().DidCombatBegin()) {
                if (playerRoom.TryToCloseDoors(MainGameManager.GetPlayer().transform.position)) {
                    playerRoom.GetCombatManager().BeginCombat();
                }
                // change room's state to start spawning enemies
                /*
                ...
                */
            }
        }
    }

    private void UpdatePlayerRoomID() {
        // updates the ID of the room player is currently in and was previously in
        int newPlayerCurRoomID = RoomIDOfObject(MainGameManager.GetPlayer());
        if (newPlayerCurRoomID != playerCurRoomID) {
            this.playerLastRoomID = playerCurRoomID;
        }
        this.playerCurRoomID = newPlayerCurRoomID;
    }

    public int RoomIDOfObject(GameObject obj) {
        for (int id = 0; id < roomManagers.Count; id++) {
            RoomManager rm = roomManagers[id];
            Vector2 positionOfRoom = rm.PositionInRoomToPositionInWorld(new Vector2Int(0, 0));
            if (isObjectInsideRectangle(obj.transform.position, positionOfRoom, rm.RoomWidth(), rm.RoomHeight())) {
                //print(Time.time + ". Object is in room " + id);
                return id;
            }
        }
        throw new System.Exception("ERROR: object outside of all rooms!");
    }

    private bool isObjectInsideRectangle(Vector2 objPos, Vector2 rectPos, float width, float height) {
        //print(Time.time + ". objPos = " + objPos + ". rectPos = " + rectPos + ". width = " + width + ". height = " + height);
        // subtract 0.5 to adjust for centering
        return objPos.x >= rectPos.x - 0.5f && objPos.x <= rectPos.x + width - 0.5f && objPos.y >= rectPos.y - 0.5f && objPos.y <= rectPos.y + height - 0.5f;
    }

    private void GenerateRoomManagers() {
        this.roomManagers = new List<RoomManager>();

        // Room 1
        GameObject newRoomManager = Instantiate(roomManagerObj, Vector3.zero, Quaternion.identity) as GameObject;
        this.roomManagers.Add(newRoomManager.GetComponent<RoomManager>());
        string[] roomVisual = new string[] {
            "##############################",
            "#............................#",
            "#..............c.............#",
            "#............................#",
            "#............................#",
            "#...........######...........#",
            "#...........######...........#",
            "#...........######...........#",
            "/...........######...........#",
            "/...........######...........#",
            "#...........######...........#",
            "#............................#",
            "#............................#",
            "#...............c............#",
            "#............................#",
            "#//###########################"
        };
        roomManagers[0].Init(new Vector2(5, 1), roomVisual, RoomType.COMBAT);


        // Room 2
        newRoomManager = Instantiate(roomManagerObj, Vector3.zero, Quaternion.identity) as GameObject;
        this.roomManagers.Add(newRoomManager.GetComponent<RoomManager>());
        roomVisual = new string[] {
            "##########",
            "..........",
            "..........",
            "##########"
        };
        roomManagers[1].Init(new Vector2(-5, 6), roomVisual, RoomType.CORRIDOR);

        
        // Room 3
        newRoomManager = Instantiate(roomManagerObj, Vector3.zero, Quaternion.identity) as GameObject;
        this.roomManagers.Add(newRoomManager.GetComponent<RoomManager>());
        roomVisual = new string[] {
            "########",
            "#......#",
            "#......#",
            "#....../",
            "#....../",
            "#......#",
            "#......#",
            "########"
        };
        roomManagers[2].Init(new Vector2(-13, 4), roomVisual, RoomType.COMBAT);
        
    }

    /*
    public static int WorldPositionToRoomIndex(Vector2 pos) {
        // TODO: implement this
        // UPDATE: unnecessary, use case already covered by RoomIDOfObject
        return 0;
    }
    */

    public RoomManager GetRoomManager(int roomIndex) {
        return roomManagers[roomIndex];
    }
}
