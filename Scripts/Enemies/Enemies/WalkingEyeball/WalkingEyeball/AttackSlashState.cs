using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using AdaptiveWizard.Assets.Scripts.Enemies.General.Interfaces;
using AdaptiveWizard.Assets.Scripts.Other.Other;

namespace AdaptiveWizard.Assets.Scripts.Enemies.Enemies.WalkingEyeball.WalkingEyeball
{
    public class AttackSlashState : IState
    {
        private WalkingEyeball walkingEyeball;
        private Animator animator;
        private Timer timer;
        private Vector2 initialDirToPlayer;


        public AttackSlashState(WalkingEyeball walkingEyeball) {
            this.walkingEyeball = walkingEyeball;
            this.animator = walkingEyeball.GetComponent<Animator>();
        }

        public int OnEnter() {
            Debug.Log("Entered AttackSlash state");
            animator.SetTrigger("TrAttackSlash");
            float animationLength = animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Base Layer")).length;
            this.timer = new Timer(animationLength);
            this.initialDirToPlayer = walkingEyeball.VectorToPlayer().normalized;
            walkingEyeball.GetWalkState().UpdateSpriteOrientation(initialDirToPlayer.x);
            return 0;
        }

        public int Update() {
            if (timer.UpdateAndCheck()) {
                return 1;
            } else {
                const float additionalMovementMagnitude = 4f;
                Vector2 additionalMovementVector = initialDirToPlayer * additionalMovementMagnitude * Time.deltaTime;
                //walkingEyeball.GetWalkState().GetEnemyMovement().PublicTryToMove(additionalMovementVector);
                return 0;
            }
            
        }

        public int OnLeave() {
            return 0;
        }
    }
}
