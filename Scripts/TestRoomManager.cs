using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRoomManager : MonoBehaviour
{
    // build options
    private const bool miniGame = true;

    // these GameObjects are here only for instantiates, never used in other ways
    public GameObject playerObj;
    public GameObject wallObj;
    public GameObject floorObj;
    public GameObject enemy1Obj;
    public GameObject enemy2Obj;
    public GameObject enemyGatlingObj;

    private const int ROOM_WIDTH = 30;
    private const int ROOM_HEIGHT = 16;

    private static bool isGameActive = true;        // false if game is paused or UI is open, true otherwise

    private Timer spawnEnemiesTimer;
    private const float INITIAL_ENEMY_SPAWN_PERIOD = 1.5f;
    private float miniGameSpawnPeriod = INITIAL_ENEMY_SPAWN_PERIOD;       
    private static GameObject player;                   // reference to the player
                                                        // I decided to make it static because I don't plan on having multiplayer PvP or coop modes
    private readonly Vector3 PLAYER_SPAWN_POS = new Vector3(0.5f, 0.5f, 0.0f);
    //private Timer restartTimer;
    //private bool shouldRestart = false;
    

    // Start is called before the first frame update
    private void Start() {
        GenerateRoom(true);     
    }

    // Update is called once per frame
    private void Update() {
        if (isGameActive) {
            if (miniGame && spawnEnemiesTimer.UpdateAndCheck()) {
                UpdateMiniGameSpawnPeriod();
                this.spawnEnemiesTimer = new Timer(miniGameSpawnPeriod);
                SpawnEnemyRandomly();
            }
        } 

        if (Input.GetKeyDown(KeyCode.P)) {
            TestRoomManager.isGameActive = !isGameActive;
        }
    }

    private void UpdateMiniGameSpawnPeriod() {
        float spawnRateAcceleration = 0.97f;
        float minSpawnPeriod = 0.4f;
        this.miniGameSpawnPeriod = Mathf.Max(minSpawnPeriod, miniGameSpawnPeriod * spawnRateAcceleration);
    }

    private void GenerateRoom(bool createNewPlayer) {
        // LimitTestingRoom();
        GenerateNormalRoom(createNewPlayer);
    }

    public void RestartLevel() {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject o in allObjects) {
            if (o.activeInHierarchy && o.tag != "MainCamera" && o.tag != "GameController" && o.tag != "UIController" && o.tag != "Player") {
                Destroy(o);
            }
        }
        this.miniGameSpawnPeriod = INITIAL_ENEMY_SPAWN_PERIOD;
        player.GetComponent<PlayerGeneral>().ResetPlayer();
        TestRoomManager.player.transform.position = PLAYER_SPAWN_POS;
        GenerateRoom(false);
    }

    private void GenerateNormalRoom(bool createNewPlayer) {
        // Walls and floor
        for (int i = 0; i < ROOM_WIDTH; i++) {
            for (int j = 0; j < ROOM_HEIGHT; j++) {
                Vector3 coordinates = new Vector3((float) (i + 0.5 - ROOM_WIDTH / 2.0), (float) (j + 0.5 - ROOM_HEIGHT / 2.0), 0);
                if (i == 0 || i == ROOM_WIDTH - 1 || j == 0 || j == ROOM_HEIGHT - 1) {
                    GameObject newWall = Instantiate(wallObj, coordinates, Quaternion.identity) as GameObject;
                }
                else {
                    GameObject newFloor = Instantiate(floorObj, coordinates, Quaternion.identity) as GameObject;
                } 
            }
        }

        //test 
        // GameObject newEnemy1 = Instantiate(enemy1Obj, PLAYER_SPAWN_POS, Quaternion.identity) as GameObject;

        // Player
        if (createNewPlayer) {
            TestRoomManager.player = Instantiate(playerObj, PLAYER_SPAWN_POS, Quaternion.identity) as GameObject;
        }

        // Enemies
        if (miniGame) {
            this.spawnEnemiesTimer = new Timer(miniGameSpawnPeriod);
            //SpawnEnemyRandomly();
        }
        else {
            
            for (int i = 0; i < 3; i++) {
                // Melee
                // Instantiate(enemy1Obj, new Vector3(2.0f, -2.0f + 2*i, 0f), Quaternion.identity) as GameObject;
                for (int j = 0; j < 3; j++) {
                    // Ranged
                    // Instantiate(enemy2Obj, new Vector3(4.0f + 2.5f * j, -2.5f + 2.5f*i, 0f), Quaternion.identity) as GameObject;
                }
            }
            Instantiate(enemyGatlingObj, new Vector3(4.0f, 0f, 0f), Quaternion.identity);
        }

    }

    private void SpawnEnemyRandomly() {
        float enemyDeterminator = Random.Range(0f, 1f);
        if (enemyDeterminator < 0.45) {
            Vector2 spawnPos = RandomSpawnPos(1f);
            Instantiate(enemy1Obj, spawnPos, Quaternion.identity);
        }
        else if (enemyDeterminator < 0.95) {
            Vector2 spawnPos = RandomSpawnPos(1f);
            Instantiate(enemy2Obj, spawnPos, Quaternion.identity);
        }
        else {
            Vector2 spawnPos = RandomSpawnPos(1.5f);
            Instantiate(enemyGatlingObj, spawnPos, Quaternion.identity);
        }
    }

    private Vector2 RandomSpawnPos(float enemySize) {
        float minimumDistanceFromPlayer = 4f;
        float distanceFromPlayer;
        Vector2 spawnPos;
        do {
            float minXPos = enemySize / 2f + 1f - ROOM_WIDTH / 2f;
            float minYPos = enemySize / 2f + 1f - ROOM_HEIGHT / 2f;
            float maxXPos = -enemySize / 2f - 1f + ROOM_WIDTH / 2f;
            float maxYPos = -enemySize / 2f - 1f + ROOM_HEIGHT / 2f;
            float xPos = (float) (Random.Range(minXPos, maxXPos));
            float yPos = (float) (Random.Range(minYPos, maxYPos));
            //

            // good:
            //float xPos = (float) (enemySize / 2 + 1 - ROOM_WIDTH / 2.0);
            //float yPos = (float) (enemySize / 2 + 1 - ROOM_HEIGHT / 2.0);
            //float xPos = (float) ((ROOM_WIDTH - enemySize / 2) - 1 - ROOM_WIDTH / 2.0);
            //float yPos = (float) ((ROOM_HEIGHT - enemySize / 2) - 1 - ROOM_HEIGHT / 2.0);
            spawnPos = new Vector2(xPos, yPos);
            distanceFromPlayer = (spawnPos - (Vector2) player.transform.position).magnitude;
        } while (distanceFromPlayer < minimumDistanceFromPlayer);
        return spawnPos;
    }

    private void LimitTestingRoom() {
        for (int i = 0; i < ROOM_WIDTH; i++) {
            for (int j = 0; j < ROOM_HEIGHT; j++) {
                Vector3 coordinates = new Vector3((float) (i + 0.5 - ROOM_WIDTH / 2.0), (float) (j + 0.5 - ROOM_HEIGHT / 2.0), 1);
                if (i < 145 || i > 155 || j < 45 || j > 55) {
                    GameObject newWall = Instantiate(wallObj, coordinates, Quaternion.identity) as GameObject;
                }
                else {
                    GameObject newFloor = Instantiate(floorObj, coordinates, Quaternion.identity) as GameObject;
                } 
            }
        }
        for (int i = 0; i < 200; i++) {
            TestRoomManager.player = Instantiate(playerObj, new Vector3(Random.Range(-4f, 4f), Random.Range(-4f, 4f), -1), Quaternion.identity) as GameObject;
        }
    }

    public static bool IsGameActive() {
        return isGameActive;
    }

    public static GameObject GetPlayer() {
        return player;
    }
}
