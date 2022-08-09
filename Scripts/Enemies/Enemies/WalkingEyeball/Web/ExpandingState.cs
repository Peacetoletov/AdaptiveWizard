using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using AdaptiveWizard.Assets.Scripts.Enemies.General.Interfaces;


/*
Expanding state occurs after the web spawns and lasts until the animation finishes. Changes to Flying state afterwards.
Additionally, the web doesn't rotate in expanding state, as it wouldn't be noticable anyway.
*/
namespace AdaptiveWizard.Assets.Scripts.Enemies.Enemies.WalkingEyeball.Web
{
    public class ExpandingState : IState
    {
        private readonly Web web;
        private readonly Animator animator;
        private readonly BoxCollider2D initialCollider;
        private readonly float speed;
        private readonly float rotateSpeed;
        Vector2 direction;


        public ExpandingState(Web web, BoxCollider2D initialCollider, Vector2 direction, float speed, float rotateSpeed) {
            this.web = web;
            this.animator = web.GetComponent<Animator>();
            this.initialCollider = initialCollider;
            this.speed = speed;
            this.direction = direction;
            this.rotateSpeed = rotateSpeed;
        }

        public int OnEnter() {
            //Debug.Log("Entered Expanding state");
            animator.SetTrigger("TrExpand");
            return 0;
        }

        public int StateUpdate() {
            Utility.Rotate(web, rotateSpeed);
            int movementReturnCode = Utility.MoveAndCheckCollision(web, direction, speed, initialCollider);
            if (movementReturnCode == 1) {
                // Player was hit
                return 2;
            } else if (movementReturnCode == 2) {
                // Wall was hit
                return 3;
            }
            // Nothing was hit

            if (DidAnimationEnd()) {
                // Animation finished playing
                return 1;
            }
            // Nothing extraordinary happened
            return 0;
        }

        private bool DidAnimationEnd() {
            float normalizedTime = animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Base Layer")).normalizedTime;
            return normalizedTime >= 1;
        }
    }
}