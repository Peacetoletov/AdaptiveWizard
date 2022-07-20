using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

/*
RoomGenerator chooses and instantiates prefabs in a room.
*/

/* TODO:
    - randomized floor
    - randomized back walls
    - all other kinds of walls (chosen based on neighbouring tiles)
    - doors
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
        List<(GameObject, List<List<int>>)> listOfWallsWithListsOfFlags;


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

        public List<Door> GetDoors() {
            return doors;
        }
    }
}
