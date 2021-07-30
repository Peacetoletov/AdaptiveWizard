using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : AbstractEnemy
{
    private BoxCollider2D boxCollider;
    private float speed = 2.0f;
    private Vector2 lastMovementVector = Vector2.zero;
    private Vector3 lastRecordedPlayerPosition;
    private bool hasPlayerPositionChangedSinceLastMovement = false;


    private void Start() {
        base.Start(30f);
        this.boxCollider = GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate() {
        if (TestRoomManager.IsGameActive()) {
            TryToMove();

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

    private void TryToMove() {
        Vector2 movementVector = (RepulsionVector() + DirectionToPlayer()).normalized * speed * Time.deltaTime;
        const float largeDirectionChange = 30f;        // what is considered a large change in direction (in degrees)
        // print("angle = " + Vector2.Angle(movementVector, lastMovementVector));
        if (Vector2.Angle(movementVector, lastMovementVector) < largeDirectionChange) {
            // move in the calculated direction if the direction is similar to the last frame's direction
            Move(movementVector);
        }
        // if degree between this frame's movement vector and last frame's movement vector is very large 
        // (indicating a rapid change of direction, possible sign of wiggling in place)
        else {
            if (hasPlayerPositionChangedSinceLastMovement) {
                // move in the calculated direction if the direction is largely different but player has moved
                Move(movementVector);
            }
            // do not move in the calculated direction if the direction is largely different and player has not moved (prevent wiggling in place)
        }
    }

    private void Move(Vector2 movementVector) {
        // set variables related to preventing wiggling in place
        this.lastMovementVector = movementVector;
        this.lastRecordedPlayerPosition = TestRoomManager.GetPlayer().transform.position;
        this.hasPlayerPositionChangedSinceLastMovement = false;

        MoveOnOneAxis(movementVector.x, new Vector2(movementVector.x, 0));
        MoveOnOneAxis(movementVector.y, new Vector2(0, movementVector.y));
    }

    private void MoveOnOneAxis(float delta, Vector2 axisVector) {
        const float extraBoxDistance = 0.001f;      // without this buffer, enemies could possibly get stuck in a wall on rare occasions (presumably due to floating point errors)
        RaycastHit2D hit;
        hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, axisVector, Mathf.Abs(delta) + extraBoxDistance, LayerMask.GetMask("Wall"));
        if (hit.collider != null) {
            // wall ahead; don't move on this axis
            return;
        }
        if (axisVector.x == 0) {
            transform.Translate(0, delta, 0);
        }
        else {
            transform.Translate(delta, 0, 0);
        }
        // Currently, I will leave this function as is. Enemies might move very slowly next to a wall but that might be fixed by implementing
        // a pathfinding algortihm and calling it every time an enemy is close to a wall for a long time (1 second).
        // Or maybe it won't fix it. Then I would have to correctly implement fast movement next to a wall, which would require an additional box cast,
        // because I would attemp to move by a bigger distance then previously used in a box cast on one axis. Additionally, I would need to find a fix
        // for wiggling in place next to a wall.
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
            if (distance == 0) {
                // special case when two enemies are in the exact same position and (therefore) normal approach doesn't work
                repulsionVector = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            }
            finalRepulsionVector += repulsionVector;
        }
        return finalRepulsionVector;
    }
}
