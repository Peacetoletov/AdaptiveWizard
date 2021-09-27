using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Items;

public class ChestLocalContent
{
    private int gold = 0;
    private List<(ActiveItem, GameObject)> activeItems = new List<(ActiveItem, GameObject)>();
    private List<(PassiveItem, GameObject)> passiveItems = new List<(PassiveItem, GameObject)>();
    // spell, spellUpgrade, skillUpgrade, gold...

    public ChestLocalContent() {
        // initialize some content
        this.gold = 15;
        this.activeItems.Add((new HealthPotion(), MainGameManager.GetUI_Manager().GetUI_ActiveItemsManager().healthPotionUI_Prefab));
        this.activeItems.Add((new ManaPotion(), MainGameManager.GetUI_Manager().GetUI_ActiveItemsManager().manaPotionUI_Prefab));
        this.passiveItems.Add((new HealthCrystal(), MainGameManager.GetUI_Manager().GetUI_PassiveItemsManager().healthCrystalUI_Prefab));
        this.passiveItems.Add((new ManaCrystal(), MainGameManager.GetUI_Manager().GetUI_PassiveItemsManager().manaCrystalUI_Prefab));
    }

    // Gold
    public int GetGold() {
        return this.gold;
    }

    public void RemoveGold() {
        this.gold = 0;
    }


    // Active items
    public int GetActiveItemsSize() {
        return activeItems.Count;
    }

    public (ActiveItem, GameObject) GetActiveItem(int index) {
        return activeItems[index];
    }

    public void RemoveActiveItem(int index) {
        this.activeItems.RemoveAt(index);
    }

    // Passive items
    public int GetPassiveItemsSize() {
        return passiveItems.Count;
    }

    public (PassiveItem, GameObject) GetPassiveItem(int index) {
        return passiveItems[index];
    }

    public void RemovePassiveItem(int index) {
        this.passiveItems.RemoveAt(index);
    }

    // other methods for other reward types...
}
