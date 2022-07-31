using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using AdaptiveWizard.Assets.Scripts.Other.GameManagers;
using AdaptiveWizard.Assets.Scripts.Enemies.Pathfinding;
 
 // TODO: refactor this class
namespace AdaptiveWizard.Assets.Scripts.Other.Rooms
{
    public class AbstractRoom : MonoBehaviour
    {        
        private char[,] baseRoomVisual;        // visual representation of the base room (doors are viewed as walls)
        private Node[,] roomNodes;          // 2D array of floor nodes of the room, used for pathfinding. 2D array allows for easy heuristic calculation

        private Vector2 posOffset;             // specifies the relative position of this room from position [0, 0] in world coordinates

        private List<Door> doors;        // list of doors in this room


        // prefab objects
        public GameObject roomIO_Obj;
        
        

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
        public virtual void Init(Vector2 posOffset, char[,] baseRoomVisual, List<Teleporter> teleporters) {
            this.posOffset = posOffset - new Vector2(0, 1);
            // ^ Must subtract 1 from y offset to account for sometic walls taking up 2 vertical spaces
            this.baseRoomVisual = baseRoomVisual;
            GenerateRoom();
            //SetTeleportersToDoors(teleporters);       // TEMPORARILY COMMENTED OUT

            // testing
            /*
            print("Room visual:");
            for (int y = 0; y < baseRoomVisual.GetLength(0); y++) {
                string row = "";
                for (int x = 0; x < baseRoomVisual.GetLength(1); x++) {
                    row += baseRoomVisual[y, x];
                }
                print(row);
            }
            */
        }

        public void Update() {
            /*
            if (MainGameManager.IsGameActive()) {
                RoomManager rm = MainGameManager.GetRoomManager();
                // if player is in the same room as this combat manager
                if (rm.GetRoom(rm.GetCurActiveRoomIndex()) == this) {
                    // If player is fully outside the bounding box of this room, teleport player to another room.
                    // This room is determined by the doors closest to the player. 
                    // Note that this way of room transitioning only works if all doors are at the edges of rooms.
                    if (IsPlayerOutsideBoundingBox()) {
                        TeleportPlayerToAnotherRoom();
                    }
                }
            }
            */
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
        
        private void GenerateRoom() {
            //this.doors = roomIO.GetDoors();       // TEMPORARILY COMMENTED OUT
            CreateRoomNodes();
            InitializeRoomNodes();
        }
        

        public char TileSymbolAtPosition(int x, int y) {
            return baseRoomVisual[RoomHeight() - y - 1, x];
        }

        private void CreateRoomNodes() {
            this.roomNodes = new Node[RoomHeight(), RoomWidth()];
            for (int y = 0; y < RoomHeight(); y++) {
                for (int x = 0; x < RoomWidth(); x++) {
                    // first, set null to each element
                    SetRoomNode(x, y, null);
                    if (TileSymbolAtPosition(x, y) == '.') {
                        // second, change null to a node if needed
                        SetRoomNode(x, y, new Node(new Vector2Int(x, y)));
                    }
                }
            }
        }

        private void SetRoomNode(int x, int y, Node node) {
            this.roomNodes[RoomHeight() - y - 1, x] = node;
        }

        public Node GetRoomNode(int x, int y) {
            return roomNodes[RoomHeight() - y - 1, x];
        }

        private void InitializeRoomNodes() {
            for (int y = 0; y < RoomHeight(); y++) {
                for (int x = 0; x < RoomWidth(); x++) {
                    if (TileSymbolAtPosition(x, y) != '.') {
                        continue;
                    }
                    // bottom left
                    if (x - 1 >= 0 && y - 1 >= 0 && IsDiagonalUnobstructed(x - 1, y - 1)) {
                        GetRoomNode(x, y).AddNeighbour(GetRoomNode(x - 1, y - 1));
                    }
                    // bottom
                    if (y - 1 >= 0 && TileSymbolAtPosition(x, y - 1) == '.') {
                        GetRoomNode(x, y).AddNeighbour(GetRoomNode(x, y - 1));
                    }
                    // bottom right
                    if (x + 1 < RoomWidth() && y - 1 >= 0 && IsDiagonalUnobstructed(x, y - 1)) {
                        GetRoomNode(x, y).AddNeighbour(GetRoomNode(x + 1, y - 1));
                    }
                    // left
                    if (x - 1 >= 0 && TileSymbolAtPosition(x - 1, y) == '.') {
                        GetRoomNode(x, y).AddNeighbour(GetRoomNode(x - 1, y));
                    }
                    // right
                    if (x + 1 < RoomWidth() && TileSymbolAtPosition(x + 1, y) == '.') {
                        GetRoomNode(x, y).AddNeighbour(GetRoomNode(x + 1, y));
                    }
                    // top left
                    if (x - 1 >= 0 && y + 1 < RoomHeight() && IsDiagonalUnobstructed(x - 1, y)) {
                        GetRoomNode(x, y).AddNeighbour(GetRoomNode(x - 1, y + 1));
                    }
                    // top
                    if (y + 1 < RoomHeight() && TileSymbolAtPosition(x, y + 1) == '.') {
                        GetRoomNode(x, y).AddNeighbour(GetRoomNode(x, y + 1));
                    }
                    // top right
                    if (x + 1 < RoomWidth() && y + 1 < RoomHeight() && IsDiagonalUnobstructed(x, y)) {
                        GetRoomNode(x, y).AddNeighbour(GetRoomNode(x + 1, y + 1));
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
            MainGameManager.GetRoomManager().SetCurActiveRoomIndex(teleporter.GetAssocaitedRoomIndex());
        }

        public Node WorldPositionToNode(Vector2 position) {
            int x = (int) Mathf.Round(position.x - posOffset.x);
            int y = (int) Mathf.Round(position.y - posOffset.y);
            /*
            print($"x = {x}, y = {y}");
            print($"roomNodes == null? {roomNodes == null}");
            print($"roomNodes dimensions: {roomNodes.GetLength(0)}x{roomNodes.GetLength(1)}");
            print($"roomNodes[x, y] == null? {roomNodes[x, y] == null}");
            */
            return GetRoomNode(x, y);
        }
        

        public Vector2 PositionInRoomToPositionInWorld(Vector2Int roomPos) {
            Vector2 worldPos = new Vector2(roomPos.x + posOffset.x, roomPos.y + posOffset.y);
            return worldPos;
        }

        public int RoomWidth() {
            return baseRoomVisual.GetLength(1);
        }

        public int RoomHeight() {
            return baseRoomVisual.GetLength(0);
        }

        public Vector2 GetPosOffset() {
            return posOffset;
        }

        public void OpenDoors() {
            foreach (Door door in doors) {
                door.Open();
            }
        }
    }
}
