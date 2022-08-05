using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdaptiveWizard.Assets.Scripts.Other.Other;
using AdaptiveWizard.Assets.Scripts.Enemies.Movement.Pathfinding;


/*
Class for keeping path related variables together.
*/
namespace AdaptiveWizard.Assets.Scripts.Enemies.Movement.General
{
    public class PathManager
    {
        // Class with definition of the direction and distance of the path
        public Path path;

        // Position in world coordinates from which the enemy started following the path
        public Vector2 movementOrigin;

        // Timer to count how long the enemy has been following the path
        public Timer followingTime;

        // Signals whether the enemy is currently following a path
        public bool isFollowing = false;
    }
}
