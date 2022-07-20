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
            for (int y = 0; y < room.RoomHeight(); y++) {
                for (int x = 0; x < roomVisual[y].Length; x++) {
                    Vector3 coordinates = (Vector3) room.PositionInRoomToPositionInWorld(new Vector2Int(x, y));
                    char symbol = room.TileSymbolAtPosition(x, y);
                    if (symbol == '.') {
                        if (IsWall(x, y - 1, roomVisual)) {
                            Instantiate(SelectCosmeticWall(x, y, roomVisual), coordinates, Quaternion.identity);
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
                            Instantiate(SelectCosmeticWall(x, y, roomVisual), coordinates, Quaternion.identity);
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

        private GameObject SelectCosmeticWall(int x, int y, string[] roomVisual) {
            // Current tile is actually floor/void but will look like a wall to create an illusion of depth.
            
            /*
            I know that the current tile is non-wall and the tile below the current one is a wall.
            Which wall tile to select depends on the left, right and bottom neighbours of the wall below the
            current tile.
            */

            /*
            Order of operations in 'if' branches:
            Left > Right > Below
            Floor > Wall > Void
            */

            // Floor current
            if (IsFloor(x, y, roomVisual)) {
                // Wall bottom left
                if (IsWall(x - 1, y - 1, roomVisual)) {
                    // Wall bottom right
                    if (IsWall(x + 1, y - 1, roomVisual)) {
                        // Floor bottom bottom
                        if (IsFloor(x, y - 2, roomVisual)) {
                            return twoConnectorBlue15Obj;
                        }
                        // Non-floor bottom bottom
                        else {
                            return edgeBlue03Obj;
                        }
                    }
                    // Non-wall bottom right
                    else {
                        // Floor bottom bottom
                        if (IsFloor(x, y - 2, roomVisual)) {
                            return threeConnectorBlue16Obj;
                        }
                        // Non-floor bottom bottom
                        else {
                            // Floor bottom bottom left
                            if (IsFloor(x - 1, y - 2, roomVisual)) {
                                return threeConnectorBlue01Obj;
                            }
                            // Non-floor bottom bottom left
                            else {
                                return twoConnectorBlue02Obj;
                            }
                        }
                    }
                }
                // Non-wall bottom left
                else {
                    // Wall bottom right
                    if (IsWall(x + 1, y - 1, roomVisual)) {
                        // Floor bottom bottom
                        if (IsFloor(x, y - 2, roomVisual)) {
                            return threeConnectorBlue15Obj;
                        }
                        // Non-floor bottom bottom
                        else {
                            // Floor bottom bottom right
                            if (IsFloor(x + 1, y - 2, roomVisual)) {
                                return threeConnectorBlue02Obj;
                            }
                            // Non-floor bottom bottom right
                            else {
                                return twoConnectorBlue01Obj;
                            }
                        }
                    }
                    // Non-wall bottom right
                    else {
                        // Floor bottom bottom
                        if (IsFloor(x, y - 2, roomVisual)) {
                            return fourConnectorBlue02Obj;
                        }
                        // Non-floor bottom bottom
                        else {
                            return threeConnectorBlue14Obj;
                        }
                    }
                }
            }
            // Void current
            else {
                // Wall bottom left
                if (IsWall(x - 1, y - 1, roomVisual)) {
                    // Wall bottom right
                    if (IsWall(x + 1, y - 1, roomVisual)) {
                        return edgeBlue04Obj;
                    }
                    // Void bottom right
                    if (IsVoid(x + 1, y - 1, roomVisual)) {
                        return halfEdgeBlue02Obj;
                    }
                }
                // Void bottom left
                else if (IsVoid(x - 1, y - 1, roomVisual)) {
                    return halfEdgeBlue01Obj;
                }
            }
            


            // remove later, replace by an exception
            Assert.IsTrue(false);
            return lakeCornerBlue01Obj;
        }

        private GameObject SelectWall(int x, int y, string[] roomVisual) {
            /*
            This function selects a wall object to be instantiated at x, y. It looks at neighbouring tiles to
            determine which object to select. If there are multiple options, one is chosen randomly based on
            predefined priorities.
            */

            /*
            Order of operations in 'if' branches:
            Above > Left > Right > Below
            Floor > Wall > Void
            */

            /*
            EMERGENCY MEETING:
            I didn't realize how depth worked in this tileset. Turns out that some tiles are actually floors/voids
            despite having the sprite of a wall. This is the case every time there is a back wall - above it is
            a floor/void but has a wall sprite. This fact ruins all my previous work and I need to find a new solution,
            both to room representation (where do I put '#' symbols?) and to room generation. It also means additional
            work with colliders and possibly having the wall become semi-transparent when the player or enemies are
            behind it.

            Maybe I will look at the current tile and:
                - if it's void or floor, look at the 3 tiles below it (straight below, below and left, below and right). If any
                  of these are walls, place a "cosmetic" wall on the current tile. To determine which wall to select, I will also
                  need to look at tiles above and to the left and right.
                    - UPDATE: this doesn't work but maybe it can be fixed if I look at the left, right and bottom neighbours of the
                              wall below the current tile.
                - if it's wall, look at the tile straight below. If it's void, do nothing (the wall has already been placed by the
                  tile above). If it's wall, place a wall. Select it by looking at tiles to the left and right (maybe also above?).
                  Here, I'm looking for floor tiles. The number and position of neighbouring floor tiles determines the current wall.

            // Note: every wall needs to border with a floor (diagonals count too)
            //  This fact could be useful when determining which wall to select.
            */





            /*
            // Floor above
            if (IsFloor(x, y + 1, roomVisual)) {
                // Floor left
                if (IsFloor(x - 1, y, roomVisual)) {
                    // Wall right
                    if (IsWall(x + 1, y, roomVisual)) {
                        // Wall below
                        if (IsWall(x, y - 1, roomVisual)) {
                            return twoConnectorBlue01Obj;
                        }
                    }
                }
                // Wall left
                else if (IsWall(x - 1, y, roomVisual)) {
                    // Floor right
                    if (IsFloor(x + 1, y, roomVisual)) {
                        // Wall below
                        if (IsWall(x, y - 1, roomVisual)) {
                            return twoConnectorBlue02Obj;
                        }
                    }
                    // Wall right
                    else if (IsWall(x + 1, y, roomVisual)) {
                        // Void below
                        if (IsVoid(x, y + 1, roomVisual)) {
                            return edgeBlue03Obj;
                        }
                    }
                }
            }
            // Wall above
            else if (IsWall(x, y + 1, roomVisual)) {
                // Floor left
                if (IsFloor(x - 1, y, roomVisual)) {
                    // Void right
                    if (IsVoid(x + 1, y, roomVisual)) {
                        // Wall below
                        if (IsWall(x, y + 1, roomVisual)) {
                            return edgeBlue02Obj;
                        }
                    }
                }
                // Void left
                else if (IsVoid(x - 1, y, roomVisual)) {
                    // Wall below
                    if (IsWall(x, y - 1, roomVisual)) {
                        // Floor right
                        if (IsFloor(x + 1, y, roomVisual)) {
                            return edgeBlue01Obj;
                        }
                    }
                }

                // Left and right irrelevant, floor below
                if (IsFloor(x, y - 1, roomVisual)) {
                    return ChooseRandomBackWall();
                }
            }
            // Void above
            else if (IsVoid(x, y + 1, roomVisual)) {
                // Wall left
                if (IsWall(x - 1, y, roomVisual)) {
                    // Wall right
                    if (IsWall(x + 1, y, roomVisual)) {
                        // Wall below
                        if (IsWall(x, y - 1, roomVisual)) {
                            return edgeBlue04Obj;
                        }
                    }
                    // Void right
                    if (IsVoid(x + 1, y, roomVisual)) {
                        // Wall below
                        if (IsWall(x, y - 1, roomVisual)) {
                            return halfEdgeBlue02Obj;
                        }
                    }
                }
                // Void left
                else if (IsVoid(x - 1, y, roomVisual)) {
                    // Wall right
                    if (IsWall(x + 1, y, roomVisual)) {
                        // Wall below
                        if (IsWall(x, y - 1, roomVisual)) {
                            return halfEdgeBlue01Obj;
                        }
                    }
                }
            }
            */
           


            // remove later, replace by an exception
            return fourConnectorBlue02Obj;
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
