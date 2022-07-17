using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace AdaptiveWizard.Assets.Scripts.Enemies.Pathfinding
{
    public static class Pathfinder
    {
        // List of modified nodes that need to be reset after performing an A* search
        private static List<Node> modifiedNodes = new List<Node>();

        public static Path DirectionAndDistanceUntilFirstTurn(Node start, Node target, int roomIndex) {
            /*
            Returns a Path structure that can be used to navigate an enemy towards the player.
            This function calls the A* algorithm, constructs a Path object from the modified 
            Node objects and finally resets the nodes.
            */
            A_Star(start, target.GetPosition());                                        // modify nodes
            Path path = CreatePath(target, start.GetPosition(), roomIndex);             // create a path from modified nodes
            ResetModifiedNodes();
            return path;
        }

        private static void A_Star(Node start, Vector2Int targetPosition) {
            // Priority queue of open nodes
            PriorityQueue<Node> openNodes = new PriorityQueue<Node>();

            /*
            Given the priority queue implementation, I cannot simply change the priority of a given element. 
            Therefore, instead of adjusting the priority of elements in the queue, I will add them to the 
            queue again with a higher priority. Whenever I dequeue, I need to check if the node hasn't been 
            examined already (as determined by the "closed" flag of a node).
            */

            // Initialization
            AddToOpenNodes(start, 0, targetPosition, openNodes, modifiedNodes);

            // Main loop
            while (!openNodes.IsEmpty()) {
                Node node = openNodes.Dequeue();
                if (node.GetPosition() == targetPosition) {
                    break;
                }

                foreach (Node neighbour in node.GetNeighbours()) {
                    if (neighbour.IsClosed()) {
                        continue;
                    }
                    int distanceToNeighbour = node.GetDistance() + DistanceBetweenNeighbours(node, neighbour);
                    if (!neighbour.IsOpen()) {
                        AddToOpenNodes(neighbour, distanceToNeighbour, targetPosition, openNodes, modifiedNodes);
                        neighbour.SetParent(node);
                    }
                    else if (distanceToNeighbour < neighbour.GetDistance()) {
                        // neighbour is already in openNodes but a better path was found
                        neighbour.SetDistance(distanceToNeighbour);
                        openNodes.Enqueue(neighbour);        // add again, this time with a higher priority; 
                                                            // lower priority node will be skipped because it will be closed
                        neighbour.SetParent(node);
                    }
                }
                node.Close();
            }
        }

        private static Path CreatePath(Node target, Vector2Int startPos, int roomIndex) {
            /*
            This function creates a Path object by tracing the path from target node to start node, using the parent of each node.
            During this process, I keep track of the direction of the current parent. If there are multiple nodes in the same
            direction, I increment a counter. If there is a different parent direction from the last parent direction, I reset
            the counter back to 1. After reaching the start node, I can use this counter and the last parent direction to determined
            the direction and distance from the start node before a change in direction occurs. Finally, I return these values
            in the form of a Path object.
            */
            if (target.GetParent() == null) {
                // Special case when target node == start node
                // Theoretically, there should never be an opportunity to call A* in such situation, but just in case
                return new Path(Vector2.left, 0f, startPos, roomIndex);     // direction chosen arbitrarily, as it doesn't matter because of 0 distance
            }
            Vector2Int lastParentPositionDelta = new Vector2Int(0, 0);
            int nodesInOneDirection = 1;
            Node node = target;
            Node parent;
            while ((parent = node.GetParent()) != null) {
                Vector2Int parentPositionDelta = parent.GetPosition() - node.GetPosition();
                if (parentPositionDelta == lastParentPositionDelta) {
                    nodesInOneDirection++;
                } else {
                    lastParentPositionDelta = parentPositionDelta;
                    nodesInOneDirection = 1;
                }
                node = parent;
            }
            Vector2 direction = -lastParentPositionDelta;         // reverse direction: from (child->parent) to (parent->child)
            int distance = DistanceBetweenNeighbours(lastParentPositionDelta) * nodesInOneDirection;
            return new Path(direction, distance, startPos, roomIndex);
        }

        private static void AddToOpenNodes(Node node, int distance, Vector2Int targetPosition, PriorityQueue<Node> openNodes, List<Node> modifiedNodes) {
            // must first set distance and heuristic, then enqueue
            node.SetDistance(distance);
            node.OpenAndSetHeuristic(targetPosition);
            openNodes.Enqueue(node);
            Pathfinder.modifiedNodes.Add(node);
        }

        private static int DistanceBetweenNeighbours(Vector2Int positionDelta) {
            if (AreNeighboursDiagonal(positionDelta)) {
                return 14;
            }
            return 10;
        }

        private static int DistanceBetweenNeighbours(Node node1, Node node2) {
            return DistanceBetweenNeighbours(node1.GetPosition() - node2.GetPosition());
        }

        private static bool AreNeighboursDiagonal(Vector2Int positionDelta) {
            return (Math.Abs(positionDelta.x) + Math.Abs(positionDelta.y) == 2);
        }

        private static void ResetModifiedNodes() {
            foreach (Node node in modifiedNodes) {
                node.Reset();
            }
            Pathfinder.modifiedNodes.Clear();
        }
    }
}
