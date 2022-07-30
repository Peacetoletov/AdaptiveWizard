using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using AdaptiveWizard.Assets.Scripts.Enemies.AbstractClasses.MovingEnemy;
using AdaptiveWizard.Assets.Scripts.Enemies.Interfaces;
using AdaptiveWizard.Assets.Scripts.Other.GameManagers;


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
            CreateStates(GetComponent<Animator>());
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

        private void CreateStates(Animator animator) {
            this.idleState = new IdleState(animator);
            this.walkState = new WalkState(animator);
        }

        private void UpdateState() {
            int returnCode = curState.Update();
            if (returnCode != 0) {
                curState.OnLeave();
                if (curState is IdleState) {
                    EnterState(walkState);
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