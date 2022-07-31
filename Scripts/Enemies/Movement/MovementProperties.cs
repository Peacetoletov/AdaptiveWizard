using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AdaptiveWizard.Assets.Scripts.Enemies.Movement
{
    /*
    Struct for keeping movement and anti-wiggling related variables together.
    */
    public class MovementProperties
    {
        // Vector of the last performed movement
        public Vector2 lastMovementVector = Vector2.zero;

        // Player's last position in world coordinates
        public Vector3 lastRecordedPlayerPosition;

        // Signals whether player's position had changed since the last time this enemy moved
        public bool hasPlayerPositionChangedSinceLastMovement = false;
    }
}
