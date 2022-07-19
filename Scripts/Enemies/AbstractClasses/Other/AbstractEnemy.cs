using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdaptiveWizard.Assets.Scripts.Other.Rooms;
using AdaptiveWizard.Assets.Scripts.UI;
using AdaptiveWizard.Assets.Scripts.Player.Inventory;
using AdaptiveWizard.Assets.Scripts.Items.Active.Classes.Concrete;
using AdaptiveWizard.Assets.Scripts.Other.GameManagers;


namespace AdaptiveWizard.Assets.Scripts.Enemies.AbstractClasses.Other
{
    public abstract class AbstractEnemy : MonoBehaviour
    {
        private static int ID_Counter = 0;
        private int ID;
        private float maxHealth;
        private float curHealth;
        private CombatManager combatManager;

        // Extra distance is used when spawning enemies to ensure they don't spawn too close to a wall. 
        // This distance is also added to delta when checking collisions (casting a box). Without this 
        // buffer, enemies could possibly get stuck in a wall on rare occasions (presumably due to
        // floating point errors).
        public const float extraDistanceFromWall = 0.01f;

        protected virtual void Start(float maxHealth) {
            // set ID and increment ID counter
            this.ID = AbstractEnemy.ID_Counter++;

            // set health
            this.maxHealth = maxHealth;
            this.curHealth = maxHealth;
        }

        public void SetCombatManager(CombatManager combatManager) {
            this.combatManager = combatManager;
        }

        protected Vector2 DirectionToPlayer() {
            // Returns a vector directed from this enemy to the player 
            return MainGameManager.GetPlayer().transform.position - transform.position;
        }

        public void TakeDamage(float damage) {
            this.curHealth -= damage;
            //print("Took " + damage + " damage");
            CheckDeath();
        }

        private void CheckDeath() {
            if (curHealth <= 0) {
                Destroy(gameObject);

                combatManager.OnEnemyDeath();
                
                // temporarily
                ScoreScriptUI.IncreaseScore((int) Mathf.Round(maxHealth / 30f));
            }

            // temporary - small chance to receive a potion after killing an enemy
            // even more temporary - always receive a potion after killing an enemy
            //  if (Random.Range(0f, 1f) < 0.04f) {
            if (Random.Range(0f, 1f) < 1f) {
                print("Killed an enemy, adding an active item");
                if (Random.Range(0f, 1f) < 0.5f) {
                    InventoryManager.activeItemManager.AddItem(new HealthPotion());
                } else {
                    InventoryManager.activeItemManager.AddItem(new ManaPotion());
                }
            }
        }

        public int GetID() {
            return this.ID;
        }
    }
}
