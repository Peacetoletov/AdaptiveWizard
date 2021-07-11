using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRoomManager : MonoBehaviour
{

    public GameObject player;
    public GameObject wall;
    public GameObject floor;
    public GameObject enemy1;
    public GameObject enemy2;
    private Coroutine spawnEnemiesCoroutine;
    private static bool isRunActive = true;

    private const bool miniGame = false;
    private const int ROOM_WIDTH = 30;
    private const int ROOM_HEIGHT = 16;

    // Start is called before the first frame update
    void Start()
    {
        GenerateRoom();     
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GenerateRoom() {
        /*
        if (limitTesting) {
            //this.ROOM_WIDTH = 300;
            //this.ROOM_HEIGHT = 100;
            LimitTestingRoom();
        }
        else {
            //this.ROOM_WIDTH = 30;
            //this.ROOM_HEIGHT = 16;
            GenerateNormalRoom();
        }   
        */
        GenerateNormalRoom();
    }

    public void RestartLevel() {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject o in allObjects) {
            if (o.activeInHierarchy && o.tag != "MainCamera" && o.tag != "GameController" && o.tag != "UIController") {
                Destroy(o);
            }
        }
        isRunActive = false;
        if (miniGame) {
            StopCoroutine(spawnEnemiesCoroutine);
        }
        StartCoroutine(WaitAndGenerateRoom(2f));
    }

    private IEnumerator WaitAndGenerateRoom(float seconds) {
        yield return new WaitForSeconds(seconds);
        GenerateRoom();
        isRunActive = true;
    }


    private void GenerateNormalRoom() {
        // Walls and floor
        for (int i = 0; i < ROOM_WIDTH; i++) {
            for (int j = 0; j < ROOM_HEIGHT; j++) {
                Vector3 coordinates = new Vector3((float) (i + 0.5 - ROOM_WIDTH / 2.0), (float) (j + 0.5 - ROOM_HEIGHT / 2.0), 0);
                if (i == 0 || i == ROOM_WIDTH - 1 || j == 0 || j == ROOM_HEIGHT - 1) {
                    GameObject newWall = Instantiate(wall, coordinates, Quaternion.identity) as GameObject;
                }
                else {
                    GameObject newFloor = Instantiate(floor, coordinates, Quaternion.identity) as GameObject;
                } 
            }
        }

        // Player
        GameObject newPlayer = Instantiate(player, new Vector3(-1.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;

        // Enemies
        if (miniGame) {
            this.spawnEnemiesCoroutine = StartCoroutine(startSpawningEnemies(newPlayer));
        }
        else {
            
            for (int i = 0; i < 3; i++) {
                GameObject newEnemy1 = Instantiate(enemy1, new Vector3(2.0f, -2.0f + 2*i, 0.0f), Quaternion.identity) as GameObject;
                for (int j = 0; j < 3; j++) {
                    GameObject newEnemy2 = Instantiate(enemy2, new Vector3(4.0f + 2.5f * j, -2.5f + 2.5f*i, 0.0f), Quaternion.identity) as GameObject;
                }
            }
            
        }

    }

    private IEnumerator startSpawningEnemies(GameObject player) {
        float spawnRate = 1.5f;
        float spawnRateAcceleration = 0.97f;
        float maxSpawnRate = 0.35f;
        float minimumDistanceFromPlayer = 4f;
        float distanceFromPlayer;
        Vector2 spawnPos;
        while (true) {
            do {
                float xPos = (float) (Random.Range(1f, ROOM_WIDTH - 2f) + 0.5 - ROOM_WIDTH / 2.0);
                float yPos = (float) (Random.Range(1f, ROOM_HEIGHT - 2f) + 0.5 - ROOM_HEIGHT / 2.0);
                spawnPos = new Vector2(xPos, yPos);
                distanceFromPlayer = (spawnPos - (Vector2) player.transform.position).magnitude;
            } while (distanceFromPlayer < minimumDistanceFromPlayer);

            float enemyDeterminator = Random.Range(0f, 1f);
            if (enemyDeterminator < 0.5) {
                Instantiate(enemy1, spawnPos, Quaternion.identity);
            }
            else {
                Instantiate(enemy2, spawnPos, Quaternion.identity);
            }
            
            yield return new WaitForSeconds(spawnRate);
            if (spawnRate > maxSpawnRate) {
                spawnRate *= spawnRateAcceleration;
            }
        }
    }

    private void LimitTestingRoom() {
        for (int i = 0; i < ROOM_WIDTH; i++) {
            for (int j = 0; j < ROOM_HEIGHT; j++) {
                Vector3 coordinates = new Vector3((float) (i + 0.5 - ROOM_WIDTH / 2.0), (float) (j + 0.5 - ROOM_HEIGHT / 2.0), 1);
                if (i < 145 || i > 155 || j < 45 || j > 55) {
                    GameObject newWall = Instantiate(wall, coordinates, Quaternion.identity) as GameObject;
                }
                else {
                    GameObject newFloor = Instantiate(floor, coordinates, Quaternion.identity) as GameObject;
                } 
            }
        }
        for (int i = 0; i < 200; i++) {
            GameObject newPlayer = Instantiate(player, new Vector3(Random.Range(-4f, 4f), Random.Range(-4f, 4f), -1), Quaternion.identity) as GameObject;
        }
    }

    public static bool GetIsRunActive() {
        return isRunActive;
    }
}
