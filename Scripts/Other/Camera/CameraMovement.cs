using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdaptiveWizard.Assets.Scripts.Other.GameManagers;


namespace AdaptiveWizard.Assets.Scripts.Other.Camera
{
    public class CameraMovement : MonoBehaviour
    {
        private GameObject player;
        private Vector3 offset = new Vector3(0f, 0f, -10f);

        private void LateUpdate()
        {
            if (player == null && MainGameManager.IsGameActive()) {
                this.player = MainGameManager.GetPlayer();
            }
            if (player != null) {
                transform.position = player.transform.position + offset;
            }
        }
        
    }
}
