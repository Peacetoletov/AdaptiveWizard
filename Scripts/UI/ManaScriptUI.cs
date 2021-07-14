using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaScriptUI : MonoBehaviour
{
    private PlayerGeneral player;
    private Text manaText;

    private void Start() {
        this.manaText = gameObject.GetComponent<Text>();
    }

    private void Update() {        
        if (player == null && TestRoomManager.IsGameActive()) {
            this.player = TestRoomManager.GetPlayer().GetComponent<PlayerGeneral>();
        }

        if (player != null) {
            this.manaText.text = "Mana: " + player.GetCurMana();
        }
    }
}
