using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdaptiveWizard.Assets.Scripts.Items.Passive.Classes.Abstract;
using AdaptiveWizard.Assets.Scripts.Items.Passive.Interfaces;

namespace AdaptiveWizard.Assets.Scripts.Items.Passive.Classes.Concrete
{
    public class DivineSphere : PassiveItem, GoldModifier
    {
        private const float multiplier = 1.35f;
        
        public float ModifyGold(float gold) {
            return gold * multiplier;
        }
    }
}
