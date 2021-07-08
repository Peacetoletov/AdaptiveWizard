using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    private PlayerGeneral player;
    public Text healthText;
    
    private void Start() {
        /*
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("Player");
        if (objectsWithTag.Length != 1) {
            string message = "ERROR! Incorrect number of \"player\" objects found when attempting to display player health. " +
                             "Number of player objects found: " + objectsWithTag.Length;
            throw new System.InvalidOperationException(message);
        }
        this.player = objectsWithTag[0].GetComponent<PlayerGeneral>();
        */
    }

    private void Update()
    {   
        if (player == null) {
            GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("Player");
            if (objectsWithTag.Length != 1) {
                healthText.text = "Health: 0";
                return;
            }
            this.player = objectsWithTag[0].GetComponent<PlayerGeneral>();
        }

        if (player != null) {
            healthText.text = "Health: " + player.GetCurHealth();
        }
    }
}
