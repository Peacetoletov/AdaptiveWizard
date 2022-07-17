using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
Score is a temporary feature that will not be in that final game. However, it increases current game quality
by a lot and is easy to implement, so that's why I decided to implement it.
*/


namespace AdaptiveWizard.Assets.Scripts.UI 
{
    public class ScoreScriptUI : MonoBehaviour
    {
        private static int score = 0;
        private Text scoreText;

        private void Start() {
            this.scoreText = gameObject.GetComponent<Text>();
        }

        private void Update() {
            this.scoreText.text = "Score: " + score;
        }

        public static void IncreaseScore(int amount) {
            ScoreScriptUI.score += amount;
        }

        public static void ResetScore() {
            ScoreScriptUI.score = 0;
        }
    }
}
