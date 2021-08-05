using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PausedScriptUI : MonoBehaviour
{
    public Text pausedText;

    private void Update() {
        // At least for now, I will not check whether game is active in UI update functions.

        this.pausedText.text = "";
        if (!MainGameManager.IsGameActive()) {
            this.pausedText.text = "Paused";
        }
    }
}
