using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Items;

namespace InventoryNS {
    public class PassiveItemsManager
    {
        private List<PlayerHealthModifier> playerHealthModifiers = new List<PlayerHealthModifier>();
        private List<GoldModifier> goldModifiers = new List<GoldModifier>();

        public PassiveItemsManager() {
            //Debug.Log("created passive items manager");
        }


        public void AddItem(PassiveItem item) {
            if (item is PlayerHealthModifier) {
                //Debug.Log("added to playerHealthModifiers");
                playerHealthModifiers.Add(item as PlayerHealthModifier);
            } else if (item is GoldModifier) {
                //Debug.Log("added to GoldModifier");
                goldModifiers.Add(item as GoldModifier);
            } 
        }

        public void DeleteItem(PassiveItem item) {
            // TODO: implement this function (this has low priority and can wait)
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
