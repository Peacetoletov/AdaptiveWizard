using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// TODO: create 1 additional classes for UI management of chest content
namespace AdaptiveWizard.Assets.Scripts.UI
{
    public class UI_Manager : MonoBehaviour
    {
        // GameObjects used for instantiates
        public GameObject canvasObj;
        public GameObject UI_passiveItemsManagerPrefab;
        public GameObject UI_chestContentManagerPrefab;
        public GameObject UI_chestContentBackgroundPrefab;
        public GameObject UI_chestContentSlotPrefab;


        // Submanagers
        private UI_PassiveItemsManager UI_passiveItemsManager;
        private UI_ActiveItemsManager UI_activeItemsManager;
        private UI_ChestContentManager UI_chestContentManager;


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
            
            this.UI_passiveItemsManager = Instantiate(UI_passiveItemsManagerPrefab, Vector2.zero, Quaternion.identity).GetComponent<UI_PassiveItemsManager>();
            UI_passiveItemsManager.Init(canvasObj);

            // Active items manager must be already in the scene (cannot be instantiated from here) because it needs references to the active item boxes in the scene
            this.UI_activeItemsManager = (UI_ActiveItemsManager) FindObjectOfType(typeof(UI_ActiveItemsManager));

            this.UI_chestContentManager = Instantiate(UI_chestContentManagerPrefab, Vector2.zero, Quaternion.identity).GetComponent<UI_ChestContentManager>();
            UI_chestContentManager.Init(canvasObj);
        }

        public UI_PassiveItemsManager GetUI_PassiveItemsManager() {
            return UI_passiveItemsManager;
        }

        public UI_ActiveItemsManager GetUI_ActiveItemsManager() {
            return UI_activeItemsManager;
        }

        public UI_ChestContentManager GetUI_ChestContentManager() {
            return UI_chestContentManager;
        }


        /*
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
        */
    }
}
