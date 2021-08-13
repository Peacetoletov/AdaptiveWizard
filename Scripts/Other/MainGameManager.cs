using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Clean up manager classes, remove old code that got commented out
public class MainGameManager : MonoBehaviour
{
    // build options
    public const bool minigame = true;

    // public GameObjects used for instantiating
    public GameObject playerObj;
    public GameObject roomManagerObj;
    public GameObject minigameManagerObj;


    private static bool isGameActive = true;        // false if game is paused or UI is open, true otherwise
    private readonly Vector3 PLAYER_SPAWN_POS = new Vector3(-5.5f, -0.5f, 0.0f);
    private static GameObject player;                   // reference to the player
                                                        // I decided to make it static because I don't plan on having multiplayer PvP or coop modes

    private static UI_Manager UI_manager;
    private static MinigameManager minigameManager;
    private static RoomManager roomManager;

    
    private void Start() {
        CreateAndInitializePlayer();

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

        MainGameManager.roomManager = Instantiate(roomManagerObj, Vector2.zero, Quaternion.identity).GetComponent<RoomManager>();
        if (minigame) {
            MainGameManager.minigameManager = Instantiate(minigameManagerObj, Vector2.zero, Quaternion.identity).GetComponent<MinigameManager>();
        }    
    }

    private void CreateAndInitializePlayer() {
        MainGameManager.player = Instantiate(playerObj, PLAYER_SPAWN_POS, Quaternion.identity) as GameObject;
        player.GetComponent<PlayerGeneral>().Initialize();
    }

    private void Update() {
        if (isGameActive) {
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
                InventoryNS.Inventory.activeItemsManager.UseItem(0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2)) {
                InventoryNS.Inventory.activeItemsManager.UseItem(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3)) {
                InventoryNS.Inventory.activeItemsManager.UseItem(2);
            }

            /*
            // test
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
            MainGameManager.isGameActive = !isGameActive;
        }
    }


    public void RestartGame() {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject o in allObjects) {
            if (o.activeInHierarchy && o.tag != "MainCamera" && o.tag != "GameController" && o.tag != "UIController" && o.tag != "Player") {
                Destroy(o);
            }
        }
        /*
        // TODO: change
        //this.miniGameSpawnPeriod = INITIAL_ENEMY_SPAWN_PERIOD;
        player.GetComponent<PlayerGeneral>().ResetPlayer();
        MainGameManager.player.transform.position = PLAYER_SPAWN_POS;
        //GenerateRoom(false);
        */

        player.GetComponent<PlayerGeneral>().ResetPlayer();
        MainGameManager.player.transform.position = PLAYER_SPAWN_POS;
        MainGameManager.roomManager.Restart();
        if (minigame) {
            MainGameManager.minigameManager.Restart();
        }
    }


    public static bool IsGameActive() {
        return isGameActive;
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
