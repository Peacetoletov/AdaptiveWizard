using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using AdaptiveWizard.Assets.Scripts.Enemies.General.Interfaces;
using AdaptiveWizard.Assets.Scripts.Other.Other;
//using AdaptiveWizard.Assets.Scripts.Other.GameManagers;     // just testing
//using AdaptiveWizard.Assets.Scripts.Player.Other;           // just testing


namespace AdaptiveWizard.Assets.Scripts.Enemies.Enemies.WalkingEyeball.WalkingEyeball
{
    public class IdleState : IState
    {
        private Animator animator;
        private Timer timer;

        // just testing
        WalkingEyeball walkingEyeball;

        public IdleState(WalkingEyeball walkingEyeball) {
            this.animator = walkingEyeball.GetComponent<Animator>();
            this.walkingEyeball = walkingEyeball;
        }

        public int OnEnter() {
            Debug.Log("Entered Idle state");
            animator.SetTrigger("TrIdle");
            this.timer = new Timer(2.5f);
            return 0;
        }

        public int Update() {
            if (timer.UpdateAndCheck()) {
                return 1;
            }
            /*
            Vector2 eyeballPos = walkingEyeball.transform.position;
            Vector2 playerPos = MainGameManager.GetPlayer().GetComponent<AbstractPlayer>().transform.position;
            Debug.Log($"Eyeball position: {eyeballPos}. Player position: {playerPos}");
            float signedAngle = Vector2.SignedAngle(Vector2.right, playerPos - eyeballPos);
            Debug.Log($"Signed angle from enemy to player: {signedAngle}");
            */
            return 0;
        }
    }
}
