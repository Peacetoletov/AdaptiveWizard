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
namespace AdaptiveWizard.Assets.Scripts.Other.Rooms
{
    public class RoomManager : MonoBehaviour
    {

        // public GameObjects used for instantiating
        public GameObject roomObj;
        public GameObject roomIO_Obj;


        private List<Room> rooms;

        // Index of the room that the player is currently in
        private int curActiveRoomIndex;


        private void Start() {
            Restart();
        }

        public void Restart() {
            GenerateRooms();
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

        private void GenerateRooms() {
            this.rooms = new List<Room>();

            // Room 1
            GameObject newRoom = Instantiate(roomObj, Vector3.zero, Quaternion.identity) as GameObject;
            this.rooms.Add(newRoom.GetComponent<Room>());
            // Note: every wall needs to border with a floor (diagonals count too)
            //  This fact could be useful when determining which wall to select.
            
            List<Teleporter> teleporters = new List<Teleporter> {
                new Teleporter(new Vector2(0, -5), 666),
                new Teleporter(new Vector2(-5, 0), 69),
                new Teleporter(new Vector2(14, 0), 1),
                new Teleporter(new Vector2(0, 5), 420),
            };
            RoomIO rio = Instantiate(roomIO_Obj, Vector3.zero, Quaternion.identity).GetComponent<RoomIO>();
            Vector2 posOffset = new Vector2(0, 0);
            char[,] baseRoomVisual = rio.LoadRoom(10, posOffset, new RoomIO.RoomDoorFlags(true, true, true, true));
            this.rooms[0].Init(posOffset, baseRoomVisual, RoomType.COMBAT, teleporters);
            
        }

        public Room GetRoom(int roomIndex) {
            return rooms[roomIndex];
        }

        public int GetCurActiveRoomIndex() {
            return curActiveRoomIndex;
        }

        public void SetCurActiveRoomIndex(int curActiveRoomIndex) {
            this.curActiveRoomIndex = curActiveRoomIndex;
        }
    }
}
