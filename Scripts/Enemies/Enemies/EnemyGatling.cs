using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGatling : MovingEnemy
{
    public GameObject projectile;
    private Timer projectileSpawnTimer;

    private void Start() {
        // initialize
        base.Start(200f);
        
        float projectileSpawnPeriod = 0.33f;
        this.projectileSpawnTimer = new Timer(projectileSpawnPeriod);
    }

    private void Update() {
        if (TestRoomManager.IsGameActive()) {
            if (projectileSpawnTimer.UpdateAndCheck()) {
                SpawnProjectile();
            }
        }
    }

    private void SpawnProjectile() {
        Vector2 direction = DirectionToPlayer();
        Vector2 spawnPos = (Vector2) transform.position + new Vector2(0f, -0.25f);   // Adding a fixed offset so that the projectiles spawn at the mouth
        GameObject newProjectile = Instantiate(projectile, spawnPos, Quaternion.identity) as GameObject;
        EnemyGatlingProjectile projectileScript = newProjectile.GetComponent<EnemyGatlingProjectile>();
        projectileScript.Start(direction);
    }
}
