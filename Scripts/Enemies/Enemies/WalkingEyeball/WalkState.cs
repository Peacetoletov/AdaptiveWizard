using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using AdaptiveWizard.Assets.Scripts.Enemies.Interfaces;
using AdaptiveWizard.Assets.Scripts.Other.Other;
using AdaptiveWizard.Assets.Scripts.Enemies.Movement;


namespace AdaptiveWizard.Assets.Scripts.Enemies.Enemies.WalkingEyeball
{
    public class WalkState : IState
    {
        private WalkingEyeball walkingEyeball;
        private Animator animator;
        //private Timer timer;

        private SpriteRenderer spriteRenderer;

        //private EnemyMovement movement;

        public WalkState(WalkingEyeball walkingEyeball, BoxCollider2D terrainCollider) {
            this.walkingEyeball = walkingEyeball;
            this.animator = walkingEyeball.GetComponent<Animator>();
            this.spriteRenderer = walkingEyeball.GetComponent<SpriteRenderer>();
            //this.movement = new EnemyMovement(2f, terrainCollider, walkingEyeball);
        }

        public int OnEnter() {
            Debug.Log("Entered Walk state");
            animator.SetTrigger("TrWalk");
            //this.timer = new Timer(4f);
            return 0;
        }

        public int Update() {

            float distanceToPlayer = walkingEyeball.VectorToPlayer().magnitude;
            if (distanceToPlayer < 2f) {
                // Change to slash attack state
                return 1;
            } 
            else if (distanceToPlayer < 4f) {
                // Move closer to player, try to get into melee range
                /*
                movement.UpdateMovementTowardsPlayer();
                UpdateSpriteOrientation(movement.GetLastMovementVector().x);
                */
                return 0;
            }
            else {
                // Move into position for a ranged attack
                // TODO: I need to do a lot of changes to enemy movement in order to implement this. Yikes.
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

        /*
        public EnemyMovement GetEnemyMovement() {
            return movement;
        }
        */

        public int OnLeave() {
            return 0;
        }
    }
}
