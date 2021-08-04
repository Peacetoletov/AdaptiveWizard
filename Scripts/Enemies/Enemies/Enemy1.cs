using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// TODO: after cleaning this class up, optimize repulsion vector to only take into account enemies within a very
// short radius. This should incidentally also fix a problem where enemies sometimes get stuck near corners of walls.
public class Enemy1 : MovingEnemy
{
    //private BoxCollider2D boxCollider;
    //private float speed = 2.0f;

    /*
    // Movement
    private Vector2 lastMovementVector = Vector2.zero;
    private Vector3 lastRecordedPlayerPosition;
    private bool hasPlayerPositionChangedSinceLastMovement = false;

    // Pathfinding
    private bool isFollowingPath = false;
    private bool justFinishedFollowingPath = false;
    private Path path;
    private Vector2 pathOrigin;
    private Timer pathFollowingTime;


    // Going towards first path node
    private bool isGoingTowardsFirstPathNode = false;
    private Vector2 goingTowardsFirstPathNodeOrigin;        // the world position from which this enemy started going towards first path node
    */


    protected void Start() {
        base.Start(30f, 2f);
        //this.boxCollider = GetComponent<BoxCollider2D>();
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();

        /*
        if (TestRoomManager.IsGameActive()) {
            // possibly do something here or in Update() in the future
        }
        */
    }

    /*
    private void FixedUpdate() {
        if (TestRoomManager.IsGameActive()) {
            // TryToMove();
            DetermineMovement();

            // update player's position; must be done as the last thing in the update method
            // note: computing this in each enemy is inefficient, could be optimized in future
            UpdatePlayersPosition();
        }
    }

    private void ResetPathRelatedVariables(bool justFinishedFollowingPath) {
        // TODO: call this function in start() with false as the argument
        this.isFollowingPath = false;
        this.justFinishedFollowingPath = justFinishedFollowingPath;
        this.isGoingTowardsFirstPathNode = false;
    }

    private void UpdatePlayersPosition() {
        const float minPlayerPositionDelta = 0.1f;      // how much player's position must change to be considered different from the last stored position
        if ((TestRoomManager.GetPlayer().transform.position - lastRecordedPlayerPosition).magnitude > minPlayerPositionDelta) {
            this.hasPlayerPositionChangedSinceLastMovement = true;
        }
    }

    private void DetermineMovement() {
        // if this enemy is following a path, stop following it if it was already following it for a certain time 
        if (isFollowingPath && pathFollowingTime.UpdateAndCheck()) {
            ResetPathRelatedVariables(true);
            print("1 second has passed, switching to simple movement");
        }

        if (isFollowingPath) {
            FollowPath();
            //print("following path");
        } else {
            TryToMoveSimply();
            //print("trying to move simply");
        }
    }

    private void FollowPath() {
        if (!isGoingTowardsFirstPathNode) {
            Vector2 movementVector = (RepulsionVector() + path.DirectionInWorldCoordinates()).normalized * speed * Time.deltaTime;
            bool moved = Move(movementVector);
            if ((pathOrigin - (Vector2) transform.position).magnitude > path.DistanceInWorldCoordinates()) {
                print("no longer following path");
                ResetPathRelatedVariables(true);
            }
            if (!moved) {
                // Enemy is trying to follow a path but isn't able to do so because a wall is blocking movement
                // Note: this is not an issue with the pathfinder, rather with rounding errors when mapping enemy's position
                // to a node.
                // Move towards the starting node until the desired path can be followed.
                //print("wall is blocking path");
                this.isGoingTowardsFirstPathNode = true;
                this.goingTowardsFirstPathNodeOrigin = transform.position;
            }
        }
        if (isGoingTowardsFirstPathNode) {
            Vector2 directionTowardsFirstPathNode = path.StartNodePositionInWorldCoordinates() - goingTowardsFirstPathNodeOrigin;
            Vector2 movementVector = (RepulsionVector() + directionTowardsFirstPathNode).normalized * speed * Time.deltaTime;
            Move(movementVector);

            
            //If an enemy needs to go towards the first path node, it means a rounding error had occured. This means that the enemy
            //is at most 0.5 nodes away from the first path node (assuming only straight movement, but diagonal movement should be
            //impossible in this scenario). Therefore, I can check the distance traveled towards the first path node with a hard coded
            //constant distnace of 0.5 nodes.
            //Note that this stops working when very narrow corridors (1 node wide) are introduced.
            
            const float desiredDistance = 0.5f;

            if ((pathOrigin - (Vector2) transform.position).magnitude > desiredDistance) {
                print("reached first path node");
                // Update the path origin, as the current position is where the actual path following starts.
                this.pathOrigin = transform.position;
                this.isGoingTowardsFirstPathNode = false;
            }
        }
    }

    private void TryToMoveSimply() {
        Vector2 movementVector = (RepulsionVector() + DirectionToPlayer()).normalized * speed * Time.deltaTime;
        const float largeDirectionChange = 30f;        // what is considered a large change in direction (in degrees)
        // print("angle = " + Vector2.Angle(movementVector, lastMovementVector));
        if (justFinishedFollowingPath) {
            // possibly move in a different direction, ignoring anti-wiggling rules
            Move(movementVector);
            justFinishedFollowingPath = false;
        }
        else if (Vector2.Angle(movementVector, lastMovementVector) < largeDirectionChange) {
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
            //print("cannot move because of anti-wiggling rules");
        }
    }

    private bool Move(Vector2 movementVector) {
        // set variables related to preventing wiggling in place
        this.lastMovementVector = movementVector;
        this.lastRecordedPlayerPosition = TestRoomManager.GetPlayer().transform.position;
        this.hasPlayerPositionChangedSinceLastMovement = false;

        bool canMoveOnX;
        bool canMoveOnY;
        bool movedOnX = MoveOnOneAxis(movementVector.x, new Vector2(movementVector.x, 0), out canMoveOnX);
        bool movedOnY = MoveOnOneAxis(movementVector.y, new Vector2(0, movementVector.y), out canMoveOnY);
        if (!isFollowingPath && (!canMoveOnX || !canMoveOnY)) {
            // wall is blocking movement on at least one axis, need to perform A*
            Node nodeOnThisPos = TestRoomManager.WorldPositionToNode(gameObject);
            Node nodeOnPlayerPos = TestRoomManager.WorldPositionToNode(TestRoomManager.GetPlayer());
            this.isFollowingPath = true;
            this.pathFollowingTime = new Timer(1f);
            this.path = Pathfinder.DirectionAndDistanceUntilFirstTurn(nodeOnThisPos, nodeOnPlayerPos);
            this.pathOrigin =  transform.position;
            print("Path direction: " + path.DirectionInWorldCoordinates() + ". Distance of this direction: " + path.DistanceInWorldCoordinates());
            
            
            //TODO: Problem - enemies get stuck. The root of the problem lies in rounding errors with regard to converting the world position to a node.
            //If an enemy just barely stumbles into a wall, pathfinder may not take it into account, and return a path that involves going directly into a wall.
            //This is not a problem with the pathfinder, as it thought that the enemy was not in front of a wall.
            //
            //POSSIBLE SOLUTION:
            //When following a path, return a bool to see if I was successful in moving in at least one direction. In general, I should be free to move
            //in both directions, but sometimes only one due to repulsive forces. If I cannot move in either direction, I can be certain that there was
            //a position rounding error, and I can temporarily change direction towards the position of the node that was taken as the starting node in
            //the A* algorithm. This means I move less than half a block in the desired direction before starting to follow the path as intended.
            //NOTE: this does not work for very narrow corridors, and enemies could wiggle at the start of the corridor, failing to fit into it.
            //This is not a real problem because rooms will not be designed with very narrow corridors.
        }

        // return true if this enemy moved on at least one axis
        //print("movedOnX = " + movedOnX + "; movedOnY = " + movedOnY);
        bool movedOnAtLeastOneAxis = movedOnX || movedOnY;
        return movedOnAtLeastOneAxis;
    }

    private bool MoveOnOneAxis(float delta, Vector2 axisVector, out bool canMove) {
        // Returns true if movement on the given axis was perfmormed, false otherwise. 
        // Additionally, out parameter canMove signals whether movement on the given axis was possible
        // (regardless of whether it was actually performed or not). 
        if (Mathf.Abs(delta) < 0.0000001) {
            //print("delta = " + delta);
            // don't move if delta is 0
            canMove = true;
            return false;
        }
        
        // ^ this doesn't work because it doesn't distinguish between when I actually cannot move and when I can move but don't want to (delta is 0)
        const float extraBoxDistance = 0.001f;      // without this buffer, enemies could possibly get stuck in a wall on rare occasions (presumably due to floating point errors)
        RaycastHit2D hit;
        hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, axisVector, Mathf.Abs(delta) + extraBoxDistance, LayerMask.GetMask("Wall"));
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
        // Currently, I will leave this function as is. Enemies might move very slowly next to a wall but that might be fixed by implementing
        // a pathfinding algortihm and calling it every time an enemy is close to a wall for a long time (1 second).
        // Or maybe it won't fix it. Then I would have to correctly implement fast movement next to a wall, which would require an additional box cast,
        // because I would attemp to move by a bigger distance then previously used in a box cast on one axis. Additionally, I would need to find a fix
        // for wiggling in place next to a wall.
    }

    private Vector2 RepulsionVector() {
        
        //IDEA: Maybe I don't need to calculate repulsion from all other enemies, as the forces from enemies far away are negligable anyway.
        //I could use Physics.OverlapSphere function to get only enemies within a certain (very small) radius and perform repulsion calculations
        //with just those. Movement could be simple for the most part, only when enemies get close to player will they start to base their movement
        //on their mutual positions.

        //Inspired by magnetic forces, an enemy is repulsed from each other enemy based on the inverse of their distance squared.
        //This method computes and returns the sum of all repulsion vectors, one from each other enemy.
        
        //return Vector2.zero;
        
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
    */
}
