using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using AdaptiveWizard.Assets.Scripts.Enemies.General.Interfaces;


/*
Fading away occurs after the web hits something.
*/
namespace AdaptiveWizard.Assets.Scripts.Enemies.Enemies.WalkingEyeball.Web
{
    public class FadingAwayState : IState
    {
        private readonly SpriteRenderer spriteRenderer;
        private const float totalFadeTime = 0.25f;
        private float timeInit;


        public FadingAwayState(Web web) {
            this.spriteRenderer = web.GetComponent<SpriteRenderer>();
        }

        public int OnEnter() {
            this.timeInit = Time.time;
            return 0;
        }

        public int StateUpdate() {
            // Calculate new alpha
            float time = Time.time - timeInit;
            float alpha = 1 - (time / totalFadeTime);

            // Return 1 if the object has completely faded away
            if (alpha <= 0) {
                return 1;
            }

            // Update alpha
            Color tmp = spriteRenderer.color;
            tmp.a = alpha;
            spriteRenderer.color = tmp;

            // Return 0 if nothing extraordinary happened
            return 0;
        }
    }
}