using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdaptiveWizard.Assets.Scripts.Other.GameManagers;


namespace AdaptiveWizard.Assets.Scripts.Enemies.Pathfinding
{
    // The Path class defines how an enemy should move after performing an A* search
    public class Path
    {
        // what direction the enemy should start going in as soon as a path is found
        // this direction is in room coordinates, meaning that top left corner is (0, 0)
        private Vector2 direction;

        /*
        enemy will follow the given direction until the distance traveled in such direction
        is greater than this distance 
        */
        private int distance;

        public Path(Vector2 direction, int distance) {
            this.direction = direction.normalized;
            this.distance = distance;
        }

        public Vector2 GetDirection() {
            return direction;
        }

        public float GetDistance() {
            return distance;
        }
    }
}
