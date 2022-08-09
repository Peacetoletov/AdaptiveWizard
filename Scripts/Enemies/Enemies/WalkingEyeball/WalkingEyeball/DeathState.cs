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
    public class DeathState : IState
    {
        private Animator animator;
        private Timer timer;


        public DeathState(WalkingEyeball walkingEyeball) {
            this.animator = walkingEyeball.GetComponent<Animator>();
        }

        public int OnEnter() {
            //Debug.Log("Entered Death state");
            animator.SetTrigger("TrDeath");
            float deathAnimationLength = animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Base Layer")).length;
            this.timer = new Timer(deathAnimationLength);
            return 0;
        }

        public int StateUpdate() {
            if (timer.UpdateAndCheck()) {
                // Stop playing animation
                animator.enabled = false;
                return 1;
            }
            return 0;
        }
    }
}
