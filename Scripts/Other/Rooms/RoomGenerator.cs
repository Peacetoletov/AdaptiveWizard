using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

/*
RoomGenerator chooses and instantiates prefabs in a room.
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
                        Instantiate(floorBlue01Obj, coordinates, Quaternion.identity);
                    } else if (symbol == '#') {
                        Instantiate(wallBlue01Obj, coordinates, Quaternion.identity);
                    } else if (symbol == '/') {
                        // door
                        GameObject newDoor = Instantiate(doorBlue07Obj, coordinates, Quaternion.identity) as GameObject;
                        this.doors.Add(newDoor.GetComponent<Door>());
                    } else {
                        // void space, generate nothing here (symbol == '-')
                    }
                }
            }
        }

        public List<Door> GetDoors() {
            return doors;
        }
    }
}
