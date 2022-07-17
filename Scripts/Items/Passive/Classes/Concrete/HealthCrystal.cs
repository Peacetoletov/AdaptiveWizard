using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdaptiveWizard.Assets.Scripts.Items.Passive.Classes.Abstract;
using AdaptiveWizard.Assets.Scripts.Items.Passive.Interfaces;

namespace AdaptiveWizard.Assets.Scripts.Items.Passive.Classes.Concrete
{
    public class HealthCrystal : PassiveItem, PlayerHealthModifier
    {
        // Health Crystal increases player health by 20 %
        private const float multiplier = 1.2f;

        public float ModifyHealth(float health) {
            return health * multiplier;
        }
    }
}
