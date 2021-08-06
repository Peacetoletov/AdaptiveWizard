using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Items;

namespace InventoryNS {
    public class PassiveItems
    {
        private List<PassiveItem> allItems = new List<PassiveItem>();
        private List<PlayerHealthModifier> playerHealthModifiers = new List<PlayerHealthModifier>();
        private List<GoldModifier> goldModifiers = new List<GoldModifier>();

        public PassiveItems() {
            //Debug.Log("created passive items object");
        }

        public void AddItem(PassiveItem item) {
            this.allItems.Add(item);
            
            if (item is PlayerHealthModifier) {
                Debug.Log("added to playerHealthModifiers");
                playerHealthModifiers.Add(item as PlayerHealthModifier);
            } else if (item is GoldModifier) {
                Debug.Log("added to GoldModifier");
                goldModifiers.Add(item as GoldModifier);
            } 
        }
    }
}
