using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public GameObject canvasObj;
    public GameObject healthCrystalUI_Obj;
    public GameObject divineSphereUI_Obj;

    void Start() {
        //print("UI manager is running");
        GameObject itemUI1 = Instantiate(healthCrystalUI_Obj) as GameObject;
        itemUI1.transform.SetParent(canvasObj.transform, false);
        itemUI1.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(20f, -20f, 0f);

        GameObject itemUI2 = Instantiate(divineSphereUI_Obj) as GameObject;
        itemUI2.transform.SetParent(canvasObj.transform, false);
        itemUI2.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(90f, -20f, 0f);

        GameObject itemUI3 = Instantiate(healthCrystalUI_Obj) as GameObject;
        itemUI3.transform.SetParent(canvasObj.transform, false);
        itemUI3.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(160f, -20f, 0f);
        
    }

    /*
    public void UpdatePassiveItems() {
        // will take the inventory (or a list of passive items) as an argument  
    }
    */
}
