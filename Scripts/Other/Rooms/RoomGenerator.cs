using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Globalization;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

/*
RoomGenerator chooses and instantiates prefabs in a room.
*/

/* CURRENT TODO: 
Save handcrafted rooms into a file. Create a new folder (named Room1, Room2 etc.) and create 2 files here.
The first file will contain the id of each object in the room, with values -1 in spaces with no object. 
The second file will contain visual representation of the room, which will be used for enemy pathfinding.
I can probably remove doors from the visual representation of the room, because they are not relevant to enemies
(they serve the same function as walls to enemies). I should add lakes to the visual representation, because they
might be relevant to enemies with ranged attacks. This can however wait.  
*/

/* ADDITIONAL TODO:
Check if all back walls are truly equivalent. I have a feeling that some are supposed to be used when there is floor
on the left/right, based on a darker vertical line at the side, possibly to create a bigger contrast between the wall
and floor. These vertical lines also look kind of weird when in the middle of a long back wall.
    TODO: *correctly* randomize back walls
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

        // Path to saved rooms
        const string baseRoomsPath = @"Assets/Resources/Saved files/Rooms";

        // Specifies culture to enforce dot as a float separator (as opposed to comma)
        private static readonly System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");

        // list of doors in this room
        private List<Door> doors = new List<Door>();    

        // TODO: document how flags are used
        // NEWER TODO: remove this
        List<(GameObject, List<List<int>>)> listOfWallsWithListsOfFlags;

        // Class containing all information that needs to be stored in a file to recreate a room
        private class RoomObjectData 
        {
            private readonly int id;
            private readonly Vector2 pos;
            private readonly Quaternion rotation;
            
            public int Id { 
                get { return id; } 
            }
            public Vector2 Pos {
                get { return pos; }
            }
            public Quaternion Rotation { 
                get { return rotation; }
            }

            public RoomObjectData(int id, Vector2 pos, Quaternion rotation) {
                this.id = id;
                this.pos = pos;
                this.rotation = rotation;
            }
        }

        // Class which for each side determines whether there should be doors or walls on that side of the room
        public class RoomDoorFlags
        {
            private readonly bool top;
            private readonly bool left;
            private readonly bool right;
            private readonly bool bottom;

            public bool Top {
                get { return top; }
            }
            public bool Left {
                get { return left; }
            }
            public bool Right {
                get { return right; }
            }
            public bool Bottom {
                get { return bottom; }
            }

            public RoomDoorFlags(bool top, bool left, bool right, bool bottom) {
                this.top = top;
                this.left = left;
                this.right = right;
                this.bottom = bottom;
            }
        }

        // Caching
        private static Dictionary<string, int> objectNameToIDTable;         // Table mapping GameObject scene names to IDs 
        private static Dictionary<int, GameObject> objectIDToPrefabTable;   // Table mapping object IDs to GameObject prefabs
        private static HashSet<int> solidWallIDs;
        private static HashSet<int> cosmeticWallIDs;                        // Cosmetic walls act as floors on position x,y and as solid walls on position x,y-1 (1 space below)
        private static HashSet<int> verticalDoorIDs;                        // Vertical doors act as walls on position x,y-1        // remove later
        private static HashSet<int> floorIDs;
        private static HashSet<int> lakeIDs;
        private static HashSet<int> controlIDs;                             // control objects aren't real tiles, instead they represent sets of tiles which can be dynamically chosen 


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

            print("Saving a handcrafted room.");

            List<RoomObjectData> roomObjectsData = GatherObjectsInRoom(roomRadius);
            CreateFiles(roomObjectsData);
        }

        private static List<RoomObjectData> GatherObjectsInRoom(int roomRadius) {
            (Vector2 origin, Vector2Int dimensions) = GetOriginAndDimensionsOfHandcraftedRoom(roomRadius);
            char[,] roomVisual = new char[dimensions.y + 1, dimensions.x];      // Adding 1 to height is needed because cosmetic walls take up 2 spaces
            Fill2DArray(roomVisual, '-');
            List<RoomObjectData> roomObjectsData = new List<RoomObjectData>();

            GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>() ;
            foreach (GameObject go in allObjects) {
                int id = ObjectNameToID(go.name.Split(' ')[0]);
                if (go.activeInHierarchy && id != -1) {
                    Vector2 pos = ((Vector2) go.transform.position) - origin;
                    if (pos.x >= -roomRadius && pos.x <= roomRadius && pos.y >= -roomRadius && pos.y <= roomRadius) {
                        // AddObjectToRoomVisual(id, Vector2Int.RoundToInt(pos), roomVisual);
                        roomObjectsData.Add(new RoomObjectData(id, pos, go.transform.rotation));
                    }                        
                }
            }

            return roomObjectsData;
        }

        private static void AddObjectToRoomVisual(int id, Vector2Int pos, char[,] roomVisual) {
            /*
            TODO: change when roomVisual is created. Currently, I create it when saving the room and write it into a file alongside
            room objects. Instead, I should only write a file with objects. When loading the room and reading objects from a file,
            create roomVisual based on the objects read. This probably slightly increases computational time but also removes the
            need to open another file so the impact on performance should be minimal.
            */
            if (IsSolidWall(id)) {
                roomVisual[roomVisual.GetLength(0) - pos.y - 2, pos.x] = '#';
            } else if (IsCosmeticWall(id)) {
                if (roomVisual[roomVisual.GetLength(0) - pos.y - 2, pos.x] != '#') {
                    // Floor must not overwrite doors
                    roomVisual[roomVisual.GetLength(0) - pos.y - 2, pos.x] = '.';
                }
                roomVisual[roomVisual.GetLength(0) - pos.y - 1, pos.x] = '#';
            } else if (IsVerticalDoor(id)) {
                // TODO: remove this later when I introduce dynamic creation of door sections 
                roomVisual[roomVisual.GetLength(0) - pos.y - 1, pos.x] = '#';
            } else if (IsFloor(id)) {
                if (roomVisual[roomVisual.GetLength(0) - pos.y - 2, pos.x] != '#') {
                    // Floor must not overwrite doors
                    roomVisual[roomVisual.GetLength(0) - pos.y - 2, pos.x] = '.';
                }
            } else if (IsLake(id)) {
                roomVisual[roomVisual.GetLength(0) - pos.y - 2, pos.x] = 'o';
            }
        }

        private static void CreateFiles(List<RoomObjectData> roomObjectsData) {
            string dirPath = CreateRoomDirectory();
            CreateFileWithObjectIDs(dirPath, roomObjectsData);
        }

        private static string CreateRoomDirectory() {
            int roomID = 1;
            string dirPath;
            while (Directory.Exists(dirPath = baseRoomsPath + @"/Room" + roomID)) {
                roomID++;
            }
            Directory.CreateDirectory(dirPath);
            return dirPath;
        }

        private static void CreateFileWithObjectIDs(string dirPath, List<RoomObjectData> roomObjectsData) {
            using (StreamWriter writer = new StreamWriter(dirPath + @"/objects.csv")) {  
                writer.WriteLine("ID,Position X,Position Y,Rotation X,Rotation Y,Rotation Z");
                foreach (RoomObjectData objData in roomObjectsData) {
                    string line = Convert.ToString(objData.Id, RoomGenerator.cultureInfo) + "," + 
                                    Convert.ToString(objData.Pos.x, RoomGenerator.cultureInfo) + "," + 
                                    Convert.ToString(objData.Pos.y, RoomGenerator.cultureInfo) + "," + 
                                    Convert.ToString(objData.Rotation.x, RoomGenerator.cultureInfo) + "," + 
                                    Convert.ToString(objData.Rotation.y, RoomGenerator.cultureInfo) + "," +
                                    Convert.ToString(objData.Rotation.z, RoomGenerator.cultureInfo);
                    writer.WriteLine(line);
                }
            }
        }

        private static void CreateFileWithRoomVisual(string dirPath, char[,] roomVisual) {
            using (StreamWriter writer = new StreamWriter(dirPath + @"/visual.txt")) {  
                for (int y = 0; y < roomVisual.GetLength(0); y++) {
                    string line = "";
                    for (int x = 0; x < roomVisual.GetLength(1); x++) {
                        line += roomVisual[y, x];
                    }
                    writer.WriteLine(line);    
                } 
            }
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
                    ObjectNameToID("DoorBlue07"),
                    ObjectNameToID("DoorBlue09"),
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

        private static bool IsVerticalDoor(int id) {
            if (RoomGenerator.verticalDoorIDs == null) {
                RoomGenerator.verticalDoorIDs = new HashSet<int> {
                    ObjectNameToID("DoorBlue02"),
                    ObjectNameToID("DoorBlue04"),
                    ObjectNameToID("DoorBlue05"),
                    ObjectNameToID("DoorBlue06"),
                    ObjectNameToID("DoorBlue08"),
                    ObjectNameToID("DoorBlue10"),
                };
            }

            return RoomGenerator.verticalDoorIDs.Contains(id);
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
            if (RoomGenerator.lakeIDs == null) {
                RoomGenerator.lakeIDs = new HashSet<int> {
                    ObjectNameToID("LakeCornerBlue01"),
                    ObjectNameToID("LakeCornerBlue02"),
                    ObjectNameToID("LakeCornerBlue03"),
                    ObjectNameToID("LakeCornerBlue04"),
                    ObjectNameToID("LakeDoubleEdgeBlue01"),
                    ObjectNameToID("LakeEdgeBlue01"),
                    ObjectNameToID("LakeEdgeBlue02"),
                    ObjectNameToID("LakeEdgeBlue03"),
                    ObjectNameToID("LakeEdgeBlue04"),
                    ObjectNameToID("LakeMiddleBlue01"),
                    ObjectNameToID("LakeSmallBlue01"),
                    ObjectNameToID("LakeUBlue01"),
                    ObjectNameToID("LakeUBlue02")
                };
            }

            return RoomGenerator.lakeIDs.Contains(id);
        }

        private static bool IsControl(int id) {
            if (RoomGenerator.controlIDs == null) {
                RoomGenerator.controlIDs = new HashSet<int> {
                    ObjectNameToID("DoorSectionTop"),
                    ObjectNameToID("DoorSectionLeft"),
                    ObjectNameToID("DoorSectionRight"),
                    ObjectNameToID("DoorSectionBottom")
                };
            }

            return RoomGenerator.controlIDs.Contains(id);
        }

        private static (Vector2, Vector2Int) GetOriginAndDimensionsOfHandcraftedRoom(int roomRadius) {
            /*
            Returns the position of the left-most, bottom-most object of a handcrafted room around world position [0, 0],
            along with the dimensions of this room.
            */

            float topMost = float.NegativeInfinity;
            float leftMost = float.PositiveInfinity;
            float rightMost = float.NegativeInfinity;
            float bottomMost = float.PositiveInfinity;

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

            if (float.IsInfinity(topMost)) {
                throw new Exception("Handcrafted room is empty!");
            }

            return (new Vector2(leftMost, bottomMost), new Vector2Int((int) Math.Round(rightMost - leftMost + 1), (int) Math.Round(topMost - bottomMost + 1)));
        }

        public void LoadRoom(int roomID, Vector3 origin, RoomDoorFlags doorFlags) {
            print($"Loading room {roomID}");
            string path = baseRoomsPath + @"/Room" + roomID + @"/objects.csv";
            try {
                string[] lines = System.IO.File.ReadAllLines(path);
                for (int i = 1; i < lines.Length; i++) {
                    LoadObject(lines[i], origin, doorFlags);
                }
            } 
            catch (Exception e) {
                Debug.Log($"An error occured when trying to read file at {path}. Exception message: {e.Message}");
                throw e;
            }
        }

        private void LoadObject(string csvObjectData, Vector3 origin, RoomDoorFlags doorFlags) {
            // CSV data format: ID,Position X,Position Y,Rotation X,Rotation Y,Rotation Z
            
            string[] data = csvObjectData.Split(',');
            try {
                int id = int.Parse(data[0], NumberStyles.Any, RoomGenerator.cultureInfo);
                Vector3 pos = origin + new Vector3(float.Parse(data[1], NumberStyles.Any, RoomGenerator.cultureInfo), 
                                                   float.Parse(data[2], NumberStyles.Any, RoomGenerator.cultureInfo));
                if (IsControl(id)) {
                    ProcessControlObject(id, pos, doorFlags);
                } else {
                    Vector3 rotation = new Vector3(float.Parse(data[3], NumberStyles.Any, RoomGenerator.cultureInfo), 
                                                   float.Parse(data[4], NumberStyles.Any, RoomGenerator.cultureInfo), 
                                                   float.Parse(data[5], NumberStyles.Any, RoomGenerator.cultureInfo));
                    Instantiate(ObjectIDToPrefab(id), pos, Quaternion.Euler(rotation.x, rotation.y, rotation.z));
                }
                
            }
            catch (FormatException) {
                Debug.Log($"Unable to parse '{data}'");
            }
            
        }

        private void ProcessControlObject(int id, Vector3 pos, RoomDoorFlags doorFlags) {
            if (id == ObjectNameToID("DoorSectionTop")) {
                InstantiateDoorSectionTop(pos, doorFlags.Top);
            } else if (id == ObjectNameToID("DoorSectionLeft")) {
                InstantiateDoorSectionLeft(pos, doorFlags.Left);
            } else if (id == ObjectNameToID("DoorSectionRight")) {
                InstantiateDoorSectionRight(pos, doorFlags.Right);
            } else if (id == ObjectNameToID("DoorSectionBottom")) {
                InstantiateDoorSectionBottom(pos, doorFlags.Bottom);
            }
            
        }

        private void InstantiateDoorSectionTop(Vector3 pos, bool door) {
            if (door) {
                Instantiate(edgeBlue01Obj, pos + new Vector3(-1, 2, 0), Quaternion.identity);
                Instantiate(ChooseRandomFloor(), pos + new Vector3(0, 2, 0), Quaternion.identity);
                Instantiate(ChooseRandomFloor(), pos + new Vector3(1, 2, 0), Quaternion.identity);
                Instantiate(edgeBlue02Obj, pos + new Vector3(2, 2, 0), Quaternion.identity);
                Instantiate(twoConnectorBlue04Obj, pos + new Vector3(-1, 1, 0), Quaternion.identity);
                Instantiate(ChooseRandomFloor(), pos + new Vector3(0, 1, 0), Quaternion.identity);
                Instantiate(ChooseRandomFloor(), pos + new Vector3(1, 1, 0), Quaternion.identity);
                Instantiate(twoConnectorBlue03Obj, pos + new Vector3(2, 1, 0), Quaternion.identity);
                Instantiate(wallBlue11Obj, pos + new Vector3(-1, 0, 0), Quaternion.identity);
                Instantiate(ChooseRandomFloor(), pos + new Vector3(0, 0, 0), Quaternion.identity);
                Instantiate(doorBlue07Obj, pos + new Vector3(0, 0, 0), Quaternion.identity);
                Instantiate(ChooseRandomFloor(), pos + new Vector3(1, 0, 0), Quaternion.identity);
                Instantiate(doorBlue07Obj, pos + new Vector3(1, 0, 0), Quaternion.Euler(0, 180, 0));
                Instantiate(wallBlue02Obj, pos + new Vector3(2, 0, 0), Quaternion.identity);
            } else {
                Instantiate(edgeBlue04Obj, pos + new Vector3(-1, 1, 0), Quaternion.identity);
                Instantiate(edgeBlue04Obj, pos + new Vector3(0, 1, 0), Quaternion.identity);
                Instantiate(edgeBlue04Obj, pos + new Vector3(1, 1, 0), Quaternion.identity);
                Instantiate(edgeBlue04Obj, pos + new Vector3(2, 1, 0), Quaternion.identity);
                Instantiate(wallBlue08Obj, pos + new Vector3(-1, 0, 0), Quaternion.identity);
                Instantiate(wallBlue09Obj, pos + new Vector3(0, 0, 0), Quaternion.identity);
                Instantiate(wallBlue04Obj, pos + new Vector3(1, 0, 0), Quaternion.identity);
                Instantiate(wallBlue03Obj, pos + new Vector3(2, 0, 0), Quaternion.identity);
            }      
        }

        private void InstantiateDoorSectionLeft(Vector3 pos, bool door) {
            if (door) {
                Instantiate(edgeBlue04Obj, pos + new Vector3(-2, 1, 0), Quaternion.identity);
                Instantiate(edgeBlue04Obj, pos + new Vector3(-1, 1, 0), Quaternion.identity);
                Instantiate(twoConnectorBlue04Obj, pos + new Vector3(0, 1, 0), Quaternion.identity);
                Instantiate(wallBlue07Obj, pos + new Vector3(-2, 0, 0), Quaternion.identity);
                Instantiate(wallBlue10Obj, pos + new Vector3(-1, 0, 0), Quaternion.identity);
                Instantiate(wallBlue11Obj, pos + new Vector3(0, 0, 0), Quaternion.identity);
                Instantiate(doorBlue05Obj, pos + new Vector3(0, 0, 0), Quaternion.identity);
                Instantiate(ChooseRandomFloor(), pos + new Vector3(-2, -1, 0), Quaternion.identity);
                Instantiate(ChooseRandomFloor(), pos + new Vector3(-1, -1, 0), Quaternion.identity);
                Instantiate(ChooseRandomFloor(), pos + new Vector3(0, -1, 0), Quaternion.identity);
                Instantiate(doorBlue05Obj, pos + new Vector3(0, -1, 0), Quaternion.identity);
                Instantiate(edgeBlue03Obj, pos + new Vector3(-2, -2, 0), Quaternion.identity);
                Instantiate(edgeBlue03Obj, pos + new Vector3(-1, -2, 0), Quaternion.identity);
                Instantiate(twoConnectorBlue02Obj, pos + new Vector3(0, -2, 0), Quaternion.identity);
            } else {
                Instantiate(edgeBlue01Obj, pos + new Vector3(0, 1, 0), Quaternion.identity);
                Instantiate(edgeBlue01Obj, pos + new Vector3(0, 0, 0), Quaternion.identity);
                Instantiate(edgeBlue01Obj, pos + new Vector3(0, -1, 0), Quaternion.identity);
                Instantiate(edgeBlue01Obj, pos + new Vector3(0, -2, 0), Quaternion.identity);
            }            
        }

        private void InstantiateDoorSectionRight(Vector3 pos, bool door) {
            if (door) {
                Instantiate(twoConnectorBlue03Obj, pos + new Vector3(0, 1, 0), Quaternion.identity);
                Instantiate(edgeBlue04Obj, pos + new Vector3(1, 1, 0), Quaternion.identity);
                Instantiate(edgeBlue04Obj, pos + new Vector3(2, 1, 0), Quaternion.identity);
                Instantiate(wallBlue02Obj, pos + new Vector3(0, 0, 0), Quaternion.identity);
                Instantiate(doorBlue05Obj, pos + new Vector3(0, 0, 0), Quaternion.identity);
                Instantiate(wallBlue07Obj, pos + new Vector3(1, 0, 0), Quaternion.identity);
                Instantiate(wallBlue09Obj, pos + new Vector3(2, 0, 0), Quaternion.identity);
                Instantiate(ChooseRandomFloor(), pos + new Vector3(0, -1, 0), Quaternion.identity);
                Instantiate(doorBlue05Obj, pos + new Vector3(0, -1, 0), Quaternion.identity);
                Instantiate(ChooseRandomFloor(), pos + new Vector3(1, -1, 0), Quaternion.identity);
                Instantiate(ChooseRandomFloor(), pos + new Vector3(2, -1, 0), Quaternion.identity);
                Instantiate(twoConnectorBlue01Obj, pos + new Vector3(0, -2, 0), Quaternion.identity);
                Instantiate(edgeBlue03Obj, pos + new Vector3(1, -2, 0), Quaternion.identity);
                Instantiate(edgeBlue03Obj, pos + new Vector3(2, -2, 0), Quaternion.identity);
            } else {
                Instantiate(edgeBlue02Obj, pos + new Vector3(0, 1, 0), Quaternion.identity);
                Instantiate(edgeBlue02Obj, pos + new Vector3(0, 0, 0), Quaternion.identity);
                Instantiate(edgeBlue02Obj, pos + new Vector3(0, -1, 0), Quaternion.identity);
                Instantiate(edgeBlue02Obj, pos + new Vector3(0, -2, 0), Quaternion.identity);
            }  
        }

        private void InstantiateDoorSectionBottom(Vector3 pos, bool door) {
            if (door) {
                Instantiate(twoConnectorBlue02Obj, pos + new Vector3(-1, 0, 0), Quaternion.identity);
                Instantiate(ChooseRandomFloor(), pos + new Vector3(0, 0, 0), Quaternion.identity);
                Instantiate(ChooseRandomFloor(), pos + new Vector3(1, 0, 0), Quaternion.identity);
                Instantiate(twoConnectorBlue01Obj, pos + new Vector3(2, 0, 0), Quaternion.identity);
                Instantiate(edgeBlue01Obj, pos + new Vector3(-1, -1, 0), Quaternion.identity);
                Instantiate(ChooseRandomFloor(), pos + new Vector3(0, -1, 0), Quaternion.identity);
                Instantiate(doorBlue07Obj, pos + new Vector3(0, -1, 0), Quaternion.identity);
                Instantiate(ChooseRandomFloor(), pos + new Vector3(1, -1, 0), Quaternion.identity);
                Instantiate(doorBlue07Obj, pos + new Vector3(1, -1, 0), Quaternion.Euler(0, 180, 0));
                Instantiate(edgeBlue02Obj, pos + new Vector3(2, -1, 0), Quaternion.identity);
                Instantiate(edgeBlue01Obj, pos + new Vector3(-1, -2, 0), Quaternion.identity);
                Instantiate(ChooseRandomFloor(), pos + new Vector3(0, -2, 0), Quaternion.identity);
                Instantiate(ChooseRandomFloor(), pos + new Vector3(1, -2, 0), Quaternion.identity);
                Instantiate(edgeBlue02Obj, pos + new Vector3(2, -2, 0), Quaternion.identity);
                Instantiate(edgeBlue01Obj, pos + new Vector3(-1, -3, 0), Quaternion.identity);
                Instantiate(ChooseRandomFloor(), pos + new Vector3(0, -3, 0), Quaternion.identity);
                Instantiate(ChooseRandomFloor(), pos + new Vector3(1, -3, 0), Quaternion.identity);
                Instantiate(edgeBlue02Obj, pos + new Vector3(2, -3, 0), Quaternion.identity);
            } else {
                Instantiate(edgeBlue03Obj, pos + new Vector3(-1, 0, 0), Quaternion.identity);
                Instantiate(edgeBlue03Obj, pos + new Vector3(0, 0, 0), Quaternion.identity);
                Instantiate(edgeBlue03Obj, pos + new Vector3(1, 0, 0), Quaternion.identity);
                Instantiate(edgeBlue03Obj, pos + new Vector3(2, 0, 0), Quaternion.identity);
            } 
        }

        private GameObject ObjectIDToPrefab(int id) {
            if (RoomGenerator.objectIDToPrefabTable == null) {
                RoomGenerator.objectIDToPrefabTable = new Dictionary<int, GameObject> {
                    { ObjectNameToID("BonesBlue01"), bonesBlue01Obj },
                    { ObjectNameToID("CobwebBlue01"), cobwebBlue01Obj },
                    { ObjectNameToID("CobwebBlue02"), cobwebBlue02Obj },
                    { ObjectNameToID("CobwebBlue03"), cobwebBlue03Obj },
                    { ObjectNameToID("CobwebBlue04"), cobwebBlue04Obj },                    
                    { ObjectNameToID("RocksBlue01"), rocksBlue01Obj },
                    { ObjectNameToID("SkullBlue01"), skullBlue01Obj },
                    { ObjectNameToID("SkullBlue02"), skullBlue02Obj },
                    { ObjectNameToID("DoorBlue02"), doorBlue02Obj },
                    { ObjectNameToID("DoorBlue04"), doorBlue04Obj },
                    { ObjectNameToID("DoorBlue05"), doorBlue05Obj },
                    { ObjectNameToID("DoorBlue06"), doorBlue06Obj },
                    { ObjectNameToID("DoorBlue07"), doorBlue07Obj },
                    { ObjectNameToID("DoorBlue08"), doorBlue08Obj },
                    { ObjectNameToID("DoorBlue09"), doorBlue09Obj },
                    { ObjectNameToID("DoorBlue10"), doorBlue10Obj },
                    { ObjectNameToID("FloorBlue01"), floorBlue01Obj },
                    { ObjectNameToID("FloorBlue02"), floorBlue02Obj },
                    { ObjectNameToID("FloorBlue03"), floorBlue03Obj },
                    { ObjectNameToID("FloorBlue04"), floorBlue04Obj },
                    { ObjectNameToID("FloorBlue05"), floorBlue05Obj },
                    { ObjectNameToID("FloorBlue06"), floorBlue06Obj },
                    { ObjectNameToID("FloorBlue07"), floorBlue07Obj },
                    { ObjectNameToID("LakeCornerBlue01"), lakeCornerBlue01Obj },
                    { ObjectNameToID("LakeCornerBlue02"), lakeCornerBlue02Obj },
                    { ObjectNameToID("LakeCornerBlue03"), lakeCornerBlue03Obj },
                    { ObjectNameToID("LakeCornerBlue04"), lakeCornerBlue04Obj },
                    { ObjectNameToID("LakeDoubleEdgeBlue01"), lakeDoubleEdgeBlue01Obj },
                    { ObjectNameToID("LakeEdgeBlue01"), lakeEdgeBlue01Obj },
                    { ObjectNameToID("LakeEdgeBlue02"), lakeEdgeBlue02Obj },
                    { ObjectNameToID("LakeEdgeBlue03"), lakeEdgeBlue03Obj },
                    { ObjectNameToID("LakeEdgeBlue04"), lakeEdgeBlue04Obj },
                    { ObjectNameToID("LakeMiddleBlue01"), lakeMiddleBlue01Obj },
                    { ObjectNameToID("LakeSmallBlue01"), lakeSmallBlue01Obj },
                    { ObjectNameToID("LakeUBlue01"), lakeUBlue01Obj },
                    { ObjectNameToID("LakeUBlue02"), lakeUBlue02Obj },
                    { ObjectNameToID("StairsBlue01"), stairsBlue01Obj },
                    { ObjectNameToID("StairsBlue02"), stairsBlue02Obj },
                    { ObjectNameToID("StairsBlue03"), stairsBlue03Obj },
                    { ObjectNameToID("StairsBlue04"), stairsBlue04Obj },
                    { ObjectNameToID("WallBlue01"), wallBlue01Obj },
                    { ObjectNameToID("WallBlue02"), wallBlue02Obj },
                    { ObjectNameToID("WallBlue03"), wallBlue03Obj },
                    { ObjectNameToID("WallBlue04"), wallBlue04Obj },
                    { ObjectNameToID("WallBlue05"), wallBlue05Obj },
                    { ObjectNameToID("WallBlue06"), wallBlue06Obj },
                    { ObjectNameToID("WallBlue07"), wallBlue07Obj },
                    { ObjectNameToID("WallBlue08"), wallBlue08Obj },
                    { ObjectNameToID("WallBlue09"), wallBlue09Obj },
                    { ObjectNameToID("WallBlue10"), wallBlue10Obj },
                    { ObjectNameToID("WallBlue11"), wallBlue11Obj },
                    { ObjectNameToID("TwoConnectorBlue01"), twoConnectorBlue01Obj },
                    { ObjectNameToID("TwoConnectorBlue02"), twoConnectorBlue02Obj },
                    { ObjectNameToID("TwoConnectorBlue03"), twoConnectorBlue03Obj },
                    { ObjectNameToID("TwoConnectorBlue04"), twoConnectorBlue04Obj },
                    { ObjectNameToID("TwoConnectorBlue05"), twoConnectorBlue05Obj },
                    { ObjectNameToID("TwoConnectorBlue06"), twoConnectorBlue06Obj },
                    { ObjectNameToID("TwoConnectorBlue07"), twoConnectorBlue07Obj },
                    { ObjectNameToID("TwoConnectorBlue08"), twoConnectorBlue08Obj },
                    { ObjectNameToID("TwoConnectorBlue09"), twoConnectorBlue09Obj },
                    { ObjectNameToID("TwoConnectorBlue10"), twoConnectorBlue10Obj },
                    { ObjectNameToID("TwoConnectorBlue11"), twoConnectorBlue11Obj },
                    { ObjectNameToID("TwoConnectorBlue12"), twoConnectorBlue12Obj },
                    { ObjectNameToID("TwoConnectorBlue13"), twoConnectorBlue13Obj },
                    { ObjectNameToID("TwoConnectorBlue14"), twoConnectorBlue14Obj },
                    { ObjectNameToID("TwoConnectorBlue15"), twoConnectorBlue15Obj },
                    { ObjectNameToID("TwoConnectorBlue16"), twoConnectorBlue16Obj },
                    { ObjectNameToID("TwoConnectorBlue17"), twoConnectorBlue17Obj },
                    { ObjectNameToID("TwoConnectorBlue18"), twoConnectorBlue18Obj },
                    { ObjectNameToID("TwoConnectorBlue19"), twoConnectorBlue19Obj },
                    { ObjectNameToID("TwoConnectorBlue20"), twoConnectorBlue20Obj },
                    { ObjectNameToID("ThreeConnectorBlue01"), threeConnectorBlue01Obj },
                    { ObjectNameToID("ThreeConnectorBlue02"), threeConnectorBlue02Obj },
                    { ObjectNameToID("ThreeConnectorBlue03"), threeConnectorBlue03Obj },
                    { ObjectNameToID("ThreeConnectorBlue04"), threeConnectorBlue04Obj },
                    { ObjectNameToID("ThreeConnectorBlue05"), threeConnectorBlue05Obj },
                    { ObjectNameToID("ThreeConnectorBlue06"), threeConnectorBlue06Obj },
                    { ObjectNameToID("ThreeConnectorBlue07"), threeConnectorBlue07Obj },
                    { ObjectNameToID("ThreeConnectorBlue08"), threeConnectorBlue08Obj },
                    { ObjectNameToID("ThreeConnectorBlue09"), threeConnectorBlue09Obj },
                    { ObjectNameToID("ThreeConnectorBlue10"), threeConnectorBlue10Obj },
                    { ObjectNameToID("ThreeConnectorBlue11"), threeConnectorBlue11Obj },
                    { ObjectNameToID("ThreeConnectorBlue12"), threeConnectorBlue12Obj },
                    { ObjectNameToID("ThreeConnectorBlue13"), threeConnectorBlue13Obj },
                    { ObjectNameToID("ThreeConnectorBlue14"), threeConnectorBlue14Obj },
                    { ObjectNameToID("ThreeConnectorBlue15"), threeConnectorBlue15Obj },
                    { ObjectNameToID("ThreeConnectorBlue16"), threeConnectorBlue16Obj },
                    { ObjectNameToID("ThreeConnectorBlue17"), threeConnectorBlue17Obj },
                    { ObjectNameToID("FourConnectorBlue01"), fourConnectorBlue01Obj },
                    { ObjectNameToID("FourConnectorBlue02"), fourConnectorBlue02Obj },
                    { ObjectNameToID("ArchBlue01"), archBlue01Obj },
                    { ObjectNameToID("ArchBlue02"), archBlue02Obj },
                    { ObjectNameToID("EdgeBlue01"), edgeBlue01Obj },
                    { ObjectNameToID("EdgeBlue02"), edgeBlue02Obj },
                    { ObjectNameToID("EdgeBlue03"), edgeBlue03Obj },
                    { ObjectNameToID("EdgeBlue04"), edgeBlue04Obj },
                    { ObjectNameToID("EdgeWithArchBlue01"), edgeWithArchBlue01Obj },
                    { ObjectNameToID("EdgeWithArchBlue02"), edgeWithArchBlue02Obj },
                    { ObjectNameToID("HalfEdgeBlue01"), halfEdgeBlue01Obj },
                    { ObjectNameToID("HalfEdgeBlue02"), halfEdgeBlue02Obj },
                    { ObjectNameToID("QuarterEdgeBlue01"), quarterEdgeBlue01Obj },
                    { ObjectNameToID("QuarterEdgeBlue02"), quarterEdgeBlue02Obj },
                    { ObjectNameToID("VoidBlue01"), voidBlue01Obj }
                };
            }

            GameObject value;
            bool hasValue = RoomGenerator.objectIDToPrefabTable.TryGetValue(id, out value);

            if (hasValue) {
                return value;
            }

            Debug.Log($"Could not map object ID {id} to a prefab - such key is missing!");
            throw new ArgumentException($"Could not map object ID {id} to a prefab - such key is missing!");            
        }

        private static int ObjectNameToID(string objectName) {
            if (RoomGenerator.objectNameToIDTable == null) {
                RoomGenerator.objectNameToIDTable = new Dictionary<string, int> {
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
                    { "VoidBlue01", 103 },
                    { "DoorSectionTop", 104 },
                    { "DoorSectionLeft", 105 },
                    { "DoorSectionRight", 106 },
                    { "DoorSectionBottom", 107 }
                };
            }

            int value;
            bool hasValue = RoomGenerator.objectNameToIDTable.TryGetValue(objectName, out value);

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
