using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdaptiveWizard.Assets.Scripts.Player.Other;


namespace AdaptiveWizard.Assets.Scripts.Spells.SpellManagers
{
    public class ExplosionManager : AbstractSpellManager
    {
        private GameObject explosionObj;
        private const float manaCost = 20f;
        private const float cooldown = 0.8f;


        public override void Init() {
            this.explosionObj = Resources.Load("Prefabs/Other/explosionPrefab") as GameObject;
            base.Init(manaCost, cooldown);
        }

        public override void CastSpell(AbstractPlayer player) {
            GameObject newExplosion = Object.Instantiate(explosionObj, player.transform.position, Quaternion.identity) as GameObject;
        }
    }
}
