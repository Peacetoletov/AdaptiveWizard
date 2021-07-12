using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractEnemy : MonoBehaviour
{
    private float maxHealth;
    private float curHealth;

    protected void Start(float maxHealth) {
        // set health
        this.maxHealth = maxHealth;
        this.curHealth = maxHealth;
    }

    public void TakeDamage(float damage) {
        this.curHealth -= damage;
        //print("Took " + damage + " damage");
        CheckDeath();
    }

    private void CheckDeath() {
        if (curHealth <= 0) {
            Destroy(gameObject);
        }
    }

    protected Vector2 DirectionToPlayer() {
        return TestRoomManager.GetPlayer().transform.position - transform.position;
    }
}
