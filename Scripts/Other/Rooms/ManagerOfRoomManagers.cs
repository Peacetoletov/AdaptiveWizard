using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdaptiveWizard.Assets.Scripts.Other.Rooms;

/* TODO: - add a "void" tile to the visual representation of a room DONE 
         - add a distant room DONE
         - add doors (teleporters) that connect them
            - each frame, check if the bounding box of the room and player's collision box collide DONE. If they don't,
              find the nearest door and teleport player to the corresponding room. Assume that player cannot
              jump over room boundaries using their abilities.
            - add cosmetic blocks near doors
                - first determine where these blocks should be placed based on the relative position of the doors
                  in the room, then place the blocks there
                
*/
public class ManagerOfRoomManagers : MonoBehaviour
{

    // public GameObjects used for instantiating
    public GameObject roomManagerObj;


    private List<RoomManager> roomManagers;

    // Index of the room that the player is currently in
    private int curActiveRoomIndex;


    private void Start() {
        Restart();
    }

    public void Restart() {
        GenerateRoomManagers();
        InitPlayerRoomID();
    }

    private void InitPlayerRoomID() {
        // temporary (?) solution
        this.curActiveRoomIndex = 0;        // temporarily changed from 0
    }

    private void Update() {
        /*
        if (MainGameManager.IsGameActive()) {

        }
        */
    }

    private void GenerateRoomManagers() {
        this.roomManagers = new List<RoomManager>();

        // Room 1
        GameObject newRoomManager = Instantiate(roomManagerObj, Vector3.zero, Quaternion.identity) as GameObject;
        this.roomManagers.Add(newRoomManager.GetComponent<RoomManager>());
        string[] roomVisual = new string[] {
            "-----######//######---",
            "----#.....#..#.....#--",
            "---#................#-",
            "###..................#",
            "#..................../",
            "#..................../",
            "###..................#",
            "---#................#-",
            "----#..............#--",
            "-----######//######---"
        };
        List<Teleporter> teleporters = new List<Teleporter> {
            new Teleporter(new Vector2(0, -5), 666),
            new Teleporter(new Vector2(0, -5), 666),
            new Teleporter(new Vector2(14, 0), 1),
            new Teleporter(new Vector2(14, 0), 1),
            new Teleporter(new Vector2(0, 5), 420),
            new Teleporter(new Vector2(0, 5), 420)
        };
        this.roomManagers[0].Init(new Vector2(-10, 1), roomVisual, RoomType.COMBAT, teleporters);
        

        
        //Room 2
        
        newRoomManager = Instantiate(roomManagerObj, Vector3.zero, Quaternion.identity) as GameObject;
        this.roomManagers.Add(newRoomManager.GetComponent<RoomManager>());
        roomVisual = new string[] {
            "--#######--",
            "-#.......#-",
            "#.........#",
            "/.........#",
            "/.........#",
            "#.........#",
            "-#.......#-",
            "--#######--"
        };
        teleporters = new List<Teleporter> {
            new Teleporter(new Vector2(-14, 0), 0),
            new Teleporter(new Vector2(-14, 0), 0),
        };
        roomManagers[1].Init(new Vector2(24, 2), roomVisual, RoomType.COMBAT, teleporters);
        
    }

    public RoomManager GetRoomManager(int roomIndex) {
        return roomManagers[roomIndex];
    }

    public int GetCurActiveRoomIndex() {
        return curActiveRoomIndex;
    }

    public void SetCurActiveRoomIndex(int curActiveRoomIndex) {
        this.curActiveRoomIndex = curActiveRoomIndex;
    }
}
