using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private GameObject player;
    private Vector3 offset = new Vector3(0f, 0f, -10f);

    private void LateUpdate()
    {
        if (TestRoomManager.GetIsRunActive()) {
            if (player == null) {
                SetPlayerReference();
            }
            transform.position = player.transform.position + offset;
        }
    }

    private void SetPlayerReference() {
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("Player");
        if (objectsWithTag.Length != 1) {
            throw new System.InvalidOperationException("ERROR! Incorrect number of \"player\" objects: " + objectsWithTag.Length);
        }
        this.player = objectsWithTag[0];
    }
}
