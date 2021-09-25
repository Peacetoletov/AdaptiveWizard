using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Items;

public class ChestLocalContent
{
    private List<(ActiveItem, GameObject)> activeItems = new List<(ActiveItem, GameObject)>();
    private List<PassiveItem> passiveItems = new List<PassiveItem>();
    // spell, spellUpgrade, skillUpgrade, gold...

    public ChestLocalContent() {
        // initialize some content
        this.activeItems.Add((new HealthPotion(), MainGameManager.GetUI_Manager().GetUI_ActiveItemsManager().healthPotionUI_Prefab));
        this.activeItems.Add((new ManaPotion(), MainGameManager.GetUI_Manager().GetUI_ActiveItemsManager().manaPotionUI_Prefab));
        this.activeItems.Add((new HealthPotion(), MainGameManager.GetUI_Manager().GetUI_ActiveItemsManager().healthPotionUI_Prefab));
    }


    public int GetActiveItemsSize() {
        return activeItems.Count;
    }

    public (ActiveItem, GameObject) GetActiveItem(int index) {
        return activeItems[index];
    }

    // other methods for other reward types...
}
