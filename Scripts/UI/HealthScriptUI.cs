using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthScriptUI : MonoBehaviour
{
    private PlayerGeneral player;
    private Text healthText;

    private void Start() {
        this.healthText = gameObject.GetComponent<Text>();
    }

    private void Update() {        
        if (player == null && TestRoomManager.IsGameActive()) {
            this.player = TestRoomManager.GetPlayer().GetComponent<PlayerGeneral>();
        }

        if (player != null) {
            this.healthText.text = "Health: " + player.GetCurHealth();
        }
    }
}
