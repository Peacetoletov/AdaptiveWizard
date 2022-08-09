using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using AdaptiveWizard.Assets.Scripts.Enemies.General.Interfaces;
using AdaptiveWizard.Assets.Scripts.Enemies.Movement.General;
using AdaptiveWizard.Assets.Scripts.Enemies.General.AbstractClasses;


/*
Walk state occurs whenever the enemy isn't idle or attacking.
Walk state ends if:
    a) Enemy is very close to the player, enters melee attack state.
    b) Enemy is far from the player and can shoot the player from the current position, enters 
       ranged attack state.
In some situations, it might not be reasonably possible to find a suitable ranged attack position.
In such situations, the enemy permanently switches into melee mode and will never attempt ranged
attacks again.
*/
namespace AdaptiveWizard.Assets.Scripts.Enemies.Enemies.WalkingEyeball.WalkingEyeball
{
    public class WalkState : IState
    {
        private WalkingEyeball walkingEyeball;
        private Animator animator;
        private SpriteRenderer spriteRenderer;
        private EnemyMovement movement;
        private const float speed = 2.25f;
        private const float projectileMaxTravelDistance = 20f;
        private const float meleeRange = 2f;
        private const float almostMeleeRange = 4.5f;

        // Variables related to ranged attacks
        private readonly BoxCollider2D projectileCollider;
        private bool seekingRangedAttack = false;
        private Vector2 rangedAttackPosition;
        private bool rangedAttacksAllowed = true;

        public WalkState(WalkingEyeball walkingEyeball, BoxCollider2D projectileCollider) {
            this.walkingEyeball = walkingEyeball;
            this.animator = walkingEyeball.GetComponent<Animator>();
            this.spriteRenderer = walkingEyeball.GetComponent<SpriteRenderer>();
            this.movement = new EnemyMovement(walkingEyeball);
            this.projectileCollider = projectileCollider;
        }

        public int OnEnter() {
            //Debug.Log("Entered Walk state");
            animator.SetTrigger("TrWalk");
            return 0;
        }

        public int StateUpdate() {
            UpdateRangedAttackPositionIfNecessary();
            if (IsInMeleeRange()) {
                // Change to slash attack state
                return 1;
            } 
            else if (!ShouldAttemptRangedAttack()) {
                // If the enemy is close to the player or is not allowed to perform ranged attacks, move closer
                // to the player and try to get into melee range
                movement.MoveTowardsPlayer(speed);
                UpdateSpriteOrientation(movement.GetLastMovementVector().x);
                return 0;
            }
            else {
                // Move into position for a ranged attack
                int movementReturnCode = movement.MoveTowardsPosition(speed, rangedAttackPosition);
                UpdateSpriteOrientation(movement.GetLastMovementVector().x);

                // If player can be hit with a ranged attack from the current position, change to ranged attack state
                if (CanHitPlayerWithRangedAttackFromCurrentPosition()) {
                    ResetState();
                    return 2;
                }

                // If position cannot be reached, permanently switch to melee mode
                if (movementReturnCode == 2) {
                    this.rangedAttacksAllowed = false;
                }

                // If target position is reached but cannot hit player from the position, find a new position
                if (movementReturnCode == 1) {
                    try {
                        this.rangedAttackPosition = RangedAttackState.rapf.Find(walkingEyeball.transform.position, projectileCollider, projectileMaxTravelDistance);
                    } catch (NoSuitablePositionException) {
                        this.rangedAttacksAllowed = false;
                    }
                }

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

        private void UpdateRangedAttackPositionIfNecessary() {
            if (DistanceToPlayer() < almostMeleeRange) {
                seekingRangedAttack = false;
            } else {
                if (!seekingRangedAttack) {
                    // Just got far enough from the player to seek a ranged attack. Needs to find a good position.
                    try {
                        this.rangedAttackPosition = RangedAttackState.rapf.Find(walkingEyeball.transform.position, projectileCollider, projectileMaxTravelDistance);
                    } catch (NoSuitablePositionException) {
                        this.rangedAttacksAllowed = false;
                    }
                }
                seekingRangedAttack = true;
            }
        }

        private bool IsInMeleeRange() {
            return DistanceToPlayer() < meleeRange;
        }

        private float DistanceToPlayer() {
            return walkingEyeball.VectorToPlayer().magnitude;
        }

        public bool ShouldAttemptRangedAttack() {
            return rangedAttacksAllowed && DistanceToPlayer() > almostMeleeRange;
        }

        public bool CanHitPlayerWithRangedAttackFromCurrentPosition() {
            Vector2 webSpawnPos = RangedAttackState.WebSpawnPos(walkingEyeball.transform.position);
            return RangedAttackState.rapf.CanHit(webSpawnPos, projectileCollider, projectileMaxTravelDistance);
        }

        private void ResetState() {
            seekingRangedAttack = false;
        }

        public EnemyMovement GetMovement() {
            return movement;
        }
    }
}
