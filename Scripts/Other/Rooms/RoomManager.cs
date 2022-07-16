using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using AdaptiveWizard.Assets.Scripts.Other.Rooms;
 
 
public class RoomManager : MonoBehaviour
{
    // public GameObjects used for instantiating
    public GameObject wallObj;
    public GameObject floorObj;
    public GameObject doorObj;
    public GameObject enemy1Obj;        // only for testing
    public GameObject enemy2Obj;        // only for testing
    public GameObject enemyGatlingObj;        // only for testing
    public GameObject combatManagerObj;

    
    private string[] roomVisual;        // visual representation of the room
    private Pathfinding.Node[,] roomNodes;          // 2D array of floor nodes of the room, used for pathfinding. 2D array allows for easy heuristic calculation

    private Vector2 posOffset;             // specifies the relative position of this room from position [0, 0] in world coordinates
    private RoomType type;                // type of this room (combat, event, shop)

    private List<Door> doors = new List<Door>();        // list of doors in this room

    private CombatManager combatManager;                  // combat related variables (only applies to combat rooms)


    private bool varForTestingOnly = true;      // remove this later
    

    /*
    Initializes room variables, must be called immediately after creating the room manager.

    Parameters:
    posOffset - specifies the relative position of this room from position [0, 0] in world coordinates
    roomVisual - visual representation of tiles in the room
    type - type of the room
    doorTeleportDistances - list of teleport distances, one for each door. The order of elements in this list determines
                            which door will be associated with the given teleport distance. The room is searched left
                            to right, top to bottom, and whenever a door is encountered, it is given the next teleport 
                            distance from the list.
    */
    public void Init(Vector2 posOffset, string[] roomVisual, RoomType type, List<Teleporter> teleporters) {
        this.posOffset = posOffset;
        this.roomVisual = roomVisual;
        this.type = type;
        System.Array.Reverse(roomVisual);       // I want the position [0, 0] to be in the bottom left corner, just like in world coordinates
        AssertRowsLength();

        if (type == RoomType.COMBAT) {
            this.combatManager = Instantiate(combatManagerObj, Vector3.zero, Quaternion.identity).GetComponent<CombatManager>();
            this.combatManager.Init(this);
        }
        GenerateRoom();
        SetTeleportersToDoors(teleporters);
    }

    public void Update() {
        if (MainGameManager.IsGameActive()) {
            ManagerOfRoomManagers morm = MainGameManager.GetManagerOfRoomManagers();
            // if player is in the same room as this combat manager
            if (morm.GetRoomManager(morm.GetCurActiveRoomIndex()) == this) {
                // If player is fully outside the bounding box of this room, teleport player to another room.
                // This room is determined by the doors closest to the player. 
                // Note that this way of room transitioning only works if all doors are at the edges of rooms.
                if (IsPlayerOutsideBoundingBox()) {
                    TeleportPlayerToAnotherRoom();
                }
            }
        }
    }

    private bool IsPlayerOutsideBoundingBox() {
        Vector2 playerSize = MainGameManager.GetPlayer().GetComponent<BoxCollider2D>().bounds.size;
        Vector2 playerPos = MainGameManager.GetPlayer().transform.position;

        // Return true if player is crossing at least one boundary of the bounding box
        return playerPos.x < posOffset.x - playerSize.x || 
                playerPos.x > posOffset.x + RoomWidth() ||
                playerPos.y < posOffset.y - playerSize.y ||
                playerPos.y > posOffset.y + RoomHeight();
    }

    private void AssertRowsLength() {
        // Confirms that all rows have the same length
        foreach (string row in roomVisual) {
            Assert.IsTrue(row.Length == roomVisual[0].Length);
        }
    }
    
    private void GenerateRoom() {
        // Environment
        for (int y = 0; y < RoomHeight(); y++) {
            for (int x = 0; x < roomVisual[y].Length; x++) {
                Vector3 coordinates = (Vector3) PositionInRoomToPositionInWorld(new Vector2Int(x, y));
                char symbol = TileSymbolAtPosition(x, y);
                if (symbol == '.') {
                    Instantiate(floorObj, coordinates, Quaternion.identity);
                } else if (symbol == '#') {
                    Instantiate(wallObj, coordinates, Quaternion.identity);
                } else if (symbol == '/') {
                    // door
                    GameObject newDoor = Instantiate(doorObj, coordinates, Quaternion.identity) as GameObject;
                    this.doors.Add(newDoor.GetComponent<Door>());
                } else {
                    // void space, generate nothing here (symbol == '-')
                }
            }
        }
        CreateRoomNodes();
        InitializeRoomNodes();

        // Whatever needs to be tested here
        if (!MainGameManager.minigame) {
            //Instantiate(enemyGatlingObj, new Vector3(12f, 0f, 0f), Quaternion.identity);
        }
    }
    

    public char TileSymbolAtPosition(int x, int y) {
        return roomVisual[y][x];
    }

    private void CreateRoomNodes() {
        this.roomNodes = new Pathfinding.Node[RoomWidth(), RoomHeight()];
        for (int y = 0; y < RoomHeight(); y++) {
            for (int x = 0; x < roomVisual[y].Length; x++) {
                // first, set null to each element
                this.roomNodes[x, y] = null;
                if (TileSymbolAtPosition(x, y) == '.') {
                    // second, change null to a node if needed
                    this.roomNodes[x, y] = new Pathfinding.Node(new Vector2Int(x, y));
                }
            }
        }
    }

    private void InitializeRoomNodes() {
        for (int y = 0; y < RoomHeight(); y++) {
            for (int x = 0; x < roomVisual[y].Length; x++) {
                if (TileSymbolAtPosition(x, y) != '.') {
                    continue;
                }
                // bottom left
                if (x - 1 >= 0 && y - 1 >= 0 && IsDiagonalUnobstructed(x - 1, y - 1)) {
                    this.roomNodes[x, y].AddNeighbour(this.roomNodes[x - 1, y - 1]);
                }
                // bottom
                if (y - 1 >= 0 && TileSymbolAtPosition(x, y - 1) == '.') {
                    this.roomNodes[x, y].AddNeighbour(this.roomNodes[x, y - 1]);
                }
                // bottom right
                if (x + 1 < RoomWidth() && y - 1 >= 0 && IsDiagonalUnobstructed(x, y - 1)) {
                    this.roomNodes[x, y].AddNeighbour(this.roomNodes[x + 1, y - 1]);
                }
                // left
                if (x - 1 >= 0 && TileSymbolAtPosition(x - 1, y) == '.') {
                    this.roomNodes[x, y].AddNeighbour(this.roomNodes[x - 1, y]);
                }
                // right
                if (x + 1 < RoomWidth() && TileSymbolAtPosition(x + 1, y) == '.') {
                    this.roomNodes[x, y].AddNeighbour(this.roomNodes[x + 1, y]);
                }
                // top left
                if (x - 1 >= 0 && y + 1 < RoomHeight() && IsDiagonalUnobstructed(x - 1, y)) {
                    this.roomNodes[x, y].AddNeighbour(this.roomNodes[x - 1, y + 1]);
                }
                // top
                if (y + 1 < RoomHeight() && TileSymbolAtPosition(x, y + 1) == '.') {
                    this.roomNodes[x, y].AddNeighbour(this.roomNodes[x, y + 1]);
                }
                // top right
                if (x + 1 < RoomWidth() && y + 1 < RoomHeight() && IsDiagonalUnobstructed(x, y)) {
                    this.roomNodes[x, y].AddNeighbour(this.roomNodes[x + 1, y + 1]);
                }
            }
        }
    }

    private bool IsDiagonalUnobstructed(int lowerX, int lowerY) {
        int upperX = lowerX + 1;
        int upperY = lowerY + 1;
        return (TileSymbolAtPosition(lowerX, lowerY) == '.' && TileSymbolAtPosition(lowerX, upperY) == '.' &&
                TileSymbolAtPosition(upperX, lowerY) == '.' && TileSymbolAtPosition(upperX, upperY) == '.');
    }

    private void SetTeleportersToDoors(List<Teleporter> teleporters) {
        Assert.IsTrue(doors.Count == teleporters.Count);
        for (int i = 0; i < doors.Count; i++) {
            doors[i].SetTeleporter(teleporters[i]);
        }
    }

    private void TeleportPlayerToAnotherRoom() {
        // Find the nearest door 
        Vector2 playerPos = MainGameManager.GetPlayer().transform.position;
        float minDist = Vector2.Distance(playerPos, doors[0].transform.position);
        Door doorWithMinDist = doors[0];
        foreach (Door door in doors) {
            float curDist = Vector2.Distance(playerPos, door.transform.position);
            if (curDist < minDist) {
                minDist = curDist;
                doorWithMinDist = door;
            }
        }

        // Get the teleporter of the nearest door
        Teleporter teleporter = doorWithMinDist.GetTeleporter();

        // Teleport player using the teleport distance of the nearest door
        MainGameManager.GetPlayer().transform.Translate(teleporter.GetTeleportDist());
        
        // Change the current active room ID
        MainGameManager.GetManagerOfRoomManagers().SetCurActiveRoomIndex(teleporter.GetAssocaitedRoomIndex());
    }

    public Pathfinding.Node WorldPositionToNode(GameObject obj) {
        return WorldPositionToNode(obj.transform.position);
    }

    public Pathfinding.Node WorldPositionToNode(Vector2 position) {
        int x = (int) Mathf.Round(position.x - posOffset.x);
        int y = (int) Mathf.Round(position.y - posOffset.y);
        return roomNodes[x, y];
    }
    

    public Vector2 PositionInRoomToPositionInWorld(Vector2Int roomPos) {
        Vector2 worldPos = new Vector2(roomPos.x + posOffset.x, roomPos.y + posOffset.y);
        return worldPos;
    }

    public int RoomWidth() {
        return roomVisual[0].Length;
    }

    public int RoomHeight() {
        return roomVisual.Length;
    }

    public Vector2 GetPosOffset() {
        return posOffset;
    }

    public bool IsCombatRoom() {
        return type == RoomType.COMBAT;
    }

    public CombatManager GetCombatManager() {
        return combatManager;
    }

    public void OpenDoors() {
        foreach (Door door in doors) {
            door.Open();
        }
    }
}
