using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : StraightProjectile
{
    private CircleCollider2D circleCollider;
    private PlayerGeneral player;
    private SpriteRenderer spriteRenderer;
    private const float baseSpeed = 15f;

    private void Start() {
        this.circleCollider = GetComponent<CircleCollider2D>();
        this.spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Start(Vector2 direction, PlayerGeneral player) {
        this.player = player;
        float damage = 40f;
        float acceleration = -15f;
        base.Start(direction, baseSpeed, damage, acceleration);
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();
        if (TestRoomManager.IsGameActive()) {
            // Adjust sprite's alpha based on current speed, destroy this object if speed turns negative
            if (GetSpeed() < 0) {
                Destroy(gameObject);
                return;
            }
            AdjustAlphaBasedOnSpeed();

            // Check collisions and move
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, circleCollider.radius, Vector2.zero, 0f, LayerMask.GetMask("Enemy", "Wall"));
            if (hit.collider == null) {
                transform.position += (Vector3) GetDirection().normalized * GetSpeed() * Time.deltaTime;   
            }
            else {
                GameObject collidingObj = hit.collider.gameObject;
                if (collidingObj.layer == LayerMask.NameToLayer("Enemy")) {
                    // damage the colliding enemy
                    AbstractEnemy enemy = collidingObj.GetComponent<AbstractEnemy>();
                    enemy.TakeDamage(GetDamage());

                    // restore mana to player
                    this.player.AddMana(10f);
                }
                // destroy self
                Destroy(gameObject);
            }
        }
    }

    private void AdjustAlphaBasedOnSpeed() {
        Debug.Assert(GetSpeed() > 0);
        float newAlpha = Mathf.Min((GetSpeed() * 4) / baseSpeed, 1);
        Color tmp = spriteRenderer.color;
        tmp.a = newAlpha;
        spriteRenderer.color = tmp;
    }
}
