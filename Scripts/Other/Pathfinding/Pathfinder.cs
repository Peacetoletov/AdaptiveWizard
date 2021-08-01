using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// TODO: check if I can make this class static
// TODO: add comments everywhere
public class Pathfinder //: MonoBehaviour
{
    // List of modified nodes that need to be reset after performing an A* search
    private List<Node> modifiedNodes = new List<Node>();

    public Path DirectionAndDistanceUntilFirstTurn(Node start, Node target) {
        // Returns a Path structure that can be used to navigate an enemy towards the player
        // this function calls the A* algorithm, constructs a Path object from the modified Node objects
        // and finally resets the nodes.

        // modify nodes
        A_Star(start, target.GetPosition());

        // create path
        Path path = CreatePath(target);

        // reset all modified nodes
        ResetModifiedNodes(modifiedNodes);

        // return path
        return path;
    }

    private void A_Star(Node start, Vector2Int targetPosition) {
        // Priority queue of open nodes
        PriorityQueue<Node> openNodes = new PriorityQueue<Node>();

        // Initialization
        AddToOpenNodes(start, 0, targetPosition, openNodes, modifiedNodes);

        /*
        Given the priority queue implementation, I cannot simply change the priority of a given element. Therefore, instead of adjusting the priority of
        elements in the queue, I will add them to the queue again with a higher priority. 
        Whenever I dequeue, I need to check if the node hasn't been examined already (as determined by the "closed" flag of a node).
        */

        //https://www.google.cz/search?q=a*+pseudocode&hl=cs&authuser=0&tbm=isch&sxsrf=ALeKk03c8nHqgf_vF-uCcyWQDoaCgftVoQ%3A1627743114860&source=hp&biw=1920&bih=969&ei=imMFYfDaMcOulwSn1rKABQ&oq=a*+ps&gs_lcp=CgNpbWcQAxgAMgQIABATMgYIABAeEBMyBggAEB4QEzIGCAAQHhATMgYIABAeEBMyBggAEB4QEzIGCAAQHhATMgYIABAeEBMyBggAEB4QEzIGCAAQHhATOgQIIxAnOgsIABCABBCxAxCDAToICAAQgAQQsQM6BQgAEIAEOggIABCxAxCDAToECAAQGFCxAljHDWDTFWgAcAB4AIABUogB2AKSAQE1mAEAoAEBqgELZ3dzLXdpei1pbWc&sclient=img#imgrc=MAM_cgwl0J_SyM
        // Main loop
        while (!openNodes.IsEmpty()) {
            Node node = openNodes.Dequeue();
            //Debug.Log("Currently covering node " + node.GetPosition());
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
                    //Debug.Log("Adding node " + neighbour.GetPosition() + " to the queue, with priority " + neighbour.AdjustedDistance());
                }
                else if (distanceToNeighbour < neighbour.GetDistance()) {
                    // neighbour is already in openNodes but a better path was found
                    neighbour.SetDistance(distanceToNeighbour);
                    openNodes.Enqueue(neighbour);        // add again, this time with a higher priority; 
                                                         // lower priority node will be skipped because it will be closed
                    neighbour.SetParent(node);
                    //Debug.Log("Adding node " + neighbour.GetPosition() + " to the queue again, with priority " + neighbour.AdjustedDistance());
                }
            }

            node.Close();
        }
    }

    private Path CreatePath(Node target) {
        if (target.GetParent() == null) {
            // Special case when target node == start node
            // Theoretically, there should never be an opportunity to call A* in such situation, but just in case
            return new Path(Vector2.left, 0f);     // direction chosen arbitrarily, as it doesn't matter because of 0 distance
        }
        Vector2Int lastParentPositionDelta = new Vector2Int(0, 0);
        int blocksInOneDirection = 1;
        Node node = target;
        Node parent;
        while ((parent = node.GetParent()) != null) {
            Vector2Int parentPositionDelta = parent.GetPosition() - node.GetPosition();
            if (parentPositionDelta == lastParentPositionDelta) {
                blocksInOneDirection++;
            } else {
                lastParentPositionDelta = parentPositionDelta;
                blocksInOneDirection = 1;
            }
            node = parent;
        }
        Vector2 direction = -lastParentPositionDelta;         // reverse direction: (child->parent) -> (parent->child)
        int distance = DistanceBetweenNeighbours(lastParentPositionDelta) * blocksInOneDirection;
        Debug.Log("direction = " + direction + "; distance = " + distance);
        return new Path(direction, distance);
    }

    private void AddToOpenNodes(Node node, int distance, Vector2Int targetPosition, PriorityQueue<Node> openNodes, List<Node> modifiedNodes) {
        // must first set distance and heuristic, then enqueue
        node.SetDistance(distance);
        node.OpenAndSetHeuristic(targetPosition);
        openNodes.Enqueue(node);
        this.modifiedNodes.Add(node);
    }

    private int DistanceBetweenNeighbours(Vector2Int positionDelta) {
        if (AreNeighboursDiagonal(positionDelta)) {
            return 14;
        }
        return 10;
    }

    private int DistanceBetweenNeighbours(Node node1, Node node2) {
        /*
        int positionDelta = (int) Math.Abs(node1.GetPosition().x - node2.GetPosition().x) + Math.Abs(node1.GetPosition().y - node2.GetPosition().y);
        if (positionDelta == 1) {
            // vertical or horizontal
            return 10;
        }
        // diagonal
        return 14;
        */
        /*
        if (AreNeighboursDiagonal(node1, node2)) {
            return 14;
        }
        return 10;
        */
        return DistanceBetweenNeighbours(node1.GetPosition() - node2.GetPosition());
    }

    /*
    private bool AreNeighboursDiagonal(Node node1, Node node2) {
        Vector2Int direction = node1.GetPosition() - node2.GetPosition();
        return (Math.Abs(direction.x) + Math.Abs(direction.y) == 2);
    }
    */

    private bool AreNeighboursDiagonal(Vector2Int positionDelta) {
        return (Math.Abs(positionDelta.x) + Math.Abs(positionDelta.y) == 2);
    }

    private void ResetModifiedNodes(List<Node> modifiedNodes) {
        foreach (Node node in modifiedNodes) {
            node.Reset();
        }
    }
}
