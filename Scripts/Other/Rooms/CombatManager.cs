using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    // public GameObjects used for instantiating
    public GameObject enemy1Obj;
    public GameObject enemy2Obj;
    public GameObject enemyGatlingObj;


    private RoomManager rm;                       // reference to the room manager of this combat
    private bool didCombatBegin = false;          // did enemies start spawning?
    private bool isCombatActive = false;          // are there any enemies alive or will more enemies spawn?

    public void Init(RoomManager rm) {
        this.rm = rm;
    }

    public void BeginCombat() {
        // begin combat, start spawning enemies
        this.didCombatBegin = true;
        this.isCombatActive = true;

        for (int i = 0; i < 1; i++) {
            Vector2 spawnPos = RandomSpawnPos(1f);
            AbstractEnemy enemy = Instantiate(enemy1Obj, spawnPos, Quaternion.identity).GetComponent<AbstractEnemy>();
            enemy.SetCombatManager(this);
        }
        
    }

    public void OnEnemyDeath() {
        // temporarily simplified
        this.isCombatActive = false;
        rm.OpenDoors();
    }

    private bool IsRoomCleared() {
        return didCombatBegin && !isCombatActive;
    }

    public bool DidCombatBegin() {
        return didCombatBegin;
    }

    // TODO: fix a bug that causes enemies to sometimes spawn too close to a wall, getting themselves stuck
    // ^ UPDATE: An easy way to replicate this bug is to go into a very small room and spawn multiple enemies in there.
    // I noticed that when the bug occurs, I get a following error: IndexOutOfBoundsException: Index was outside of the bounds of the array. RoomManager.WorldPositionToNode
    
    // TODO: fix a bug that breaks pathfinding (enemies seem to go the opposite way of the shortest path, this 
    // is very likely related to the way I reverse the visual representation of the room)
    private Vector2 RandomSpawnPos(float enemySize) {
        Vector2 spawnPosInRoomCoordinates;
        Vector2 spawnPosInWorldCoordinates;
        float distanceFromPlayer;
        do {
            // this loop finds a random position measured in room coordinates, meaning from 0 to room height/length, not in world coordinates
            float xPosInRoomCoordinates = (float) (Random.Range(0f, rm.RoomWidth() - 1));
            float yPosInRoomCoordinates = (float) (Random.Range(0f, rm.RoomHeight() - 1));
            
            spawnPosInRoomCoordinates = new Vector2(xPosInRoomCoordinates, yPosInRoomCoordinates);
            spawnPosInWorldCoordinates = new Vector2((spawnPosInRoomCoordinates.x + rm.GetPosOffset().x), (spawnPosInRoomCoordinates.y + rm.GetPosOffset().y));
            distanceFromPlayer = (spawnPosInWorldCoordinates - (Vector2) MainGameManager.GetPlayer().transform.position).magnitude;
        } while (!IsSpawnPosValid(spawnPosInRoomCoordinates, enemySize, distanceFromPlayer));

        return spawnPosInWorldCoordinates;
        //return Vector2.zero;
    }

    private bool IsSpawnPosValid(Vector2 spawnPosInRoomCoordinates, float enemySize, float distanceFromPlayer) {
        const float minimumDistanceFromPlayer = 4f;     
        if (distanceFromPlayer < minimumDistanceFromPlayer) {
            return false;
        }

        Vector2 spawnPos = spawnPosInRoomCoordinates;
        float enemyDiameter = enemySize / 2f;
        if (spawnPos.x - enemyDiameter < 0 || spawnPos.x + enemyDiameter > rm.RoomWidth() ||
                spawnPos.y - enemyDiameter < 0 || spawnPos.y + enemyDiameter > rm.RoomHeight()) {
            return false;
        }

        int leftBound = (int) Mathf.Round(spawnPos.x - enemyDiameter);
        int rightBound = (int) Mathf.Round(spawnPos.x + enemyDiameter);
        int upperBound = (int) Mathf.Round(spawnPos.y - enemyDiameter);
        int lowerBound = (int) Mathf.Round(spawnPos.y + enemyDiameter);

        for (int x = leftBound; x <= rightBound; x++) {
            for (int y = upperBound; y <= lowerBound; y++) {
                if (rm.TileSymbolAtPosition(x, y) != '.') {
                    return false;
                }
            }
        }

        return true;
    }
}
