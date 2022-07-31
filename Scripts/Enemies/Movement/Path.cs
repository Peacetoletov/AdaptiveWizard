using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdaptiveWizard.Assets.Scripts.Other.Other;


/*
Class for keeping path related variables together.
*/
namespace AdaptiveWizard.Assets.Scripts.Enemies.Movement
{
    public class Path
    {
        // Local struct for keeping together variables related to going to the first node on a path
        public class FirstNode {
            // Signals whether the enemy is currently going towards the first node on the path
            public bool goingTowards = false;

            // Position in world coordinates from which the enemy started going towards first path node
            public Vector2 movementOrigin;
        }

        // Struct with definition of the direction and distance of the path
        public Pathfinding.Path path;

        // Position in world coordinates from which the enemy started following the path
        public Vector2 movementOrigin;

        // How long the enemy has been following the path
        public Timer followingTime;

        // Signals whether the enemy is currently following a path
        public bool isFollowing = false;

        // Signals whether the enemy stopped following the path in the last frame
        public bool justFinishedFollowing = false;
        public FirstNode firstNode;

        
        public Path() {
            this.firstNode = new FirstNode();
        }
    }
}
