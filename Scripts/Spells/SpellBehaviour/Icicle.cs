using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Icicle : StraightProjectile
{
    private BoxCollider2D boxCollider;
    private CapsuleCollider2D capsuleCollider;
    private float angle;
    

    private void Start() {
        this.boxCollider = GetComponent<BoxCollider2D>();
        this.capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    public void Start(Vector2 direction, float angle) {
        float speed = 20f;
        float damage = 51f;
        base.Start(direction, speed, damage);

        this.angle = angle;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }


    protected override void FixedUpdate() {
        base.FixedUpdate();
        if (MainGameManager.IsGameActive()) {
            EvaluateCollisionWithCapsuleCollider();
            EvaluateCollisionWithBoxCollider();
            transform.position += (Vector3) GetDirection().normalized * GetSpeed() * Time.deltaTime;
        }
    }

    private void EvaluateCollisionWithCapsuleCollider() {
        Vector2 offsetRotated = Quaternion.AngleAxis(angle, Vector3.forward) * capsuleCollider.offset;
        RaycastHit2D hit = Physics2D.CapsuleCast((Vector2) transform.position + offsetRotated, capsuleCollider.size, 
                                                 CapsuleDirection2D.Horizontal, angle, Vector2.zero, 0, LayerMask.GetMask("Enemy", "Wall"));
        if (hit.collider != null) {
            GameObject collidingObj = hit.collider.gameObject;
            if (collidingObj.layer == LayerMask.NameToLayer("Enemy")) {
                // damage the colliding enemy
                AbstractEnemy enemy = collidingObj.GetComponent<AbstractEnemy>();
                enemy.TakeDamage(GetDamage());
            }
            // destroy self
            Destroy(gameObject);
        }
    }

    private void EvaluateCollisionWithBoxCollider() {
        Vector2 offsetRotated = Quaternion.AngleAxis(angle, Vector3.forward) * boxCollider.offset;
        RaycastHit2D hit = Physics2D.BoxCast((Vector2) boxCollider.transform.position + offsetRotated, boxCollider.size,
                                             angle, Vector2.zero, 0, LayerMask.GetMask("Enemy", "Wall"));
        if (hit.collider != null) {
            GameObject collidingObj = hit.collider.gameObject;
            if (collidingObj.layer == LayerMask.NameToLayer("Enemy")) {
                // damage the colliding enemy
                AbstractEnemy enemy = collidingObj.GetComponent<AbstractEnemy>();
                enemy.TakeDamage(GetDamage());
            }
            // destroy self
            Destroy(gameObject);
        } 
    }
}
