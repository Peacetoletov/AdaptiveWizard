using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using AdaptiveWizard.Assets.Scripts.Enemies.General.Interfaces;
using AdaptiveWizard.Assets.Scripts.Other.Other;
using AdaptiveWizard.Assets.Scripts.Enemies.Movement.General;


namespace AdaptiveWizard.Assets.Scripts.Enemies.Enemies.WalkingEyeball.WalkingEyeball
{
    public class WalkState : IState
    {
        private WalkingEyeball walkingEyeball;
        private Animator animator;
        private SpriteRenderer spriteRenderer;
        private EnemyMovement movement;
        private BoxCollider2D projectileCollider;
        private RangedAttackPositionFinder rapf;
        private const float speed = 2f;
        private const float projectileMaxTravelDistance = 20f;

        // Variables related to ranged attacks
        private bool seekingRangedAttack = false;
        private Vector2 rangedAttackPosition;
        private bool rangedAttacksAllowed = true;

        public WalkState(WalkingEyeball walkingEyeball, BoxCollider2D projectileCollider) {
            this.walkingEyeball = walkingEyeball;
            this.animator = walkingEyeball.GetComponent<Animator>();
            this.spriteRenderer = walkingEyeball.GetComponent<SpriteRenderer>();
            this.projectileCollider = projectileCollider;
            this.movement = new EnemyMovement(walkingEyeball);

            this.rapf = new RangedAttackPositionFinder();
        }

        public int OnEnter() {
            //Debug.Log("Entered Walk state");
            animator.SetTrigger("TrWalk");
            //this.timer = new Timer(4f);
            return 0;
        }

        public int Update() {
            
            float distanceToPlayer = walkingEyeball.VectorToPlayer().magnitude;
            const float meleeRange = 2f;
            const float almostMeleeRange = 3.5f;
            UpdateRangedAttackPositionIfNecessary(distanceToPlayer, almostMeleeRange);

            if (distanceToPlayer < meleeRange) {
                // Change to slash attack state
                Debug.Log("In melee range.");
                return 1;
            } 
            else if (distanceToPlayer < almostMeleeRange || !rangedAttacksAllowed) {
                // If the enemy is close to the player or is not allowed to perform ranged attacks, move closer
                // to the player and try to get into melee range
                Debug.Log("Almost in melee range. Moving closer to player.");
                movement.MoveTowardsPlayer(speed);
                UpdateSpriteOrientation(movement.GetLastMovementVector().x);
                return 0;
            }
            else {
                // Move into position for a ranged attack
                //Debug.Log($"Moving towards position: {rangedAttackPosition}");
                int movementReturnCode = movement.MoveTowardsPosition(speed, rangedAttackPosition);
                //Debug.Log($"Movement vector (x100): {movement.GetLastMovementVector() * 100}");
                

                // If player can be hit with a ranged attack from the current position, change to ranged attack state
                if (rapf.CanHit(walkingEyeball.transform.position, projectileCollider.size, projectileMaxTravelDistance)) {
                    Debug.Log("Player can be hit with a ranged attack from the current position. Returning 2.");
                    return 2;
                }

                // If position cannot be reached, permanently switch to melee mode
                if (movementReturnCode == 2) {
                    Debug.Log("Permanently switching to melee mode!");
                    this.rangedAttacksAllowed = false;
                }

                // If target position is reached but cannot hit player from the position, find a new position
                if (movementReturnCode == 1) {
                    Debug.Log("Position reached can player cannot be hit. Trying to find a new position.");
                    this.rangedAttackPosition = rapf.Find(walkingEyeball.transform.position, projectileCollider.size, projectileMaxTravelDistance);
                }

                //Debug.Log("Returning 0");
                return 0;
            }
            
        }

        public void UpdateSpriteOrientation(float xDir) {
            if (xDir > 0) {
                spriteRenderer.flipX = true;
            } else if (xDir < 0) {
                spriteRenderer.flipX = false;
            }
            // Don't change orientation if xDir == 0
        }

        private void UpdateRangedAttackPositionIfNecessary(float distanceToPlayer, float almostMeleeRange) {
            if (distanceToPlayer < almostMeleeRange) {
                seekingRangedAttack = false;
            } else {
                if (!seekingRangedAttack) {
                    // Just got far enough from the player to seek a ranged attack. Needs to find a good position.
                    this.rangedAttackPosition = rapf.Find(walkingEyeball.transform.position, projectileCollider.size, projectileMaxTravelDistance);
                }
                seekingRangedAttack = true;
            }
        }

        /*
        public EnemyMovement GetEnemyMovement() {
            return movement;
        }
        */
    }
}
