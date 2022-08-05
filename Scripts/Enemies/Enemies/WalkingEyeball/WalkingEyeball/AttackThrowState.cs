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
    public class AttackThrowState : IState
    {
        WalkingEyeball walkingEyeball;
        private Animator animator;
        private Timer timer;
        private const float range = 20;
        private readonly GameObject webObj;
        
        // testing
        private bool spawned = false;


        public AttackThrowState(WalkingEyeball walkingEyeball) {
            this.walkingEyeball = walkingEyeball;
            this.animator = walkingEyeball.GetComponent<Animator>();
        }

        public int OnEnter() {
            //animator.SetTrigger("TrAttackThrow");
            Debug.Log($"Time on enter: {animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Base Layer")).normalizedTime}");
            animator.Play("Attack Throw", 0, 0f);
            Debug.Log($"Time after custom play method: {animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Base Layer")).normalizedTime}");
            //this.timer = new Timer(1.5f);
            Debug.Log("Entered AttackThrow state");
            return 0;
        }

        public int Update() {
            Debug.Log($"Animation time: {animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Base Layer")).normalizedTime}");
            const float spawnFrame = 5;
            const int totalFrames = 10;
            const float spawnTime = spawnFrame / totalFrames;
            if (!spawned && animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Base Layer")).normalizedTime >= spawnTime) {
                // TODO - make both spawn directions work
                Vector2 pos = walkingEyeball.transform.position - new Vector3(1, 0, 0);
                walkingEyeball.InstantiateWeb(pos);
                Debug.Log("Spawned!");
                this.spawned = true;

                // TODO - possibly change state to Idle for a couple seconds after spawning a web?
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
