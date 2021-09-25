using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: fix a bug that causes the game to crash when I open a chest, click on the rewards, close the chest, then open the chest again
/* Error message:
MissingReferenceException: The object of type 'GameObject' has been destroyed but you are still trying to access it.
Your script should either check if it is null or you should not destroy the object.
UI_ChestContentManager.ShowChestContent () (at Assets/Scripts/UI/UI_ChestContentManager.cs:45)
Chest.Update () (at Assets/Scripts/Other/Rooms/Chest.cs:28)
*/
public class UI_ChestContentManager : MonoBehaviour
{
    // GameObjects used for instantiates
    public GameObject UI_chestContentBackgroundPrefab;
    public GameObject UI_chestContentSlotPrefab;


    private GameObject canvasObj;
    private GameObject chestContentBackground;
    private List<GameObject> chestContentSlots;


    public void Init(GameObject canvasObj) {
        this.canvasObj = canvasObj;
    }


    public void ShowChestContent(ChestLocalContent content) {
        // TODO: make this prefab better suited for different types of screen sizes (currently it only looks good with 1920x1080 resolution)
        // TODO: this will need even more work when I start to replace the simple backgrounds with actual sprites (but it should mostly be
        // just figuring out the unity engine, not modifying code)

        // Create the background
        this.chestContentBackground = Instantiate(UI_chestContentBackgroundPrefab) as GameObject;
        this.chestContentBackground.transform.SetParent(canvasObj.transform, false);

        // TODO: Create multiple UI sorting layers to differenciate between multiple overlapping UI elements (currently,
        // the order in which the overlapping UI elements are drawn is undefined, which could cause severe problems in the future,
        // despite working as intented currently)
        // Update: Maybe Unity is smart enough to lay children on top of parents? Not sure, needs testing

        // Create content slots
        // TODO: If player clicks on a non-bottom-most slot, make all slots below the one that was clicked move up
        this.chestContentSlots = new List<GameObject>();

        for (int i = 0; i < content.GetActiveItemsSize(); i++) {
            this.chestContentSlots.Add(Instantiate(UI_chestContentSlotPrefab) as GameObject);
            this.chestContentSlots[i].transform.SetParent(chestContentBackground.transform, false);
            this.chestContentSlots[i].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0f, -20f - 110f * i, 0f);
            (Items.ActiveItem, GameObject) activeItem = content.GetActiveItem(i);
            this.chestContentSlots[i].GetComponent<ChestContentSlotUI>().Init(activeItem.Item1, activeItem.Item2);
        }
        
    }

    public void HideChestContent() {
        Destroy(chestContentBackground);
        this.chestContentBackground = null;

        // I don't need to explicitely destroy chest content's slots because I'm destroying their parent and Unity will handle
        // destruction of the children
    }
}
