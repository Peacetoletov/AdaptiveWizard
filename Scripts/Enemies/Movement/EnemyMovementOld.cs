using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using AdaptiveWizard.Assets.Scripts.Other.GameManagers;
using AdaptiveWizard.Assets.Scripts.Enemies.AbstractClasses;
using AdaptiveWizard.Assets.Scripts.Other.Other;


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
namespace AdaptiveWizard.Assets.Scripts.Enemies.Movement
{
    public class EnemyMovementOld
    {
        // Current speed of the enemy. Possibly divide this into curSpeed and baseSpeed in future.
        private float speed;

        // Box collider used for collision detecting with terrain. Every enemy's terrain collider will be a box, regardless
        // of enemy shape. More precise colliders can be used for collision with player and player spells.
        // For square-shaped enemies, one collider can be used for collision checking with both terrain and player/spells.
        private BoxCollider2D terrainCollider;

        // The enemy in question
        private AbstractEnemy enemy;

        // Class with variables related to general movement
        private MovementProperties movementProperties;

        // Struct with variables related following a path
        private PathManager pathManager;

        // Index of the room that this enemy spawned in
        private int roomIndex;


        public EnemyMovementOld(float speed, BoxCollider2D terrainCollider, AbstractEnemy enemy) {
            // Initialize variables
            this.speed = speed;
            this.terrainCollider = terrainCollider;
            this.enemy = enemy;
            this.movementProperties = new MovementProperties();
            this.pathManager = new PathManager();
            this.roomIndex = MainGameManager.GetRoomManager().GetCurActiveRoomIndex();
        }

        public (bool, bool) PublicTryToMove(Vector2 movementVector) {
            // This method is used exclusively for movement caused by external sources, such as animations, crowd control etc.
            // It cannot be reasonably used by the internal movement system without large refactoring.
            // Some control information is unused provided/required by the method MoveOnOneAxis() is discarded. 
            bool canMoveOnX;
            bool canMoveOnY;
            bool movedOnX = MoveOnOneAxis(movementVector.x, new Vector2(movementVector.x, 0), out canMoveOnX);
            bool movedOnY = MoveOnOneAxis(movementVector.y, new Vector2(0, movementVector.y), out canMoveOnY);
            return (movedOnX, movedOnY);
        }
        
        public void UpdateMovementTowardsPlayer() {
            DetermineMovement();

            // Update player's position; must be done as the last thing in the update method
            UpdatePlayersPosition();
        }

        private void StopFollowingPath() {
            // Sets control variables that signal to stop following a path
            Assert.IsTrue(pathManager.isFollowing);
            //print(Time.time + ". Stopped following path");
            this.pathManager.isFollowing = false;
            this.pathManager.justFinishedFollowing = true;
            this.pathManager.firstNode.goingTowards = false;
        }

        private void UpdatePlayersPosition() {
            // Player's position is needed to prevent wiggling in place.
            // Note: computing this in each enemy is inefficient, could be optimized in future
            const float minPlayerPositionDelta = 0.1f;      // how much player's position must change to be considered different from the last stored position
            if ((MainGameManager.GetPlayer().transform.position - movementProperties.lastRecordedPlayerPosition).magnitude > minPlayerPositionDelta) {
                this.movementProperties.hasPlayerPositionChangedSinceLastMovement = true;
            }
        }

        
        private void DetermineMovement() {
            // Decide whether to move simply, or to follow a path
            if (pathManager.isFollowing && pathManager.followingTime.UpdateAndCheck()) {
                // If this enemy is following a path and has already been following it for a certain time, stop following it.
                StopFollowingPath();
                //print("1 second has passed, switching to simple movement");
            }

            if (pathManager.isFollowing) {
                //print(Time.time + ". following path");
                MoveOnPath();
            } else {
                //print(Time.time + ". trying to move simply");
                TryToMoveSimply();
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
            3) If this enemy is going towards the first path node, it moves in the required direction, adjusted for repulsion forces.
            */
            if (!pathManager.firstNode.goingTowards) {
                Vector2 movementVector = (RepulsionVector() + pathManager.path.GetDirection()).normalized * speed * Time.deltaTime;
                //print("Moving on path. movementVector: " + (RepulsionVector() + path.path.DirectionInWorldCoordinates()).normalized * speed);
                bool moved = Move(movementVector);
                if ((pathManager.movementOrigin - (Vector2) enemy.transform.position).magnitude > pathManager.path.DistanceInWorldCoordinates()) {
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
                    this.pathManager.firstNode.goingTowards = true;
                    this.pathManager.firstNode.movementOrigin = enemy.transform.position;
                }
            }
            if (pathManager.firstNode.goingTowards) {
                Vector2 directionTowardsFirstPathNode = pathManager.path.StartNodePositionInWorldCoordinates() - pathManager.firstNode.movementOrigin;
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
                if ((pathManager.movementOrigin - (Vector2) enemy.transform.position).magnitude > desiredDistance) {
                    //print("reached first path node");
                    // First node was reached. Update the path origin, as the current position is where the actual path following starts.
                    this.pathManager.movementOrigin = enemy.transform.position;
                    this.pathManager.firstNode.goingTowards = false;
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
            if (pathManager.justFinishedFollowing) {
                // Enemy can finish following a path and sharply change direction when they start moving simply. This is intended,
                // anti-wiggling rules can be ignored in this case. 
                Move(movementVector);
                pathManager.justFinishedFollowing = false;
            }
            // Anti-wiggling condition
            else if (Vector2.Angle(movementVector, movementProperties.lastMovementVector) < largeDirectionChange ||
                    movementProperties.hasPlayerPositionChangedSinceLastMovement) {
                /* 
                Move in the calculated direction if the direction is similar to the last frame's direction, or if the direction
                is significantly different but player position has also changed (possibly teleported).
                Do NOT move if the direction is significantly different and player has not moved, as this is a clear sign of wiggling. 
                */
                Move(movementVector);
            }
        }

        private Vector2 DirectionToPlayer() {
            // Returns a vector directed from this enemy to the player 
            return MainGameManager.GetPlayer().transform.position - enemy.transform.position;
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
            //print(Time.time + ". is following path = " + path.isFollowing);
            //print(Time.time + ". movedOnX = " + movedOnX + ". movedOnY = " + movedOnY);
            if (!pathManager.isFollowing && (!canMoveOnX || !canMoveOnY)) {
                // Wall is blocking movement on at least one axis, need to perform A* to find a path.
                Pathfinding.Node nodeOnThisPos = MainGameManager.GetRoomManager().GetCurRoom().WorldPositionToNode(enemy.transform.position);
                Pathfinding.Node nodeOnPlayerPos = MainGameManager.GetRoomManager().GetCurRoom().WorldPositionToNode(MainGameManager.GetPlayer().transform.position);
                this.pathManager.isFollowing = true;
                this.pathManager.followingTime = new Timer(1f);
                this.pathManager.path = Pathfinding.Pathfinder.DirectionAndDistanceUntilFirstTurn(nodeOnThisPos, nodeOnPlayerPos);
                this.pathManager.movementOrigin = enemy.transform.position;
                //print(Time.time + ". Wall is blocking movement on at least one axis, need to perform A* to find a path");
                //print(Time.time + ". Path direction: " + path.path.GetDirection() + ". Distance of this direction: " + path.path.DistanceInWorldCoordinates());
            }

            // Return true if this enemy moved at least on one axis
            //print("movedOnX = " + movedOnX + "; movedOnY = " + movedOnY);
            bool movedAtLeastOnOneAxis = movedOnX || movedOnY;
            //print(Time.time + ". Moved at least on one axis = " + movedAtLeastOnOneAxis);
            return movedAtLeastOnOneAxis;
        }

        private void UpdateAntiWigglingVariables(Vector2 movementVector) {
            // These variables are used in the anti-wiggling condition
            this.movementProperties.lastMovementVector = movementVector;
            this.movementProperties.lastRecordedPlayerPosition = MainGameManager.GetPlayer().transform.position;
            this.movementProperties.hasPlayerPositionChangedSinceLastMovement = false;
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
                // No need to attempt movement if delta is 0
                canMove = true;
                return false;
            }

            RaycastHit2D hit = Physics2D.BoxCast(enemy.transform.position, terrainCollider.size, 0, axisVector, Mathf.Abs(delta) + 
                                                AbstractEnemy.extraDistanceFromWall, LayerMask.GetMask("Wall", "Lake"));
            if (hit.collider != null) {
                // wall ahead; cannot move on this axis
                canMove = false;
                return false;
            }
            if (axisVector.x == 0) {
                enemy.transform.Translate(0, delta, 0);
            }
            else {
                enemy.transform.Translate(delta, 0, 0);
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
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(enemy.transform.position, circleRadius, LayerMask.GetMask("Enemy"));
            foreach (Collider2D collider in hitColliders) {
                // Each enemy within a short radius influences the final repulsion vector
                AbstractEnemy anotherEnemy = collider.GetComponent<AbstractEnemy>();
                if (anotherEnemy.GetID() == enemy.GetID()) {
                    // Ignore itself
                    continue;
                }
                Vector2 repulsionDirection = (enemy.transform.position - anotherEnemy.transform.position).normalized;
                float distance = (enemy.transform.position - anotherEnemy.transform.position).magnitude;
                float force = repulsionConstant / (distance * distance);
                Vector2 repulsionVector = new Vector2(repulsionDirection.x * force, repulsionDirection.y * force);
                if (distance == 0) {
                    // Special case when two enemies are in the exact same position and (therefore) normal approach doesn't work
                    repulsionVector = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                }
                finalRepulsionVector += repulsionVector;
            }
            return finalRepulsionVector;
        }

        public Vector2 GetLastMovementVector() {
            return movementProperties.lastMovementVector;
        }
    }
}
