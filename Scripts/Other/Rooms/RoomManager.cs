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
            string[] roomVisual = new string[] {
                "-----######//######---",
                "----#.....#..#.....#--",
                "---#................#-",
                "###..................#",
                "#..................../",
                "#..................../",
                "###..................#",
                "---#................#-",
                "----#..............#--",
                "-----######//######---"
            };
            List<Teleporter> teleporters = new List<Teleporter> {
                new Teleporter(new Vector2(0, -5), 666),
                new Teleporter(new Vector2(0, -5), 666),
                new Teleporter(new Vector2(14, 0), 1),
                new Teleporter(new Vector2(14, 0), 1),
                new Teleporter(new Vector2(0, 5), 420),
                new Teleporter(new Vector2(0, 5), 420)
            };
            this.rooms[0].Init(new Vector2(-10, 1), roomVisual, RoomType.COMBAT, teleporters);
            

            
            //Room 2
            
            newRoom = Instantiate(roomObj, Vector3.zero, Quaternion.identity) as GameObject;
            this.rooms.Add(newRoom.GetComponent<Room>());
            roomVisual = new string[] {
                "--#######--",
                "-#.......#-",
                "#.........#",
                "/.........#",
                "/.........#",
                "#.........#",
                "-#.......#-",
                "--#######--"
            };
            teleporters = new List<Teleporter> {
                new Teleporter(new Vector2(-14, 0), 0),
                new Teleporter(new Vector2(-14, 0), 0),
            };
            rooms[1].Init(new Vector2(24, 2), roomVisual, RoomType.COMBAT, teleporters);
            
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
