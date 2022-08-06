using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using AdaptiveWizard.Assets.Scripts.Enemies.General.Interfaces;
using AdaptiveWizard.Assets.Scripts.Other.Other;
using AdaptiveWizard.Assets.Scripts.Player.Other;
using AdaptiveWizard.Assets.Scripts.Other.GameManagers;
using AdaptiveWizard.Assets.Scripts.Enemies.Enemies.WalkingEyeball.Web;

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
        private SpriteRenderer spriteRenderer;
        private Animator animator;
        private Timer timer;
        private const float range = 20;
        private bool spawned;
        //private AbstractPlayer player;
        private Vector2 spawnPos;
        private Vector2 direction;


        public RangedAttackState(WalkingEyeball walkingEyeball) {
            this.walkingEyeball = walkingEyeball;
            this.spriteRenderer = walkingEyeball.GetComponent<SpriteRenderer>();
            this.animator = walkingEyeball.GetComponent<Animator>();
            //this.player = MainGameManager.GetPlayer().GetComponent<AbstractPlayer>();
        }

        public int OnEnter() {
            animator.SetTrigger("TrRangedAttack");
            animator.Play("Ranged Attack", 0, 0f);      // Setting animation time to 0
            Debug.Log("Entered Ranged Attack state");
            this.spawned = false;
            spriteRenderer.flipX = direction.x > 0;
            return 0;
        }

        public void SetAttackProperties(Vector2 spawnPos, Vector2 direction) {
            this.spawnPos = spawnPos;
            this.direction = direction;
        }

        public int Update() {
            Debug.Log($"Animation time: {animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Base Layer")).normalizedTime}");
            const float spawnFrame = 5;
            const float totalFrames = 10;
            const float fps = 12;

            float animationTime = animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Base Layer")).normalizedTime;

            if (!spawned && animationTime >= spawnFrame / fps) {
                // TODO - make both spawn directions work
                GameObject web = walkingEyeball.InstantiateWeb(spawnPos);
                web.GetComponent<Web.Web>().SetDirection(direction);
                this.spawned = true;

                //Debug.Log("Spawned!");
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
