using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestContentSlotUI : MonoBehaviour
{
    public Button button;

	void Start() {
        button.onClick.AddListener(TaskOnClick);
	}

    void TaskOnClick(){
		//Debug.Log("You have clicked the button!");
        Destroy(gameObject);
	}
}
