using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdaptiveWizard.Assets.Scripts.Items.Active.Classes.Abstract;
using AdaptiveWizard.Assets.Scripts.Items.Active.Classes.Concrete;


namespace AdaptiveWizard.Assets.Scripts.UI
{
    public class UI_ActiveItemsManager : MonoBehaviour
    {
        // GameObjects used for instantiates
        public GameObject healthPotionUI_Prefab;
        public GameObject manaPotionUI_Prefab;


        // UI elements from scene
        public GameObject activeItemBox;
        private GameObject activeItemObject;


        public void UpdateActiveItem(ActiveItem item) {
            DestroyActiveItemObject();
            if (item != null) {
                GameObject itemObj;
                if (item is HealthPotion) {
                    itemObj = Instantiate(healthPotionUI_Prefab) as GameObject;
                }
                else {
                    itemObj = Instantiate(manaPotionUI_Prefab) as GameObject;
                }
                itemObj.transform.SetParent(activeItemBox.transform, false);
                this.activeItemObject = itemObj;
            }
        }

        private void DestroyActiveItemObject() {
            Destroy(activeItemObject);
            activeItemObject = null;
        }
    }
}
