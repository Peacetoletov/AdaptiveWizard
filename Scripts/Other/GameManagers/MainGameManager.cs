using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Clean up manager classes, remove old code that got commented out
public class MainGameManager : MonoBehaviour
{
    // build options
    public const bool minigame = false;     // must be false until room management is fully implemented

    // constants
    public const int PIXELS_PER_UNIT = 16;          // how many pixels are in 1 unit. Currently 16, might change to 32 in the future if I want more detailed art

    // public GameObjects used for instantiating
    public GameObject playerObj;
    public GameObject managerOfRoomManagersObj;
    public GameObject minigameManagerObj;


    private static bool isGameActive = true;        // false if game is paused or UI is open, true otherwise
    private readonly Vector3 PLAYER_SPAWN_POS = new Vector3(-0.5f, 7.5f, 0.0f);
    private static GameObject player;                   // reference to the player
                                                        // I decided to make it static because I don't plan on having multiplayer PvP or coop modes

    private static UI_Manager UI_manager;
    private static MinigameManager minigameManager;
    private static ManagerOfRoomManagers managerOfRoomManagers;

    
    private void Start() {
        CreateAndInitializePlayer();

        MainGameManager.managerOfRoomManagers = Instantiate(managerOfRoomManagersObj, Vector2.zero, Quaternion.identity).GetComponent<ManagerOfRoomManagers>();

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

        player.GetComponent<PlayerGeneral>().ResetPlayer();
        MainGameManager.player.transform.position = PLAYER_SPAWN_POS;
        managerOfRoomManagers.Restart();
        if (minigame) {
            minigameManager.Restart();
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

    public static ManagerOfRoomManagers GetManagerOfRoomManagers() {
        return managerOfRoomManagers;
    }
}
