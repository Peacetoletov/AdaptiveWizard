using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Items;

public class ChestContentSlotUI : MonoBehaviour
{
    // Reference to the button that this script is attached to
    public Button button;

    // GameObjects used for instantiates (though not all instantiates in this class, especially not instantiates of items)
    public GameObject goldIconPrefab;


    // The chest that this content slot belongs to
    private Chest chest;

    // Icon that should be displayed on this slot
    private GameObject rewardIcon;

    // ID of this slot. ID is unique between slots of the same type (active items, passive items, ...) but doesn't have to be unique between slots
    // of different types, e.g. a slot for a passive item may have an ID of 0, while a slot for a passive item may also have an ID of 0.
    private int slotTypeID = 0;


    // Only one of these will be non-null and non-zero
    private int gold = 0;
    private ActiveItem activeItem = null;
    private PassiveItem passiveItem = null;
    // spell, spellUpgrade, skillUpgrade, gold...

    public void Init(int gold, Chest chest) {
        this.gold = gold;
        Init(goldIconPrefab, 0, chest);
    }

    public void Init(ActiveItem activeItem, GameObject iconPrefab, int slotTypeID, Chest chest) {
        this.activeItem = activeItem;
        Init(iconPrefab, slotTypeID, chest);
    }

    public void Init(PassiveItem passiveItem, GameObject iconPrefab, int slotTypeID, Chest chest) {
        this.passiveItem = passiveItem;
        Init(iconPrefab, slotTypeID, chest);
    }

    private void Init(GameObject iconPrefab, int slotTypeID, Chest chest) {
        this.rewardIcon = Instantiate(iconPrefab) as GameObject;
        this.rewardIcon.transform.SetParent(gameObject.transform, false);
        this.rewardIcon.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
        this.rewardIcon.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
        this.rewardIcon.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        this.rewardIcon.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        this.slotTypeID = slotTypeID;
        this.chest = chest;
    }

	private void Start() {
        button.onClick.AddListener(TaskOnClick);
	}

    private void TaskOnClick(){
		//Debug.Log("You have clicked the button!");
        Destroy(gameObject);

        if (gold != 0) {
            InventoryNS.Inventory.AddGold(gold);
            chest.GetLocalContent().RemoveGold();
        }
        else if (activeItem != null) {
            InventoryNS.Inventory.activeItemsManager.AddItem(activeItem);
            chest.GetLocalContent().RemoveActiveItem(slotTypeID);
        }
        else if (passiveItem != null) {
            InventoryNS.Inventory.passiveItemsManager.AddItem(passiveItem);
            chest.GetLocalContent().RemovePassiveItem(slotTypeID);
        }

        MainGameManager.GetUI_Manager().GetUI_ChestContentManager().UpdateContentSlots(chest);
	}
}
