using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using AdaptiveWizard.Assets.Scripts.Enemies.General.Interfaces;


/*
Flying state occurs after expanding state animation finishes.
*/
namespace AdaptiveWizard.Assets.Scripts.Enemies.Enemies.WalkingEyeball.Web
{
    public class FlyingState : IState
    {
        private readonly Web web;
        private readonly BoxCollider2D finalCollider;
        private readonly float speed;
        private readonly float rotateSpeed;
        private Vector2 direction;

        // variables related to rotation
        private float timeSinceEnter = 0;


        public FlyingState(Web web, BoxCollider2D finalCollider, Vector2 direction, float speed, float rotateSpeed) {
            this.web = web;
            this.finalCollider = finalCollider;
            this.speed = speed;
            this.direction = direction;
            this.rotateSpeed = rotateSpeed;
        }

        public int OnEnter() {
            return 0;
        }

        public int StateUpdate() {
            this.timeSinceEnter += Time.fixedDeltaTime;
            float rotateBy;
            if (rotateSpeed > 0) {
                rotateBy = Math.Max(rotateSpeed - timeSinceEnter * 125, 0);
            } else {
                rotateBy = Math.Min(rotateSpeed + timeSinceEnter * 125, 0);
            }
            Utility.Rotate(web, rotateBy);

            int movementReturnCode = Utility.MoveAndCheckCollision(web, direction, speed, finalCollider);
            if (movementReturnCode == 1) {
                // Player was hit
                return 1;
            } 
            if (movementReturnCode == 2) {
                // Wall was hit
                return 2;
            }
            // Nothing was hit
            return 0;
        }
    }
}