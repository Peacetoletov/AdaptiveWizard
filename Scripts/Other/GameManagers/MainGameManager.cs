using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdaptiveWizard.Assets.Scripts.UI;
using AdaptiveWizard.Assets.Scripts.Other.Rooms;
using AdaptiveWizard.Assets.Scripts.Player.Other;
using AdaptiveWizard.Assets.Scripts.Player.Inventory;


// TODO: Clean up manager classes, remove old code that got commented out
namespace AdaptiveWizard.Assets.Scripts.Other.GameManagers
{
    public class MainGameManager : MonoBehaviour
    {
        // build options
        public const bool minigame = false;     // must be false until room management is fully implemented

        // constants
        public const int PIXELS_PER_UNIT = 16;          // how many pixels are in 1 unit. Currently 16, might change to 32 in the future if I want more detailed art

        // public GameObjects used for instantiating
        public GameObject playerObj;
        public GameObject roomManagerObj;
        public GameObject minigameManagerObj;

        // testing, remove later
        public GameObject RoomIO_Obj;


        /* TODO: GameState will need to get reworked and changed to an int called UI_displayLayer. When the game is in normal state, it will be set to 0,
        because no UI will be displayed, with the exception of basic overlay (hp, mana, gold, current items, spell and active item boxes etc.). When the
        player pauses the game or opens a chest, UI_displayLayer will increment to 1. Here is the important part - if the player is interacting with UI
        and causes another layer of UI to pop up (such as when looking at the content of a chest, clicking on a "spell orb / 2 random spell upgrades" reward
        and then having to choose between two rewards in a separate UI from the chest's UI), UI_displayLayer will increment from 1 to 2, making it easy
        to make the initial chest content UI buttons temporarily inactive, for example.
        */
        public enum GameState {
            // Most common, player can freely move around
            ACTIVE,

            // Player cannot move and only interacts with gameplay related UI (chests)
            PARTIALLY_ACTIVE,

            // Player cannot move and only interacts with gameplay unrelated UI (menu, options)
            INACTIVE
        }
        private static GameState gameState = GameState.ACTIVE;
        private static Vector3 player_spawn_pos;
        private static GameObject player;                   // reference to the player
                                                            // I decided to make it static because I don't plan on having multiplayer PvP or coop modes

        private static UI_Manager UI_manager;
        private static MinigameManager minigameManager;
        private static RoomManager roomManager;

        
        private void Start() {
            InitializePlayer();

            MainGameManager.roomManager = Instantiate(roomManagerObj, Vector2.zero, Quaternion.identity).GetComponent<RoomManager>();

            MainGameManager.UI_manager = (UI_Manager) FindObjectOfType(typeof(UI_Manager));

            /*
            //test
            Items.PassiveItem test = new Items.HealthCrystal();
            InventoryNS.Inventory.passiveItemsManager.AddItem(test);
            Items.PassiveItem test2 = new Items.DivineSphere();
            InventoryNS.Inventory.passiveItemsManager.AddItem(test2);
            Items.PassiveItem test3 = new Items.DivineSphere();
            InventoryNS.Inventory.passiveItemsManager.AddItem(test3);
            */

            //MainGameManager.managerOfRoomManagers = Instantiate(managerOfRoomManagersObj, Vector2.zero, Quaternion.identity).GetComponent<ManagerOfRoomManagers>();
            if (minigame) {
                MainGameManager.minigameManager = Instantiate(minigameManagerObj, Vector2.zero, Quaternion.identity).GetComponent<MinigameManager>();
            }    
        }

        private void InitializePlayer() {
            GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
            Debug.Assert(playerObjects.Length <= 1);
            if (playerObjects.Length == 0) {
                // Create a new player object and assign it to this manager
                MainGameManager.player_spawn_pos = new Vector3(5, 5, 0);
                MainGameManager.player = Instantiate(playerObj, player_spawn_pos, Quaternion.identity) as GameObject;
            } else {
                // Assign the existing player object to this manager
                MainGameManager.player_spawn_pos = playerObjects[0].transform.position;
                MainGameManager.player = playerObjects[0];
            }
            player.GetComponent<PlayerGeneral>().Initialize();
        }

        private void Update() {
            if (IsGameActive()) {
                /*
                if (miniGame && spawnEnemiesTimer.UpdateAndCheck()) {
                    UpdateMiniGameSpawnPeriod();
                    this.spawnEnemiesTimer = new Timer(miniGameSpawnPeriod);
                    SpawnEnemyRandomlyInComplexRoom();       
                }
                */
                if (minigame) {
                    //minigameManager.Update();
                }

                // using active items
                if (Input.GetKeyDown(KeyCode.Alpha1)) {
                    InventoryManager.activeItemManager.UseItem();
                }

                // Saving a hand-crafted room
                if (Input.GetKeyDown(KeyCode.H)) {
                    RoomIO.SaveHandcraftedRoom();
                }
                
                // Loading a room (just testing for now)
                if (Input.GetKeyDown(KeyCode.L)) {
                    //RoomIO rg = new RoomIO();
                    RoomIO rg = Instantiate(RoomIO_Obj, Vector3.zero, Quaternion.identity).GetComponent<RoomIO>();
                    rg.LoadRoom(5, new Vector2(150, -30), new RoomIO.RoomDoorFlags(true, true, false, false));
                    //rg.LoadRoom(4, new Vector2(200, -30), new RoomIO.RoomDoorFlags(false, false, false, false));
                }
                
                // test
                /*
                if (Input.GetKeyDown(KeyCode.N)) {
                    InventoryNS.Inventory.passiveItemsManager.AddItem(new Items.HealthCrystal());
                }
                if (Input.GetKeyDown(KeyCode.M)) {
                    InventoryNS.Inventory.passiveItemsManager.AddItem(new Items.ManaCrystal());
                }
                if (Input.GetKeyDown(KeyCode.V)) {
                    InventoryNS.Inventory.activeItemsManager.AddItem(new Items.HealthPotion());
                }
                if (Input.GetKeyDown(KeyCode.B)) {
                    InventoryNS.Inventory.activeItemsManager.AddItem(new Items.ManaPotion());
                }
                */
            } 

            if (Input.GetKeyDown(KeyCode.P)) {
                if (gameState == GameState.ACTIVE) {
                    MainGameManager.gameState = GameState.INACTIVE;
                } else if (gameState == GameState.INACTIVE) {
                    MainGameManager.gameState = GameState.ACTIVE;
                }
            }
        }


        public void RestartGame() {
            GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
            foreach (GameObject o in allObjects) {
                if (o.activeInHierarchy && o.tag != "MainCamera" && o.tag != "GameController" && o.tag != "UIController" && o.tag != "Player") {
                    Destroy(o);
                }
            }

            player.GetComponent<PlayerGeneral>().ResetPlayer();
            MainGameManager.player.transform.position = player_spawn_pos;
            roomManager.Restart();
            if (minigame) {
                minigameManager.Restart();
            }
        }

        public static void SetGameState(GameState state) {
            MainGameManager.gameState = state;
        }

        public static bool IsGameActive() {
            return gameState == GameState.ACTIVE;
        }

        public static bool IsGamePartiallyActive() {
            return gameState == GameState.PARTIALLY_ACTIVE;
        }

        public static bool IsGameInactive() {
            return gameState == GameState.INACTIVE;
        }

        public static UI_Manager GetUI_Manager() {
            return UI_manager;
        }

        public static GameObject GetPlayer() {
            return player;
        }

        public static RoomManager GetRoomManager() {
            return roomManager;
        }
    }
}
