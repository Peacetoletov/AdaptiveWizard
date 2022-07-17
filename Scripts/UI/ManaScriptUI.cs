using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AdaptiveWizard.Assets.Scripts.Player.Other;
using AdaptiveWizard.Assets.Scripts.Other.GameManagers;


namespace AdaptiveWizard.Assets.Scripts.UI
{
    public class ManaScriptUI : MonoBehaviour
    {
        private PlayerGeneral player;
        private Text manaText;

        private void Start() {
            this.manaText = gameObject.GetComponent<Text>();
        }

        private void Update() {        
            if (player == null && MainGameManager.IsGameActive()) {
                this.player = MainGameManager.GetPlayer().GetComponent<PlayerGeneral>();
            }

            if (player != null) {
                this.manaText.text = "Mana: " + player.GetCurMana();
            }
        }
    }
}
