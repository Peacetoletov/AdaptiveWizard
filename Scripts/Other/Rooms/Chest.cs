using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    // used for instantiates
    public GameObject interactionPromptIconObj;


    private Vector2 SIZE;

    public void Init(Vector2 SIZE) {
        this.SIZE = SIZE;
    }

    private void Update() {
        // TODO: check if player is close, and spawn a prompt icon if so
    }
}
