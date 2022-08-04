using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdaptiveWizard.Assets.Scripts.Other.Other;
using AdaptiveWizard.Assets.Scripts.Other.GameManagers;
using AdaptiveWizard.Assets.Scripts.Enemies.General.AbstractClasses;


/*
TODO: maybe I was doing the collision detection in a very inefficient and complicated way.
Check out CircleCastAll function (https://docs.huihoo.com/unity/5.5/Documentation/ScriptReference/Physics2D.CircleCastAll.html)
and other Physics2D functions (https://docs.huihoo.com/unity/5.5/Documentation/ScriptReference/Physics2D.html).
*/
namespace AdaptiveWizard.Assets.Scripts.Spells.SpellBehaviour
{
    public class Cannonball : StraightProjectile
    {
        /*
        Cannonball is bigger than the player, so if the player is standing close to a wall and fires a cannonball
        parallel to the wall, it could instantly collide with the wall and disappear. To prevent this issue, I will
        use two distinct colliders - one of the same size as the sprite, used for detecting collision with enemies.
        The second collider will smaller (about the same size as player) and will be used for detection collision
        with walls.
        */
        public CircleCollider2D circleColliderForEnemies;
        public CircleCollider2D circleColliderForWalls;
        

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
            this.enemiesHitID = new List<int>();
        }

        public void Start(Vector2 direction) {
            float speed = 9f;
            float damage = 101f;
            base.Start(direction, speed, damage);
        }

        protected override void FixedUpdate() {
            base.FixedUpdate();
            if (MainGameManager.IsGameActive()) {
                // find all enemies and walls hit, change their layer
                List<GameObject> collidingEnemies = new List<GameObject>();
                FindCollidingEnemiesAndChangeTheirLayer(collidingEnemies);

                // damage enemies, change their layer back
                DamageEnemiesAndChangeLayerBack(collidingEnemies);
                
                // destroy this object if at least one wall was hit
                DestroySelfIfHitWall();

                // move
                transform.position += (Vector3) GetDirection().normalized * GetSpeed() * Time.deltaTime; 
            }
        }

        private void FindCollidingEnemiesAndChangeTheirLayer(List<GameObject> collidingEnemies) {
            /*
            This function finds all colliding enemies and puts them into the collidingEnemies list. As a side effect, 
            the layer of all colliding enemies is changed to "Tmp" and must be changed back later.
            */

            while (true) {
                RaycastHit2D hit = Physics2D.CircleCast(transform.position, circleColliderForEnemies.radius, Vector2.zero, 0f, LayerMask.GetMask("Enemy"));
                if (hit.collider == null) {
                    break;
                }
                GameObject objectHit = hit.transform.gameObject;
                collidingEnemies.Add(objectHit);

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

        private void DestroySelfIfHitWall() {
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, circleColliderForWalls.radius, Vector2.zero, 0f, LayerMask.GetMask("Wall"));
            if (hit.collider != null) {
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
}
