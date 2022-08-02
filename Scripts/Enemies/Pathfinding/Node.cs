using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace AdaptiveWizard.Assets.Scripts.Enemies.Pathfinding
{
    /*
    A node represents a floor block. Floor is the only block that can be freely walked on.
    This class represents all values necessary to find the (shortest) path between two nodes
    using the A* pathfinding algortihm, defined in Pathfinder class.
    */
    public class Node : IComparable<Node>
    {
        

        // Position of this node relative to other nodes in the room, used for calculating the A* heuristic
        private Vector2Int position;
        
        // List of up to 8 neighbour nodes
        private List<Node> neighbours;

        // Distance from the start node; straight neighbours have a distance of 10, diagonal neighbours have a distance of 14
        private int distance;

        // A* heuristic, estimating the distance between this node and target node
        private int heuristic;

        // Whether this node is currently in the priority queue, ready to be covered
        private bool open;

        // Whether this node was already covered, therefore removed from the priority queue.
        private bool closed;

        // Predecessor of this node
        private Node parent;

        /*
        // Distance (as number of nodes) to the nearest wall/lake on the vertical axis
        private int nearestVerticalWallDist;

        // Distance (as number of nodes) to the nearest wall/lake on the horizontal axis
        private int nearestHorizontalWallDist;
        */


        public Node(Vector2Int position) {
            // Neighbours are set by another function, called from another class
            this.neighbours = new List<Node>();
            this.position = position;
            Reset();
        }

        public void AddNeighbour(Node neighbour) {
            UnityEngine.Assertions.Assert.IsTrue(neighbour != null);
            this.neighbours.Add(neighbour);
        }

        private int AdjustedDistance() {
            return distance + heuristic;
        }

        public void Reset() {
            this.distance = 1000000;
            this.heuristic = 1000000;
            // ^ these values are for all practical intents and purposes equal to infinity and won't cause an overflow if they're added together
            this.open = false;
            this.closed = false;
            this.parent = null;
        }

        public void SetDistance(int distance) {
            this.distance = distance;
        }

        public void OpenAndSetHeuristic(Vector2Int targetPosition) {
            this.open = true;
            SetHeuristic(targetPosition);
        }

        private void SetHeuristic(Vector2Int targetPosition) {
            // Modification of Manhattan heuristic to include diagonals
            // I'm using 10 as the distance between two straight neighbours, and 14 as the distance between two diagonal ones
            int xDelta = (int) Math.Abs(targetPosition.x - position.x);
            int yDelta = (int) Math.Abs(targetPosition.y - position.y);
            int max = Math.Max(xDelta, yDelta);
            int min = Math.Min(xDelta, yDelta);
            this.heuristic = 10 * (max - min) + 14 * min;
        }

        public void Close() {
            this.open = false;
            this.closed = true;
        }

        public int CompareTo(Node other) {
            // This method is required by the IComparable<Node> interface and is used to determine the order of nodes in a priority queue
            return AdjustedDistance() - other.AdjustedDistance();
        }

        public Vector2Int GetPosition() {
            return position;
        }

        public List<Node> GetNeighbours() {
            return neighbours;
        }

        public bool IsClosed() {
            return closed;
        }

        public bool IsOpen() {
            return open;
        }

        public int GetDistance() {
            return distance;
        }

        public Node GetParent() {
            return parent;
        }

        public void SetParent(Node parent) {
            this.parent = parent;
        }

        /*
        public int GetNearestVerticalWallDist() {
            return nearestVerticalWallDist;
        }

        public void SetNearestVerticalWallDist(int dist) {
            this.nearestVerticalWallDist = dist;
        }

        public int GetNearestHorizontalWallDist() {
            return nearestHorizontalWallDist;
        }

        public void SetNearestHorizontalWallDist(int dist) {
            this.nearestHorizontalWallDist = dist;
        }
        */
    }
}
