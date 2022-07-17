using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdaptiveWizard.Assets.Scripts.Items.Active.Classes.Abstract;
using AdaptiveWizard.Assets.Scripts.Items.Passive.Classes.Abstract;
using AdaptiveWizard.Assets.Scripts.Other.Rooms;

// TODO: fix a bug that causes the game to crash when I open a chest, click on the rewards, close the chest, then open the chest again
/* Error message:
MissingReferenceException: The object of type 'GameObject' has been destroyed but you are still trying to access it.
Your script should either check if it is null or you should not destroy the object.
UI_ChestContentManager.ShowChestContent () (at Assets/Scripts/UI/UI_ChestContentManager.cs:45)
Chest.Update () (at Assets/Scripts/Other/Rooms/Chest.cs:28)
*/

namespace AdaptiveWizard.Assets.Scripts.UI
{
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


        public void ShowChestContent(Chest chest) {
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
            this.chestContentSlots = new List<GameObject>();
            int contentSlotCounter = 0;

            
            // Gold
            int gold = chest.GetLocalContent().GetGold();
            if (gold != 0) {
                this.chestContentSlots.Add(Instantiate(UI_chestContentSlotPrefab) as GameObject);
                this.chestContentSlots[contentSlotCounter].transform.SetParent(chestContentBackground.transform, false);
                this.chestContentSlots[contentSlotCounter].GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -20f - 110f * contentSlotCounter);
                this.chestContentSlots[contentSlotCounter].GetComponent<ChestContentSlotUI>().Init(gold, chest);
                contentSlotCounter++;
            }

            // Active items
            for (int i = 0; i < chest.GetLocalContent().GetActiveItemsSize(); i++) {
                this.chestContentSlots.Add(Instantiate(UI_chestContentSlotPrefab) as GameObject);
                this.chestContentSlots[contentSlotCounter].transform.SetParent(chestContentBackground.transform, false);
                this.chestContentSlots[contentSlotCounter].GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -20f - 110f * contentSlotCounter);
                (ActiveItem, GameObject) activeItem = chest.GetLocalContent().GetActiveItem(i);
                this.chestContentSlots[contentSlotCounter].GetComponent<ChestContentSlotUI>().Init(activeItem.Item1, activeItem.Item2, i, chest);
                contentSlotCounter++;
            }
            
            // Passive items
            
            for (int i = 0; i < chest.GetLocalContent().GetPassiveItemsSize(); i++) {
                this.chestContentSlots.Add(Instantiate(UI_chestContentSlotPrefab) as GameObject);
                this.chestContentSlots[contentSlotCounter].transform.SetParent(chestContentBackground.transform, false);
                this.chestContentSlots[contentSlotCounter].GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -20f - 110f * contentSlotCounter);
                (PassiveItem, GameObject) passiveItem = chest.GetLocalContent().GetPassiveItem(i);
                this.chestContentSlots[contentSlotCounter].GetComponent<ChestContentSlotUI>().Init(passiveItem.Item1, passiveItem.Item2, i, chest);
                contentSlotCounter++;
            }
            
            
        }

        public void HideChestContent() {
            Destroy(chestContentBackground);
            this.chestContentBackground = null;

            // I don't need to explicitely destroy chest content's slots because I'm destroying their parent and Unity will handle
            // destruction of the children
        }

        public void UpdateContentSlots(Chest chest) {
            // This is kind of inefficient but gets the job done and is easy to understand (just destroys everything and creates it again)
            HideChestContent();
            ShowChestContent(chest);
        }
    }
}
