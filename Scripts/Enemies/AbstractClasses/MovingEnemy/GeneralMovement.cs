using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Movement {
    public class GeneralMovement
    {
        public Vector2 lastMovementVector = Vector2.zero;
        public Vector3 lastRecordedPlayerPosition;
        public bool hasPlayerPositionChangedSinceLastMovement = false;
    }
}
