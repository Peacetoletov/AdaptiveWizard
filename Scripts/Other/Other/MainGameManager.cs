using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Clean up manager classes, remove old code that got commented out
public class MainGameManager : MonoBehaviour
{
    // build options
    public const bool minigame = false;

    // public GameObjects used for instantiating
    public GameObject playerObj;
    public GameObject roomManagerObj;
    public GameObject minigameManagerObj;

    // these GameObjects are here only for instantiates, never used in other ways
    /*
    public GameObject playerObj;
    public GameObject wallObj;
    public GameObject floorObj;
    */
    /*
    public GameObject enemy1Obj;
    public GameObject enemy2Obj;
    public GameObject enemyGatlingObj;
    */

    private static bool isGameActive = true;        // false if game is paused or UI is open, true otherwise
    private readonly Vector3 PLAYER_SPAWN_POS = new Vector3(-5.5f, -0.5f, 0.0f);
    private static GameObject player;                   // reference to the player
                                                        // I decided to make it static because I don't plan on having multiplayer PvP or coop modes

    /*            
    // variables related to enemies
    private Timer spawnEnemiesTimer;
    private const float INITIAL_ENEMY_SPAWN_PERIOD = 1.5f;
    private float miniGameSpawnPeriod = INITIAL_ENEMY_SPAWN_PERIOD;       
    */
    private static MinigameManager minigameManager;
    
    /*
    // variables of rooms
    private static string[] roomVisual;        // visual representation of the room
    private static Pathfinding.Node[,] roomNodes;          // 2D array of floor nodes of the room, used for pathfinding. 2D array allows for easy heuristic calculation
    */
    private static RoomManager roomManager;

    
    private void Start() {
        CreateAndInitializePlayer();

        //test
        Items.PassiveItem test = new Items.HealthCrystal();
        InventoryNS.Inventory.passiveItems.AddItem(test);
        Items.PassiveItem test2 = new Items.DivineSphere();
        InventoryNS.Inventory.passiveItems.AddItem(test2);
        Items.PassiveItem test3 = new Items.DivineSphere();
        InventoryNS.Inventory.passiveItems.AddItem(test3);

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
        } 

        if (Input.GetKeyDown(KeyCode.P)) {
            MainGameManager.isGameActive = !isGameActive;
        }

        // test
        /*
        if (Input.GetMouseButtonDown(0)) {
            PositionInRoomToNode(player);
        }
        */
    }


    public void RestartGame() {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject o in allObjects) {
            if (o.activeInHierarchy && o.tag != "MainCamera" && o.tag != "GameController" && o.tag != "UIController" && o.tag != "Player") {
                Destroy(o);
            }
        }
        // TODO: change
        //this.miniGameSpawnPeriod = INITIAL_ENEMY_SPAWN_PERIOD;
        player.GetComponent<PlayerGeneral>().ResetPlayer();
        MainGameManager.player.transform.position = PLAYER_SPAWN_POS;
        //GenerateRoom(false);
    }


    public static bool IsGameActive() {
        return isGameActive;
    }

    public static GameObject GetPlayer() {
        return player;
    }

    public static RoomManager GetRoomManager() {
        return roomManager;
    }
}
