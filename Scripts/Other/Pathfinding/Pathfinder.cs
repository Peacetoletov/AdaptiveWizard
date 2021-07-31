using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder //: MonoBehaviour
{
    // Priority queue of open nodes
    private PriorityQueue<Node> openNodes;

    // List of modified nodes that need to be reset after performing an A* search
    private List<Node> modifiedNodes;

    public Pathfinder() {
        this.openNodes = new PriorityQueue<Node>();
        this.modifiedNodes = new List<Node>();
    }

    public Path DirectionAndFirstTurn(Node start, Node target) {
        // Returns a Path structure that can be used to navigate an enemy towards the player
        // this function calls the A* algorithm, constructs a Path object from the modified Node objects
        // and finally resets the nodes.

        A_Star(start, target);

        return null;        // change
    }

    private void A_Star(Node start, Node target) {
        // Initialization
        start.SetDistance(0);
        //this.openNodes.Enqueue(start);

        //test
        this.openNodes.Enqueue(start);
        this.openNodes.Enqueue(start);
        this.openNodes.Enqueue(start);
        this.openNodes.Enqueue(target);
        this.openNodes.Enqueue(target);

        // Main loop
        //while ()


        // reset all modified nodes
        ResetModifiedNodes();
    }

    private void AddToOpenNodes(Node node, Vector2Int targetPosition) {
        this.openNodes.Enqueue(node);
        node.OpenAndSetHeuristic(targetPosition);
    }

    private void ResetModifiedNodes() {
        foreach (Node node in modifiedNodes) {
            node.Reset();
        }
    }
}
