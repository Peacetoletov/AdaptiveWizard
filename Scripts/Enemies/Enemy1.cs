using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : AbstractEnemy
{
    private float speed = 2.0f;

    // Start is called before the first frame update
    private void Start() {
        base.Start(30f);
    }

    private void FixedUpdate() {
        if (TestRoomManager.IsGameActive()) {
            // MoveTowardsPlayer();
            MoveTowardsPlayerAndRepulseFromOtherEnemies();
        }
    }

    /*
    private void MoveTowardsPlayer() {
        transform.position += (Vector3) DirectionToPlayer().normalized * speed * Time.deltaTime; 
    }
    */

    private void MoveTowardsPlayerAndRepulseFromOtherEnemies() {
        // Vector2 repulsionVector = RepulsionVector();
        // Vector2 movementVector = (repulsionVector + DirectionToPlayer()).normalized * speed * Time.deltaTime;
        // transform.position += (Vector3) movementVector; 
        transform.position += (Vector3) DirectionToPlayer().normalized * speed * Time.deltaTime; 
    }

    private Vector2 RepulsionVector() {
        /*
        UPDATE: This does not work. Calling an O(n^2) method each frame is not realistic.
        */
        return Vector2.zero;

        /*
        Inspired by magnetic forces, an enemy is repulsed from each other enemy based on the inverse of their distance squared.
        This method computes and returns the sum of all repulsion vectors, one from each other enemy.
        */

        /*
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
            print("distance = " + distance);
            float force = repulsionConstant / (distance * distance);
            // float force = 1 / (distance);       // only a test
            print("direction = " + repulsionDirection);
            Vector2 repulsionVector = new Vector2(repulsionDirection.x * force, repulsionDirection.y * force);
            print("repulsionVector = " + repulsionVector);
            finalRepulsionVector += repulsionVector;
        }
        return finalRepulsionVector;
        */        

        // ^ This theoretically works but is unusable because it's insanely inefficient and makes the game lag hard with just 10 enemies.
        // It might be salvageable if it turns out that the FindObjectsOfType() function takes most time. This needs to be tested.
        // Otherwise, this whole approach must be scratched.
    }
}
