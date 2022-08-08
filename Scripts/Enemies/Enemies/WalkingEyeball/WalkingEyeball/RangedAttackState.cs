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
        private Vector2 direction;
        public static RangedAttackPositionFinder rapf = new RangedAttackPositionFinder();

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
            Debug.Log("Entered Ranged Attack state");
            this.animator.SetTrigger("TrRangedAttack");
            this.timeSinceEnter = 0;
            this.spawned = false;
            this.direction = rapf.DirectionToPlayer(WebSpawnPos(walkingEyeball.transform.position));
            this.spriteRenderer.flipX = direction.x > 0;
            return 0;
        }

        public int Update() {
            //Debug.Log($"Animation time: {animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Base Layer")).normalizedTime}");
            
            this.timeSinceEnter += Time.fixedDeltaTime;

            float animationTime = animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Base Layer")).normalizedTime;

            //if (!spawned && animationTime >= spawnFrame / fps) {
            if (!spawned && timeSinceEnter >= spawnFrame / fps) {
                Spawn();
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

        public static Vector2 WebSpawnPos(Vector2 walkingEyeballPos) {
            Vector2 dirToPlayer = rapf.DirectionToPlayer(walkingEyeballPos);
            Vector2 webSpawnOffset = dirToPlayer.x > 0 ? Vector2.right : Vector2.left;
            return (Vector2) walkingEyeballPos + webSpawnOffset;
        }

        private void Spawn() {
            Vector2 spawnPos = WebSpawnPos(walkingEyeball.transform.position);
            GameObject web = walkingEyeball.InstantiateWeb(spawnPos);
            web.GetComponent<Web.Web>().SetDirection(direction);
            this.spawned = true;
            //Debug.Log("Spawned!");
        }

        private void SetAttackProperties() {
            //this.spawnPos = spawnPos;
            //this.direction = direction;
        }
    }
}
