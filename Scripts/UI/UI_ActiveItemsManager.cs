using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Items;

public class UI_ActiveItemsManager : MonoBehaviour
{
    // GameObjects used for instantiates
    public GameObject healthPotionUI_Prefab;
    public GameObject manaPotionUI_Prefab;


    // UI elements from scene
    public GameObject[] activeItemBoxes;


    private GameObject[] activeItemObjects = new GameObject[3];


    public void UpdateActiveItems(ActiveItem[] items) {
        DestroyActiveItemObjects();
        for (int i = 0; i < 3; i++) {
            if (items[i] != null) {
                GameObject itemObj;
                if (items[i] is HealthPotion) {
                    itemObj = Instantiate(healthPotionUI_Prefab) as GameObject;
                }
                else {
                    itemObj = Instantiate(manaPotionUI_Prefab) as GameObject;
                }
                itemObj.transform.SetParent(activeItemBoxes[i].transform, false);
                this.activeItemObjects[i] = itemObj;
            }
        }
    }

    private void DestroyActiveItemObjects() {
        for (int i = 0; i < 3; i++) {
            Destroy(activeItemObjects[i]);
            activeItemObjects[i] = null;
        }
    }
}
