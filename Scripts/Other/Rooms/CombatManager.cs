using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdaptiveWizard.Assets.Scripts.Other.GameManagers;
using AdaptiveWizard.Assets.Scripts.Enemies.AbstractClasses;


namespace AdaptiveWizard.Assets.Scripts.Other.Rooms
{
    public class CombatManager : MonoBehaviour
    {
        // public GameObjects used for instantiating
        public GameObject enemy1Obj;
        public GameObject enemy2Obj;
        public GameObject enemyGatlingObj;
        public GameObject walkingEyeballObj;
        public GameObject chestObj;


        private AbstractRoom room;                       // reference to the room of this combat
        private bool isCombatActive = false;          // are there any enemies alive or will more enemies spawn?

        private const int totalEnemies = 2;
        private int enemiesDead = 0;


        public void Init(AbstractRoom room) {
            this.room = room;
        }

        public void Update() {
            // TEMPORARY, INEFFICIENT SOLUTION
            if (MainGameManager.IsGameActive()) {
                // if player is in the same room as this combat manager
                if (MainGameManager.GetRoomManager().GetCurRoom() == room) {
                    if (!isCombatActive && totalEnemies != enemiesDead) {
                        BeginCombat();
                    }
                }
            }
            
        }

        public void BeginCombat() {
            // begin combat, start spawning enemies
            this.isCombatActive = true;

            // TODO: make enemy spawning more complex, implement spawn waves etc.
            for (int i = 0; i < totalEnemies; i++) {
                Vector2 spawnPos = RandomSpawnPos(1f);
                //Vector2 spawnPos = new Vector2(21, 5);
                //AbstractEnemy enemy = Instantiate(enemy1Obj, spawnPos, Quaternion.identity).GetComponent<AbstractEnemy>();
                AbstractEnemy enemy = Instantiate(walkingEyeballObj, spawnPos, Quaternion.identity).GetComponent<AbstractEnemy>();
                enemy.SetCombatManager(this);
            }
        }

        public void OnEnemyDeath() {
            // TEMPORARILY COMMENTED OUT
            /*
            this.enemiesDead++;
            if (enemiesDead == totalEnemies) {
                this.isCombatActive = false;
                SpawnChest();
                room.OpenDoors();
            }
            */
        }

        private void SpawnChest() {
            // TODO: this
            // Instantiate(chestObj, CHEST_POS, Quaternion.identity);
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
                float xPosInRoomCoordinates = (float) (Random.Range(0f, room.RoomWidth() - 1));
                float yPosInRoomCoordinates = (float) (Random.Range(0f, room.RoomHeight() - 1));
                //float xPosInRoomCoordinates = 5f;
                //float yPosInRoomCoordinates = 16.5f;

                spawnPosInRoomCoordinates = new Vector2(xPosInRoomCoordinates, yPosInRoomCoordinates);
                spawnPosInWorldCoordinates = new Vector2((spawnPosInRoomCoordinates.x + room.GetPosOffset().x), (spawnPosInRoomCoordinates.y + room.GetPosOffset().y));
                distanceFromPlayer = (spawnPosInWorldCoordinates - (Vector2) MainGameManager.GetPlayer().transform.position).magnitude;
            } while (!IsSpawnPosValid(spawnPosInRoomCoordinates, enemySize, distanceFromPlayer));
            

            return spawnPosInWorldCoordinates;
        }

        private bool IsSpawnPosValid(Vector2 spawnPosInRoomCoordinates, float enemySize, float distanceFromPlayer) {
            const float minimumDistanceFromPlayer = 4f;     
            if (distanceFromPlayer < minimumDistanceFromPlayer) {
                //print("SpawnPos is not valid. Too close to player");
                return false;
            }

            Vector2 spawnPos = spawnPosInRoomCoordinates;
            float enemyDiameter = enemySize / 2f;
            if (spawnPos.x - enemyDiameter < 0 || spawnPos.x + enemyDiameter > room.RoomWidth() ||
                    spawnPos.y - enemyDiameter < 0 || spawnPos.y + enemyDiameter > room.RoomHeight()) {
                //print("SpawnPos is not valid. Outside of valid bounds");
                return false;
            }

            int leftBound = (int) Mathf.Round(spawnPos.x - enemyDiameter - AbstractEnemy.extraDistanceFromWall);
            int rightBound = (int) Mathf.Round(spawnPos.x + enemyDiameter + AbstractEnemy.extraDistanceFromWall);
            int upperBound = (int) Mathf.Round(spawnPos.y - enemyDiameter - AbstractEnemy.extraDistanceFromWall);
            int lowerBound = (int) Mathf.Round(spawnPos.y + enemyDiameter + AbstractEnemy.extraDistanceFromWall);

            for (int x = leftBound; x <= rightBound; x++) {
                for (int y = upperBound; y <= lowerBound; y++) {
                    if (room.TileSymbolAtPosition(x, y) != '.') {
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
}
