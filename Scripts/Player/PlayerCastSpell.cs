using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCastSpell : PlayerAbstract
{
    public GameObject fireball;

    void Update() {
        if (TestRoomManager.IsGameActive()) {
            if (Input.GetMouseButtonDown(0)) {      // 0 = left click
                // I have only 1 basic spell so far. In future, I will first need to check what spell the player is holding, then casting it.
                Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                float offset = 0.8f;       // how far away from the player will the fireball spawn 
                Vector2 spawnPos = (Vector2) transform.position + direction.normalized * offset;
                GameObject newFireball = Instantiate(fireball, spawnPos, Quaternion.identity) as GameObject;
                Fireball fireballScript = newFireball.GetComponent<Fireball>();
                fireballScript.Init(direction);
            }
        }
    }

    public override void Reset() {
        // This function will need to be updated once there are variables that meed to be reset in this class
    }
}
