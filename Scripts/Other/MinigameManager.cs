using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Class responsible for spawning enemies if the "minigame" build option is on.
*/
public class MinigameManager : MonoBehaviour
{
    
    // public GameObjects used for instantiating
    public GameObject enemy1Obj;
    public GameObject enemy2Obj;
    public GameObject enemyGatlingObj;
    
    // Variables related to spawning enemies
    private Timer spawnTimer;
    private const float INITIAL_SPAWN_PERIOD = 1.5f;
    private float spawnPeriod = INITIAL_SPAWN_PERIOD;

    // Syntactic sugar
    private RoomManager roomManager;


    private void Start() {
        this.roomManager = MainGameManager.GetRoomManager();
        Restart();
    }

    public void Restart() {
        this.spawnTimer = new Timer(INITIAL_SPAWN_PERIOD);
        this.spawnPeriod = INITIAL_SPAWN_PERIOD;
    }

    private void Update() {
        if (MainGameManager.IsGameActive() && spawnTimer.UpdateAndCheck()) {
            UpdateSpawnPeriod();
            this.spawnTimer = new Timer(spawnPeriod);
            SpawnEnemyRandomly();
        }
    }

    private void UpdateSpawnPeriod() {
        float spawnRateAcceleration = 0.97f;        //0.97    
        float minSpawnPeriod = 0.4f;                //0.4
        this.spawnPeriod = Mathf.Max(minSpawnPeriod, spawnPeriod * spawnRateAcceleration);
    }

    private void SpawnEnemyRandomly() {
        float enemyDeterminator = Random.Range(0f, 1f);
        if (enemyDeterminator < 0.45) {            // 0.45
            Vector2 spawnPos = RandomSpawnPos(1f);
            Instantiate(enemy1Obj, spawnPos, Quaternion.identity);
        }
        else if (enemyDeterminator < 0.95) {        // 0.95
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
        do {
            // this loop finds a random position measured in room coordinates, meaning from 0 to room height/length, not in world coordinates
            float xPosInRoomCoordinates = (float) (Random.Range(0f, roomManager.RoomWidth() - 1));
            float yPosInRoomCoordinates = (float) (Random.Range(0f, roomManager.RoomHeight() - 1));
            
            spawnPosInRoomCoordinates = new Vector2(xPosInRoomCoordinates, yPosInRoomCoordinates);
            float inverseY = roomManager.RoomHeight() - 1 - yPosInRoomCoordinates;
            spawnPosInWorldCoordinates = new Vector2((spawnPosInRoomCoordinates.x + 0.5f - roomManager.RoomWidth() / 2f), (inverseY + 0.5f - roomManager.RoomHeight() / 2f));
            // ^ this only works if there is only 1 room
            distanceFromPlayer = (spawnPosInWorldCoordinates - (Vector2) MainGameManager.GetPlayer().transform.position).magnitude;
        } while (!IsSpawnPosValid(spawnPosInRoomCoordinates, enemySize, distanceFromPlayer));

        return spawnPosInWorldCoordinates;
    }

    private bool IsSpawnPosValid(Vector2 spawnPosInRoomCoordinates, float enemySize, float distanceFromPlayer) {
        const float minimumDistanceFromPlayer = 4f;     
        if (distanceFromPlayer < minimumDistanceFromPlayer) {
            return false;
        }

        Vector2 spawnPos = spawnPosInRoomCoordinates;
        float enemyDiameter = enemySize / 2f;
        if (spawnPos.x - enemyDiameter < 0 || spawnPos.x + enemyDiameter > roomManager.RoomWidth() ||
                spawnPos.y - enemyDiameter < 0 || spawnPos.y + enemyDiameter > roomManager.RoomHeight()) {
            return false;
        }

        int leftBound = (int) Mathf.Round(spawnPos.x - enemyDiameter);
        int rightBound = (int) Mathf.Round(spawnPos.x + enemyDiameter);
        int upperBound = (int) Mathf.Round(spawnPos.y - enemyDiameter);
        int lowerBound = (int) Mathf.Round(spawnPos.y + enemyDiameter);

        for (int x = leftBound; x <= rightBound; x++) {
            for (int y = upperBound; y <= lowerBound; y++) {
                if (roomManager.TileSymbolAtPosition(x, y) == '#') {
                    return false;
                }
            }
        }

        return true;
    }    
}
