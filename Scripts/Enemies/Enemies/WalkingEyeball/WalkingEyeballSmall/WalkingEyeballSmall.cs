using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using AdaptiveWizard.Assets.Scripts.Enemies.General.Interfaces;
using AdaptiveWizard.Assets.Scripts.Other.GameManagers;
using AdaptiveWizard.Assets.Scripts.Enemies.General.AbstractClasses;


/*
KNOWN ISSUES:
- when I rapidly fire at the (big) eyeball and small eyeballs spawn and also get killed, I sometimes get a NullPointerException
  referencing the OnTakeDamage() method in AbstractEnemy. Investigate and fix ASAP.
*/
namespace AdaptiveWizard.Assets.Scripts.Enemies.Enemies.WalkingEyeball.WalkingEyeballSmall
{
    public class WalkingEyeballSmall : AbstractEnemy
    {
        private IState curState;
        private SpawnState spawnState;
        private WalkState walkState;
        private AttackState attackState;
        private DeathState deathState;


        public override void Init() {
            base.Init(75f);
            CreateStates();
            EnterState(spawnState);
        }

        public void FixedUpdate() {
            if (MainGameManager.IsGameActive()) {
                UpdateState();
            }
        }
        
        protected override void OnTakeDamage(float damage) {
            base.OnTakeDamage(damage);
            if (IsDead() && !(curState is DeathState)) {
                EnterState(deathState);
            }
        }
        

        private void CreateStates() {
            this.spawnState = new SpawnState(this);
            this.walkState = new WalkState(this);
            this.deathState = new DeathState(this);
            this.attackState = new AttackState(this);
        }

        private void UpdateState() {
            int returnCode = curState.StateUpdate();
            if (returnCode != 0) {
                if (curState is SpawnState) {
                    EnterState(walkState);
                } else if (curState is WalkState) {
                    EnterState(attackState);
                } else if (curState is AttackState) {
                    EnterState(walkState);
                } else if (curState is DeathState) {
                    Destroy(gameObject);
                }
            }
        }

        private int EnterState(IState state) {
            this.curState = state;
            return state.OnEnter();
        }

        public WalkState GetWalkState() {
            return walkState;
        }
    }
}