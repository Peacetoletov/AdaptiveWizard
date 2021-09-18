using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items {
    public class ManaCrystal : PassiveItem, MaxManaModifier
    {
        // Mana Crystal increases mana by 25 %
        private const float multiplier = 1.25f;

        public float ModifyMana(float mana) {
            return mana * multiplier;
        }
    }
}
