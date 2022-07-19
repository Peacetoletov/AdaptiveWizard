using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdaptiveWizard.Assets.Scripts.Other.Rooms;


// TODO: doors probably shouldn't have an Open state, rather they should just disappear when opened (after some animation)
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

            // this might need to get changed in the future but works for now
            // (it works fine if doors start locked and disappear when opened)
            if (isOpen) {
                for (var i = gameObject.transform.childCount - 1; i >= 0; i--) {
                    Object.Destroy(gameObject.transform.GetChild(i).gameObject);
                }
            }
        }

        
        public void SetTeleporter(Teleporter teleporter) {
            this.teleporter = teleporter;
        }
        
        public Teleporter GetTeleporter() {
            return teleporter;
        }

    }
}
