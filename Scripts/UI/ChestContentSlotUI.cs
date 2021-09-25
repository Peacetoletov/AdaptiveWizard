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


    public void Init(ActiveItem activeItem, GameObject iconPrefab) {
        this.activeItem = activeItem;
        Init(iconPrefab);
    }

    public void Init(PassiveItem passiveItem, GameObject iconPrefab) {
        this.passiveItem = passiveItem;
        Init(iconPrefab);
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
