using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    // public GameObjects used for instantiating
    public GameObject enemy1Obj;
    public GameObject enemy2Obj;
    public GameObject enemyGatlingObj;
    public GameObject chestObj;


    private RoomManager rm;                       // reference to the room manager of this combat
    private bool didCombatBegin = false;          // did enemies start spawning?
    private bool isCombatActive = false;          // are there any enemies alive or will more enemies spawn?

    private const int totalEnemies = 10;
    private int enemiesDead = 0;

    public void Init(RoomManager rm) {
        this.rm = rm;
    }

    public void BeginCombat() {
        // begin combat, start spawning enemies
        this.didCombatBegin = true;
        this.isCombatActive = true;

        // TODO: make enemy spawning more complex, implement spawn waves etc.
        for (int i = 0; i < totalEnemies; i++) {
            Vector2 spawnPos = RandomSpawnPos(1f);
            AbstractEnemy enemy = Instantiate(enemy1Obj, spawnPos, Quaternion.identity).GetComponent<AbstractEnemy>();
            enemy.SetCombatManager(this);
        }
        
    }

    public void OnEnemyDeath() {
        this.enemiesDead++;
        if (enemiesDead == totalEnemies) {
            this.isCombatActive = false;
            SpawnChest();
            rm.OpenDoors();
        }
    }

    private void SpawnChest() {
        // TODO: rework the design of chest spawning. Currently, I have at least 2 position in each room where a chest can spawn, and immediately
        // spawns at one of those positions when the room is cleared. The reworked design will only have 1 position (which can be graphically highlighted
        // to show where the player should expect the chest) and the chest will only spawn once the position becomes unobstructed by the player.
        GameObject chest = Instantiate(chestObj, Vector2.zero, Quaternion.identity) as GameObject;
        List<Vector2> possiblePositions = rm.PossibleChestWorldPositions();
        foreach (Vector2 position in possiblePositions) {
            // Move the chest to a possible position, and if it collides with the player, try another position.
            // This feels super hacky but works
            chest.transform.position = position;
            BoxCollider2D chestCollider = chest.GetComponent<BoxCollider2D>();
            RaycastHit2D hit = Physics2D.BoxCast(chest.transform.position, chestCollider.size, 0, Vector2.zero, 0, LayerMask.GetMask("Player"));
            if (hit.collider == null) {
                // not colliding with player, this position is ok
                return;
            }
        }
        throw new System.Exception("ERROR: No viable chest spawn position!");
    }

    private bool IsRoomCleared() {
        return didCombatBegin && !isCombatActive;
    }

    public bool DidCombatBegin() {
        return didCombatBegin;
    }

    // TODO: fix a bug that causes enemies to sometimes spawn too close to a wall, getting themselves stuck
    // ^ UPDATE: An easy way to replicate this bug is to go into a very small room and spawn multiple enemies in there.
    // UPDATE: I found specific spawn coordinates that trigger this bug.
    // UPDATE: This might be caused by the broken pathfinding. Fix that bug first. DONE, this bug still persists. Ugh.
    // UPDATE: The root of the problem lies in the Move() function, specifically in incorrect collision detection (function MoveOnOneAxis()).
    // Enemy is considered as colliding, even though there is still some space between the enemy and the wall.
    // UPDATE: For some unknown reason, BoxCast always registers a hit even with direction away from the wall and zero delta. For now,
    // a reasonable solution is to ensure that enemies spawn a small distance away from walls.
    private Vector2 RandomSpawnPos(float enemySize) {
        Vector2 spawnPosInRoomCoordinates;
        Vector2 spawnPosInWorldCoordinates = Vector2.zero;
        float distanceFromPlayer;
        
        do {
            // this loop finds a random position measured in room coordinates, meaning from 0 to room height/length, not in world coordinates
            float xPosInRoomCoordinates = (float) (Random.Range(0f, rm.RoomWidth() - 1));
            float yPosInRoomCoordinates = (float) (Random.Range(0f, rm.RoomHeight() - 1));
            //float xPosInRoomCoordinates = 1.77902ff;
            //float yPosInRoomCoordinates = 1.005072f;

            spawnPosInRoomCoordinates = new Vector2(xPosInRoomCoordinates, yPosInRoomCoordinates);
            spawnPosInWorldCoordinates = new Vector2((spawnPosInRoomCoordinates.x + rm.GetPosOffset().x), (spawnPosInRoomCoordinates.y + rm.GetPosOffset().y));
            distanceFromPlayer = (spawnPosInWorldCoordinates - (Vector2) MainGameManager.GetPlayer().transform.position).magnitude;
        } while (!IsSpawnPosValid(spawnPosInRoomCoordinates, enemySize, distanceFromPlayer));
        
        
        /*
        for (int i = 0; i < 1000; i++) {
            // this loop finds a random position measured in room coordinates, meaning from 0 to room height/length, not in world coordinates
            //float xPosInRoomCoordinates = (float) (Random.Range(0f, rm.RoomWidth() - 1));
            //float yPosInRoomCoordinates = (float) (Random.Range(0f, rm.RoomHeight() - 1));
            float xPosInRoomCoordinates = 1.77902f;
            float yPosInRoomCoordinates = 1.005072f;
            // ^ these coordinates are problematic - the spawn position is valid, but enemy gets stuck.

            //float xPosInRoomCoordinates = 2.5f;
            //float yPosInRoomCoordinates = 2.5f;

            spawnPosInRoomCoordinates = new Vector2(xPosInRoomCoordinates, yPosInRoomCoordinates);
            spawnPosInWorldCoordinates = new Vector2((spawnPosInRoomCoordinates.x + rm.GetPosOffset().x), (spawnPosInRoomCoordinates.y + rm.GetPosOffset().y));
            distanceFromPlayer = (spawnPosInWorldCoordinates - (Vector2) MainGameManager.GetPlayer().transform.position).magnitude;

            
            if (IsSpawnPosValid(spawnPosInRoomCoordinates, enemySize, distanceFromPlayer)) {
                break;
            }
        }
        */
        

        return spawnPosInWorldCoordinates;
        //return Vector2.zero;
    }

    private bool IsSpawnPosValid(Vector2 spawnPosInRoomCoordinates, float enemySize, float distanceFromPlayer) {
        const float minimumDistanceFromPlayer = 4f;     
        if (distanceFromPlayer < minimumDistanceFromPlayer) {
            //print("SpawnPos is not valid. Too close to player");
            return false;
        }

        Vector2 spawnPos = spawnPosInRoomCoordinates;
        float enemyDiameter = enemySize / 2f;
        if (spawnPos.x - enemyDiameter < 0 || spawnPos.x + enemyDiameter > rm.RoomWidth() ||
                spawnPos.y - enemyDiameter < 0 || spawnPos.y + enemyDiameter > rm.RoomHeight()) {
            //print("SpawnPos is not valid. Outside of valid bounds");
            return false;
        }

        int leftBound = (int) Mathf.Round(spawnPos.x - enemyDiameter - AbstractEnemy.extraDistanceFromWall);
        int rightBound = (int) Mathf.Round(spawnPos.x + enemyDiameter + AbstractEnemy.extraDistanceFromWall);
        int upperBound = (int) Mathf.Round(spawnPos.y - enemyDiameter - AbstractEnemy.extraDistanceFromWall);
        int lowerBound = (int) Mathf.Round(spawnPos.y + enemyDiameter + AbstractEnemy.extraDistanceFromWall);

        for (int x = leftBound; x <= rightBound; x++) {
            for (int y = upperBound; y <= lowerBound; y++) {
                if (rm.TileSymbolAtPosition(x, y) != '.') {
                    //print("SpawnPos is not valid. Position is colliding with a non-floor block.");
                    //print("spawnPos.x = " + spawnPos.x + ". spawnPos.y = " + spawnPos.y + ". leftBound = " + leftBound + ". rightBound = " + rightBound + ". upperBound = " + upperBound + ". lowerBound = " + lowerBound);
                    return false;
                }
            }
        }

        //print("SpawnPos is valid.");
        return true;
    }
}
