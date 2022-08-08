using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using AdaptiveWizard.Assets.Scripts.Enemies.General.Interfaces;
using AdaptiveWizard.Assets.Scripts.Other.Other;


namespace AdaptiveWizard.Assets.Scripts.Enemies.Enemies.WalkingEyeball.Web
{
    public class FlyingState : IState
    {
        private Web web;
        private BoxCollider2D finalCollider;
        private float speed;
        private Vector2 direction;


        public FlyingState(Web web, BoxCollider2D finalCollider, Vector2 direction, float speed) {
            this.web = web;
            this.finalCollider = finalCollider;
            this.speed = speed;
            this.direction = direction;
        }

        public int OnEnter() {
            return 0;
        }

        public int StateUpdate() {
            //int movementReturnCode = Utility.MoveAndCheckCollision(web, Vector2.left, speed, finalCollider);
            int movementReturnCode = Utility.MoveAndCheckCollision(web, direction, speed, finalCollider);
            if (movementReturnCode == 1) {
                // Player was hit
                return 1;
            } else if (movementReturnCode == 2) {
                // Wall was hit
                return 2;
            }
            // Nothing was hit
            return 0;
        }
    }
}