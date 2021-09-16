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
    public GameObject healthCrystalUI_Prefab;
    public GameObject divineSphereUI_Prefab;
    public GameObject manaCrystalUI_Prefab;

    public GameObject healthPotionUI_Prefab;
    public GameObject manaPotionUI_Prefab;

    public GameObject chestContentBackgroundUI_Prefab;
    public GameObject chestContentSlotUI_Prefab;

    // passive items
    List<GameObject> passiveItemObjects = new List<GameObject>();
    readonly Vector3 passiveItemOffset = new Vector3(70f, 0f, 0f);
    readonly Vector3 basePassiveItemPos = new Vector3(20f, -20f, 0f);
    Vector3 nextPassiveItemPos = new Vector3(20f, -20f, 0f);

    // active items
    public GameObject[] activeItemBoxes;
    private GameObject[] activeItemObjects = new GameObject[3];

    // chests
    private GameObject chestContentBackground;
    private GameObject chestContentSlot;

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
                itemObj = Instantiate(healthCrystalUI_Prefab) as GameObject;
            } else if (item is DivineSphere) {
                itemObj = Instantiate(divineSphereUI_Prefab) as GameObject;
            } else {
                itemObj = Instantiate(manaCrystalUI_Prefab) as GameObject;
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

    public void ShowChestContent() {
        // TODO: make this prefab better suited for different types of screen sizes (currently it only looks good with 1920x1080 resolution)
        // TODO: this will need even more work when I start to replace the simple backgrounds with actual sprites (but it should mostly be
        // just figuring out the unity engine, not modifying code)

        // Create the background
        this.chestContentBackground = Instantiate(chestContentBackgroundUI_Prefab) as GameObject;
        this.chestContentBackground.transform.SetParent(canvasObj.transform, false);

        // Create content slots
        // For now, I will only have one slot - TODO: soon implement multiple slots (as a list)
        // TODO: Create multiple UI sorting layers to differenciate between multiple overlapping UI elements (currently,
        // the order in which the overlapping UI elements are drawn is undefined, which could cause severe problems in the future,
        // despite working as intented currently)
        this.chestContentSlot = Instantiate(chestContentSlotUI_Prefab) as GameObject;
        this.chestContentSlot.transform.SetParent(chestContentBackground.transform, false);
        chestContentSlot.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0f, -20f, 0f);
    }

    public void HideChestContent() {
        Destroy(chestContentBackground);
        this.chestContentBackground = null;

        // I don't need to explicitely destroy chest content's slots because I'm destroying their parent and Unity will handle
        // destruction of the children
    }
}
