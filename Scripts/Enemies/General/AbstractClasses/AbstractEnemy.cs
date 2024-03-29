using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using AdaptiveWizard.Assets.Scripts.Other.Rooms;
using AdaptiveWizard.Assets.Scripts.UI;
using AdaptiveWizard.Assets.Scripts.Player.Inventory;
using AdaptiveWizard.Assets.Scripts.Items.Active.Classes.Concrete;
using AdaptiveWizard.Assets.Scripts.Other.GameManagers;


// TODO: update this class
namespace AdaptiveWizard.Assets.Scripts.Enemies.General.AbstractClasses
{
    public abstract class AbstractEnemy : MonoBehaviour
    {
        // Box collider used for collision detecting with terrain. Every enemy's terrain collider will be a box, regardless
        // of enemy shape. More precise colliders can be used for collision with player and player spells.
        // For square-shaped enemies, one collider can be used for collision checking with both terrain and player/spells.
        public BoxCollider2D terrainCollider;
        
        private static int ID_Counter = 0;
        private int ID;
        private float maxHealth;
        private float curHealth;
        private bool dead = false;
        private CombatManager combatManager;

        // Variables related to flashing white when taking taking
        private Material material;
        private const float flashDuration = 0.015f;
        private float flashTime = 0f;
        private bool isFlashing = false;

        // Variables related to (pausing) animations
        private Animator animator;

        // Extra distance is used when spawning enemies to ensure they don't spawn too close to a wall. 
        // This distance is also added to delta when checking collisions (casting a box). Without this 
        // buffer, enemies could possibly get stuck in a wall on rare occasions (presumably due to
        // floating point errors).
        public const float extraDistanceFromWall = 0.01f;


        public abstract void Init();

        protected void Init(float maxHealth) {
            // set ID and increment ID counter
            this.ID = AbstractEnemy.ID_Counter++;

            // set health
            this.maxHealth = maxHealth;
            this.curHealth = maxHealth;

            // get components
            this.material = GetComponent<SpriteRenderer>().material;
            this.animator = GetComponent<Animator>();
        }

        protected virtual void Update() {
            if (MainGameManager.IsGameActive()) {
                // unpausing animation
                this.animator.enabled = true;

                // Flashing white
                if (flashTime > flashDuration) {
                    this.flashTime = 0;
                    this.isFlashing = false;
                    material.SetFloat("_AddedColorValue", 0);
                }
                if (isFlashing) {
                    this.flashTime += Time.deltaTime;
                }
            }
            else {
                // pausing animation
                this.animator.enabled = false;
            }
        }

        public Vector2 VectorToPlayer() {
            return MainGameManager.GetPlayer().transform.position - transform.position;
        }

        public void SetCombatManager(CombatManager combatManager) {
            this.combatManager = combatManager;
        }

        public void TakeDamage(float damage) {
            this.curHealth -= damage;
            //print("Took " + damage + " damage");
            OnTakeDamage(damage);
        }

        protected virtual void OnTakeDamage(float damage) {
            // flashing white
            this.isFlashing = true;
            material.SetFloat("_AddedColorValue", 1);

            // death?
            if (!dead) {
                CheckDeath();
            }
        }

        private void CheckDeath() {
            if (curHealth <= 0) {
                //Destroy(gameObject);

                this.dead = true;
                gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                //Debug.Log("Enemy is dead");
                combatManager.OnEnemyDeath();
                
                // temporarily
                ScoreScriptUI.IncreaseScore((int) Mathf.Round(maxHealth / 30f));
            }

            // temporary - small chance to receive a potion after killing an enemy
            // even more temporary - always receive a potion after killing an enemy
            //  if (Random.Range(0f, 1f) < 0.04f) {
            /*
            if (Random.Range(0f, 1f) < 1f) {
                print("Killed an enemy, adding an active item");
                if (Random.Range(0f, 1f) < 0.5f) {
                    InventoryManager.activeItemManager.AddItem(new HealthPotion());
                } else {
                    InventoryManager.activeItemManager.AddItem(new ManaPotion());
                }
            }
            */
        }

        public int GetID() {
            return ID;
        }

        public bool IsDead() {
            return dead;
        }

        protected CombatManager GetCombatManager() {
            return combatManager;
        }
    }
}
