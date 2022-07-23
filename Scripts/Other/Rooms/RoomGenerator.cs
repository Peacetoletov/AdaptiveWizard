using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

/*
RoomGenerator chooses and instantiates prefabs in a room.
*/

/* CURRENT TODO: 
Save handcrafted rooms into a file. Create a new folder (named Room1, Room2 etc.) and create 2 files here.
The first file will contain two rows: first row will contain dimensions of the room, second row will contain
pairs of object coordinates and object id.
The second file will contain visual representation of the room, which will be used for enemy pathfinding.
I can probably remove doors from the visual representation of the room, because they are not relevant to enemies
(they serve the same function as walls to enemies). I should add lakes to the visual representation, because they
might be relevant to enemies with ranged attacks. This can however wait.  
*/

/* ADDITIONAL TODO:
Check if all back walls are truly equivalent. I have a feeling that some are supposed to be used when there is floor
on the left/right, based on a darker vertical line at the side, possibly to create a bigger contrast between the wall
and floor. These vertical lines also look kind of weird when in the middle of a long back wall.
*/
namespace AdaptiveWizard.Assets.Scripts.Other.Rooms
{
    public class RoomGenerator : MonoBehaviour
    {
        /* 
        ##################################################
        ##################  Variables  ###################
        ##################################################
        */

        // list of doors in this room
        private List<Door> doors = new List<Door>();    

        // TODO: document how flags are used
        // NEWER TODO: remove this
        List<(GameObject, List<List<int>>)> listOfWallsWithListsOfFlags;

        // Caching
        private static Dictionary<string, int> objectNameToID_Table;        // Table mapping GameObject scene names to IDs 
        private static HashSet<int> solidWallIDs;
        private static HashSet<int> cosmeticWallIDs;                        // Cosmetic walls act as floors on position x,y and as solid walls on position x,y+1 
        private static HashSet<int> floorIDs;
        private static HashSet<int> lakeIDs;


        /* 
        ##################################################
        ######  GameObjects used for instantiating  ######
        ##################################################
        */

        // Cosmetics
        public GameObject bonesBlue01Obj;
        public GameObject cobwebBlue01Obj;
        public GameObject cobwebBlue02Obj;
        public GameObject cobwebBlue03Obj;
        public GameObject cobwebBlue04Obj;
        public GameObject rocksBlue01Obj;
        public GameObject skullBlue01Obj;
        public GameObject skullBlue02Obj;

        // Doors
        public GameObject doorBlue02Obj;
        public GameObject doorBlue04Obj;
        public GameObject doorBlue05Obj;
        public GameObject doorBlue06Obj;
        public GameObject doorBlue07Obj;
        public GameObject doorBlue08Obj;
        public GameObject doorBlue09Obj;
        public GameObject doorBlue10Obj;

        // Floor
        public GameObject floorBlue01Obj;
        public GameObject floorBlue02Obj;
        public GameObject floorBlue03Obj;
        public GameObject floorBlue04Obj;
        public GameObject floorBlue05Obj;
        public GameObject floorBlue06Obj;
        public GameObject floorBlue07Obj;

        // Lake
        public GameObject lakeCornerBlue01Obj;
        public GameObject lakeCornerBlue02Obj;
        public GameObject lakeCornerBlue03Obj;
        public GameObject lakeCornerBlue04Obj;
        public GameObject lakeDoubleEdgeBlue01Obj;
        public GameObject lakeEdgeBlue01Obj;
        public GameObject lakeEdgeBlue02Obj;
        public GameObject lakeEdgeBlue03Obj;
        public GameObject lakeEdgeBlue04Obj;
        public GameObject lakeMiddleBlue01Obj;
        public GameObject lakeSmallBlue01Obj;
        public GameObject lakeUBlue01Obj;
        public GameObject lakeUBlue02Obj;

        // Stairs
        public GameObject stairsBlue01Obj;
        public GameObject stairsBlue02Obj;
        public GameObject stairsBlue03Obj;
        public GameObject stairsBlue04Obj;

        // Back walls
        public GameObject wallBlue01Obj;
        public GameObject wallBlue02Obj;
        public GameObject wallBlue03Obj;
        public GameObject wallBlue04Obj;
        public GameObject wallBlue05Obj;
        public GameObject wallBlue06Obj;
        public GameObject wallBlue07Obj;
        public GameObject wallBlue08Obj;
        public GameObject wallBlue09Obj;
        public GameObject wallBlue10Obj;
        public GameObject wallBlue11Obj;

        // 2-Connector walls
        public GameObject twoConnectorBlue01Obj;
        public GameObject twoConnectorBlue02Obj;
        public GameObject twoConnectorBlue03Obj;
        public GameObject twoConnectorBlue04Obj;
        public GameObject twoConnectorBlue05Obj;
        public GameObject twoConnectorBlue06Obj;
        public GameObject twoConnectorBlue07Obj;
        public GameObject twoConnectorBlue08Obj;
        public GameObject twoConnectorBlue09Obj;
        public GameObject twoConnectorBlue10Obj;
        public GameObject twoConnectorBlue11Obj;
        public GameObject twoConnectorBlue12Obj;
        public GameObject twoConnectorBlue13Obj;
        public GameObject twoConnectorBlue14Obj;
        public GameObject twoConnectorBlue15Obj;
        public GameObject twoConnectorBlue16Obj;
        public GameObject twoConnectorBlue17Obj;
        public GameObject twoConnectorBlue18Obj;
        public GameObject twoConnectorBlue19Obj;
        public GameObject twoConnectorBlue20Obj;

        // 3-Connector walls
        public GameObject threeConnectorBlue01Obj;
        public GameObject threeConnectorBlue02Obj;
        public GameObject threeConnectorBlue03Obj;
        public GameObject threeConnectorBlue04Obj;
        public GameObject threeConnectorBlue05Obj;
        public GameObject threeConnectorBlue06Obj;
        public GameObject threeConnectorBlue07Obj;
        public GameObject threeConnectorBlue08Obj;
        public GameObject threeConnectorBlue09Obj;
        public GameObject threeConnectorBlue10Obj;
        public GameObject threeConnectorBlue11Obj;
        public GameObject threeConnectorBlue12Obj;
        public GameObject threeConnectorBlue13Obj;
        public GameObject threeConnectorBlue14Obj;
        public GameObject threeConnectorBlue15Obj;
        public GameObject threeConnectorBlue16Obj;
        public GameObject threeConnectorBlue17Obj;

        // 4-Connector walls
        public GameObject fourConnectorBlue01Obj;
        public GameObject fourConnectorBlue02Obj;

        // Wall edges
        public GameObject archBlue01Obj;
        public GameObject archBlue02Obj;
        public GameObject edgeBlue01Obj;
        public GameObject edgeBlue02Obj;
        public GameObject edgeBlue03Obj;
        public GameObject edgeBlue04Obj;
        public GameObject edgeWithArchBlue01Obj;
        public GameObject edgeWithArchBlue02Obj;
        public GameObject halfEdgeBlue01Obj;
        public GameObject halfEdgeBlue02Obj;
        public GameObject quarterEdgeBlue01Obj;
        public GameObject quarterEdgeBlue02Obj;

        // Void
        public GameObject voidBlue01Obj;


        /*
        ##################################################
        ###############  Room generation  ################
        ##################################################
        */

        public void Generate(Room room, string[] roomVisual) {
            
            /*
            InitWallFlags();

            for (int y = 0; y < room.RoomHeight(); y++) {
                for (int x = 0; x < roomVisual[y].Length; x++) {
                    Vector3 coordinates = (Vector3) room.PositionInRoomToPositionInWorld(new Vector2Int(x, y));
                    char symbol = room.TileSymbolAtPosition(x, y);
                    if (symbol == '.') {
                        if (IsWall(x, y - 1, roomVisual)) {
                            Instantiate(SelectWall(x, y, roomVisual), coordinates, Quaternion.identity);
                        } else {
                            Instantiate(ChooseRandomFloor(), coordinates, Quaternion.identity);
                        }
                    } else if (symbol == '#') {
                        Instantiate(SelectWall(x, y, roomVisual), coordinates, Quaternion.identity);
                    } else if (symbol == '/') {
                        // door
                        GameObject newDoor = Instantiate(doorBlue07Obj, coordinates, Quaternion.identity) as GameObject;
                        this.doors.Add(newDoor.GetComponent<Door>());
                    } else if (symbol == '-') {
                        // void
                        if (IsWall(x, y - 1, roomVisual)) {
                            Instantiate(SelectWall(x, y, roomVisual), coordinates, Quaternion.identity);
                        }
                    } else {
                        // this branch should never execute
                    }
                }
            }
            */
        }

        private GameObject ChooseRandomFloor() {
            List<(GameObject, float)> objectsWithPriorities = new List<(GameObject, float)> {
                (floorBlue01Obj, 1.5f),
                (floorBlue02Obj, 2),
                (floorBlue03Obj, 5),
                (floorBlue04Obj, 2),
                (floorBlue05Obj, 0.5f),
                (floorBlue06Obj, 0.33f),
                (floorBlue07Obj, 0.5f),
            };

            return ChooseRandomObject(objectsWithPriorities);
        }

        private GameObject ChooseRandomBackWall() {
            List<(GameObject, float)> objectsWithPriorities = new List<(GameObject, float)> {
                (wallBlue01Obj, 1),
                (wallBlue02Obj, 1),
                (wallBlue03Obj, 1),
                (wallBlue04Obj, 1),
                (wallBlue05Obj, 1),
                (wallBlue06Obj, 1),
                (wallBlue07Obj, 1),
                (wallBlue08Obj, 1),
                (wallBlue09Obj, 1),
                (wallBlue10Obj, 1),
                (wallBlue11Obj, 1),
            };

            return ChooseRandomObject(objectsWithPriorities);
        }

        private GameObject ChooseRandomObject(List<(GameObject, float)> objectsWithPriorities) {
            float prioritiesSum = objectsWithPriorities.Aggregate(0f, (x, y) => x + y.Item2);
            float rand = UnityEngine.Random.Range(0, prioritiesSum);
            float unusedPrioritiesSum = 0;
            foreach ((GameObject obj, float priority) in objectsWithPriorities) {
                if (rand <= unusedPrioritiesSum + priority) {
                    return obj;
                }
                unusedPrioritiesSum += priority;
            }

            throw new Exception("An error occured while choosing a random object in RoomGenerator!");
        }

        public List<Door> GetDoors() {
            return doors;
        }

        public static void SaveHandcraftedRoom(int roomRadius=100) {
            /* Saves a hand-crafted room into a file. 
            
            This room must be located near world coordinates [0, 0] and all tiles of the room must have
            x and y coordinates between -100 and 100 (room radius == 100). No tile from another room can
            be in this range of coordinates.
            */

            print("Calling SaveHandcraftedRoom()");

            (Vector2Int origin, Vector2Int dimensions) = GetOriginAndDimensionsOfHandcraftedRoom(roomRadius);
            print("Origin = " + origin + ", dimensions = " + dimensions);

            int[,] roomObjectIDs = new int[dimensions.y, dimensions.x];
            char[,] roomVisual = new char[dimensions.y + 1, dimensions.x];      // Adding 1 to height is needed because cosmetic walls take up 2 spaces
            Fill2DArray(roomObjectIDs, -1);
            Fill2DArray(roomVisual, '-');

            GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>() ;
            foreach (GameObject go in allObjects) {
                int id = ObjectNameToID(go.name.Split(' ')[0]);
                if (go.activeInHierarchy && id != -1) {
                    Vector2Int pos = Vector2Int.RoundToInt(go.transform.position) - origin;
                    if (pos.x >= -roomRadius && pos.x <= roomRadius && pos.y >= -roomRadius && pos.y <= roomRadius) {
                        roomObjectIDs[pos.y, pos.x] = id;
                        if (IsSolidWall(id)) {
                            roomVisual[roomVisual.GetLength(0) - pos.y - 2, pos.x] = '#';
                        } else if (IsCosmeticWall(id)) {
                            roomVisual[roomVisual.GetLength(0) - pos.y - 2, pos.x] = '.';
                            roomVisual[roomVisual.GetLength(0) - pos.y - 1, pos.x] = '#';
                        } else if (IsFloor(id)) {
                            roomVisual[roomVisual.GetLength(0) - pos.y - 2, pos.x] = '.';
                        } else if (IsLake(id)) {
                            roomVisual[roomVisual.GetLength(0) - pos.y - 2, pos.x] = 'o';
                        }
                    }
                }
            }

            for (int y = 0; y < roomVisual.GetLength(0); y++) {
                string line = "";
                for (int x = 0; x < roomVisual.GetLength(1); x++) {
                    line += roomVisual[y, x];
                }
                print($"Line {y}: {line}");
            }
            // TODO: write this ^ and IDs into files
        }

        private static void Fill2DArray<T> (T[,] array, T value) {
            for (int y = 0; y < array.GetLength(0); y++) {
                for (int x = 0; x < array.GetLength(1); x++) {
                    array[y, x] = value;
                }
            }
        }

        private static bool IsSolidWall(int id) {
            if (RoomGenerator.solidWallIDs == null) {
                RoomGenerator.solidWallIDs = new HashSet<int> {
                    ObjectNameToID("DoorBlue02"),
                    ObjectNameToID("DoorBlue04"),
                    ObjectNameToID("DoorBlue05"),
                    ObjectNameToID("DoorBlue06"),
                    ObjectNameToID("DoorBlue07"),
                    ObjectNameToID("DoorBlue08"),
                    ObjectNameToID("DoorBlue09"),
                    ObjectNameToID("DoorBlue10"),
                    ObjectNameToID("WallBlue01"),
                    ObjectNameToID("WallBlue02"),
                    ObjectNameToID("WallBlue03"),
                    ObjectNameToID("WallBlue04"),
                    ObjectNameToID("WallBlue05"),
                    ObjectNameToID("WallBlue06"),
                    ObjectNameToID("WallBlue07"),
                    ObjectNameToID("WallBlue08"),
                    ObjectNameToID("WallBlue09"),
                    ObjectNameToID("WallBlue10"),
                    ObjectNameToID("WallBlue11"),
                    ObjectNameToID("TwoConnectorBlue03"),
                    ObjectNameToID("TwoConnectorBlue04"),
                    ObjectNameToID("TwoConnectorBlue07"),
                    ObjectNameToID("TwoConnectorBlue08"),
                    ObjectNameToID("TwoConnectorBlue09"),
                    ObjectNameToID("TwoConnectorBlue10"),
                    ObjectNameToID("TwoConnectorBlue11"),
                    ObjectNameToID("TwoConnectorBlue12"),
                    ObjectNameToID("TwoConnectorBlue13"),
                    ObjectNameToID("TwoConnectorBlue14"),
                    ObjectNameToID("TwoConnectorBlue16"),
                    ObjectNameToID("TwoConnectorBlue17"),
                    ObjectNameToID("TwoConnectorBlue18"),
                    ObjectNameToID("TwoConnectorBlue19"),
                    ObjectNameToID("TwoConnectorBlue20"),
                    ObjectNameToID("ThreeConnectorBlue03"),
                    ObjectNameToID("ThreeConnectorBlue04"),
                    ObjectNameToID("ThreeConnectorBlue06"),
                    ObjectNameToID("ThreeConnectorBlue07"),
                    ObjectNameToID("ThreeConnectorBlue08"),
                    ObjectNameToID("ThreeConnectorBlue09"),
                    ObjectNameToID("ThreeConnectorBlue10"),
                    ObjectNameToID("ThreeConnectorBlue11"),
                    ObjectNameToID("ThreeConnectorBlue12"),
                    ObjectNameToID("ThreeConnectorBlue13"),
                    ObjectNameToID("ThreeConnectorBlue17"),
                    ObjectNameToID("FourConnectorBlue01"),
                    ObjectNameToID("ArchBlue01"),
                    ObjectNameToID("ArchBlue02"),
                    ObjectNameToID("EdgeBlue01"),
                    ObjectNameToID("EdgeBlue02"),
                    ObjectNameToID("EdgeBlue04"),
                    ObjectNameToID("EdgeWithArchBlue01"),
                    ObjectNameToID("EdgeWithArchBlue02"),
                    ObjectNameToID("HalfEdgeBlue01"),
                    ObjectNameToID("HalfEdgeBlue02"),
                    ObjectNameToID("QuarterEdgeBlue01"),
                    ObjectNameToID("QuarterEdgeBlue02")
                };
            }

            return RoomGenerator.solidWallIDs.Contains(id);
        }

        private static bool IsCosmeticWall(int id) {
            if (RoomGenerator.cosmeticWallIDs == null) {
                RoomGenerator.cosmeticWallIDs = new HashSet<int> {
                    ObjectNameToID("TwoConnectorBlue01"),
                    ObjectNameToID("TwoConnectorBlue02"),
                    ObjectNameToID("TwoConnectorBlue05"),
                    ObjectNameToID("TwoConnectorBlue06"),
                    ObjectNameToID("TwoConnectorBlue15"),
                    ObjectNameToID("ThreeConnectorBlue01"),
                    ObjectNameToID("ThreeConnectorBlue02"),
                    ObjectNameToID("ThreeConnectorBlue05"),
                    ObjectNameToID("ThreeConnectorBlue14"),
                    ObjectNameToID("ThreeConnectorBlue15"),
                    ObjectNameToID("ThreeConnectorBlue16"),
                    ObjectNameToID("FourConnectorBlue02"),
                    ObjectNameToID("EdgeBlue03")
                };
            }

            return RoomGenerator.cosmeticWallIDs.Contains(id);
        }

        private static bool IsFloor(int id) {
            if (RoomGenerator.floorIDs == null) {
                RoomGenerator.floorIDs = new HashSet<int> {
                    ObjectNameToID("FloorBlue01"),
                    ObjectNameToID("FloorBlue02"),
                    ObjectNameToID("FloorBlue03"),
                    ObjectNameToID("FloorBlue04"),
                    ObjectNameToID("FloorBlue05"),
                    ObjectNameToID("FloorBlue06"),
                    ObjectNameToID("FloorBlue07"),                    
                };
            }

            return RoomGenerator.floorIDs.Contains(id);
        }

        private static bool IsLake(int id) {
            return false;
        }

        private static (Vector2Int, Vector2Int) GetOriginAndDimensionsOfHandcraftedRoom(int roomRadius) {
            /*
            Returns the position of the left-most, bottom-most object of a handcrafted room around world position [0, 0],
            along with the dimensions of this room.
            */

            int topMost = -int.MaxValue;
            int leftMost = int.MaxValue;
            int rightMost = -int.MaxValue;
            int bottomMost = int.MaxValue;

            GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>() ;
            foreach (GameObject go in allObjects) {
                if (go.activeInHierarchy && ObjectNameToID(go.name.Split(' ')[0]) != -1) {
                    Vector2Int worldPos = Vector2Int.RoundToInt(go.transform.position);
                    if (worldPos.x >= -roomRadius && worldPos.x <= roomRadius && worldPos.y >= -roomRadius && worldPos.y <= roomRadius) {
                        topMost = worldPos.y > topMost ? worldPos.y : topMost;
                        leftMost = worldPos.x < leftMost ? worldPos.x : leftMost;
                        rightMost = worldPos.x > rightMost ? worldPos.x : rightMost;
                        bottomMost = worldPos.y < bottomMost ? worldPos.y : bottomMost; 
                    }
                }
            }

            if (topMost == -int.MaxValue) {
                throw new Exception("Handcrafted room is empty!");
            }

            return (new Vector2Int(leftMost, bottomMost), new Vector2Int(rightMost - leftMost + 1, topMost - bottomMost + 1));
        }

        private static int ObjectNameToID(string objectName) {
            if (RoomGenerator.objectNameToID_Table == null) {
                RoomGenerator.objectNameToID_Table = new Dictionary<string, int> {
                    { "BonesBlue01", 1 },
                    { "CobwebBlue01", 2 },
                    { "CobwebBlue02", 3 },
                    { "CobwebBlue03", 4 },
                    { "CobwebBlue04", 5 },
                    { "RocksBlue01", 6 },
                    { "SkullBlue01", 7 },                
                    { "SkullBlue02", 8 },
                    { "DoorBlue02", 9 },
                    { "DoorBlue04", 10 },
                    { "DoorBlue05", 11 },              
                    { "DoorBlue06", 12 },
                    { "DoorBlue07", 13 },
                    { "DoorBlue08", 14 },
                    { "DoorBlue09", 15 },
                    { "DoorBlue10", 16 },
                    { "FloorBlue01", 17 },
                    { "FloorBlue02", 18 },
                    { "FloorBlue03", 19 },
                    { "FloorBlue04", 20 },
                    { "FloorBlue05", 21 },
                    { "FloorBlue06", 22 },
                    { "FloorBlue07", 23 },
                    { "LakeCornerBlue01", 24 },
                    { "LakeCornerBlue02", 25 },
                    { "LakeCornerBlue03", 26},
                    { "LakeCornerBlue04", 27},
                    { "LakeDoubleEdgeBlue01", 28 },
                    { "LakeEdgeBlue01", 29 },
                    { "LakeEdgeBlue02", 30 },
                    { "LakeEdgeBlue03", 31 },
                    { "LakeEdgeBlue04", 32 },
                    { "LakeMiddleBlue01", 33 },
                    { "LakeSmallBlue01", 34 },
                    { "LakeUBlue01", 35 },
                    { "LakeUBlue02", 36 },
                    { "StairsBlue01", 37 },
                    { "StairsBlue02", 38 },
                    { "StairsBlue03", 39 },
                    { "StairsBlue04", 40 },
                    { "WallBlue01", 41 },
                    { "WallBlue02", 42 },
                    { "WallBlue03", 43 },
                    { "WallBlue04", 44 },
                    { "WallBlue05", 45 },
                    { "WallBlue06", 46 },
                    { "WallBlue07", 47 },
                    { "WallBlue08", 48 },
                    { "WallBlue09", 49 },
                    { "WallBlue10", 50 },
                    { "WallBlue11", 51 },
                    { "TwoConnectorBlue01", 52 },
                    { "TwoConnectorBlue02", 53 },
                    { "TwoConnectorBlue03", 54 },
                    { "TwoConnectorBlue04", 55 },
                    { "TwoConnectorBlue05", 56 },
                    { "TwoConnectorBlue06", 57 },
                    { "TwoConnectorBlue07", 58 },
                    { "TwoConnectorBlue08", 59 },
                    { "TwoConnectorBlue09", 60 },
                    { "TwoConnectorBlue10", 61 },
                    { "TwoConnectorBlue11", 62 },                    
                    { "TwoConnectorBlue12", 63 },
                    { "TwoConnectorBlue13", 64 },
                    { "TwoConnectorBlue14", 65 },
                    { "TwoConnectorBlue15", 66 },
                    { "TwoConnectorBlue16", 67 },
                    { "TwoConnectorBlue17", 68 },
                    { "TwoConnectorBlue18", 69 },
                    { "TwoConnectorBlue19", 70 },
                    { "TwoConnectorBlue20", 71 },
                    { "ThreeConnectorBlue01", 72 },
                    { "ThreeConnectorBlue02", 73 },
                    { "ThreeConnectorBlue03", 74 },                  
                    { "ThreeConnectorBlue04", 75 },
                    { "ThreeConnectorBlue05", 76 },
                    { "ThreeConnectorBlue06", 77 },
                    { "ThreeConnectorBlue07", 78 },
                    { "ThreeConnectorBlue08", 79 },
                    { "ThreeConnectorBlue09", 80 },
                    { "ThreeConnectorBlue10", 81 },
                    { "ThreeConnectorBlue11", 82 },
                    { "ThreeConnectorBlue12", 83 },
                    { "ThreeConnectorBlue13", 84 },
                    { "ThreeConnectorBlue14", 85 },
                    { "ThreeConnectorBlue15", 86 },
                    { "ThreeConnectorBlue16", 87 },
                    { "ThreeConnectorBlue17", 88 },
                    { "FourConnectorBlue01", 89 },
                    { "FourConnectorBlue02", 90 },
                    { "ArchBlue01", 91 },
                    { "ArchBlue02", 92 },
                    { "EdgeBlue01", 93 },
                    { "EdgeBlue02", 94 },
                    { "EdgeBlue03", 95 },
                    { "EdgeBlue04", 96 },
                    { "EdgeWithArchBlue01", 97 },
                    { "EdgeWithArchBlue02", 98 },
                    { "HalfEdgeBlue01", 99 },
                    { "HalfEdgeBlue02", 100 },
                    { "QuarterEdgeBlue01", 101 },
                    { "QuarterEdgeBlue02", 102 },
                    { "VoidBlue01", 103 }
                };
            }

            int value;
            bool hasValue = RoomGenerator.objectNameToID_Table.TryGetValue(objectName, out value);

            if (hasValue) {
                return value;
            }

            return -1;
        }


        

        /* 
        ###############################################################
        ######  METHODS BELOW ARE DEPRECATED, DELETE THEM LATER  ######
        ###############################################################
        */

        private void InitWallFlags() {
            // Returns a list with lists of flags for each type of wall
            /*
            TODO: write a better description, both here and to the method that selects a wall
            */

            /*
            This could be made slightly more efficient if it was initialized once per game, not once per room, but this 
            optimization can wait for now. (possibly put this into RoomManager)
            */


            /*
            Each flag is made of 3 bits, each corresponds to one tile type. If the bit is 1, that tile type is allowed.
            Order of tile types: Wall, Floor, Void
            */


            // TODO: this shouldn't be recreated every time this method is called, this should be cached in this class

            this.listOfWallsWithListsOfFlags = new List<(GameObject, List<List<int>>)> {
                (edgeBlue01Obj, new List<List<int>> {
                    new List<int> {
                        0b111, 0b100, 0b110,
                        0b101, 0b100, 0b010,
                        0b101, 0b100, 0b011,
                        0b111, 0b111, 0b111
                    },
                    new List<int> {
                        0b111, 0b111, 0b111,
                        0b101, 0b100, 0b100,
                        0b101, 0b100, 0b010,
                        0b111, 0b111, 0b111
                    }
                }),
                
                (edgeBlue02Obj, new List<List<int>> {
                    new List<int> {
                        0b110, 0b100, 0b111,
                        0b010, 0b100, 0b101,
                        0b011, 0b111, 0b101,
                        0b111, 0b111, 0b111
                    },
                    new List<int> {
                        0b111, 0b111, 0b111,
                        0b100, 0b100, 0b101,
                        0b010, 0b100, 0b101,
                        0b111, 0b111, 0b111
                    }
                }),
                
                // just for testing, this will match anything
                
                (fourConnectorBlue02Obj, new List<List<int>> {
                    new List<int> {
                        0b111, 0b111, 0b111,
                        0b111, 0b111, 0b111,
                        0b111, 0b111, 0b111,
                        0b111, 0b111, 0b111
                    },
                })
                
            };
        }

        private GameObject SelectWall(int x, int y, string[] roomVisual) {

            List<int> neighbourhood = GetRelevantNeighbourhood(x, y, roomVisual);
            

            if (x == 0 && y == 0) {
                // testing
                print("Tile types in the neighbourhood of tile at x == 0 and y == 0");
                foreach (int tileType in neighbourhood) {
                    print(tileType);
                }
            }
            

            foreach ((GameObject wall, List<List<int>> listOfFlags) in listOfWallsWithListsOfFlags) {
                foreach (List<int> flags in listOfFlags) {
                    bool allMatching = true;
                    for (int i = 0; i < 12; i++) {
                        if ((neighbourhood[i] & flags[i]) == 0) {
                            allMatching = false;
                            break;
                        }
                    }
                    if (allMatching) {
                        return wall;
                    }
                }
            }

            throw new Exception("Error in RoomGenerator: Could not find any wall to select!");
        }

        private List<int> GetRelevantNeighbourhood(int x, int y, string[] roomVisual) {
            /* Returns a list with values of all 12 relevant neighbouring tile types, as shown in this image:
                    .  .  .
                    .  _  .
                    .  .  .
                    .  .  .
            
            Symbol _ represents the current tile. If a tile is out of the room's bounds, it is viewed as void.
            0b100 represents Wall, 0b010 represents Floor, 0b001 represents Void.
            */

            List<int> neighbourhood = new List<int>();

            for (int yy = 1; yy >= -2; yy--) {
                for (int xx = -1; xx <= 1; xx++) {
                    if (!IsWithinRoomBounds(x + xx, y + yy, roomVisual)) {
                        neighbourhood.Add(0b001);
                    } else {
                        if (roomVisual[y + yy][x + xx] == '#') {
                            neighbourhood.Add(0b100);
                        } else if (roomVisual[y + yy][x + xx] == '.') {
                            neighbourhood.Add(0b010);
                        } else if (roomVisual[y + yy][x + xx] == '-') {
                            neighbourhood.Add(0b001);
                        } else {
                            throw new Exception("Encountered unknown symbol in roomVisual when generating a room!");
                        }
                    }
                }
            }

            return neighbourhood;
        }

        
        private bool IsWall(int x, int y, string[] roomVisual) {
            if (IsWithinRoomBounds(x, y, roomVisual)) {
                return roomVisual[y][x] == '#';
            }
            return false;
        }

        private bool IsVoid(int x, int y, string[] roomVisual) {
            if (IsWithinRoomBounds(x, y, roomVisual)) {
                return roomVisual[y][x] == '-';
            }
            return true;
        }

        private bool IsFloor(int x, int y, string[] roomVisual) {
            if (IsWithinRoomBounds(x, y, roomVisual)) {
                return roomVisual[y][x] == '.';
            }
            return false;
        }

        private bool IsWithinRoomBounds(int x, int y, string[] roomVisual) {
            return x >= 0 && y >= 0 && y < roomVisual.Count() && x < roomVisual[0].Count();
        }
    }
}
