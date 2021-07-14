using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : StraightProjectile
{
    private CircleCollider2D circleCollider;
    private PlayerGeneral player;

    private void Start() {
        this.circleCollider = GetComponent<CircleCollider2D>();
    }

    public void Start(Vector2 direction, PlayerGeneral player) {
        float speed = 3.0f;
        float damage = 40f;
        this.player = player;
        base.Start(direction, speed, damage);
    }

    private void FixedUpdate() {
        if (TestRoomManager.IsGameActive()) {
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, circleCollider.radius, GetDirection(), 0f, LayerMask.GetMask("Enemy", "Wall"));
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
}
