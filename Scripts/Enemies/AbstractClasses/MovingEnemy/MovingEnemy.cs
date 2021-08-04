using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Movement;

/*
ISSUE: Currently, there is weird looking behavior related to changing from following a path to simple movement.
This is easy to see when there are lots of enemies on one side of a long wall, with player being on the other 
side of the wall. They calculate their path to be parallel to the wall, and when they start following it, they
start walking about 45 degrees away from the wall, due to repulsion forces. After walking a short distance in this
direction, they switch to simple movement, and because player is on the other side of the wall, they basically
turn 180 degrees and start walking backwards, only to meet a wall, calculate a path parallel to the wall, and ¨
the cycle repeats.

POSSIBLE SOLUTION: Whenever an enemy would switch from following a path to simple movement, they could first
perform an A* to see the direction of the shortest path, and if the direction of a simple path is too different,
the enemy would start following the new path with a new 1 second path following timer, instead of starting to move
simply.

Currently, this issue isn't bothering me *that* much and I would rather work on other parts of the game, but this
could be revisited and fixed in the future.
*/

/*
This abstract class allows enemies to simply (in a straight line) move towards the player and repulse each other,
so that they don't stack on top of each other. Additionally, if an enemy hits a wall, they will find a path to
the player and start following it for a short while, before switching back to simple movement. 
*/
public abstract class MovingEnemy : AbstractEnemy
{
    // add short comments here; long comments will be in each class
    // Box collider used for collision detecting with terrain. Every enemy's terrain collider will be a box, regardless
    // of enemy shape. More precise colliders can be used for collision with player and player spells.
    public BoxCollider2D terrainCollider;

    // Current speed of the enemy. Possibly divide this into curSpeed and baseSpeed in future.
    private float speed;

    // Struct with variables related to general movement
    private GeneralMovement generalMovement;

    // Struct with variables related following a path
    private Path path;


    protected virtual void Start(float maxHealth, float speed) {
        // Initialize variables
        base.Start(maxHealth);
        this.speed = speed;
        this.generalMovement = new GeneralMovement();
        this.path = new Path();
    }
    
    protected virtual void FixedUpdate() {
        // Move each frame
        if (TestRoomManager.IsGameActive()) {
            DetermineMovement();

            // Update player's position; must be done as the last thing in the update method
            UpdatePlayersPosition();
        }
    }

    private void StopFollowingPath() {
        // Sets control variables that signal to stop following a path
        UnityEngine.Assertions.Assert.IsTrue(path.isFollowing);
        this.path.isFollowing = false;
        this.path.justFinishedFollowing = true;
        this.path.firstNode.goingTowards = false;
    }

    private void UpdatePlayersPosition() {
        // Player's position is needed to prevent wiggling in place.
        // Note: computing this in each enemy is inefficient, could be optimized in future
        const float minPlayerPositionDelta = 0.1f;      // how much player's position must change to be considered different from the last stored position
        if ((TestRoomManager.GetPlayer().transform.position - generalMovement.lastRecordedPlayerPosition).magnitude > minPlayerPositionDelta) {
            this.generalMovement.hasPlayerPositionChangedSinceLastMovement = true;
        }
    }

    
    private void DetermineMovement() {
        // Decide whether to move simply, or to follow a path
        if (path.isFollowing && path.followingTime.UpdateAndCheck()) {
            // If this enemy is following a path and has already been following it for a certain time, stop following it.
            StopFollowingPath();
            //print("1 second has passed, switching to simple movement");
        }

        if (path.isFollowing) {
            MoveOnPath();
            //print("following path");
        } else {
            TryToMoveSimply();
            //print("trying to move simply");
        }
    }

    private void MoveOnPath() {
        /*
        This function is called each frame when this enemy is following a path.
        1) In the base case, enemy will just move in the direction specified in path.path, adjusted by repulsion forces.
        Then, if the distance traveled is large enough (again specified in path.path), enemy will stop following the path
        and switch to simple movement.
        2) If movement in the path direction is not possible on either axis, this enemy is considered stuck due to rounding
        errors when estimating the node this enemy was standing on during the path creating process. To overcome this issue,
        enemy must first move towards the first node on the path, and only then can continue in the original path direction.
        3) If this enemy is going towards the first path node, it moves in the required direction, adjusted by repulsion forces.
        */
        if (!path.firstNode.goingTowards) {
            Vector2 movementVector = (RepulsionVector() + path.path.DirectionInWorldCoordinates()).normalized * speed * Time.deltaTime;
            //print("Moving on path. movementVector: " + (RepulsionVector() + path.path.DirectionInWorldCoordinates()).normalized * speed);
            bool moved = Move(movementVector);
            if ((path.movementOrigin - (Vector2) transform.position).magnitude > path.path.DistanceInWorldCoordinates()) {
                // print("no longer following path");
                StopFollowingPath();
            }
            if (!moved) {
                /*
                Enemy is trying to follow a path but isn't able to do so because a wall is blocking movement
                Note: this is not an issue with the pathfinder, rather with rounding errors when mapping enemy's
                position to a node.
                */
                // print("wall is blocking path. Starting to go towards first path node.");
                // Move towards the starting node until the desired path can be followed.
                this.path.firstNode.goingTowards = true;
                this.path.firstNode.movementOrigin = transform.position;
            }
        }
        if (path.firstNode.goingTowards) {
            Vector2 directionTowardsFirstPathNode = path.path.StartNodePositionInWorldCoordinates() - path.firstNode.movementOrigin;
            Vector2 movementVector = (RepulsionVector() + directionTowardsFirstPathNode).normalized * speed * Time.deltaTime;
            Move(movementVector);

            /*
            If an enemy needs to go towards the first path node, it means a rounding error had occured. This means that the enemy
            is at most 0.5 nodes away from the first path node (assuming only straight movement, but diagonal movement should be
            impossible in this scenario). Therefore, I can check the distance traveled towards the first path node with a hard coded
            constant distnace of 0.5 nodes.
            Note that this stops working when very narrow corridors (1 node wide) are introduced.
            */
            
            const float desiredDistance = 0.5f;
            if ((path.movementOrigin - (Vector2) transform.position).magnitude > desiredDistance) {
                //print("reached first path node");
                // First node was reached. Update the path origin, as the current position is where the actual path following starts.
                this.path.movementOrigin = transform.position;
                this.path.firstNode.goingTowards = false;
            }
        }
    }
    
    private void TryToMoveSimply() {
        /*
        Simple movement is composed of two vectors: straight line between this enemy and the player (ignoring terrain), and repulsion
        forces from other enemies (currently all enemies, planning to change it to only enemies within a short radius).
        Simple movement can fail in two scenarios:
        1) A wall is in the way
        2) Enemy would wiggle in place due to repulsion forces, which usually happens when the player is stationary and multiple
        enemies are around the player. These small back and forth wiggling movements look weird and are generally unwanted.
        Multiple variables are checked for possible signs of wiggling, and if a wiggling condition is met, enemy doesn't move.

        */
        Vector2 movementVector = (RepulsionVector() + DirectionToPlayer()).normalized * speed * Time.deltaTime;
        const float largeDirectionChange = 30f;        // what is considered a large change in direction (in degrees)
        // print("angle = " + Vector2.Angle(movementVector, lastMovementVector));
        if (path.justFinishedFollowing) {
            // Enemy can finish following a path and sharply change direction when they start moving simply. This is intended,
            // anti-wiggling rules can be ignored in this case. 
            Move(movementVector);
            path.justFinishedFollowing = false;
        }
        // Anti-wiggling condition
        else if (Vector2.Angle(movementVector, generalMovement.lastMovementVector) < largeDirectionChange ||
                 generalMovement.hasPlayerPositionChangedSinceLastMovement) {
            /* 
            Move in the calculated direction if the direction is similar to the last frame's direction, or if the direction
            is significantly different but player position has also changed (possibly teleported).
            Do NOT move if the direction is significantly different and player has not moved, as this is a clear sign of wiggling. 
            */
            Move(movementVector);
        }
    }

    private bool Move(Vector2 movementVector) {
        /*
        Update anti-wiggling variables and try to move in the direction and magnitude of movementVector.
        If simple movement is not possible, find a path and start following it.
        */
        UpdateAntiWigglingVariables(movementVector);

        // Try to move on each axis
        bool canMoveOnX;
        bool canMoveOnY;
        bool movedOnX = MoveOnOneAxis(movementVector.x, new Vector2(movementVector.x, 0), out canMoveOnX);
        bool movedOnY = MoveOnOneAxis(movementVector.y, new Vector2(0, movementVector.y), out canMoveOnY);
        if (!path.isFollowing && (!canMoveOnX || !canMoveOnY)) {
            // Wall is blocking movement on at least one axis, need to perform A* to find a path.
            Pathfinding.Node nodeOnThisPos = TestRoomManager.WorldPositionToNode(gameObject);
            Pathfinding.Node nodeOnPlayerPos = TestRoomManager.WorldPositionToNode(TestRoomManager.GetPlayer());
            this.path.isFollowing = true;
            this.path.followingTime = new Timer(1f);
            this.path.path = Pathfinding.Pathfinder.DirectionAndDistanceUntilFirstTurn(nodeOnThisPos, nodeOnPlayerPos);
            this.path.movementOrigin = transform.position;
            //print("Path direction: " + path.path.DirectionInWorldCoordinates() + ". Distance of this direction: " + path.path.DistanceInWorldCoordinates());
        }

        // Return true if this enemy moved at least on one axis
        //print("movedOnX = " + movedOnX + "; movedOnY = " + movedOnY);
        bool movedAtLeastOnOneAxis = movedOnX || movedOnY;
        return movedAtLeastOnOneAxis;
    }

    private void UpdateAntiWigglingVariables(Vector2 movementVector) {
        // These variables are used in the anti-wiggling condition
        this.generalMovement.lastMovementVector = movementVector;
        this.generalMovement.lastRecordedPlayerPosition = TestRoomManager.GetPlayer().transform.position;
        this.generalMovement.hasPlayerPositionChangedSinceLastMovement = false;
    }

    
    private bool MoveOnOneAxis(float delta, Vector2 axisVector, out bool canMove) {
        /*
        Move on axis axisVector by amount delta, if no wall is in the way.
        Returns true if movement on the given axis was perfmormed, false otherwise. 
        Additionally, out parameter canMove signals whether movement on the given axis was possible
        (regardless of whether it was actually performed or not). 
        */
        if (Mathf.Abs(delta) < 0.0000001) {
            //print("delta = " + delta);
            // No need to attempt movement is delta is 0
            canMove = true;
            return false;
        }

        const float extraBoxDistance = 0.001f;      // without this buffer, enemies could possibly get stuck in a wall on rare occasions (presumably due to floating point errors)
        RaycastHit2D hit;
        hit = Physics2D.BoxCast(transform.position, terrainCollider.size, 0, axisVector, Mathf.Abs(delta) + extraBoxDistance, LayerMask.GetMask("Wall"));
        if (hit.collider != null) {
            // wall ahead; cannot move on this axis
            canMove = false;
            return false;
        }
        if (axisVector.x == 0) {
            transform.Translate(0, delta, 0);
        }
        else {
            transform.Translate(delta, 0, 0);
        }
        canMove = true;
        return true;
    }

    private Vector2 RepulsionVector() {
        
        /*
        TODO: I don't need to calculate repulsion from all other enemies, as the forces from enemies far away are negligable anyway.
        I could use Physics.OverlapSphere function to get only enemies within a certain (very small) radius and perform repulsion calculations
        with just those. Movement could be simple for the most part, only when enemies get close to player will they start to base their movement
        on their mutual positions.
        */

        /*
        Inspired by magnetic forces, an enemy is repulsed from each other enemy based on the inversion of their distance squared.
        This method computes and returns the sum of all repulsion vectors, one from each other enemy.
        */
        //return Vector2.zero;

        
        Vector2 finalRepulsionVector = Vector2.zero;
        const float repulsionConstant = 0.75f;

        const float circleRadius = 3.5f;
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, circleRadius, LayerMask.GetMask("Enemy"));
        foreach (Collider2D collider in hitColliders) {
            // Each enemy within a short radius influences the final repulsion vector
            AbstractEnemy enemy = collider.GetComponent<AbstractEnemy>();
            if (enemy.GetID() == GetID()) {
                // Ignore itself
                continue;
            }
            Vector2 repulsionDirection = (transform.position - enemy.transform.position).normalized;
            float distance = (transform.position - enemy.transform.position).magnitude;
            float force = repulsionConstant / (distance * distance);
            Vector2 repulsionVector = new Vector2(repulsionDirection.x * force, repulsionDirection.y * force);
            if (distance == 0) {
                // Special case when two enemies are in the exact same position and (therefore) normal approach doesn't work
                repulsionVector = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            }
            finalRepulsionVector += repulsionVector;
        }
        return finalRepulsionVector;
        

        // OLD VERSION
        /*
        Vector2 finalRepulsionVector = Vector2.zero;
        const float repulsionConstant = 0.5f;       
        AbstractEnemy[] enemies = FindObjectsOfType<AbstractEnemy>();
        foreach (AbstractEnemy enemy in enemies) {
            // Each enemy influences the final repulsion vector
            if (enemy.GetID() == GetID()) {
                // Ignore itself
                continue;
            }
            Vector2 repulsionDirection = (transform.position - enemy.transform.position).normalized;
            float distance = (transform.position - enemy.transform.position).magnitude;
            float force = repulsionConstant / (distance * distance);
            Vector2 repulsionVector = new Vector2(repulsionDirection.x * force, repulsionDirection.y * force);
            if (distance == 0) {
                // Special case when two enemies are in the exact same position and (therefore) normal approach doesn't work
                repulsionVector = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            }
            finalRepulsionVector += repulsionVector;
        }
        return finalRepulsionVector;
        */
    }
}
