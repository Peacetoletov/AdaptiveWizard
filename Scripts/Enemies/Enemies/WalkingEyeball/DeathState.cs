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
    public class DeathState : IState
    {
        private Animator animator;
        private Timer timer;


        public DeathState(Animator animator) {
            this.animator = animator;
        }

        public int OnEnter() {
            //animator.SetTrigger("TrIdle");
            //this.timer = new Timer(1.5f);
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
