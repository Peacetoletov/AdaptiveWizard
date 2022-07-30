using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace AdaptiveWizard.Assets.Scripts.Other.Rooms
{
    public class CombatRoom : AbstractRoom
    {
        // combat related variables
        private CombatManager combatManager;

        // prefab objects
        public GameObject combatManagerObj;


        public override void Init(Vector2 posOffset, char[,] baseRoomVisual, List<Teleporter> teleporters) {
            base.Init(posOffset, baseRoomVisual, teleporters);
            this.combatManager = Instantiate(combatManagerObj, Vector3.zero, Quaternion.identity).GetComponent<CombatManager>();
            this.combatManager.Init(this);
        }

        public CombatManager GetCombatManager() {
            return combatManager;
        }
    }
}
