using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items {
    public class HealthCrystal : PassiveItem, PlayerHealthModifier
    {
        // Health Crystal increases player health by 20 %
        private const float multiplier = 1.2f;

        public float ModifyHealth(float health) {
            return health * multiplier;
        }
    }
}
