using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdaptiveWizard.Assets.Scripts.Player.Other;
using AdaptiveWizard.Assets.Scripts.Spells.SpellBehaviour;


namespace AdaptiveWizard.Assets.Scripts.Spells.SpellManagers
{
    public class FireballManager : AbstractSpellManager
    {
        private GameObject fireballObj;
        private const float manaCost = 0f;
        private const float cooldown = 0.1f;        // balanced cooldown is 0.4f

        
        public override void Init() {
            this.fireballObj = Resources.Load("Prefabs/Other/fireballPrefab") as GameObject;
            base.Init(manaCost, cooldown);
        }

        public override void CastSpell(AbstractPlayer player) {
            // player argument must be of a stricter type PlayerGeneral in this case
            Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - player.transform.position;
            float offset = 0.8f;       // how far away from the player will the fireball spawn 
            Vector2 spawnPos = (Vector2) player.transform.position + direction.normalized * offset;
            GameObject fireball = Object.Instantiate(fireballObj, spawnPos, Quaternion.identity) as GameObject;
            Fireball fireballScript = fireball.GetComponent<Fireball>();
            fireballScript.Start(direction, (PlayerGeneral) player);

            // testing - spawn this 99 more times
            /*
            for (int i = 1; i < 100; i++) {
                offset += 0.001f * i;
                spawnPos = (Vector2) player.transform.position + direction.normalized * offset;
                fireball = Object.Instantiate(fireballObj, spawnPos, Quaternion.identity) as GameObject;
                fireballScript = fireball.GetComponent<Fireball>();
                fireballScript.Start(direction, (PlayerGeneral) player);
            }
            */
        }
    }
}
