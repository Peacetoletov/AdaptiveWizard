using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: possibly divide this into RoomManager and Room classes, with one (static?) RoomManager class containing multiple Room objects
// TODO: come up for a solution to having multiple rooms (how does PositionInRoomToPositionInWorld() work with many rooms?) - maybe
// add variable roomID to each Room object, and keep a table mapping roomIDs to world coordinate offsets?
// Also add this roomID to an enemy when it spawns
// TODO: add a method that converts an object's (enemy's) position to a roomID. I can use the fact that two rooms' bounding boxes will
// never overlap
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


    private void Start() {
        Restart();
    }

    public void Restart() {
        this.roomManagers = new List<RoomManager>();
        GenerateRoomManagers();
    }

    private void GenerateRoomManagers() {
        // TODO: iterate through all room managers here
        GameObject newRoomManager = Instantiate(roomManagerObj, Vector3.zero, Quaternion.identity) as GameObject;
        this.roomManagers.Add(newRoomManager.GetComponent<RoomManager>());
    }

    public static int WorldPositionToRoomIndex(Vector2 pos) {
        // TODO: implement this
        return 0;
    }

    public RoomManager GetRoomManager(int roomIndex) {
        return roomManagers[roomIndex];
        //return roomManagers[0];
    }
}
