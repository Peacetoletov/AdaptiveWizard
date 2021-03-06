using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdaptiveWizard.Assets.Scripts.Other.GameManagers;
using AdaptiveWizard.Assets.Scripts.Other.Other;
using AdaptiveWizard.Assets.Scripts.Player.Other;


namespace AdaptiveWizard.Assets.Scripts.Enemies.Enemies
{
    public class Enemy2Projectile : StraightProjectile
    {
        private CircleCollider2D circleCollider;

        private void Start() {
            this.circleCollider = GetComponent<CircleCollider2D>();
        }
        
        public void Start(Vector2 direction) {
            float speed = 2.0f;
            float damage = 10f;
            base.Start(direction, speed, damage);
        }

        protected override void FixedUpdate() {
            base.FixedUpdate();
            if (MainGameManager.IsGameActive()) {
                RaycastHit2D hit = Physics2D.CircleCast(transform.position, circleCollider.radius, GetDirection(), 0f, LayerMask.GetMask("Player", "Wall"));
                if (hit.collider == null) {
                    transform.position += (Vector3) GetDirection().normalized * GetSpeed() * Time.deltaTime; 
                }
                else {
                    GameObject collidingObj = hit.collider.gameObject;
                    if (collidingObj.layer == LayerMask.NameToLayer("Player")) {
                        // damage player
                        PlayerGeneral playerScript = collidingObj.GetComponent<PlayerGeneral>();
                        playerScript.TakeDamage(GetDamage());
                    }
                    // destroy self
                    Destroy(gameObject);
                }
            }
        }
    }
}
