using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Movement {
    public class Path
    {
        public class FirstNode {
            public bool goingTowards = false;
            public Vector2 movementOrigin;        // the world position from which this enemy started going towards first path node
        }

        public Pathfinding.Path path;
        public Vector2 movementOrigin;
        public Timer followingTime;
        public bool isFollowing = false;
        public bool justFinishedFollowing = false;
        public FirstNode firstNode;

        
        public Path() {
            this.firstNode = new FirstNode();
        }
    }
}
