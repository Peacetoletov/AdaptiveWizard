using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;


namespace AdaptiveWizard.Assets.Scripts.Enemies.Movement
{
    public class EnemyMovement
    {
        /*
        ##################################################
        #################  Variables  ####################
        ##################################################
        */

        private enum MovementType {
            UNINITIALIZED,
            PLAYER, 
            POSITION, 
            DIRECTION 
        };

        private MovementType lastMovementType;


        /*
        ##################################################
        ################  Constructor  ###################
        ##################################################
        */

        public EnemyMovement() {
            ResetVariables();
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

            // TODO
        }

        public int MoveTowardsPosition(Vector2 targetPos, float speed) {
            /*
            Moves towards an arbitrary position in a room. Similar behaviour to moving towards the player -
            prefers simple movement if possibly, finds and shortly follows a path if necessary.

            This movement is affected by repulsion vectors.

            Position is considered reached if the distance between current position and target position is very small
            and the angle between vectors given by (starting position, target position) and (current position, target postiion)
            is very large (indicating that the target position has been surpassed). Returns 1 after successfully reaching
            target position.

            In rare situations, due to the forces of repulsion vectors, it might not be realistically possible to reach
            the target position. This might happen if the final movement vector (= intended movement vector + repulsion vector)
            is pointing in a significantly different direction from the target, or if the magnitude of the final movement vector
            is very small. This method returns 2 if either case is detected.

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

        private void ResetVariablesIfLastMovementTypeDoesntMatch(MovementType currentType) {
            if (lastMovementType != currentType) {
                ResetVariables();
                lastMovementType = currentType;
            }
        }

        private void ResetVariables() {
            this.lastMovementType = MovementType.UNINITIALIZED;
        }
    }
}