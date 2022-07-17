using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdaptiveWizard.Assets.Scripts.Items.Active.Classes.Abstract;
using AdaptiveWizard.Assets.Scripts.Player.Other;
using AdaptiveWizard.Assets.Scripts.Other.GameManagers;


namespace AdaptiveWizard.Assets.Scripts.Items.Active.Classes.Concrete
{
    public class HealthPotion : ActiveItem
    {
        // health potion heals the player for 25% of their max health
        private const float healAmount = 0.25f;

        public override void Use() {
            PlayerGeneral player = MainGameManager.GetPlayer().GetComponent<PlayerGeneral>();
            player.Heal(player.GetMaxHealth() * healAmount);
        }
    }
}
