using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: possibly divide this into RoomManager and Room classes, with one (static?) RoomManager class containing multiple Room objects
// TODO: come up for a solution to having multiple rooms (how does PositionInRoomToPositionInWorld() work with many rooms?) - maybe
// add variable roomID to each Node object, and keep a table mapping roomIDs to world coordinate offsets?
public class RoomManager : MonoBehaviour
{
    // public GameObjects used for instantiating
    public GameObject wallObj;
    public GameObject floorObj;
    public GameObject enemy1Obj;        // only for testing

    
    private string[] roomVisual;        // visual representation of the room
    private Pathfinding.Node[,] roomNodes;          // 2D array of floor nodes of the room, used for pathfinding. 2D array allows for easy heuristic calculation


    private void Start() {
        GenerateRoom(false);
    }

    private void GenerateRoom(bool createNewPlayer=true) {
        /*
        // Limit testing room:
        MainGameManager.roomVisual = new string[] {
            "##############################",
            "#............................#",
            "#............................#",
            "#..#####...............###...#",
            "#.............##.........#...#",
            "#.............##.........#...#",
            "#...........####.........#...#",
            "#...........####.........#...#",
            "#...#........................#",
            "#...#........................#",
            "#.....................###....#",
            "#.........###############....#",
            "#.........###############....#",
            "#............................#",
            "#............................#",
            "##############################"
        };
        */
        
        /*
        MainGameManager.roomVisual = new string[] {
            "##############################",
            "#............................#",
            "#............................#",
            "#............................#",
            "#............................#",
            "#............................#",
            "#............................#",
            "#............................#",
            "#............................#",
            "#............................#",
            "#............................#",
            "#............................#",
            "#............................#",
            "#............................#",
            "#............................#",
            "##############################"
        };
        */

        this.roomVisual = new string[] {
            "##############################",
            "#............................#",
            "#............................#",
            "#............................#",
            "#............................#",
            "#...........######...........#",
            "#...........######...........#",
            "#...........######...........#",
            "#...........######...........#",
            "#...........######...........#",
            "#...........######...........#",
            "#............................#",
            "#............................#",
            "#............................#",
            "#............................#",
            "##############################"
        };

        // Environment
        for (int x = 0; x < RoomWidth(); x++) {
            for (int y = 0; y < RoomHeight(); y++) {
                //int inverseY = roomVisual.Length - 1 - y;       // my room coordinates start at top left, but world coordinates start at bottom left
                //Vector3 coordinates = new Vector3((float) (x + 0.5 - RoomWidth() / 2.0), (float) (inverseY + 0.5 - RoomHeight() / 2.0), 0);
                Vector3 coordinates = (Vector3) PositionInRoomToPositionInWorld(new Vector2Int(x, y));
                if (TileSymbolAtPosition(x, y) == '.') {
                    Instantiate(floorObj, coordinates, Quaternion.identity);
                } else {
                    Instantiate(wallObj, coordinates, Quaternion.identity);
                }
            }
        }
        CreateRoomNodes();
        InitializeRoomNodes();

        // Player is instantiated from MainGameManager

        // Start spawning enemies
        if (MainGameManager.minigame) {
            //this.spawnEnemiesTimer = new Timer(miniGameSpawnPeriod);
        }
        else {
            // whatever needs to be tested here
            /*
            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {
                    Instantiate(enemy1Obj, new Vector3(5f + i, 0f + j, 0f), Quaternion.identity);
                }
            }
            */
            /*
            for (int i = 0; i < 10; i++) {
                for (int j = 0; j < 10; j++) {
                    Instantiate(enemy1Obj, new Vector3(3f + i*0.3f, -1f + j*0.3f, 0f), Quaternion.identity);
                }
            }
            */
            Instantiate(enemy1Obj, new Vector3(-12f, 0f, 0f), Quaternion.identity);
            Instantiate(enemy1Obj, new Vector3(12f, 0f, 0f), Quaternion.identity);
        }
    }

    public char TileSymbolAtPosition(int x, int y) {
        return roomVisual[y][x];
    }

    private void CreateRoomNodes() {
        this.roomNodes = new Pathfinding.Node[RoomWidth(), RoomHeight()];
        for (int x = 0; x < RoomWidth(); x++) {
            for (int y = 0; y < RoomHeight(); y++) {
                // first, set null to each element
                // print("x = " + x + "; y = " + y);
                this.roomNodes[x, y] = null;
                if (TileSymbolAtPosition(x, y) == '.') {
                    // second, change null to a node if needed
                    this.roomNodes[x, y] = new Pathfinding.Node(new Vector2Int(x, y));
                }
            }
        }
    }

    private void InitializeRoomNodes() {
        for (int x = 0; x < RoomWidth(); x++) {
            for (int y = 0; y < RoomHeight(); y++) {
                if (TileSymbolAtPosition(x, y) != '.') {
                    continue;
                }
                // top left
                if (x - 1 >= 0 && y - 1 >= 0 && IsDiagonalUnobstructed(x - 1, y - 1)) {
                    this.roomNodes[x, y].AddNeighbour(this.roomNodes[x - 1, y - 1]);
                }
                // top
                if (y - 1 >= 0 && TileSymbolAtPosition(x, y - 1) == '.') {
                    this.roomNodes[x, y].AddNeighbour(this.roomNodes[x, y - 1]);
                }
                // top right
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
                // bottom left
                if (x - 1 >= 0 && y + 1 < RoomHeight() && IsDiagonalUnobstructed(x - 1, y)) {
                    this.roomNodes[x, y].AddNeighbour(this.roomNodes[x - 1, y + 1]);
                }
                // bottom
                if (y + 1 < RoomHeight() && TileSymbolAtPosition(x, y + 1) == '.') {
                    this.roomNodes[x, y].AddNeighbour(this.roomNodes[x, y + 1]);
                }
                // bottom right
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

    public Pathfinding.Node WorldPositionToNode(GameObject obj) {
        // converts object's world position to room position, then returns a node that corresponds to this position
        int x = (int) Mathf.Round(obj.transform.position.x - 0.5f + RoomWidth() / 2f);
        int y = roomVisual.Length - 1 - (int) Mathf.Round(obj.transform.position.y - 0.5f + RoomHeight() / 2f);
        //print("x = " + x + ", y = " + y);
        return roomNodes[x, y];
    }

    public Vector2 PositionInRoomToPositionInWorld(Vector2Int roomPos) {
        // Takes a room position and return a corresponding world position
        // Note: this will need to change once I introduce more than 1 room
        int inverseY = roomVisual.Length - 1 - roomPos.y;       // my room coordinates start at top left, but world coordinates start at bottom left
        Vector2 worldPos = new Vector2((float) (roomPos.x + 0.5 - RoomWidth() / 2.0), (float) (inverseY + 0.5 - RoomHeight() / 2.0));
        return worldPos;
    }

    public int RoomWidth() {
        return roomVisual[0].Length;
    }

    public int RoomHeight() {
        return roomVisual.Length;
    }

    // do not use a static function for getting room nodes
    /*
    public static Pathfinding.Node[,] GetRoomNodes() {
        return roomNodes;
    }
    */
}
