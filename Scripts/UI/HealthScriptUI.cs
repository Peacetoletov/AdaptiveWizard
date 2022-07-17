using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AdaptiveWizard.Assets.Scripts.Player.Other;
using AdaptiveWizard.Assets.Scripts.Other.GameManagers;


namespace AdaptiveWizard.Assets.Scripts.UI
{
    public class HealthScriptUI : MonoBehaviour
    {
        private PlayerGeneral player;
        private Text healthText;

        private void Start() {
            this.healthText = gameObject.GetComponent<Text>();
        }

        private void Update() {        
            if (player == null && MainGameManager.IsGameActive()) {
                this.player = MainGameManager.GetPlayer().GetComponent<PlayerGeneral>();
            }

            if (player != null) {
                this.healthText.text = "Health: " + player.GetCurHealth();
            }
        }
    }
}