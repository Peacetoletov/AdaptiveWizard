using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using AdaptiveWizard.Assets.Scripts.Enemies.General.Interfaces;
//using AdaptiveWizard.Assets.Scripts.Other.Other;
using AdaptiveWizard.Assets.Scripts.Player.Other;
using AdaptiveWizard.Assets.Scripts.Other.GameManagers;
using AdaptiveWizard.Assets.Scripts.Enemies.Enemies.WalkingEyeball.Web;


namespace AdaptiveWizard.Assets.Scripts.Enemies.Enemies.WalkingEyeball.WalkingEyeball
{
    public class RangedAttackState : IState
    {
        WalkingEyeball walkingEyeball;
        private SpriteRenderer spriteRenderer;
        private Animator animator;
        private const float range = 20;
        private bool spawned;
        //private AbstractPlayer player;
        private Vector2 spawnPos;
        private Vector2 direction;

        private const float spawnFrame = 5;
        private const float totalFrames = 10;
        private const float fps = 12;
        private float timeSinceEnter;


        public RangedAttackState(WalkingEyeball walkingEyeball) {
            this.walkingEyeball = walkingEyeball;
            this.spriteRenderer = walkingEyeball.GetComponent<SpriteRenderer>();
            this.animator = walkingEyeball.GetComponent<Animator>();
            //this.player = MainGameManager.GetPlayer().GetComponent<AbstractPlayer>();
        }

        public int OnEnter() {
            animator.SetTrigger("TrRangedAttack");

            //animator.Play("Ranged Attack", 0, 0f);      // Setting animation time to 0
            this.timeSinceEnter = 0;

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
            //Debug.Log($"Animation time: {animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Base Layer")).normalizedTime}");
            
            this.timeSinceEnter += Time.fixedDeltaTime;

            float animationTime = animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Base Layer")).normalizedTime;

            //if (!spawned && animationTime >= spawnFrame / fps) {
            if (!spawned && timeSinceEnter >= spawnFrame / fps) {
                GameObject web = walkingEyeball.InstantiateWeb(spawnPos);
                web.GetComponent<Web.Web>().SetDirection(direction);
                this.spawned = true;

                //Debug.Log("Spawned!");
            }

            //if (animationTime >= totalFrames / fps) {
            if (timeSinceEnter >= totalFrames / fps) {
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
