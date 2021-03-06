using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AdaptiveWizard.Assets.Scripts.Other.GameManagers;


namespace AdaptiveWizard.Assets.Scripts.UI
{
    public class PausedScriptUI : MonoBehaviour
    {
        public Text pausedText;

        private void Update() {
            // At least for now, I will not check whether game is active in UI update functions.

            this.pausedText.text = "";
            if (MainGameManager.IsGameInactive()) {
                this.pausedText.text = "Paused";
            }
        }
    }
}
