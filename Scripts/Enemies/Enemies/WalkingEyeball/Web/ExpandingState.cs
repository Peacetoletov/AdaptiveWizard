using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using AdaptiveWizard.Assets.Scripts.Enemies.General.Interfaces;
using AdaptiveWizard.Assets.Scripts.Other.Other;


namespace AdaptiveWizard.Assets.Scripts.Enemies.Enemies.WalkingEyeball.Web
{
    public class ExpandingState : IState
    {
        private Web web;
        private Animator animator;
        private BoxCollider2D initialCollider;
        private float speed;
        Vector2 direction;


        public ExpandingState(Web web, BoxCollider2D initialCollider, Vector2 direction, float speed) {
            this.web = web;
            this.animator = web.GetComponent<Animator>();
            this.initialCollider = initialCollider;
            this.speed = speed;
            this.direction = direction;
        }

        public int OnEnter() {
            //Debug.Log("Entered Expanding state");
            animator.SetTrigger("TrExpand");
            return 0;
        }

        public int Update() {
            
            // int movementReturnCode = Utility.MoveAndCheckCollision(web, Vector2.left, speed, initialCollider);
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