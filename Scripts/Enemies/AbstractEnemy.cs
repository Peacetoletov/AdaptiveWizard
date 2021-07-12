using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractEnemy : MonoBehaviour
{
    private float maxHealth;
    private float curHealth;
    private GameObject player;      // reference to the player

    protected void Start(float maxHealth) {
        // set health
        this.maxHealth = maxHealth;
        this.curHealth = maxHealth;

        // get a reference to the player
        // print("running function start in abstract enemy");
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("Player");
        if (objectsWithTag.Length != 1) {
            string message = "ERROR! Incorrect number of \"player\" objects found when attempting to spawn an enemy. " +
                             "Number of player objects found: " + objectsWithTag.Length;
            throw new System.InvalidOperationException(message);
        }
        this.player = objectsWithTag[0];
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
        return player.transform.position - transform.position;
    }

    
    protected GameObject GetPlayer() {
        return player;
    }
}
