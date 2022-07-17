using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AdaptiveWizard.Assets.Scripts.Player.Inventory;


namespace AdaptiveWizard.Assets.Scripts.UI
{
    public class GoldScriptUI : MonoBehaviour
    {
        private Text goldText;


        private void Start() {
            this.goldText = gameObject.GetComponent<Text>();
        }

        private void Update() {        
            this.goldText.text = "Gold: " + InventoryManager.GetGold();
        }
    }
}