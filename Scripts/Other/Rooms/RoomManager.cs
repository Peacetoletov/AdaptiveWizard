using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: add a Vector2 "offset" variable and adjust methods that convert to or from world coordinates to include the offset in the calculation 
public class RoomManager : MonoBehaviour
{
     // public GameObjects used for instantiating
    public GameObject wallObj;
    public GameObject floorObj;
    public GameObject enemy1Obj;        // only for testing
    public GameObject enemy2Obj;        // only for testing
    public GameObject enemyGatlingObj;        // only for testing

    
    private string[] roomVisual;        // visual representation of the room
    private Pathfinding.Node[,] roomNodes;          // 2D array of floor nodes of the room, used for pathfinding. 2D array allows for easy heuristic calculation

    private Vector2 positionOffset;             // specifies the relative position of this room from position [0, 0] in world coordinates

    private void Start() {
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
        Restart();
    }

    public void Restart() {
        //GenerateRoom();
    }

    /*
    private void GenerateRoom() {
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

        // Whatever needs to be tested here
        if (!MainGameManager.minigame) {
            Instantiate(enemyGatlingObj, new Vector3(12f, 0f, 0f), Quaternion.identity);
        }
    }
    */

    /*
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
    */

    /*
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
    */

    /*
    private bool IsDiagonalUnobstructed(int lowerX, int lowerY) {
        int upperX = lowerX + 1;
        int upperY = lowerY + 1;
        return (TileSymbolAtPosition(lowerX, lowerY) == '.' && TileSymbolAtPosition(lowerX, upperY) == '.' &&
                TileSymbolAtPosition(upperX, lowerY) == '.' && TileSymbolAtPosition(upperX, upperY) == '.');
    }
    */

    public Pathfinding.Node WorldPositionToNode(GameObject obj) {
        // TODO: change this to incorporate room offset
        // converts object's world position to room position, then returns a node that corresponds to this position
        int x = (int) Mathf.Round(obj.transform.position.x - 0.5f + RoomWidth() / 2f);
        int y = roomVisual.Length - 1 - (int) Mathf.Round(obj.transform.position.y - 0.5f + RoomHeight() / 2f);
        //print("x = " + x + ", y = " + y);
        return roomNodes[x, y];
    }
    

    public Vector2 PositionInRoomToPositionInWorld(Vector2Int roomPos) {
        // TODO: change this to incorporate room offset
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
    
}
