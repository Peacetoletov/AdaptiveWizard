using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace AdaptiveWizard.Assets.Scripts.Other.Rooms
{
    public class Teleporter
    {
        private readonly Vector2 teleportDist;
        private readonly int associatedRoomIndex;

        public Teleporter(Vector2 teleportDist, int associatedRoomIndex) {
            this.teleportDist = teleportDist;
            this.associatedRoomIndex = associatedRoomIndex;
        }

        public Vector2 GetTeleportDist() {
            return teleportDist;
        }

        public int GetAssocaitedRoomIndex() {
            return associatedRoomIndex;
        }
    }
}