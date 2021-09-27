using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using InventoryNS;

public class GoldScriptUI : MonoBehaviour
{
    private Text goldText;


    private void Start() {
        this.goldText = gameObject.GetComponent<Text>();
    }

    private void Update() {        
        this.goldText.text = "Gold: " + Inventory.GetGold();
    }
}
