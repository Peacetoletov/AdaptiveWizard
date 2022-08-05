using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using AdaptiveWizard.Assets.Scripts.Enemies.General.Interfaces;
using AdaptiveWizard.Assets.Scripts.Other.GameManagers;
using AdaptiveWizard.Assets.Scripts.Enemies.General.AbstractClasses;


/* TODO: redesing when Idle state occurs. It should be a function of the distance between the enemy and the player (the longer
   the distance, the higher chance of entering the Idle state).
   Additionally, if the distance is short, enemy should never enter Idle state. If enemy is in Idle state and the distance
   becomes short, it enters Walk state.
*/
// TODO: possibly add more properties to abstract classes / add more interfaces, to have unified implementations of enemies
// TODO: possibly remove MovingEnemy as the superclass and instead have it tied to the WalkState
// TODO: rename AttackSlashState to MeleeAttackState, rename AttackThrowState to RangedAttackState
namespace AdaptiveWizard.Assets.Scripts.Enemies.Enemies.WalkingEyeball.WalkingEyeball
{
    public class WalkingEyeball : AbstractEnemy
    {
        private IState curState;
        private IdleState idleState;
        private WalkState walkState;
        private AttackSlashState attackSlashState;
        private AttackThrowState attackThrowState;
        private DeathState deathState;

        public BoxCollider2D projectileCollider;
        public GameObject webObj;


        protected void Start() {
            base.Init(100f);
            CreateStates(projectileCollider);
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

        private void CreateStates(BoxCollider2D projectileCollider) {
            this.idleState = new IdleState(this);
            this.walkState = new WalkState(this, projectileCollider);
            this.deathState = new DeathState(this);
            this.attackSlashState = new AttackSlashState(this);
            this.attackThrowState = new AttackThrowState(this);
        }

        private void UpdateState() {
            int returnCode = curState.Update();

            /*
            // testing
            if (GetID() == 0 && Input.GetKeyDown(KeyCode.V)) {
            //if (Input.GetKeyDown(KeyCode.V)) {
                EnterState(walkState);
            }
            */

            if (returnCode != 0) {
                if (curState is IdleState) {
                    EnterState(walkState);
                }
                else if (curState is WalkState) {
                    ProcessWalkStateReturnCode(returnCode);
                }
                else if (curState is DeathState) {
                    Destroy(gameObject);
                    //Debug.Log("Destroyed enemy");
                } else if (curState is AttackSlashState) {
                    EnterState(walkState);
                }
            }
        }

        private int EnterState(IState state) {
            this.curState = state;
            return state.OnEnter();
        }

        private void ProcessWalkStateReturnCode(int code) {
            Assert.IsTrue(code != 0);
            if (code == 1) {
                EnterState(attackSlashState);
            } else {
                EnterState(attackThrowState);
            }
        }

        public WalkState GetWalkState() {
            return walkState;
        }

        public void InstantiateWeb(Vector2 position) {
            Instantiate(webObj, position, Quaternion.identity);
        }
    }
}