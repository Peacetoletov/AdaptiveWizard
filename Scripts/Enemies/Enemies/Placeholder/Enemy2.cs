using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdaptiveWizard.Assets.Scripts.Other.GameManagers;
//using AdaptiveWizard.Assets.Scripts.Enemies.AbstractClasses.MovingEnemy;
using AdaptiveWizard.Assets.Scripts.Other.Other;


namespace AdaptiveWizard.Assets.Scripts.Enemies.Enemies
{
    /*
    public class Enemy2 : MovingEnemy
    {
        public GameObject projectile;
        private Timer projectileSpawnTimer;

        private void Start() {
            // initialize
            base.Init(100f, 1.5f);
            
            float projectileSpawnPeriod = 3.0f;
            float projectileSpawnPeriodVariance = 0.5f;
            float initialDelay = Random.Range(-0.5f * projectileSpawnPeriod, 0.5f * projectileSpawnPeriod);
            this.projectileSpawnTimer = new Timer(projectileSpawnPeriod, projectileSpawnPeriodVariance, initialDelay);
        }

        private void Update() {
            if (MainGameManager.IsGameActive()) {
                if (projectileSpawnTimer.UpdateAndCheck()) {
                    SpawnProjectile();
                }
            }
        }

        private void SpawnProjectile() {
            Vector2 direction = DirectionToPlayer();
            float offset = 0.8f;       // how far away from the enemy will the fireball spawn 
            Vector2 spawnPos = (Vector2) transform.position + direction.normalized * offset;
            GameObject newProjectile = Instantiate(projectile, spawnPos, Quaternion.identity) as GameObject;
            Enemy2Projectile projectileScript = newProjectile.GetComponent<Enemy2Projectile>();
            projectileScript.Start(direction);
        }
    }
    */
}
