using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Sprite openSpr;
    public Sprite closedSpr;

    private bool isOpen = true;

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

    public void Close() {
        this.isOpen = false;
        UpdateState();
    }

    private void UpdateState() {
        // update sprite
        gameObject.GetComponent<SpriteRenderer>().sprite = isOpen ? openSpr : closedSpr;

        // update layer (affects collision checking)
        int newLayer = isOpen ? LayerMask.NameToLayer("Default") : LayerMask.NameToLayer("Wall");
        gameObject.layer = newLayer;
    }
}
