using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2Projectile : StraightProjectile
{
    private CircleCollider2D circleCollider;

    // Start is called before the first frame update
    private void Start()
    {
        this.circleCollider = GetComponent<CircleCollider2D>();
    }
    
    public void Init(Vector2 direction) {
        float speed = 2.0f;
        float damage = 10f;
        base.Init(direction, speed, damage);
    }

    private void FixedUpdate() {
        if (TestRoomManager.IsGameActive()) {
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
