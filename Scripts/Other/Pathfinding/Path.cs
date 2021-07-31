using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path
{
    // initialDirection defines in what direction the enemy should start going as soon as a path is found
    public Vector2 initialDirection;

    // turnDirection defines in what direction the enemy should start going after moving a certain distance in the initial direction
    public Vector2 turnDirection;

    // turnDistance is the distance that the enemy must go from the initial position in the initial direction, before changing direction to turnDirection
    public float turnDistance;

    public Path(Vector2 initialDirection, Vector2 turnDirection, float turnDistance) {
        this.initialDirection = initialDirection;
        this.turnDirection = turnDirection;
        this.turnDistance = turnDistance;
    }
}
