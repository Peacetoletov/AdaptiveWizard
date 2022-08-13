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
        private WalkingEyeball walkingEyeball;
        private Animator animator;
        private FixedTimer animationTimer;
        private FixedTimer[] walkingEyeballSmallSpawnTimers = new FixedTimer[3];
        private bool[] walkingEyeballSmallSpawned = new bool[3];


        public DeathState(WalkingEyeball walkingEyeball) {
            this.walkingEyeball = walkingEyeball;
            this.animator = walkingEyeball.GetComponent<Animator>();
        }

        public int OnEnter() {
            //Debug.Log("Entered Death state");
            animator.SetTrigger("TrDeath");
            const float totalFrames = 14;
            const float fps = 12;
            float deathAnimationLength = totalFrames / fps;

            this.animationTimer = new FixedTimer(deathAnimationLength);
            
            for (int i = 0; i < 3; i++) {
                this.walkingEyeballSmallSpawnTimers[i] = new FixedTimer(deathAnimationLength * (0.4f + i * 0.1f));
            }
            return 0;
        }

        public int StateUpdate() {
            for (int i = 0; i < 3; i++) {
                // Spawn 3 small eyeballs
                if (!walkingEyeballSmallSpawned[i] && walkingEyeballSmallSpawnTimers[i].UpdateAndCheck()) {
                    Vector2 pos = (Vector2) walkingEyeball.transform.position + new Vector2(0.5f * (i - 1), -0.25f);
                    walkingEyeball.InstantiateWalkingEyeballSmall(pos);
                    this.walkingEyeballSmallSpawned[i] = true;
                }
            }
            
            if (animationTimer.UpdateAndCheck()) {
                return 1;
            }
            return 0;
        }
    }
}
