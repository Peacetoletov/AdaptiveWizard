using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestGoldAmountText : MonoBehaviour
{
    private Text goldText;

    private void Start() {
        this.goldText = gameObject.GetComponent<Text>();
    }

    public void Init(int gold) {
        this.goldText = gameObject.GetComponent<Text>();
        this.goldText.text = gold.ToString();
    }

    /*

    private void Update() {        
        this.goldText.text = "" + InventoryNS.Inventory.GetGold();
    }
    */
}
