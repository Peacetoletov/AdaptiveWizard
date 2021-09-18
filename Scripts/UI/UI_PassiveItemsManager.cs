using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Items;

public class UI_PassiveItemsManager : MonoBehaviour
{
    // GameObjects used for instantiates
    public GameObject healthCrystalUI_Prefab;
    public GameObject manaCrystalUI_Prefab;
    public GameObject divineSphereUI_Prefab;


    private GameObject canvasObj;
    private List<GameObject> objects = new List<GameObject>();
    private readonly Vector3 offset = new Vector3(70f, 0f, 0f);
    private readonly Vector3 basePos = new Vector3(20f, -20f, 0f);
    private Vector3 nextPos = new Vector3(20f, -20f, 0f);


    public void Init(GameObject canvasObj) {
        this.canvasObj = canvasObj;
    }


    public void UpdateItems(List<PassiveItem> items) {
        DestroyObjects();
        foreach (PassiveItem item in items) {
            GameObject itemObj;
            if (item is HealthCrystal) {
                itemObj = Instantiate(healthCrystalUI_Prefab) as GameObject;
            } else if (item is DivineSphere) {
                itemObj = Instantiate(divineSphereUI_Prefab) as GameObject;
            } else {
                itemObj = Instantiate(manaCrystalUI_Prefab) as GameObject;
            }
            //else if (item is DivineSphere) {
            //    itemObj = Instantiate(divineSphereUI_Obj) as GameObject;
            //} else { ... }
            itemObj.transform.SetParent(canvasObj.transform, false);
            itemObj.GetComponent<RectTransform>().anchoredPosition3D = nextPos;
            this.nextPos += offset;
            this.objects.Add(itemObj);
        }
    }

    private void DestroyObjects() {
        this.nextPos = basePos;
        foreach (GameObject obj in objects) {
            Destroy(obj);
        }
        this.objects = new List<GameObject>();
    }
}
