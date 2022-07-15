using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Sprite openSpr;
    public Sprite closedSpr;

    private bool isOpen = false;

    private void Start() {
        UpdateState();
    }

    /*
    private void Update() {
        // JUST A TEST, REMOVE THIS FUNCTION LATER!
        if (Input.GetMouseButtonDown(0)) {
            if (isOpen) {
                Close();
            } else {
                Open();
            }
        }
    }
    */

    public void Open() {
        this.isOpen = true;
        UpdateState();
    }

    private void UpdateState() {
        // update sprite
        gameObject.GetComponent<SpriteRenderer>().sprite = isOpen ? openSpr : closedSpr;

        // update layer (affects collision checking)
        int newLayer = LayerMask.NameToLayer(isOpen ? "Default" : "Wall");
        gameObject.layer = newLayer;
    }
}
