using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using AdaptiveWizard.Assets.Scripts.Enemies.AbstractClasses.MovingEnemy;
using AdaptiveWizard.Assets.Scripts.Enemies.Interfaces;
using AdaptiveWizard.Assets.Scripts.Other.GameManagers;


/* TODO: redesing when Idle state occurs. It should be a function of the distance between the enemy and the player (the longer
   the distance, the higher chance of entering the Idle state).
   Additionally, if the distance is short, enemy should never enter Idle state. If enemy is in Idle state and the distance
   becomes short, it enters Walk state.
*/
// TODO: possibly add more properties to abstract classes / add more interfaces, to have unified implementations of enemies
// TODO: possibly remove MovingEnemy as the superclass and instead have it tied to the WalkState
namespace AdaptiveWizard.Assets.Scripts.Enemies.Enemies.WalkingEyeball
{
    public class WalkingEyeball : MovingEnemy
    {
        private SpriteRenderer spriteRenderer;

        private IState curState;
        private IdleState idleState;
        private WalkState walkState;

        protected void Start() {
            base.Init(100f, 2f);
            this.spriteRenderer = GetComponent<SpriteRenderer>();
            CreateStates();
            EnterState(idleState);
        }

        protected override void FixedUpdate() {
            if (MainGameManager.IsGameActive()) {
                /*
                base.FixedUpdate();
                UpdateSpriteOrientation();
                */

                // TODO: handle states
                UpdateState();
            }
        }

        public void OnTakeDamage() {
            // TODO: probably make this protected override, and move some functionality into a higher class
        }

        private void CreateStates() {
            this.idleState = new IdleState(this);
            this.walkState = new WalkState(this);
        }

        private void UpdateState() {
            int returnCode = curState.Update();
            if (returnCode != 0) {
                curState.OnLeave();
                if (curState is IdleState) {
                    EnterState(walkState);
                }
                else if (curState is WalkState) {
                    if (returnCode == 1) {
                        EnterState(idleState);
                    }
                }
            }
        }

        private int EnterState(IState state) {
            this.curState = state;
            return state.OnEnter();
        }

        private void UpdateSpriteOrientation() {
            float xDir = base.GetLastMovementVector().x;
            if (xDir > 0) {
                spriteRenderer.flipX = true;
            } else if (xDir < 0) {
                spriteRenderer.flipX = false;
            }
            // Don't change orientation if xDir == 0
        }
    }
}