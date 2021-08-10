using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;

using Items;


// TODO: create 2 additional classes for UI management of passive items and active items
public class UI_Manager : MonoBehaviour
{
    // GameObjects used for instantiates
    public GameObject canvasObj;
    public GameObject healthCrystalUI_Obj;
    public GameObject divineSphereUI_Obj;
    public GameObject manaCrystalUI_obj;

    public GameObject healthPotionUI_obj;
    public GameObject manaPotionUI_obj;
    

    // passive items
    List<GameObject> passiveItemObjects = new List<GameObject>();
    readonly Vector3 passiveItemOffset = new Vector3(70f, 0f, 0f);
    readonly Vector3 basePassiveItemPos = new Vector3(20f, -20f, 0f);
    Vector3 nextPassiveItemPos = new Vector3(20f, -20f, 0f);

    // active items
    public GameObject[] activeItemBoxes;
    private GameObject[] activeItemObjects = new GameObject[3];
    /*
    public GameObject activeItemBox2;
    public GameObject activeItemBox3;
    */

    void Start() {
        //print("UI manager is running");
        /*
        GameObject itemUI1 = Instantiate(healthCrystalUI_Obj) as GameObject;
        itemUI1.transform.SetParent(canvasObj.transform, false);
        itemUI1.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(20f, -20f, 0f);

        GameObject itemUI2 = Instantiate(divineSphereUI_Obj) as GameObject;
        itemUI2.transform.SetParent(canvasObj.transform, false);
        itemUI2.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(90f, -20f, 0f);

        GameObject itemUI3 = Instantiate(healthCrystalUI_Obj) as GameObject;
        itemUI3.transform.SetParent(canvasObj.transform, false);
        itemUI3.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(160f, -20f, 0f);
        */
        
    }

    
    public void UpdatePassiveItems(List<PassiveItem> items) {
        DestroyPassiveItemObjects();
        foreach (PassiveItem item in items) {
            GameObject itemObj;
            if (item is HealthCrystal) {
                itemObj = Instantiate(healthCrystalUI_Obj) as GameObject;
            } else if (item is DivineSphere) {
                itemObj = Instantiate(divineSphereUI_Obj) as GameObject;
            } else {
                itemObj = Instantiate(manaCrystalUI_obj) as GameObject;
            }
            /*
            else if (item is DivineSphere) {
                itemObj = Instantiate(divineSphereUI_Obj) as GameObject;
            } else { ... }
            */
            itemObj.transform.SetParent(canvasObj.transform, false);
            itemObj.GetComponent<RectTransform>().anchoredPosition3D = nextPassiveItemPos;
            this.nextPassiveItemPos += passiveItemOffset;
            this.passiveItemObjects.Add(itemObj);
        }
    }

    private void DestroyPassiveItemObjects() {
        this.nextPassiveItemPos = basePassiveItemPos;
        foreach (GameObject obj in passiveItemObjects) {
            Destroy(obj);
        }
        this.passiveItemObjects = new List<GameObject>();
    }

    public void UpdateActiveItems(ActiveItem[] items) {
        DestroyActiveItemObjects();
        for (int i = 0; i < 3; i++) {
            if (items[i] != null) {
                GameObject itemObj;
                if (items[i] is HealthPotion) {
                    itemObj = Instantiate(healthPotionUI_obj) as GameObject;
                }
                else {
                    itemObj = Instantiate(manaPotionUI_obj) as GameObject;
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
