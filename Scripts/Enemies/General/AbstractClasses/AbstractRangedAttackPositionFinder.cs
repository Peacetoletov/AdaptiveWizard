using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using AdaptiveWizard.Assets.Scripts.Other.GameManagers;
using AdaptiveWizard.Assets.Scripts.Enemies.Movement.Pathfinding;  
using AdaptiveWizard.Assets.Scripts.Player.Other;           


namespace AdaptiveWizard.Assets.Scripts.Enemies.General.AbstractClasses
{
    public abstract class AbstractRangedAttackPositionFinder
    {
        AbstractPlayer player;

        public AbstractRangedAttackPositionFinder() {
            this.player = MainGameManager.GetPlayer().GetComponent<AbstractPlayer>();
        }

        // TODO: document this more
        public Vector2Int Find(Vector2 enemyPosition, BoxCollider2D projectileCollider, float projectileMaxTravelDistance) {
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
                if (CanHit(curNodePos, projectileCollider, projectileMaxTravelDistance)) {
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

        private float SignedAngleFromEnemyToPlayer(Vector2 enemyPosition) {
            /*
            Angles from different enemy positions (P is player):
            ..........90.........
            .179.................
            ..........P.........0
            -179.................
            .........-90.........
            */
            return Vector2.SignedAngle(Vector2.right, (Vector2) player.transform.position - enemyPosition);
        }

        private bool CanHitFromDirection(Vector2 position, BoxCollider2D projectileCollider, float projectileMaxTravelDistance) {
            float angle = SignedAngleFromEnemyToPlayer(position);
            Vector2 direction = (Vector2) player.transform.position - position;
            RaycastHit2D hit = Physics2D.BoxCast(position, projectileCollider.size, angle, direction, projectileMaxTravelDistance, 
                                                 LayerMask.GetMask("Wall", "Player"));
            if (hit.collider != null && 1 << hit.transform.gameObject.layer == LayerMask.GetMask("Player")) {
                return true;
            }
            return false;

            /*
            RaycastHit2D hit = Physics2D.Raycast(position, direction, projectileMaxTravelDistance, LayerMask.GetMask("Wall", "Player"));
            if (hit.collider != null && 1 << hit.transform.gameObject.layer == LayerMask.GetMask("Player")) {
                return true;
            }
            return false;
            */
        }

        public bool CanHit(Vector2 position, BoxCollider2D projectileCollider, float projectileMaxTravelDistance) {
            if (IsAngleValid(SignedAngleFromEnemyToPlayer(position))) {
                return CanHitFromDirection(position, projectileCollider, projectileMaxTravelDistance);
            }
            return false;
        }

        protected abstract bool IsAngleValid(float signedAngle);
    }
}