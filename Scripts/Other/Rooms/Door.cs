using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdaptiveWizard.Assets.Scripts.Other.Rooms;


namespace AdaptiveWizard.Assets.Scripts.Other.Rooms
{
    public class Door : MonoBehaviour
    {
        public Sprite openSpr;
        public Sprite closedSpr;

        private bool isOpen = false;

        /*
        Whenever player leaves the bounding box of a room, nearest door is found and the player is teleported.
        The distance is determined by the 'teleportDistance' variable.
        */
        private Teleporter teleporter;
        

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

        
        public void SetTeleporter(Teleporter teleporter) {
            this.teleporter = teleporter;
        }
        
        public Teleporter GetTeleporter() {
            return teleporter;
        }

    }
}
