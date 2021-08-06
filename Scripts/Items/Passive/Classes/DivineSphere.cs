using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items {
    public class DivineSphere : PassiveItem, GoldModifier
    {
        private float multiplier = 1.35f;
        
        public float GoldMultiplier(float gold) {
            return gold * multiplier;
        }
    }
}
