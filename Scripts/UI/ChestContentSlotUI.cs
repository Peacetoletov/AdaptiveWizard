using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Items;

public class ChestContentSlotUI : MonoBehaviour
{
    public Button button;


    private GameObject rewardIcon;


    // Only one of these will be non-null
    private ActiveItem activeItem = null;
    private PassiveItem passiveItem = null;
    // spell, spellUpgrade, skillUpgrade, gold...


    public void Init(GameObject iconPrefab, ActiveItem activeItem) {
        Init(iconPrefab);
        this.activeItem = activeItem;
    }

    public void Init(GameObject iconPrefab, PassiveItem passiveItem) {
        Init(iconPrefab);
        this.passiveItem = passiveItem;
    }

    private void Init(GameObject iconPrefab) {
        this.rewardIcon = Instantiate(iconPrefab) as GameObject;
        this.rewardIcon.transform.SetParent(gameObject.transform, false);
    }

	private void Start() {
        button.onClick.AddListener(TaskOnClick);
	}

    private void TaskOnClick(){
		//Debug.Log("You have clicked the button!");
        Destroy(gameObject);

        if (activeItem != null) {
            InventoryNS.Inventory.activeItemsManager.AddItem(activeItem);
        }
        if (passiveItem != null) {
            InventoryNS.Inventory.passiveItemsManager.AddItem(passiveItem);
        }
	}
}
