using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using AdaptiveWizard.Assets.Scripts.Enemies.AbstractClasses;
using AdaptiveWizard.Assets.Scripts.Other.GameManagers;
using AdaptiveWizard.Assets.Scripts.Other.Other;
using AdaptiveWizard.Assets.Scripts.Enemies.Pathfinding;


/*
KNOWN ISSUES:
- Enemies with large colliders tend to get stuck near wall corners. This is because the current implementation
  generally assumes 1x1 collider (or at least similar size). To fix this, I need to take mechanisms which make
  the enemy stop following a path and turn these mechanisms into functions of collider size, such that enemies
  with bigger colliders keep following the path for longer.
  Currently, this is a low priority issue, but it will need to get fixed when I add large enemies.
*/

namespace AdaptiveWizard.Assets.Scripts.Enemies.Movement
{
    public class EnemyMovement
    {
        /*
        ##################################################
        ###############  Classes, enums  #################
        ##################################################
        */

        private enum MovementType {
            UNINITIALIZED,
            PLAYER, 
            POSITION, 
            DIRECTION 
        };

        public struct StuckInfo {
            public bool StuckOnX { get; }
            public bool StuckOnY { get; }
            public Vector2 MovementVector { get; }

            public StuckInfo(bool stuckOnX, bool stuckOnY, Vector2 movementVector) {
                StuckOnX = stuckOnX;
                StuckOnY = stuckOnY;
                MovementVector = movementVector;
            }
        }

        /*
        ##################################################
        #################  Variables  ####################
        ##################################################
        */

        

        private MovementType lastMovementType;

        private AbstractEnemy enemy;

        private PathManager pathManager;

        // testing
        private bool completelyStop = false;


        /*
        ##################################################
        ################  Constructor  ###################
        ##################################################
        */

        public EnemyMovement(AbstractEnemy enemy) {
            ResetVariables();
            this.enemy = enemy;
        }

        /*
        ##################################################
        ###############  Public methods  #################
        ##################################################
        */

        /*
        Note: only one type of movement can be used at a given time. Calling one public method clears all
        variables related to other public methods.
        */

        public void MoveTowardsPlayer(float speed) {
            /*
            Moves towards the player. If there is no immediate wall, simply moves in the player direction.
            If there is an immediate wall (therefore simply moving in the player direction isn't possible),
            finds the shortest path to the player and starts following it, until either 1 second passes
            or until the first turn occurs. Afterwards, discards the path and repeats this process.

            This movement is affected by repulsion vectors.

            This movement has undefined behaviour when very close to the player. It is assumed that other
            types of movement (or no movement at all) will be used in such situations.

            This is the default movement.
            */

            if (!completelyStop) {  // testing
                ResetVariablesIfLastMovementTypeDoesntMatch(MovementType.PLAYER);
                DecideAndExecuteMovementMode(speed, MainGameManager.GetPlayer().transform.position);    
            }
            
        }

        public int MoveTowardsPosition(Vector2 targetPos, float speed) {
            /*
            Moves towards an arbitrary position in a room. Similar behaviour to moving towards the player -
            prefers simple movement if possibly, finds and shortly follows a path if necessary.

            This movement is affected by repulsion vectors.

            Position is considered reached if the distance between current position and target position is very small.
            Returns 1 after successfully reaching target position.

            In rare situations, due to the forces of repulsion vectors, it might not be realistically possible to reach
            the target position. This might happen if the final movement vector (= intended movement vector + repulsion vector)
            is pointing in a significantly different direction from the intended movement vector, or if the magnitude of the 
            final movement vector is very small. This method returns 2 if either case is detected.

            Returns 0 if nothing exceptional happens and the enemy successfully moves towards target position.
            */

            ResetVariablesIfLastMovementTypeDoesntMatch(MovementType.POSITION);

            // TODO

            return 0;
        }

        public void MoveInDirection(Vector2 direction, float speed) {
            /*
            Moves in an arbitrary direction, if possible. Considers x and y axes separately (such that if movement is impossible
            in one but possible in the other, movement in the other axis happens regardless of the first axis).

            This movement is NOT affected by repulsion vectors.
            */

            ResetVariablesIfLastMovementTypeDoesntMatch(MovementType.DIRECTION);

            // TODO
        } 


        /*
        ##################################################
        ###############  Private methods  ################
        ##################################################
        */

        private void DecideAndExecuteMovementMode(float speed, Vector2 targetPosition) {
            // Decide whether to move simply, or to follow a path

            if (pathManager.isFollowing && pathManager.followingTime.UpdateAndCheck()) {
                // If this enemy is following a path and has already been following it for a certain time, stop following it.
                Debug.Log("Stopped following path because time ran out");
                StopFollowingPath();
            }

            if (pathManager.isFollowing) {
                //Debug.Log("Moving on path");
                MoveOnPath(speed);
            } else {
                //Debug.Log("Moving simply");
                StuckInfo stuckInfo = MoveSimply(speed);
                if (stuckInfo.StuckOnX || stuckInfo.StuckOnY) {
                    //Debug.Log("Stuck. Creating a path");
                    CreatePath(targetPosition, stuckInfo);
                }
            }
        }

        private void StopFollowingPath() {
            this.pathManager.isFollowing = false;
            this.pathManager.firstNode.goingTowards = false;
        }

        private StuckInfo MoveSimply(float speed) {
            Vector2 movementVector = (RepulsionVector() + enemy.VectorToPlayer().normalized).normalized * speed * Time.deltaTime;
            //Debug.Log("Warning: repulsion vectors are commented out");
            //Vector2 movementVector = enemy.VectorToPlayer().normalized * speed * Time.deltaTime;
            return Move(movementVector);
        }

        private StuckInfo Move(Vector2 movementVector) {
            
            bool stuckOnX = false;
            bool stuckOnY = false;
            if (movementVector.x != 0) {
                stuckOnX = MoveOnAxis(movementVector.x, 'x');
            }
            if (movementVector.y != 0) {
                stuckOnY = MoveOnAxis(movementVector.y, 'y');
            }

            //return stuckOnX || stuckOnY;
            //return (stuckOnX, stuckOnY);
            return new StuckInfo(stuckOnX, stuckOnY, movementVector);
        }

        private bool MoveOnAxis(float signedDistance, char axis) {
            Vector2 boxCenter = (Vector2) enemy.transform.position + signedDistance * (axis == 'x' ? Vector2.right : Vector2.up);
            Collider2D collider = Physics2D.OverlapBox(boxCenter, enemy.terrainCollider.size, 0, LayerMask.GetMask("Wall", "Lake"));
            // ^ I'm hoping that using OverlapBox instead of BoxCast will get enemies less often stuck inside walls
            if (collider != null) {
                return true;        // wall ahead; stuck on this axis
            }
            Vector3 translateVector = axis == 'x' ? new Vector3(signedDistance, 0, 0) : new Vector3(0, signedDistance, 0);
            enemy.transform.Translate(translateVector);
            return false;       // not stuck on this axis
        }

        private void CreatePath(Vector2 targetPosition, StuckInfo stuckInfo) {
            Node enemyPosNode = MainGameManager.GetRoomManager().GetCurRoom().WorldPositionToNode(enemy.transform.position);
            Node targetPosNode = MainGameManager.GetRoomManager().GetCurRoom().WorldPositionToNode(targetPosition);
            this.pathManager.isFollowing = true;
            this.pathManager.followingTime = new Timer(1f);
            this.pathManager.path = Pathfinder.PathUntilFirstTurn(enemyPosNode, targetPosNode, stuckInfo);
            this.pathManager.movementOrigin = enemy.transform.position;
        }

        private void MoveOnPath(float speed) {
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

            Vector2 movementVector = (RepulsionVector() + pathManager.path.GetDirection()).normalized * speed * Time.deltaTime;
            Move(movementVector);
            if ((pathManager.movementOrigin - (Vector2) enemy.transform.position).magnitude > pathManager.path.DistanceInWorldCoordinates()) {
                //Debug.Log("Stopped following path because specified distance was reached");
                StopFollowingPath();
            }

            /*
            if (!pathManager.firstNode.goingTowards) {
                // 1) Base case - move in path direction
                Vector2 movementVector = (RepulsionVector() + pathManager.path.GetDirection()).normalized * speed * Time.deltaTime;
                //(bool stuckOnX, bool stuckOnY) = Move(movementVector);
                if ((pathManager.movementOrigin - (Vector2) enemy.transform.position).magnitude > pathManager.path.DistanceInWorldCoordinates()) {
                    StopFollowingPath();
                }
                
                if (stuckOnX || stuckOnY) {
                    // 2) Resolve being stuck when attempting to follow a path
                    //
                    //Enemy is trying to follow a path but isn't able to do so because a wall is blocking movement
                    //Note: this is not an issue with the pathfinder, rather with rounding errors when mapping enemy's
                    //position to a node.
                    //
                    //Note 2: 'stuck' is true even if the enemy is stuck only on one axis. However, when following a path,
                    //the enemy only ever moves on one axis, so if it is stuck on one axis, it is fully stuck and cannot move.
                    //
                    // print("wall is blocking path. Starting to go towards first path node.");
                    // Move towards the starting node until the desired path can be followed.
                    this.pathManager.firstNode.goingTowards = true;
                    this.pathManager.firstNode.movementOrigin = enemy.transform.position;

                    this.pathManager.firstNode.stuckOnX = stuckOnX;
                    this.pathManager.firstNode.stuckOnY = stuckOnY;
                }
            }
            if (pathManager.firstNode.goingTowards) {
                // 3) Move towards the path's first node 
                //Vector2 vectorTowardsFirstPathNode = pathManager.path.StartNodePositionInWorldCoordinates() - pathManager.firstNode.movementOrigin;
                Vector2 vectorTowardsFirstPathNode = new Vector2(0, 0);
                if (pathManager.firstNode.stuckOnX) {
                    vectorTowardsFirstPathNode.y += (pathManager.path.StartNodePositionInWorldCoordinates() - pathManager.firstNode.movementOrigin).y;
                }
                else {
                    Assert.IsTrue(pathManager.firstNode.stuckOnY);
                    vectorTowardsFirstPathNode.x += (pathManager.path.StartNodePositionInWorldCoordinates() - pathManager.firstNode.movementOrigin).x;
                }
                //Debug.Log($"Going towards path's first node. Movement origin: {pathManager.firstNode.movementOrigin}. First node position: {pathManager.path.StartNodePositionInWorldCoordinates()}.");
                
                //this.completelyStop = true;     //testing


                Vector2 movementVector = (RepulsionVector() + vectorTowardsFirstPathNode.normalized).normalized * speed * Time.deltaTime;
                Move(movementVector);
                
                //
                //If an enemy needs to go towards the first path node, it means a rounding error had occured. This means that the enemy
                //is at most 0.5 nodes away from the first path node (assuming only straight movement, but diagonal movement should be
                //impossible in this scenario). Therefore, I can check the distance traveled towards the first path node with a hard coded
                //constant distnace of 0.5 nodes.
                //Note that this stops working when very narrow corridors (1 node wide) are introduced.
                //
                //IDEA: maybe this cannot be a constant 0.5 distance, but needs to be tied to the collider's size?
                //
                
                
                const float desiredDistance = 0.5f;
                if ((pathManager.movementOrigin - (Vector2) enemy.transform.position).magnitude > desiredDistance) {
                    // First node was reached. Update the path origin, as the current position is where the actual path following starts.
                    //Debug.Log($"Reached {desiredDistance} distance when walking towards first path node. Stopped going towards first path node.");
                    this.pathManager.movementOrigin = enemy.transform.position;
                    this.pathManager.firstNode.goingTowards = false;
                }
                
            }
            */
        }

        private void ResetVariablesIfLastMovementTypeDoesntMatch(MovementType currentType) {
            if (lastMovementType != currentType) {
                ResetVariables();
                lastMovementType = currentType;
            }
        }

        private void ResetVariables() {
            Debug.Log("Resetting movement variables!");
            this.lastMovementType = MovementType.UNINITIALIZED;
            this.pathManager = new PathManager();
        }

        private Vector2 RepulsionVector() {
            
            /*
            OLD TODO: I don't need to calculate repulsion from all other enemies, as the forces from enemies far away are negligable anyway.
            I could use Physics.OverlapSphere function to get only enemies within a certain (very small) radius and perform repulsion calculations
            with just those. Movement could be simple for the most part, only when enemies get close to player will they start to base their movement
            on their mutual positions.

            NEW TODO: check if the old todo is still relevant.

            NEW TODO 2: Possibly make repulsion constants different for different enemies (small enemies will repulse less, big enemies will repulse more)
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
                    repulsionVector = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
                }
                finalRepulsionVector += repulsionVector;
            }
            // Debug.Log($"Repulsion vector: {finalRepulsionVector}");
            return finalRepulsionVector;
        }
    }
}