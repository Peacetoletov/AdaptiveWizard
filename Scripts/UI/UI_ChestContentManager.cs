using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ChestContentManager : MonoBehaviour
{
    // GameObjects used for instantiates
    public GameObject UI_chestContentBackgroundPrefab;
    public GameObject UI_chestContentSlotPrefab;


    private GameObject canvasObj;
    private GameObject chestContentBackground;
    private GameObject chestContentSlot;


    public void Init(GameObject canvasObj) {
        this.canvasObj = canvasObj;
    }


    public void ShowChestContent() {
        // TODO: make this prefab better suited for different types of screen sizes (currently it only looks good with 1920x1080 resolution)
        // TODO: this will need even more work when I start to replace the simple backgrounds with actual sprites (but it should mostly be
        // just figuring out the unity engine, not modifying code)

        // Create the background
        this.chestContentBackground = Instantiate(UI_chestContentBackgroundPrefab) as GameObject;
        this.chestContentBackground.transform.SetParent(canvasObj.transform, false);

        // Create content slots
        // For now, I will only have one slot - TODO: soon implement multiple slots (as a list)
        // TODO: Create multiple UI sorting layers to differenciate between multiple overlapping UI elements (currently,
        // the order in which the overlapping UI elements are drawn is undefined, which could cause severe problems in the future,
        // despite working as intented currently)
        this.chestContentSlot = Instantiate(UI_chestContentSlotPrefab) as GameObject;
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
