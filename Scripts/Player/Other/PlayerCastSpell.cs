using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdaptiveWizard.Assets.Scripts.Spells.SpellManagers;
using AdaptiveWizard.Assets.Scripts.Other.GameManagers;


namespace AdaptiveWizard.Assets.Scripts.Player.Other
{
    public class PlayerCastSpell : AbstractPlayer
    {
        private PlayerGeneral playerGeneral;
        private AbstractSpellManager[] spellManagers;

        private void Start() {
            this.playerGeneral = gameObject.GetComponent<PlayerGeneral>();
            Reset();
        }

        private void Update() {
            if (MainGameManager.IsGameActive()) {
                if (Input.GetMouseButtonDown(0)) {      // 0 = left click
                    // I have only 1 basic spell so far. In future, I will first need to check what spell the player is holding, then casting it.
                    spellManagers[0].TryToCast(playerGeneral);
                }

                if (Input.GetMouseButtonDown(1)) {      // 1 = right click
                    spellManagers[1].TryToCast(playerGeneral);
                }

                if (Input.GetKeyDown(KeyCode.Q)) {
                    spellManagers[2].TryToCast(playerGeneral);
                }

                if (Input.GetKeyDown(KeyCode.E)) {
                    spellManagers[3].TryToCast(playerGeneral);
                }
            }
        }

        public override void Reset() {
            this.spellManagers = new AbstractSpellManager[4];
            this.spellManagers[0] = gameObject.AddComponent(typeof(FireballManager)) as AbstractSpellManager;
            this.spellManagers[1] = gameObject.AddComponent(typeof(IcicleManager)) as AbstractSpellManager;
            this.spellManagers[2] = gameObject.AddComponent(typeof(CannonballManager)) as AbstractSpellManager;
            this.spellManagers[3] = gameObject.AddComponent(typeof(ExplosionManager)) as AbstractSpellManager;
            InitManagers();
        }

        private void InitManagers() {
            foreach (AbstractSpellManager manager in spellManagers) {
                manager.Init();
            }
        }

        public AbstractSpellManager GetSpellManager(int index) {
            return spellManagers[index];
        }
    }
}
