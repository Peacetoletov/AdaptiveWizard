using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items {
    public class DivineSphere : PassiveItem, GoldModifier
    {
        private const float multiplier = 1.35f;
        
        public float GoldModified(float gold) {
            return gold * multiplier;
        }
    }
}
