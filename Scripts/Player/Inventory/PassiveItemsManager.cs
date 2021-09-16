using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Items;

namespace InventoryNS {
    public class PassiveItemsManager
    {
        public List<PassiveItem> allItems = new List<PassiveItem>();
        public List<PlayerHealthModifier> playerHealthModifiers = new List<PlayerHealthModifier>();
        public List<GoldModifier> goldModifiers = new List<GoldModifier>();
        public List<MaxManaModifier> maxManaModifiers = new List<MaxManaModifier>();

        public PassiveItemsManager() {
            //Debug.Log("created passive items manager");
        }


        public void AddItem(PassiveItem item) {
            this.allItems.Add(item);
            MainGameManager.GetUI_Manager().GetPassiveItemsUI_Manager().UpdateItems(allItems);

            if (item is PlayerHealthModifier) {
                playerHealthModifiers.Add(item as PlayerHealthModifier);
                MainGameManager.GetPlayer().GetComponent<PlayerGeneral>().UpdateMaxHealth();
            } else if (item is GoldModifier) {
                goldModifiers.Add(item as GoldModifier);
            } else if (item is MaxManaModifier) {
                maxManaModifiers.Add(item as MaxManaModifier);
                MainGameManager.GetPlayer().GetComponent<PlayerGeneral>().UpdateMaxMana();
            } 
        }

        public void DeleteItem(PassiveItem item) {
            // TODO: implement this function (this has low priority and can wait, I don't need to delete items yet)
            if (item is PlayerHealthModifier) {
                // iterate through elements in playerHealthModifiers, compare their ID, delete item if IDs match
                // create a generic function for this ^
            }
            /*
            ...
            */
        }
    }
}
