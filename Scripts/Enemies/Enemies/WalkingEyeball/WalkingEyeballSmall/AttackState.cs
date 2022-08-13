using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using AdaptiveWizard.Assets.Scripts.Enemies.General.Interfaces;
using AdaptiveWizard.Assets.Scripts.Other.Other;


/*
Melee attack occurs when the enemy is very close to the player.
*/
namespace AdaptiveWizard.Assets.Scripts.Enemies.Enemies.WalkingEyeball.WalkingEyeballSmall
{
    public class AttackState : IState
    {
        private WalkingEyeballSmall walkingEyeballSmall;
        private Animator animator;
        private Timer timer;
        private Vector2 initialDirToPlayer;


        public AttackState(WalkingEyeballSmall walkingEyeballSmall) {
            this.walkingEyeballSmall = walkingEyeballSmall;
            this.animator = walkingEyeballSmall.GetComponent<Animator>();
        }

        public int OnEnter() {
            //Debug.Log("Entered Melee Attack state");
            this.animator.SetTrigger("TrAttack");
            const float totalFrames = 9;
            const float fps = 12;
            float animationLength = totalFrames / fps;
            this.timer = new Timer(animationLength);
            this.initialDirToPlayer = walkingEyeballSmall.VectorToPlayer().normalized;
            walkingEyeballSmall.GetWalkState().UpdateSpriteOrientation(initialDirToPlayer.x);
            return 0;
        }

        public int StateUpdate() {
            if (timer.UpdateAndCheck()) {
                return 1;
            } 
            const float movementSpeedDuringAttack = 4.5f;
            walkingEyeballSmall.GetWalkState().GetMovement().MoveInDirection(movementSpeedDuringAttack, initialDirToPlayer);
            return 0;            
        }
    }
}
