using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdaptiveWizard.Assets.Scripts.Other.GameManagers;


namespace AdaptiveWizard.Assets.Scripts.Other.Other
{
    public abstract class StraightProjectile : MonoBehaviour
    {
        private Vector2 direction;
        private float speed;
        private float damage;
        private float acceleration;

        protected void Start(Vector2 direction, float speed, float damage, float acceleration=0f) {
            this.direction = direction;
            this.speed = speed;
            this.damage = damage;
            this.acceleration = acceleration;
        }

        protected virtual void FixedUpdate() {
            if (MainGameManager.IsGameActive()) {
                Accelerate();
            }
        }

        private void Accelerate() {
            this.speed += acceleration * Time.deltaTime;
        }

        public Vector2 GetDirection() {
            return direction;
        }

        public float GetSpeed() {
            return speed;
        }

        public float GetDamage() {
            return damage;
        }
    }
}
