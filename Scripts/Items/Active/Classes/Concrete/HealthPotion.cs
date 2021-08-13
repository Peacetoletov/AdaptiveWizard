using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items {
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
