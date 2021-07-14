using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannonball : StraightProjectile
{
    private CircleCollider2D circleCollider;

    private void Start() {
        this.circleCollider = GetComponent<CircleCollider2D>();
    }

    public void Start(Vector2 direction) {
        float speed = 9f;
        float damage = 110f;
        base.Start(direction, speed, damage);
    }

    // TODO: split this function into multiple functions
    private void FixedUpdate() {
        if (TestRoomManager.IsGameActive()) {
            List<GameObject> collidingEnemies = new List<GameObject>();
            List<GameObject> collidingWalls = new List<GameObject>();

            // find all enemies and walls hit
            while (true) {
                RaycastHit2D hit = Physics2D.CircleCast(transform.position, circleCollider.radius, Vector2.zero, 0f, LayerMask.GetMask("Enemy", "Wall"));
                if (hit.collider == null) {
                    break;
                }

                GameObject objectHit = hit.transform.gameObject;
                if (objectHit.layer == LayerMask.NameToLayer("Enemy")) {
                    collidingEnemies.Add(objectHit);
                }
                else {
                    collidingWalls.Add(objectHit);
                }

                // temporarily change the object's layer
                objectHit.layer = LayerMask.NameToLayer("Tmp");
            }

            // perform actions on hit objects and change their layer back
            foreach (GameObject obj in collidingEnemies) {
                // damage the enemy
                AbstractEnemy enemy = obj.GetComponent<AbstractEnemy>();
                enemy.TakeDamage(GetDamage());
                
                obj.layer = LayerMask.NameToLayer("Enemy");
            }

            foreach (GameObject obj in collidingWalls) {
                obj.layer = LayerMask.NameToLayer("Wall");
            }

            // destroy this object if at least one wall was hit
            if (collidingWalls.Count > 0) {
                Destroy(gameObject);
            }

            // move
            transform.position += (Vector3) GetDirection().normalized * GetSpeed() * Time.deltaTime; 
        }
    }
}
