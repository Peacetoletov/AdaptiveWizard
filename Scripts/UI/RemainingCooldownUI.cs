using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemainingCooldownUI : MonoBehaviour
{
    public int spellIndex;      // 0 for left-most spell, 3 for right-most spell, 1 and 2 for the ones in between
    private PlayerCastSpell player;
    private Text cooldown;

    private void Start() {
        this.cooldown = gameObject.GetComponent<Text>();
    }

    private void Update() {        
        if (player == null && TestRoomManager.IsGameActive()) {
            this.player = TestRoomManager.GetPlayer().GetComponent<PlayerCastSpell>();
        }

        if (player != null) {
            Timer cooldownTimer = player.GetSpellManager(spellIndex).GetTimer();
            float time = cooldownTimer.GetTime();
            this.cooldown.text = "";
            if (Mathf.Abs(time) > 0.00001f) {
                // time != 0
                float remainingCooldown = (cooldownTimer.GetPeriod() - time);
                this.cooldown.text = remainingCooldown.ToString("F1").Replace(",", ".");
                // ^change this
            }
            //int roundedToSeconds = (int) cooldownTimer.GetPeriod();
            //this.cooldown.text = roundedToSeconds.ToString();
        }
    }
}
