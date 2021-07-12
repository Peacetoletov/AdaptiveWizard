using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthScriptUI : MonoBehaviour
{
    private PlayerGeneral player;
    public Text healthText;

    private void Update() {
        // At least for now, I will not check whether game is active in UI update functions.
        
        if (player == null) {
            GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("Player");
            if (objectsWithTag.Length != 1) {
                this.healthText.text = "Health: 0";
                return;
            }
            this.player = objectsWithTag[0].GetComponent<PlayerGeneral>();
        }

        if (player != null) {
            this.healthText.text = "Health: " + player.GetCurHealth();
        }
    }
}
