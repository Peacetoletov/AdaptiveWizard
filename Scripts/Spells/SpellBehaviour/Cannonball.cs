using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannonball : StraightProjectile
{
    private CircleCollider2D circleCollider;

    /*
    I only want to damage each colliding enemy once. To prevent damaging the same enemy multiple times,
    I'm keeping the ID of each enemy already damaged in a list, and when colliding with an enemy, I will
    check if their ID appears in this list, and if it doesn't, they will take damage and get added to this 
    list. 
    This could be inefficient if many enemies are hit, and could result in O(n^2) complexity each frame.
    If this is a significant problem, I can replace the list with a B-tree to reduce the complexity to O(n * log n).
    */
    private List<int> enemiesHitID;

    private void Start() {
        this.circleCollider = GetComponent<CircleCollider2D>();
        this.enemiesHitID = new List<int>();
    }

    public void Start(Vector2 direction) {
        float speed = 9f;
        float damage = 101f;
        base.Start(direction, speed, damage);
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();
        if (TestRoomManager.IsGameActive()) {
            // find all enemies and walls hit, change their layer
            List<GameObject> collidingEnemies = new List<GameObject>();
            List<GameObject> collidingWalls = new List<GameObject>();
            FindCollidingEnemiesAndWallsAndChangeTheirLayer(collidingEnemies, collidingWalls);

            // damage enemies, change layers of colliding objects back
            DamageEnemiesAndChangeLayerBack(collidingEnemies);
            ChangeWallLayerBack(collidingWalls);
            
            // destroy this object if at least one wall was hit
            DestroySelfIfHitWall(collidingWalls);

            // move
            transform.position += (Vector3) GetDirection().normalized * GetSpeed() * Time.deltaTime; 
        }
    }

    private void FindCollidingEnemiesAndWallsAndChangeTheirLayer(List<GameObject> collidingEnemies, List<GameObject> collidingWalls) {
        /*
        This function finds all colliding enemies and puts them into the collidingEnemies list. Same for all colliding walls and the collidingWalls list.
        As a side effect, the layer of all colliding enemies and walls is changed to "Tmp" and must be changed back later.
        */
        while (true) {
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, circleCollider.radius, Vector2.zero, 0f, LayerMask.GetMask("Enemy", "Wall"));
            if (hit.collider == null) {
                break;
            }

            // add colliding objects to their corresponding list
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
    }

    private void DamageEnemiesAndChangeLayerBack(List<GameObject> collidingEnemies) {
        foreach (GameObject obj in collidingEnemies) {
            // damage the enemy
            AbstractEnemy enemy = obj.GetComponent<AbstractEnemy>();
            int enemyID = enemy.GetID();
            if (!IsNumberInList(enemyID, enemiesHitID)) {
                // if the enemy wasn't hit yet, deal damage to it and add it to the list of hit enemies
                enemy.TakeDamage(GetDamage());
                enemiesHitID.Add(enemyID);
            } 
            
            // change the layer
            obj.layer = LayerMask.NameToLayer("Enemy");
        }
    }

    private void ChangeWallLayerBack(List<GameObject> collidingWalls) {
        foreach (GameObject obj in collidingWalls) {
            obj.layer = LayerMask.NameToLayer("Wall");
        }
    }

    private void DestroySelfIfHitWall(List<GameObject> collidingWalls) {
        if (collidingWalls.Count > 0) {
            Destroy(gameObject);
        }
    }

    private bool IsNumberInList(int number, List<int> list) {
        foreach (int value in list) {
            if (number == value) {
                return true;
            }
        }
        return false;
    }
}
