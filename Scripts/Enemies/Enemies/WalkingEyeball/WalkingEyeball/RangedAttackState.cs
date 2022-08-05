using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using AdaptiveWizard.Assets.Scripts.Enemies.General.Interfaces;
using AdaptiveWizard.Assets.Scripts.Other.Other;

/*
TODO: Currently, given that I'm using Raycast, it's possible that although a raycast can hit the player,
the subsequent web will hit a wall.
Solution: Instead of using raycast, go back to using boxcast again. However, the direction will not be a straight line,
it will instead first calculate the vector from the enemy to player's center, and if this vector's angle falls within
some allowed range, only then will boxcast be performed. This eliminates issues of both previous implementations. 
*/
namespace AdaptiveWizard.Assets.Scripts.Enemies.Enemies.WalkingEyeball.WalkingEyeball
{
    public class RangedAttackState : IState
    {
        WalkingEyeball walkingEyeball;
        private Animator animator;
        private Timer timer;
        private const float range = 20;
        private bool spawned = false;


        public RangedAttackState(WalkingEyeball walkingEyeball) {
            this.walkingEyeball = walkingEyeball;
            this.animator = walkingEyeball.GetComponent<Animator>();
        }

        public int OnEnter() {
            animator.SetTrigger("TrRangedAttack");
            //Debug.Log($"Time on enter: {animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Base Layer")).normalizedTime}");
            animator.Play("Ranged Attack", 0, 0f);
            //Debug.Log($"Time after custom play method: {animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Base Layer")).normalizedTime}");
            //this.timer = new Timer(1.5f);
            Debug.Log("Entered Ranged Attack state");
            return 0;
        }

        public int Update() {
            Debug.Log($"Animation time: {animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Base Layer")).normalizedTime}");
            const float spawnFrame = 5;
            const float totalFrames = 10;
            const float fps = 12;

            float animationTime = animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Base Layer")).normalizedTime;

            if (!spawned && animationTime >= spawnFrame / fps) {
                // TODO - make both spawn directions work
                Vector2 pos = walkingEyeball.transform.position - new Vector3(1, 0, 0);
                walkingEyeball.InstantiateWeb(pos);
                //Debug.Log("Spawned!");
                this.spawned = true;

                // TODO - possibly change state to Idle for a couple seconds after spawning a web?
            }

            if (animationTime >= totalFrames / fps) {
                return 1;
            }
            
            /*
            if (timer.UpdateAndCheck()) {
                return 1;
            }
            */
            return 0;
        }
    }
}
