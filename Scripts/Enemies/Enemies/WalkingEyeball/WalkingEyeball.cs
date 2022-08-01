using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using AdaptiveWizard.Assets.Scripts.Enemies.Interfaces;
using AdaptiveWizard.Assets.Scripts.Other.GameManagers;
using AdaptiveWizard.Assets.Scripts.Enemies.AbstractClasses;


/* TODO: redesing when Idle state occurs. It should be a function of the distance between the enemy and the player (the longer
   the distance, the higher chance of entering the Idle state).
   Additionally, if the distance is short, enemy should never enter Idle state. If enemy is in Idle state and the distance
   becomes short, it enters Walk state.
*/
// TODO: possibly add more properties to abstract classes / add more interfaces, to have unified implementations of enemies
// TODO: possibly remove MovingEnemy as the superclass and instead have it tied to the WalkState
namespace AdaptiveWizard.Assets.Scripts.Enemies.Enemies.WalkingEyeball
{
    public class WalkingEyeball : AbstractEnemy
    {
        private IState curState;
        private IdleState idleState;
        private WalkState walkState;
        private AttackSlashState attackSlashState;
        private AttackThrowState attackThrowState;
        private DeathState deathState;


        protected void Start() {
            base.Init(100f);
            CreateStates(terrainCollider);
            EnterState(idleState);
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

        private void CreateStates(BoxCollider2D terrainCollider) {
            this.idleState = new IdleState(this);
            this.walkState = new WalkState(this, terrainCollider);
            this.deathState = new DeathState(this);
            this.attackSlashState = new AttackSlashState(this);
            this.attackThrowState = new AttackThrowState(this);
        }

        private void UpdateState() {
            int returnCode = curState.Update();
            if (returnCode != 0) {
                curState.OnLeave();
                
                //testing
                EnterState(walkState);
                
                // TEMPORARILY COMMENTED OUT
                /*
                if (curState is IdleState) {
                    EnterState(walkState);
                }
                else if (curState is WalkState) {
                    //EnterState(idleState);
                    EnterState(attackSlashState);
                }
                else if (curState is DeathState) {
                    Destroy(gameObject);
                    //Debug.Log("Destroyed enemy");
                } else if (curState is AttackSlashState) {
                    EnterState(walkState);
                }
                */
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