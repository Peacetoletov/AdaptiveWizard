using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using AdaptiveWizard.Assets.Scripts.Enemies.General.Interfaces;
using AdaptiveWizard.Assets.Scripts.Other.Other;


// MAYBE TODO: possibly make small eyeballs invulnerable during the first few frames of the spawn animation
// as it currently is, they might be killed before even fully appearing and it looks kind of weird
namespace AdaptiveWizard.Assets.Scripts.Enemies.Enemies.WalkingEyeball.WalkingEyeballSmall
{
    public class SpawnState : IState
    {
        private Animator animator;
        private FixedTimer timer;


        public SpawnState(WalkingEyeballSmall walkingEyeballSmall) {
            this.animator = walkingEyeballSmall.GetComponent<Animator>();
        }

        public int OnEnter() {
            animator.SetTrigger("TrSpawn");
            const float totalFrames = 8;
            const float fps = 12;
            float spawnAnimationLength = totalFrames / fps;
            this.timer = new FixedTimer(spawnAnimationLength);
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