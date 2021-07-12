using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthScriptUI : MonoBehaviour
{
    private PlayerGeneral player;
    public Text healthText;

    private void Update() {        
        if (player == null && TestRoomManager.IsGameActive()) {
            this.player = TestRoomManager.GetPlayer().GetComponent<PlayerGeneral>();
        }

        if (player != null) {
            this.healthText.text = "Health: " + player.GetCurHealth();
        }
    }
}
