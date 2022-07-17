using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// TODO: come up with a solution to the inventory system implementation
/* Possible solution: create an interface for each type of action an item can have,
for example ChangesGoldGain, ChangesDamageDealt etc. When a new item is added to the
inventory, its type is checked and alongside the list of all items, it is added to a
list of the corresponding type. Whenever an action needs to be checked, only the
list of that action type is checked.
Cursed items will work similarly.
Active items don't need this system, as they don't need anything passively until they 
are used. If they have a temporary effect (increase damage dealt by 100% for 10 seconds),
this effect will be stored in an object called TemporaryEffects in a list of the corresponding
type, just like passive items. Additionally, each temporary effect will have an associated timer
to signal the end of the effect.
*/

namespace AdaptiveWizard.Assets.Scripts.Player.Inventory
{
    public static class InventoryManager
    {
        // to be implemented in future
        /*
        private int gold;
        private int spellOrbs;
        private int skillTreeOrbs;
        private int superchargedOrbs;
        */

        // Item managers
        public static PassiveItemsManager passiveItemsManager;
        public static ActiveItemsManager activeItemsManager;


        // Potentially create a distint class for gold management. Currently it's unnecessary.
        private static int gold = 0;

        
        public static void Initialize() {
            InventoryManager.passiveItemsManager = new PassiveItemsManager();
            InventoryManager.activeItemsManager = new ActiveItemsManager();
        }

        public static void AddGold(int amount) {
            InventoryManager.gold += amount;
        }

        public static int GetGold() {
            return InventoryManager.gold;
        }
    }
}
