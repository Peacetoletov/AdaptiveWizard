using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using AdaptiveWizard.Assets.Scripts.Enemies.General.Interfaces;
using AdaptiveWizard.Assets.Scripts.Other.Other;


/*
Idle state occurs when the enemy spawns (this might be needed to change in the future), or after 
the enemy shoots a ranged attack.
*/
namespace AdaptiveWizard.Assets.Scripts.Enemies.Enemies.WalkingEyeball.WalkingEyeball
{
    public class IdleState : IState
    {
        private Animator animator;
        private FixedTimer timer;
        private WalkingEyeball walkingEyeball;


        public IdleState(WalkingEyeball walkingEyeball) {
            this.animator = walkingEyeball.GetComponent<Animator>();
            this.walkingEyeball = walkingEyeball;
        }

        public int OnEnter() {
            //Debug.Log("Entered Idle state");
            this.animator.SetTrigger("TrIdle");
            this.timer = new FixedTimer(2.0f);
            return 0;
        }

        public int StateUpdate() {
            if (timer.UpdateAndCheck()) {
                WalkState ws = walkingEyeball.GetWalkState();
                if (ws.ShouldAttemptRangedAttack() && ws.CanHitPlayerWithRangedAttackFromCurrentPosition()) {
                    // Change to ranged attack state
                    return 2;
                }
                // Change to walk state
                return 1;
            }

            // Stay in idle state
            return 0;
        }
    }
}
