using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using AdaptiveWizard.Assets.Scripts.Other.GameManagers;
using AdaptiveWizard.Assets.Scripts.Enemies.Movement.Pathfinding;


namespace AdaptiveWizard.Assets.Scripts.Enemies.General.AbstractClasses
{
    public abstract class AbstractRangedAttackPositionFinder
    {

        // TODO: document this more
        public Vector2Int Find(Vector2 enemyPosition, Vector2 projectileBoundingBoxSize, float projectileMaxTravelDistance) {
            // Finds a position from which the enemy can shoot the player. If there are multiple such positions, 
            // try to select one of the better ones.

            //Debug.Log("Finding a new ranged position.");

            Vector2Int pos = Vector2Int.zero;
            bool found = false;
            float posScore = float.NegativeInfinity;

            List<Node> modifiedNodes = new List<Node>();
            Queue<Node> q = new Queue<Node>();
            Node root = MainGameManager.GetRoomManager().GetCurRoom().WorldPositionToNode(enemyPosition);
            EnqueueAndClose(root, q, modifiedNodes);
            while (q.Count != 0) {
                Node curNode = q.Dequeue();
                Vector2Int curNodePos = curNode.GetPosition();
                if (CanHit(curNodePos, projectileBoundingBoxSize, projectileMaxTravelDistance)) {
                    found = true;
                    float curScore = EvaluatePosition(curNodePos);
                    if (curScore > posScore) {
                        posScore = curScore;
                        pos = curNodePos;
                    } 
                } 
                if (!found) {
                    foreach (Node neighbour in curNode.GetNeighbours()) {
                        if (!neighbour.IsClosed()) {
                            EnqueueAndClose(neighbour, q, modifiedNodes);
                        }
                    }
                }
            }

            ResetModifiedNodes(modifiedNodes);

            if (found) {
                //Debug.Log($"Found a new ranged position: {pos}");
                return pos;
            }

            throw new Exception("Error: Unable to find a ranged attack position!");
        }

        private void EnqueueAndClose(Node node, Queue<Node> q, List<Node> modifiedNodes) {
            q.Enqueue(node);
            node.Close();
            modifiedNodes.Add(node);
        }

        private static void ResetModifiedNodes(List<Node> modifiedNodes) {
            foreach (Node node in modifiedNodes) {
                node.Reset();
            }
        }

        protected virtual float EvaluatePosition(Vector2Int pos) {
            return (MainGameManager.GetPlayer().transform.position - new Vector3(pos.x, pos.y, 0)).magnitude;
        }

        protected bool CanHitFromDirection(Vector2 direction, Vector2 position, Vector2 projectileBoundingBoxSize, float projectileMaxTravelDistance) {
            /*
            RaycastHit2D[] hits = Physics2D.BoxCastAll(position, projectileBoundingBoxSize, 0, direction, projectileMaxTravelDistance,
                                                       LayerMask.GetMask("Wall", "Player"));
            //Debug.Log("NEW SECTION\n");
            //Debug.Log($"Position: {position}. Raycast hits:");
            foreach (RaycastHit2D hit in hits) {
                //Debug.Log($"Hit has layer mask: {1 << hit.transform.gameObject.layer}. Player has layer mask: {LayerMask.GetMask("Player")}");
                if (1 << hit.transform.gameObject.layer == LayerMask.GetMask("Player")) {
                    return true;
                }
                return false;
            }
            return false;
            */

            RaycastHit2D hit = Physics2D.BoxCast(position, projectileBoundingBoxSize, 0, direction, projectileMaxTravelDistance,
                                                 LayerMask.GetMask("Wall", "Player"));
            if (hit.collider != null && 1 << hit.transform.gameObject.layer == LayerMask.GetMask("Player")) {
                return true;
            }
            return false;
        }

        public abstract bool CanHit(Vector2 position, Vector2 projectileBoundingBoxSize, float projectileMaxTravelDistance);
    }
}