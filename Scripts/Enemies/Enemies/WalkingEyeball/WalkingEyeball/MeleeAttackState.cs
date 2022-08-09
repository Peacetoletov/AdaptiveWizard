using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using AdaptiveWizard.Assets.Scripts.Enemies.General.Interfaces;
using AdaptiveWizard.Assets.Scripts.Other.Other;


/*
Melee attack occurs when the enemy is very close to the player, unless the enemy recently shot
a ranged attack.
*/
namespace AdaptiveWizard.Assets.Scripts.Enemies.Enemies.WalkingEyeball.WalkingEyeball
{
    public class MeleeAttackState : IState
    {
        private WalkingEyeball walkingEyeball;
        private Animator animator;
        private Timer timer;
        private Vector2 initialDirToPlayer;


        public MeleeAttackState(WalkingEyeball walkingEyeball) {
            this.walkingEyeball = walkingEyeball;
            this.animator = walkingEyeball.GetComponent<Animator>();
        }

        public int OnEnter() {
            //Debug.Log("Entered Melee Attack state");
            this.animator.SetTrigger("TrMeleeAttack");
            float animationLength = animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Base Layer")).length;
            this.timer = new Timer(animationLength);
            this.initialDirToPlayer = walkingEyeball.VectorToPlayer().normalized;
            walkingEyeball.GetWalkState().UpdateSpriteOrientation(initialDirToPlayer.x);
            return 0;
        }

        public int StateUpdate() {
            if (timer.UpdateAndCheck()) {
                return 1;
            } 
            const float movementSpeedDuringAttack = 3f;
            walkingEyeball.GetWalkState().GetMovement().MoveInDirection(movementSpeedDuringAttack, initialDirToPlayer);
            return 0;            
        }
    }
}
