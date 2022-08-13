using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using AdaptiveWizard.Assets.Scripts.Enemies.General.Interfaces;
using AdaptiveWizard.Assets.Scripts.Enemies.Movement.General;
using AdaptiveWizard.Assets.Scripts.Enemies.General.AbstractClasses;


namespace AdaptiveWizard.Assets.Scripts.Enemies.Enemies.WalkingEyeball.WalkingEyeballSmall
{
    public class WalkState : IState
    {
        private WalkingEyeballSmall walkingEyeballSmall;
        private Animator animator;
        private SpriteRenderer spriteRenderer;
        private EnemyMovement movement;
        private const float speed = 3.5f;
        private const float meleeRange = 2f;


        public WalkState(WalkingEyeballSmall walkingEyeballSmall) {
            this.walkingEyeballSmall = walkingEyeballSmall;
            this.animator = walkingEyeballSmall.GetComponent<Animator>();
            this.spriteRenderer = walkingEyeballSmall.GetComponent<SpriteRenderer>();
            this.movement = new EnemyMovement(walkingEyeballSmall);
        }

        public int OnEnter() {
            animator.SetTrigger("TrWalk");
            return 0;
        }

        public int StateUpdate() {
            if (IsInMeleeRange()) {
                // Change to attack state
                return 1;
            } 
            // Move closer to the player and try to get into melee range
            movement.MoveTowardsPlayer(speed);
            UpdateSpriteOrientation(movement.GetLastMovementVector().x);
            return 0;
        }

        public void UpdateSpriteOrientation(float xDir) {
            if (xDir > 0) {
                spriteRenderer.flipX = true;
            } else if (xDir < 0) {
                spriteRenderer.flipX = false;
            }
            // Don't change orientation if xDir == 0
        }

        private bool IsInMeleeRange() {
            return DistanceToPlayer() < meleeRange;
        }

        private float DistanceToPlayer() {
            return walkingEyeballSmall.VectorToPlayer().magnitude;
        }

        public EnemyMovement GetMovement() {
            return movement;
        }
    }
}
