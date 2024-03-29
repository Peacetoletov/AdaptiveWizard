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

*/

/*
FIXED ISSUES (just for my motivation):
- With enemy starting position (5, 11) and player starting position (13, 3), enemy starts rapidly wiggling instead of shooting a web.
    - fixed by correcting web spawn positions when checking viable ranged attack positions
- With enemy starting position (5, 11) and player starting position (13, 3), enemy shoots a web which then gets blocked by a wall. This should
  never happen.
    - fixed by adding a buffer to collider's size when checking if a ranged attack position is viable
- Enemy behaviour gets broken in the following scenario:
  1) Enemy starts at (5, 11), player starts at (13, 3).
  2) Enemy finds a position to shoot from, then walks towards it and starts to shoot.
  3) During the shooting animation, player goes up. It doesn't matter how far, it just needs to be enough to get out of enemy's attack range.
  4) Now the enemy starts doing weird stuff.
    - fixed by changing how time is animation time counted in RangedAttackState 
- When an enemy shoots and then wants to and can shoot again, it currently goes through a short walking animation between the shots. 
  Remove entering WalkState in this scenario.
    - fixed by writing more code
- Behaviour in melee attack is not working.
    - fixed
- Death animation doesn't play if the enemy gets killed just after changing to ranged attack state.
    - fixed by adding a transition to animator
*/

// MAYBE TODO: possibly add more properties to abstract classes / add more interfaces, to have unified implementations of enemies
namespace AdaptiveWizard.Assets.Scripts.Enemies.Enemies.WalkingEyeball.WalkingEyeball
{
    public class WalkingEyeball : AbstractEnemy
    {
        private IState curState;
        private IdleState idleState;
        private WalkState walkState;
        private MeleeAttackState meleeAttackState;
        private RangedAttackState rangedAttackState;
        private DeathState deathState;

        [SerializeField]
        private GameObject webObj;
        [SerializeField]
        private BoxCollider2D projectileCollider;
        [SerializeField]
        private GameObject walkingEyeballSmallObj;


        public override void Init() {
            base.Init(100f);
            CreateStates();
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

        private void CreateStates() {
            this.idleState = new IdleState(this);
            this.walkState = new WalkState(this, projectileCollider);
            this.deathState = new DeathState(this);
            this.meleeAttackState = new MeleeAttackState(this);
            this.rangedAttackState = new RangedAttackState(this);
        }

        private void UpdateState() {
            int returnCode = curState.StateUpdate();
            if (returnCode != 0) {
                if (curState is IdleState) {
                    ProcessIdleStateReturnCode(returnCode);
                } else if (curState is WalkState) {
                    ProcessWalkStateReturnCode(returnCode);
                } else if (curState is DeathState) {
                    Destroy(gameObject);
                } else if (curState is MeleeAttackState) {
                    EnterState(walkState);
                } else if (curState is RangedAttackState) {
                    EnterState(idleState);
                }
            }
        }

        private int EnterState(IState state) {
            this.curState = state;
            return state.OnEnter();
        }

        private void ProcessIdleStateReturnCode(int code) {
            Assert.IsTrue(code != 0);
            if (code == 1) {
                EnterState(walkState);
            } else {
                EnterState(rangedAttackState);
            }
        }

        private void ProcessWalkStateReturnCode(int code) {
            Assert.IsTrue(code != 0);
            if (code == 1) {
                EnterState(meleeAttackState);
            } else {
                EnterState(rangedAttackState);
            }
        }

        public WalkState GetWalkState() {
            return walkState;
        }

        public RangedAttackState GetRangedAttackState() {
            return rangedAttackState;
        }

        public GameObject InstantiateWeb(Vector2 position) {
            return Instantiate(webObj, position, Quaternion.identity);
        }

        public GameObject InstantiateWalkingEyeballSmall(Vector2 position) {
            GameObject walkingEyeballSmall = Instantiate(walkingEyeballSmallObj, position, Quaternion.identity);
            walkingEyeballSmall.GetComponent<AbstractEnemy>().Init();
            walkingEyeballSmall.GetComponent<AbstractEnemy>().SetCombatManager(base.GetCombatManager());
            return walkingEyeballSmall;
        }
    }
}