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
- Enemies with large colliders tend to get stuck near end of walls. This is because the current implementation
  generally assumes 1x1 collider (or at least similar size). To fix this, I need to take mechanisms which make
  the enemy stop following a path and turn these mechanisms into functions of collider size (and possibly speed),
  such that enemies with bigger colliders (and slower enemies) keep following the path for longer.
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

            ResetVariablesIfLastMovementTypeDoesntMatch(MovementType.PLAYER);
            DecideAndExecuteMovement(speed, MainGameManager.GetPlayer().transform.position, true);                
        }

        public int MoveTowardsPosition(float speed, Vector2 targetPos) {
            /*
            Moves towards an arbitrary position in a room. Similar behaviour to moving towards the player -
            prefers simple movement if possible, finds and shortly follows a path if necessary.

            This movement is affected by repulsion vectors.

            Position is considered reached if the distance between current position and target position is very small.
            Returns 1 after successfully reaching target position.

            In rare situations, due to the forces of repulsion vectors, it might not be realistically possible to reach
            the target position. This might happen if the final movement vector (= intended movement vector + repulsion vector)
            is pointing in a significantly different direction from the intended movement vector, or if the magnitude of the 
            final movement vector is very small. This method returns 2 if either case is detected.

            Returns 0 if nothing exceptional happens and the enemy successfully moves towards target position.
            */

            // TODO: finish this and test this
            ResetVariablesIfLastMovementTypeDoesntMatch(MovementType.POSITION);
            int returnCode = DecideAndExecuteMovement(speed, targetPos, false);
            if (returnCode != 0) {
                ResetVariables();
            }
            return returnCode;
        }

        public void MoveInDirection(float speed, Vector2 direction) {
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

        private int DecideAndExecuteMovement(float speed, Vector2 targetPosition, bool rigidMovement) {
            // Decide movement mode - whether to move simply, or to follow a path
            
            /* Variable rigidMovement determines whether this movement is rigid or soft.
            Rigid movement - movement will always be performed, regardless of situation.
            Soft movement - 
            */

            if (pathManager.isFollowing && pathManager.followingTime.UpdateAndCheck()) {
                // If this enemy is following a path and has already been following it for a certain time, stop following it.
                this.pathManager.isFollowing = false;
            }

            if (rigidMovement) {
                DecideAndExecuteRigidMovement(speed, targetPosition);
                return 0;
            } 
            return DecideAndExecuteSoftMovement(speed, targetPosition);
        }

        private void DecideAndExecuteRigidMovement(float speed, Vector2 targetPosition) {
            if (pathManager.isFollowing) {
                MoveOnPathRigid(speed);
            } else {
                MoveSimplyRigid(speed, targetPosition);
            }
        }

        private int DecideAndExecuteSoftMovement(float speed, Vector2 targetPosition) {
            if (pathManager.isFollowing) {
                return MoveOnPathSoft(speed, targetPosition);
            }
            return MoveSimplySoft(speed, targetPosition);
        }

        private void MoveSimplyRigid(float speed, Vector2 targetPosition) {
            Vector2 movementVector = (RepulsionVector() + enemy.VectorToPlayer().normalized).normalized * speed * Time.deltaTime;
            StuckInfo stuckInfo = Move(movementVector);
            if (stuckInfo.StuckOnX || stuckInfo.StuckOnY) {
                CreatePath(targetPosition, stuckInfo);
            }
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

        private void MoveOnPathRigid(float speed) {
            Vector2 movementVector = (RepulsionVector() + pathManager.path.GetDirection()).normalized * speed * Time.deltaTime;
            Move(movementVector);
            if ((pathManager.movementOrigin - (Vector2) enemy.transform.position).magnitude > pathManager.path.GetDistance()) {
                this.pathManager.isFollowing = false;
            }
        }

        private void ResetVariablesIfLastMovementTypeDoesntMatch(MovementType currentType) {
            if (lastMovementType != currentType) {
                ResetVariables();
                lastMovementType = currentType;
            }
        }

        private int MoveSimplySoft(float speed, Vector2 targetPosition) {
            // Get movement vectors
            Vector2 dirToTarget = (targetPosition - (Vector2) enemy.transform.position).normalized;
            Vector2 baseMovementVector = dirToTarget * speed * Time.deltaTime;
            Vector2 movementVectorAfterRepulsion = (RepulsionVector() + dirToTarget).normalized * speed * Time.deltaTime;

            // Soft movement fails if the angle between base movement vector and movement vector after repulsion is too large
            if (IsAngleTooBig(baseMovementVector, movementVectorAfterRepulsion)) {
                return 2;
            }

            // Move
            StuckInfo stuckInfo = Move(movementVectorAfterRepulsion);

            // Successfully end movement if close to the target position
            if (IsTargetClose(targetPosition)) {
                return 1;
            }

            // Create a path if necessary
            if (stuckInfo.StuckOnX || stuckInfo.StuckOnY) {
                CreatePath(targetPosition, stuckInfo);
            }

            // Return 0 if nothing extraordinary happens
            return 0;
        }

        private bool IsAngleTooBig(Vector2 baseMovementVector, Vector2 movementVectorAfterRepulsion) {
            const float angleTooBig = 90f;
            return Vector2.Angle(baseMovementVector, movementVectorAfterRepulsion) > angleTooBig;
        }

        private bool IsTargetClose(Vector2 targetPosition) {
            const float targetClose = 0.25f;
            return ((Vector2) enemy.transform.position - targetPosition).magnitude < targetClose;
        }

        private int MoveOnPathSoft(float speed, Vector2 targetPosition) {
            // Get movement vectors
            Vector2 baseMovementVector = pathManager.path.GetDirection() * speed * Time.deltaTime;
            Vector2 movementVectorAfterRepulsion = (RepulsionVector() + pathManager.path.GetDirection()).normalized * speed * Time.deltaTime;

            // Soft movement fails if the angle between base movement vector and movement vector after repulsion is too large
            if (IsAngleTooBig(baseMovementVector, movementVectorAfterRepulsion)) {
                return 2;
            }

            // Move
            Move(movementVectorAfterRepulsion);

            // Successfully end movement if close to the target position
            if (IsTargetClose(targetPosition)) {
                return 1;
            }

            // Stop following path if reasonably possible
            if ((pathManager.movementOrigin - (Vector2) enemy.transform.position).magnitude > pathManager.path.GetDistance()) {
                this.pathManager.isFollowing = false;
            }

            // Return 0 if nothing extraordinary happens
            return 0;
        }

        private void ResetVariables() {
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
            return finalRepulsionVector;
        }
    }
}