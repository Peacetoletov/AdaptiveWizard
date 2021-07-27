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
    private readonly Vector3 PLAYER_SPAWN_POS = new Vector3(-5.5f, 0.5f, 0.0f);
    
    // variables of complex rooms
    private string[] roomTiles;
    

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
                // SpawnEnemyRandomlyInBasicRoom();         // soon to be permanently disabled
                SpawnEnemyRandomlyInComplexRoom();       
            }
        } 

        if (Input.GetKeyDown(KeyCode.P)) {
            TestRoomManager.isGameActive = !isGameActive;
        }
    }

    private void UpdateMiniGameSpawnPeriod() {
        float spawnRateAcceleration = 0.9f;    //0.97
        float minSpawnPeriod = 0.05f;    //0.4
        this.miniGameSpawnPeriod = Mathf.Max(minSpawnPeriod, miniGameSpawnPeriod * spawnRateAcceleration);
    }

    private void GenerateRoom(bool createNewPlayer) {
        // LimitTestingRoom();
        // GenerateBasicRoom(createNewPlayer);
        GenerateComplexRoom(createNewPlayer);
    }

    private void GenerateComplexRoom(bool createNewPlayer) {
        
        this.roomTiles = new string[] {
            "##############################",
            "#....###################.....#",
            "#....###################.....#",
            "#....#########################",
            "#........................#...#",
            "#........................#...#",
            "#............####........#...#",
            "#............####........#####",
            "#............####............#",
            "#............................#",
            "#.........####################",
            "#.........####################",
            "#.........####################",
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
        int width = roomTiles[0].Length;
        int height = roomTiles.Length;
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                Vector3 coordinates = new Vector3((float) (x + 0.5 - width / 2.0), (float) (y + 0.5 - height / 2.0), 0);
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
        
        else if (enemyDeterminator < 1) {        // 0.95
            Vector2 spawnPos = RandomSpawnPos(1f);
            Instantiate(enemy2Obj, spawnPos, Quaternion.identity);
        }
        /*
        else {
            Vector2 spawnPos = RandomSpawnPos(1.5f);
            Instantiate(enemyGatlingObj, spawnPos, Quaternion.identity);
        }
        */
    }

    private Vector2 RandomSpawnPos(float enemySize) {
        Vector2 spawnPosInRoomCoordinates;
        Vector2 spawnPosInWorldCoordinates;
        float distanceFromPlayer;
        int width = roomTiles[0].Length;
        int height = roomTiles.Length;
        do {
            // this loop finds a random position measured in room coordinates, meaning from 0 to room height/length, not in world coordinates
            float xPosInRoomCoordinates = (float) (Random.Range(0f, roomTiles[0].Length - 1));
            float yPosInRoomCoordinates = (float) (Random.Range(0f, roomTiles.Length - 1));
            
            spawnPosInRoomCoordinates = new Vector2(xPosInRoomCoordinates, yPosInRoomCoordinates);
            spawnPosInWorldCoordinates = new Vector2((spawnPosInRoomCoordinates.x + 0.5f - width / 2f), (spawnPosInRoomCoordinates.y + 0.5f - height / 2f));
            // ^ this only works if there is only 1 room
            distanceFromPlayer = (spawnPosInWorldCoordinates - (Vector2) player.transform.position).magnitude;
        } while (!IsSpawnPosValid(spawnPosInRoomCoordinates, enemySize, distanceFromPlayer));

        /*
        for (int i = 0; i < 100; i++) {
            float xPosInRoomCoordinates = (float) (Random.Range(0f, roomTiles[0].Length - 1));
            float yPosInRoomCoordinates = (float) (Random.Range(0f, roomTiles.Length - 1));
            
            spawnPosInRoomCoordinates = new Vector2(xPosInRoomCoordinates, yPosInRoomCoordinates);
            spawnPosInWorldCoordinates = new Vector2((spawnPosInRoomCoordinates.x + 0.5f - width / 2f), (spawnPosInRoomCoordinates.y + 0.5f - height / 2f));
            // ^ this only works if there is only 1 room
            distanceFromPlayer = (spawnPosInWorldCoordinates - (Vector2) player.transform.position).magnitude;
            if (IsSpawnPosValid(spawnPosInRoomCoordinates, enemySize, distanceFromPlayer)) {
                break;
            }
        }
        */

        return spawnPosInWorldCoordinates;
        // return Vector2.zero;
    }

    private bool IsSpawnPosValid(Vector2 spawnPosInRoomCoordinates, float enemySize, float distanceFromPlayer) {
        // TODO: fix a bug that flags all position in a 2 block wide corridor as invalid
        // Another (possibly the same) bug is that enemies never spawn closer than 0.5 blocks from a wall
        const float minimumDistanceFromPlayer = 4f;     
        if (distanceFromPlayer < minimumDistanceFromPlayer) {
            print("bad distance from player: " + spawnPosInRoomCoordinates);
            return false;
        }

        Vector2 spawnPos = spawnPosInRoomCoordinates;
        int width = roomTiles[0].Length;
        int height = roomTiles.Length;
        float enemyDiameter = enemySize / 2f;
        if (spawnPos.x - enemyDiameter < 0 || spawnPos.x + enemyDiameter > width ||
                spawnPos.y - enemyDiameter < 0 || spawnPos.y + enemyDiameter > height) {
            print("out of bounds: " + spawnPosInRoomCoordinates);
            return false;
        }

        /*
        int leftBound = (int) Mathf.Floor(spawnPos.x - enemyDiameter);
        int rightBound = (int) Mathf.Ceil(spawnPos.x + enemyDiameter);
        int upperBound = (int) Mathf.Floor(spawnPos.y - enemyDiameter);
        int lowerBound = (int) Mathf.Ceil(spawnPos.y + enemyDiameter);
        */
        int leftBound = (int) Mathf.Round(spawnPos.x - enemyDiameter);
        int rightBound = (int) Mathf.Round(spawnPos.x + enemyDiameter);
        int upperBound = (int) Mathf.Round(spawnPos.y - enemyDiameter);
        int lowerBound = (int) Mathf.Round(spawnPos.y + enemyDiameter);

        for (int x = leftBound; x <= rightBound; x++) {
            for (int y = upperBound; y <= lowerBound; y++) {
                if (TileSymbolAtPosition(x, y) == '#') {
                    print("too close to wall. position: " + spawnPosInRoomCoordinates);
                    print("leftBound: " + leftBound + ". rightBound: " + rightBound + ". upperBound: " + upperBound + ". lowerBound: " + lowerBound);
                    // this is responsible for the bugs
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

    private void GenerateBasicRoom(bool createNewPlayer) {
        // Environment
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
            //SpawnEnemyRandomlyInBasicRoomInBasicRoom();
        }
        else {
            for (int i = 0; i < 1; i++) {
                // Melee
                
                for (int j = 0; j < 1; j++) {
                    Instantiate(enemy1Obj, new Vector3(2.0f + i * 1.5f, -4.0f + 2*j, 0f), Quaternion.identity);
                    // Ranged
                    // Instantiate(enemy2Obj, new Vector3(4.0f + 2.5f * j, -2.5f + 2.5f*i, 0f), Quaternion.identity);
                }
            }
        }

    }

    private void SpawnEnemyRandomlyInBasicRoom() {
        float enemyDeterminator = Random.Range(0f, 1f);
        if (enemyDeterminator < 0.45) {     
            Vector2 spawnPos = RandomSpawnPosInBasicRoom(1f);
            Instantiate(enemy1Obj, spawnPos, Quaternion.identity);
        }
        else if (enemyDeterminator < 0.95) {
            Vector2 spawnPos = RandomSpawnPosInBasicRoom(1f);
            Instantiate(enemy2Obj, spawnPos, Quaternion.identity);
        }
        else {
            Vector2 spawnPos = RandomSpawnPosInBasicRoom(1.5f);
            Instantiate(enemyGatlingObj, spawnPos, Quaternion.identity);
        }
    }

    private Vector2 RandomSpawnPosInBasicRoom(float enemySize) {
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
