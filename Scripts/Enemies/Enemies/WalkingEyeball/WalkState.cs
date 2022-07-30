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
    public class WalkState : IState
    {
        private Animator animator;
        //private Timer timer;


        public WalkState(Animator animator) {
            this.animator = animator;
        }

        public int OnEnter() {
            animator.SetTrigger("TrWalk");
            return 0;
        }

        public int Update() {
            return 0;
        }

        public int OnLeave() {
            return 0;
        }
    }
}