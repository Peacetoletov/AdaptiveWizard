using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Movement;

// TODO: thoroughly comment everything
/*
I found out why enemies get sometimes stuck at wall corners. They are supposed to start going towards the
first path node in case they get stuck, which is defined as being unable to move on both x and y axes.
However, due to repulsion forces, enemies may wiggle at the wall corner back and forth, and because
this is considered moving (= not stuck), the mechanism to start going towards the first path node
never activates.
This has two possible solutions:
1) Optimize repulsion force calculation by only including enemies within a very short radius. This does
not eliminate the problem completely but could reduce the impacts enough to make this problem negligible. 
2) Differentiate between path direction and repulsion direction when determining if a movement on an axis
is possible.
*/
public abstract class MovingEnemy : AbstractEnemy
{
    // add short comments here; long comments will be in each class
    public BoxCollider2D terrainCollider;      // box collider used for collision detecting with terrain
    private float speed;
    private GeneralMovement generalMovement;
    private Path path;


    protected virtual void Start(float maxHealth, float speed) {
        base.Start(maxHealth);
        this.speed = speed;
        this.generalMovement = new GeneralMovement();
        this.path = new Path();
    }
    
    protected Vector2 DirectionToPlayer() {
        return TestRoomManager.GetPlayer().transform.position - transform.position;
    }
    
    protected virtual void FixedUpdate() {
        if (TestRoomManager.IsGameActive()) {
            DetermineMovement();

            // Update player's position; must be done as the last thing in the update method
            // Note: computing this in each enemy is inefficient, could be optimized in future
            UpdatePlayersPosition();
        }
    }

    private void StopFollowingPath() {
        this.path.isFollowing = false;
        this.path.justFinishedFollowing = true;
        this.path.firstNode.goingTowards = false;
    }

    private void UpdatePlayersPosition() {
        // this function is related to a function that prevents wiggling in place
        const float minPlayerPositionDelta = 0.1f;      // how much player's position must change to be considered different from the last stored position
        if ((TestRoomManager.GetPlayer().transform.position - generalMovement.lastRecordedPlayerPosition).magnitude > minPlayerPositionDelta) {
            this.generalMovement.hasPlayerPositionChangedSinceLastMovement = true;
        }
    }

    
    private void DetermineMovement() {
        // if this enemy is following a path, stop following it if it was already following it for a certain time 
        if (path.isFollowing && path.followingTime.UpdateAndCheck()) {
            StopFollowingPath();
            print("1 second has passed, switching to simple movement");
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
        if (!path.firstNode.goingTowards) {
            Vector2 movementVector = (RepulsionVector() + path.path.DirectionInWorldCoordinates()).normalized * speed * Time.deltaTime;
            //print("Moving on path. movementVector: " + (RepulsionVector() + path.path.DirectionInWorldCoordinates()).normalized * speed);
            bool moved = Move(movementVector);
            if ((path.movementOrigin - (Vector2) transform.position).magnitude > path.path.DistanceInWorldCoordinates()) {
                // print("no longer following path");
                StopFollowingPath();
            }
            if (!moved) {
                // Enemy is trying to follow a path but isn't able to do so because a wall is blocking movement
                // Note: this is not an issue with the pathfinder, rather with rounding errors when mapping enemy's position
                // to a node.
                // Move towards the starting node until the desired path can be followed.
                //print("wall is blocking path. Starting to go towards first path node.");
                this.path.firstNode.goingTowards = true;
                this.path.firstNode.movementOrigin = transform.position;
            }
        }
        if (path.firstNode.goingTowards) {
            Vector2 directionTowardsFirstPathNode = path.path.StartNodePositionInWorldCoordinates() - path.firstNode.movementOrigin;
            Vector2 movementVector = (RepulsionVector() + directionTowardsFirstPathNode).normalized * speed * Time.deltaTime;
            Move(movementVector);

            //If an enemy needs to go towards the first path node, it means a rounding error had occured. This means that the enemy
            //is at most 0.5 nodes away from the first path node (assuming only straight movement, but diagonal movement should be
            //impossible in this scenario). Therefore, I can check the distance traveled towards the first path node with a hard coded
            //constant distnace of 0.5 nodes.
            //Note that this stops working when very narrow corridors (1 node wide) are introduced.
            
            const float desiredDistance = 0.5f;

            if ((path.movementOrigin - (Vector2) transform.position).magnitude > desiredDistance) {
                //print("reached first path node");
                // Update the path origin, as the current position is where the actual path following starts.
                this.path.movementOrigin = transform.position;
                this.path.firstNode.goingTowards = false;
            }
        }
    }
    
    private void TryToMoveSimply() {
        Vector2 movementVector = (RepulsionVector() + DirectionToPlayer()).normalized * speed * Time.deltaTime;
        const float largeDirectionChange = 30f;        // what is considered a large change in direction (in degrees)
        // print("angle = " + Vector2.Angle(movementVector, lastMovementVector));
        if (path.justFinishedFollowing) {
            // possibly move in a different direction, ignoring anti-wiggling rules
            Move(movementVector);
            path.justFinishedFollowing = false;
        }
        // TODO: possibly unify these two following conditions:
        else if (Vector2.Angle(movementVector, generalMovement.lastMovementVector) < largeDirectionChange) {
            // move in the calculated direction if the direction is similar to the last frame's direction
            Move(movementVector);
        }
        // if degree between this frame's movement vector and last frame's movement vector is very large 
        // (indicating a rapid change of direction, possible sign of wiggling in place)
        else {
            if (generalMovement.hasPlayerPositionChangedSinceLastMovement) {
                // move in the calculated direction if the direction is largely different but player has moved
                Move(movementVector);
            }
            // do not move in the calculated direction if the direction is largely different and player has not moved (prevent wiggling in place)
            //print("cannot move because of anti-wiggling rules");
        }
    }

    private bool Move(Vector2 movementVector) {
        PreventWigglingInPlace(movementVector);

        bool canMoveOnX;
        bool canMoveOnY;
        bool movedOnX = MoveOnOneAxis(movementVector.x, new Vector2(movementVector.x, 0), out canMoveOnX);
        bool movedOnY = MoveOnOneAxis(movementVector.y, new Vector2(0, movementVector.y), out canMoveOnY);
        if (!path.isFollowing && (!canMoveOnX || !canMoveOnY)) {
            // wall is blocking movement on at least one axis, need to perform A*
            Pathfinding.Node nodeOnThisPos = TestRoomManager.WorldPositionToNode(gameObject);
            Pathfinding.Node nodeOnPlayerPos = TestRoomManager.WorldPositionToNode(TestRoomManager.GetPlayer());
            this.path.isFollowing = true;
            this.path.followingTime = new Timer(1f);
            this.path.path = Pathfinding.Pathfinder.DirectionAndDistanceUntilFirstTurn(nodeOnThisPos, nodeOnPlayerPos);
            this.path.movementOrigin = transform.position;
            //print("Path direction: " + path.path.DirectionInWorldCoordinates() + ". Distance of this direction: " + path.path.DistanceInWorldCoordinates());
        }

        // return true if this enemy moved on at least one axis
        //print("movedOnX = " + movedOnX + "; movedOnY = " + movedOnY);
        bool movedOnAtLeastOneAxis = movedOnX || movedOnY;
        return movedOnAtLeastOneAxis;
    }

    private void PreventWigglingInPlace(Vector2 movementVector) {
        // set variables related to preventing wiggling in place
        this.generalMovement.lastMovementVector = movementVector;
        this.generalMovement.lastRecordedPlayerPosition = TestRoomManager.GetPlayer().transform.position;
        this.generalMovement.hasPlayerPositionChangedSinceLastMovement = false;
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
}
