using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using AdaptiveWizard.Assets.Scripts.Enemies.Interfaces;
using AdaptiveWizard.Assets.Scripts.Other.Other;


namespace AdaptiveWizard.Assets.Scripts.Enemies.Enemies.WalkingEyeball
{
    public class IdleState : IState
    {
        private Animator animator;
        private Timer timer;


        public IdleState(WalkingEyeball walkingEyeball) {
            this.animator = walkingEyeball.GetComponent<Animator>();
        }

        public int OnEnter() {
            Debug.Log("Entered Idle state");
            animator.SetTrigger("TrIdle");
            //this.timer = new Timer(2.5f);
            return 0;
        }

        public int Update() {
            /*
            if (timer.UpdateAndCheck()) {
                return 1;
            }
            */
            return 0;
        }

        public int OnLeave() {
            return 0;
        }
    }
}
