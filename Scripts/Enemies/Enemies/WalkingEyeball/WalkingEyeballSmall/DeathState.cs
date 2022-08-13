using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using AdaptiveWizard.Assets.Scripts.Enemies.General.Interfaces;
using AdaptiveWizard.Assets.Scripts.Other.Other;


namespace AdaptiveWizard.Assets.Scripts.Enemies.Enemies.WalkingEyeball.WalkingEyeballSmall
{
    public class DeathState : IState
    {
        private WalkingEyeballSmall walkingEyeballSmall;
        private Animator animator;
        private Timer timer;


        public DeathState(WalkingEyeballSmall walkingEyeballSmall) {
            this.walkingEyeballSmall = walkingEyeballSmall;
            this.animator = walkingEyeballSmall.GetComponent<Animator>();
        }

        public int OnEnter() {
            animator.SetTrigger("TrDeath");
            const float totalFrames = 12;
            const float fps = 12;
            float deathAnimationLength = totalFrames / fps;
            this.timer = new Timer(deathAnimationLength);
            return 0;
        }

        public int StateUpdate() {
            if (timer.UpdateAndCheck()) {
                return 1;
            }
            return 0;
        }
    }
}
