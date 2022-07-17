using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdaptiveWizard.Assets.Scripts.Items.Active.Classes.Abstract;
using AdaptiveWizard.Assets.Scripts.Player.Other;
using AdaptiveWizard.Assets.Scripts.Other.GameManagers;


namespace AdaptiveWizard.Assets.Scripts.Items.Active.Classes.Concrete
{
    public class ManaPotion : ActiveItem
    {
        // mana potion restores 50% of player's max mana
        private const float restoredAmount = 0.5f;

        public override void Use() {
            PlayerGeneral player = MainGameManager.GetPlayer().GetComponent<PlayerGeneral>();
            player.AddMana(player.GetMaxMana() * restoredAmount);
        }
    }
}
