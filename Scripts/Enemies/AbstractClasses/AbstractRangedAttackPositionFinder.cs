using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using AdaptiveWizard.Assets.Scripts.Other.GameManagers;
using AdaptiveWizard.Assets.Scripts.Enemies.Pathfinding;


namespace AdaptiveWizard.Assets.Scripts.Enemies.AbstractClasses
{
    public abstract class AbstractRangedAttackPositionFinder
    {

        // TODO: document this more
        public Vector2Int Find(Vector2 enemyPosition, Vector2 projectileBoundingBoxSize, float projectileMaxTravelDistance) {
            // Finds a position from which the enemy can shoot the player. If there are multiple such positions, 
            // try to select one of the better ones.

            Vector2Int pos = Vector2Int.zero;
            bool found = false;
            float posScore = float.NegativeInfinity;

            Queue<Node> q = new Queue<Node>();
            Node root = MainGameManager.GetRoomManager().GetCurRoom().WorldPositionToNode(enemyPosition);
            EnqueueAndClose(root, q);
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
                            EnqueueAndClose(neighbour, q);
                        }
                    }
                }
            }

            if (found) {
                return pos;
            }

            throw new Exception("Error: Unable to find a ranged attack position!");
        }

        private void EnqueueAndClose(Node node, Queue<Node> q) {
            q.Enqueue(node);
            node.Close();
        }

        protected virtual float EvaluatePosition(Vector2Int pos) {
            return (MainGameManager.GetPlayer().transform.position - new Vector3(pos.x, pos.y, 0)).magnitude;
        }

        protected bool CanHitFromDirection(Vector2 direction, Vector2Int position, Vector2 projectileBoundingBoxSize, float projectileMaxTravelDistance) {
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
        }

        protected abstract bool CanHit(Vector2Int position, Vector2 projectileBoundingBoxSize, float projectileMaxTravelDistance);
    }
}