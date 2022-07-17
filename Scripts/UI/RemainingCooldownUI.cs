using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AdaptiveWizard.Assets.Scripts.Player.Other;
using AdaptiveWizard.Assets.Scripts.Other.GameManagers;
using AdaptiveWizard.Assets.Scripts.Other.Other;


namespace AdaptiveWizard.Assets.Scripts.UI
{
    public class RemainingCooldownUI : MonoBehaviour
    {
        public int spellIndex;      // 0 for left-most spell, 3 for right-most spell, 1 and 2 for the ones in between
        private PlayerCastSpell player;
        private Text cooldown;

        private void Start() {
            this.cooldown = gameObject.GetComponent<Text>();
        }

        private void Update() {        
            if (player == null && MainGameManager.IsGameActive()) {
                this.player = MainGameManager.GetPlayer().GetComponent<PlayerCastSpell>();
            }

            if (player != null) {
                Timer cooldownTimer = player.GetSpellManager(spellIndex).GetTimer();
                float time = cooldownTimer.GetTime();
                this.cooldown.text = "";
                if (Mathf.Abs(time) > 0.00001f) {
                    // time != 0
                    float remainingCooldown = (cooldownTimer.GetPeriod() - time);
                    this.cooldown.text = remainingCooldown.ToString("F1").Replace(",", ".");
                }
            }
        }
    }
}
