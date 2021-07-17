using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCastSpell : AbstractPlayer
{
    public GameObject fireballObj;
    public GameObject explosionObj;
    public GameObject cannonballObj;

    private PlayerGeneral playerGeneral;

    private void Start() {
        this.playerGeneral = gameObject.GetComponent<PlayerGeneral>();
    }

    private void Update() {
        if (TestRoomManager.IsGameActive()) {
            if (Input.GetMouseButtonDown(0)) {      // 0 = left click
                // I have only 1 basic spell so far. In future, I will first need to check what spell the player is holding, then casting it.
                Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                float offset = 0.8f;       // how far away from the player will the fireball spawn 
                Vector2 spawnPos = (Vector2) transform.position + direction.normalized * offset;
                GameObject fireball = Instantiate(fireballObj, spawnPos, Quaternion.identity) as GameObject;
                Fireball fireballScript = fireball.GetComponent<Fireball>();
                fireballScript.Start(direction, playerGeneral);
            }

            if (Input.GetMouseButtonDown(1)) {      // 1 = right click
                if (playerGeneral.CheckAndSpendMana(20f)) {
                    GameObject newExplosion = Instantiate(explosionObj, transform.position, Quaternion.identity) as GameObject;
                }
            }

            if (Input.GetKeyDown(KeyCode.Q)) {
                if (playerGeneral.CheckAndSpendMana(50f)) {
                    Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                    float offset = 1.5f;       // how far away from the player will the fireball spawn 
                    Vector2 spawnPos = (Vector2) transform.position + direction.normalized * offset;
                    GameObject cannonball = Instantiate(cannonballObj, spawnPos, Quaternion.identity) as GameObject;
                    Cannonball cannonballScript = cannonball.GetComponent<Cannonball>();
                    cannonballScript.Start(direction);
                }
            }

            if (Input.GetKeyDown(KeyCode.E)) {
                new IcicleManager().TryToCast(playerGeneral);
            }
        }
    }

    public override void Reset() {
        // This function will need to be updated once there are variables that meed to be reset in this class
        // playerGeneral doesn't need to be reset
    }

    private float GetIcicleRotation(int x, int y) {
        if (y == -1) {
            if (x == -1) {
                return 90f;
            }
            if (x == 0) {
                return 90f;
            }
            // x == 1
            return 180f;
        }
        if (y == 0) {
            if (x == -1) {
                return 0f;
            }
            // x == 1
            return 180;
        }
        // y == 1
        if (x == -1) {
            return 0f;
        }
        if (x == 0) {
            return 270f;
        }
        // x == 1
        return 270f;
    }
}
