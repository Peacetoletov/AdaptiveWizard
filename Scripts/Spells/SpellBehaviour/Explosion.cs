using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdaptiveWizard.Assets.Scripts.Enemies.General.AbstractClasses;


namespace AdaptiveWizard.Assets.Scripts.Spells.SpellBehaviour
{
    public class Explosion : MonoBehaviour
    {

        private void Start() {

            float damage = 101f;

            /*
            1. Cast a circle and look for colliding objects with layer "Enemy".
            2. If an object is hit, save it in a list and temporarily change its layer to "Tmp".
            3. Perform an action on the colliding object (deal damage, ...).
            4. Repeat until CircleCast returns a null collider.
            5. Change the layer of all saved objects back to "Enemy".
            */

            CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
            List<GameObject> collidingObjects = new List<GameObject>();
            while (true) {
                RaycastHit2D hit = Physics2D.CircleCast(transform.position, circleCollider.radius, Vector2.zero, 0f, LayerMask.GetMask("Enemy"));
                if (hit.collider == null) {
                    break;
                }
                GameObject objectHit = hit.transform.gameObject;
                collidingObjects.Add(objectHit);

                // temporarily change the object's layer
                objectHit.layer = LayerMask.NameToLayer("Tmp");

                // damage the enemy
                AbstractEnemy enemy = objectHit.GetComponent<AbstractEnemy>();
                enemy.TakeDamage(damage);
            }

            // Change the layers of all colliding enemies back
            foreach (GameObject obj in collidingObjects) {
                obj.layer = LayerMask.NameToLayer("Enemy");
            }
        }
    }
}
