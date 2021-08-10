using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Items;

namespace InventoryNS {
    public class ActiveItemsManager
    {
        private ActiveItem[] items = {null, null, null};

        public ActiveItemsManager() {
            //Debug.Log("active items manager created");
        }

        public bool AddItem(ActiveItem item) {
            // return true if an item was successfully added, false if inventory is full
            if (items[0] != null && items[1] != null && items[2] != null) {
                //Debug.Log("0 = " + items[0] + ", 1 = " + items[1] + ", 2 = " + items[2]);
                //Debug.Log("inventory full");
                return false;
            }

            for (int i = 0; i < 3; i++) {
                if (items[i] != null) {
                    continue;
                }
                items[i] = item;
                break;
            }
            //Debug.Log("item added");
            MainGameManager.GetUI_Manager().UpdateActiveItems(items);
            return true;
        }

        public void UseItem(int index) {
            UnityEngine.Assertions.Assert.IsTrue(index >= 0 && index <= 2);
            if (items[index] != null) {
                items[index].Use();
                items[index] = null;
                MainGameManager.GetUI_Manager().UpdateActiveItems(items);
            }
        }
    }
}