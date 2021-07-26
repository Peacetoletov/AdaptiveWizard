using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : AbstractEnemy
{
    private float speed = 2.0f;
    private Vector2 lastMovementVector = Vector2.zero;
    private Vector3 lastRecordedPlayerPosition;
    private bool hasPlayerPositionChangedSinceLastMovement = false;


    private void Start() {
        base.Start(30f);
    }

    private void FixedUpdate() {
        if (TestRoomManager.IsGameActive()) {
            MoveTowardsPlayerAndRepulseFromOtherEnemies();

            // update player's position; must be done as the last thing in the update method
            // note: computing this in each enemy is inefficient, could be optimized in future
            UpdatePlayersPosition();
        }
    }

    private void UpdatePlayersPosition() {
        const float minPlayerPositionDelta = 0.1f;      // how much player's position must change to be considered different from the last stored position
        if ((TestRoomManager.GetPlayer().transform.position - lastRecordedPlayerPosition).magnitude > minPlayerPositionDelta) {
            this.hasPlayerPositionChangedSinceLastMovement = true;
        }
    }

    private void MoveTowardsPlayerAndRepulseFromOtherEnemies() {
        Vector2 movementVector = (RepulsionVector() + DirectionToPlayer()).normalized * speed * Time.deltaTime;
        const float largeDirectionChange = 30f;        // what is considered a large change in direction (in degrees)
        if (Vector2.Angle(movementVector, lastMovementVector) < largeDirectionChange) {
            // move in the calculated direction if the direction is similar to the last frame's direction
            Move((Vector3) movementVector);
        }
        // if degree between this frame's movement vector and last frame's movement vector is very large 
        // (indicating a rapid change of direction, possible sign of wiggling in place)
        else {
            if (hasPlayerPositionChangedSinceLastMovement) {
                // move in the calculated direction if the direction is largely different but player has moved
                Move((Vector3) movementVector);
            }
            // do not move in the calculated direction if the direction is largely different and player has not moved (prevent wiggling in place)
        }
    }

    private void Move(Vector3 movementVector) {
        transform.position += movementVector; 
        this.lastMovementVector = movementVector;
        this.lastRecordedPlayerPosition = TestRoomManager.GetPlayer().transform.position;
        this.hasPlayerPositionChangedSinceLastMovement = false;
    }

    private Vector2 RepulsionVector() {
        /*
        UPDATE: This does not work. Calling an O(n^2) method each frame is not realistic.
        UPDATE 2: I'm a complete pepega. It wasn't the complexity, it weren't the square roots. It were the print statements
                  that were responsible for most of the performance issues.
        */

        /*
        IDEA: Maybe I don't need to calculate repulsion from all other enemies, as the forces from enemies far away are negligable anyway.
        I could use Physics.OverlapSphere function to get only enemies within a certain (very small) radius and perform repulsion calculations
        with just those. Movement could be naive for the most part, only when enemies get close to player will they start to base their movement
        on their mutual positions.

        I still need to figure out how to stop enemies from wiggling around if the player stands still. Maybe always check the angle between 
        this frame's movement vector and the last frame's movement vector, and if the angle is too large (over 90 degrees), temporarily stop 
        movement, and resume movement when player's position changes by a significant amount.
        */

        // return Vector2.zero;

        /*
        Inspired by magnetic forces, an enemy is repulsed from each other enemy based on the inverse of their distance squared.
        This method computes and returns the sum of all repulsion vectors, one from each other enemy.
        */

        
        Vector2 finalRepulsionVector = Vector2.zero;
        const float repulsionConstant = 0.5f;       
        AbstractEnemy[] enemies = FindObjectsOfType<AbstractEnemy>();
        foreach (AbstractEnemy enemy in enemies) {
            if (enemy.GetID() == GetID()) {
                // ignore itself
                continue;
            }
            Vector2 repulsionDirection = (transform.position - enemy.transform.position).normalized;
            float distance = (transform.position - enemy.transform.position).magnitude;
            float force = repulsionConstant / (distance * distance);
            Vector2 repulsionVector = new Vector2(repulsionDirection.x * force, repulsionDirection.y * force);
            finalRepulsionVector += repulsionVector;
        }
        return finalRepulsionVector;
    }
}
