using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCastSpell : AbstractPlayer
{
    private PlayerGeneral playerGeneral;

    private void Start() {
        this.playerGeneral = gameObject.GetComponent<PlayerGeneral>();
    }

    private void Update() {
        if (TestRoomManager.IsGameActive()) {
            if (Input.GetMouseButtonDown(0)) {      // 0 = left click
                // I have only 1 basic spell so far. In future, I will first need to check what spell the player is holding, then casting it.
                new FireballManager().TryToCast(playerGeneral);
            }

            if (Input.GetMouseButtonDown(1)) {      // 1 = right click
                new ExplosionManager().TryToCast(playerGeneral);
            }

            if (Input.GetKeyDown(KeyCode.Q)) {
                new CannonballManager().TryToCast(playerGeneral);
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
}
