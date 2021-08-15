using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding {
    // The Path class defines how an enemy should move after performing an A* search
    public class Path
    {
        // what direction the enemy should start going in as soon as a path is found
        // this direction is in room coordinates, meaning that top left corner is (0, 0)
        private Vector2 direction;

        /*
        enemy will follow the given direction until the distance traveled in such direction
        is greater than this distance 
        */
        private float distance;

        // room position of the start node
        private Vector2Int startNodePosition;

        // index of the room that the enemy (that created this path) is in
        private readonly int roomIndex;

        public Path(Vector2 direction, float distance, Vector2Int startNodePosition, int roomIndex) {
            this.direction = direction;
            this.distance = distance;
            this.startNodePosition = startNodePosition;
            this.roomIndex = roomIndex;
        }

        public Vector2 DirectionInWorldCoordinates() {
            /*
            In the pathfinding algorithm, I use room coordinates, where top left corner has 
            coordinates  (0, 0), as opposed to world coordinates, where the bottom left corner
            has coornidates (0, 0). This function performs the transformation from room 
            coordinates into world coordinates.
            */
            return new Vector2(direction.x, -direction.y);
        }

        public float DistanceInWorldCoordinates() {
            /*
            In the pathfinding algorithm, I use room coordinates, where 10 is the distance between 
            two straight neighbours.
            In world coordinates, this distance is 1. This function performs the transformation
            from room coordinates into world coordinates.
            */
            return distance / 10f;
        }

        public Vector2 StartNodePositionInWorldCoordinates() {
            //return RoomManager.PositionInRoomToPositionInWorld(startNodePosition);

            return MainGameManager.GetManagerOfRoomManagers().GetRoomManager(roomIndex).PositionInRoomToPositionInWorld(startNodePosition);
            
            //ManagerOfRoomManagers managerOfRoomManagers = MainGameManager.GetManagerOfRoomManagers();
            //RoomManager roomManager = managerOfRoomManagers.GetRoomManager(roomIndex);
            //Vector2 worldPos = roomManager.PositionInRoomToPositionInWorld(startNodePosition);
            //return worldPos;
            //return Vector2.zero;
        }
    }
}
