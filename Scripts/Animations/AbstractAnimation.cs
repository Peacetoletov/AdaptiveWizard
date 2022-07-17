using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdaptiveWizard.Assets.Scripts.Other.GameManagers;


namespace AdaptiveWizard.Assets.Scripts.Animations
{
    public abstract class AbstractAnimation : MonoBehaviour
    {
        private Animator animator;
        
        protected virtual void Start() {
            this.animator = gameObject.GetComponent<Animator>();
        }

        protected virtual void Update() {
            // pause the animation if the game is paused
            this.animator.speed = MainGameManager.IsGameActive() ? 1f : 0f;
        }

        public Animator GetAnimator() {
            return animator;
        }
    }
}
