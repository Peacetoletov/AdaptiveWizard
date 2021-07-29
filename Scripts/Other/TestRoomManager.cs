using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRoomManager : MonoBehaviour
{
    // build options
    private const bool miniGame = false;

    // these GameObjects are here only for instantiates, never used in other ways
    public GameObject playerObj;
    public GameObject wallObj;
    public GameObject floorObj;
    public GameObject enemy1Obj;
    public GameObject enemy2Obj;
    public GameObject enemyGatlingObj;

    private static bool isGameActive = true;        // false if game is paused or UI is open, true otherwise
    private readonly Vector3 PLAYER_SPAWN_POS = new Vector3(-5.5f, 0.5f, 0.0f);
    private static GameObject player;                   // reference to the player
                                                        // I decided to make it static because I don't plan on having multiplayer PvP or coop modes
                                                        
    // variables related to enemies
    private Timer spawnEnemiesTimer;
    private const float INITIAL_ENEMY_SPAWN_PERIOD = 1.5f;
    private float miniGameSpawnPeriod = INITIAL_ENEMY_SPAWN_PERIOD;       
    
    // variables of rooms
    private string[] roomTiles;
    

    
    private void Start() {
        GenerateRoom(true);     
    }

    private void Update() {
        if (isGameActive) {
            if (miniGame && spawnEnemiesTimer.UpdateAndCheck()) {
                UpdateMiniGameSpawnPeriod();
                this.spawnEnemiesTimer = new Timer(miniGameSpawnPeriod);
                SpawnEnemyRandomlyInComplexRoom();       
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
        this.roomTiles = new string[] {
            "##############################",
            "#............................#",
            "#............................#",
            "#....####..............###...#",
            "#.............##.........#...#",
            "#.............##.........#...#",
            "#...........####.........#...#",
            "#...........####.........#...#",
            "#............................#",
            "#............................#",
            "#.....................###....#",
            "#.........###############....#",
            "#.........###############....#",
            "#............................#",
            "#............................#",
            "##############################"
        };
        
        /*
        this.roomTiles = new string[] {
            "#####",
            "#...#",
            "#...#",
            "#####",
        };
        */

        // Environment
        int roomWidth = roomTiles[0].Length;
        int roomHeight = roomTiles.Length;
        for (int x = 0; x < roomWidth; x++) {
            for (int y = 0; y < roomHeight; y++) {
                Vector3 coordinates = new Vector3((float) (x + 0.5 - roomWidth / 2.0), (float) (y + 0.5 - roomHeight / 2.0), 0);
                if (TileSymbolAtPosition(x, y) == '#') {
                    Instantiate(wallObj, coordinates, Quaternion.identity);
                } else {
                    Instantiate(floorObj, coordinates, Quaternion.identity);
                }
            }
        }

        // Player
        if (createNewPlayer) {
            TestRoomManager.player = Instantiate(playerObj, PLAYER_SPAWN_POS, Quaternion.identity) as GameObject;
        }

        // Start spawning enemies
        if (miniGame) {
            this.spawnEnemiesTimer = new Timer(miniGameSpawnPeriod);
        }
        else {
            // whatever needs to be tested here
            /*
            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {
                    Instantiate(enemy1Obj, new Vector3(5f + i, 0f + j, 0f), Quaternion.identity);
                }
            }
            */
            Instantiate(enemy1Obj, new Vector3(5f, 0f, 0f), Quaternion.identity);
            Instantiate(enemy1Obj, new Vector3(5f, 0f, 0f), Quaternion.identity);
        }
    }

    private char TileSymbolAtPosition(int x, int y) {
        // adjust for vertical inversion created by the string representation of a room
        y = roomTiles.Length - 1 - y;

        return roomTiles[y][x];
    }

    private void SpawnEnemyRandomlyInComplexRoom() {
        float enemyDeterminator = Random.Range(0f, 1f);
        if (enemyDeterminator < 0) {            // 0.45
            Vector2 spawnPos = RandomSpawnPos(1f);
            Instantiate(enemy1Obj, spawnPos, Quaternion.identity);
        }
        
        else if (enemyDeterminator < 0) {        // 0.95
            Vector2 spawnPos = RandomSpawnPos(1f);
            Instantiate(enemy2Obj, spawnPos, Quaternion.identity);
        }
        
        else {
            Vector2 spawnPos = RandomSpawnPos(1.5f);
            Instantiate(enemyGatlingObj, spawnPos, Quaternion.identity);
        }
        
    }

    private Vector2 RandomSpawnPos(float enemySize) {
        Vector2 spawnPosInRoomCoordinates;
        Vector2 spawnPosInWorldCoordinates;
        float distanceFromPlayer;
        int roomWidth = roomTiles[0].Length;
        int roomHeight = roomTiles.Length;
        do {
            // this loop finds a random position measured in room coordinates, meaning from 0 to room height/length, not in world coordinates
            float xPosInRoomCoordinates = (float) (Random.Range(0f, roomTiles[0].Length - 1));
            float yPosInRoomCoordinates = (float) (Random.Range(0f, roomTiles.Length - 1));
            
            spawnPosInRoomCoordinates = new Vector2(xPosInRoomCoordinates, yPosInRoomCoordinates);
            spawnPosInWorldCoordinates = new Vector2((spawnPosInRoomCoordinates.x + 0.5f - roomWidth / 2f), (spawnPosInRoomCoordinates.y + 0.5f - roomHeight / 2f));
            // ^ this only works if there is only 1 room
            distanceFromPlayer = (spawnPosInWorldCoordinates - (Vector2) player.transform.position).magnitude;
        } while (!IsSpawnPosValid(spawnPosInRoomCoordinates, enemySize, distanceFromPlayer));

        return spawnPosInWorldCoordinates;
    }

    private bool IsSpawnPosValid(Vector2 spawnPosInRoomCoordinates, float enemySize, float distanceFromPlayer) {
        // TODO: fix a bug that flags all position in a 2 block wide corridor as invalid
        // Another (possibly the same) bug is that enemies never spawn closer than 0.5 blocks from a wall
        const float minimumDistanceFromPlayer = 4f;     
        if (distanceFromPlayer < minimumDistanceFromPlayer) {
            return false;
        }

        Vector2 spawnPos = spawnPosInRoomCoordinates;
        int roomWidth = roomTiles[0].Length;
        int roomHeight = roomTiles.Length;
        float enemyDiameter = enemySize / 2f;
        if (spawnPos.x - enemyDiameter < 0 || spawnPos.x + enemyDiameter > roomWidth ||
                spawnPos.y - enemyDiameter < 0 || spawnPos.y + enemyDiameter > roomHeight) {
            return false;
        }

        int leftBound = (int) Mathf.Round(spawnPos.x - enemyDiameter);
        int rightBound = (int) Mathf.Round(spawnPos.x + enemyDiameter);
        int upperBound = (int) Mathf.Round(spawnPos.y - enemyDiameter);
        int lowerBound = (int) Mathf.Round(spawnPos.y + enemyDiameter);

        for (int x = leftBound; x <= rightBound; x++) {
            for (int y = upperBound; y <= lowerBound; y++) {
                if (TileSymbolAtPosition(x, y) == '#') {
                    return false;
                }
            }
        }

        return true;
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


    public static bool IsGameActive() {
        return isGameActive;
    }

    public static GameObject GetPlayer() {
        return player;
    }
}