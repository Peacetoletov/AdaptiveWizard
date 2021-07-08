using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2 : AbstractEnemy
{
    public GameObject projectile;
    private float projectileSpawnPeriod = 3.0f;
    private float projectileSpawnPeriodVariance = 0.5f;
    
    private void Start()
    {
        // initialize
        base.Start(100f);

        // start spawning projectiles
        float initialDelay = Random.Range(projectileSpawnPeriod, 2 * projectileSpawnPeriod);
        StartCoroutine(SpawnProjectilesPeriodically(initialDelay));
    }

    private IEnumerator SpawnProjectilesPeriodically(float initialDelay) {
        yield return new WaitForSeconds(initialDelay);
        while (true) {
            SpawnProjectile();
            float varianceDelay = Random.Range(0f, projectileSpawnPeriodVariance);
            yield return new WaitForSeconds(projectileSpawnPeriod + varianceDelay);
        }
    }

    private void SpawnProjectile() {
        Vector2 direction = DirectionToPlayer();
        float offset = 0.8f;       // how far away from the enemy will the fireball spawn 
        Vector2 spawnPos = (Vector2) transform.position + direction.normalized * offset;
        GameObject newProjectile = Instantiate(projectile, spawnPos, Quaternion.identity) as GameObject;
        Enemy2Projectile projectileScript = newProjectile.GetComponent<Enemy2Projectile>();
        projectileScript.Init(direction);
    }
}
