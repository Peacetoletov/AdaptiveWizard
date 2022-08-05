using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using AdaptiveWizard.Assets.Scripts.Enemies.General.Interfaces;
using AdaptiveWizard.Assets.Scripts.Other.GameManagers;
using AdaptiveWizard.Assets.Scripts.Player.Other;


namespace AdaptiveWizard.Assets.Scripts.Enemies.Enemies.WalkingEyeball.Web
{
    public class Web : MonoBehaviour
    {
        public BoxCollider2D initialCollider;
        public BoxCollider2D finalCollider;

        private IState curState;
        private ExpandingState expandingState;
        private FlyingState flyingState;
        private FadingAwayState fadingAwayState;

        private const float damage = 15f;
        private const float speed = 5f;

        
        void Start() {
            CreateStates(initialCollider, finalCollider);
            EnterState(expandingState);
        }

        void FixedUpdate() {
            if (MainGameManager.IsGameActive()) {
                UpdateState();
            }
        }

        private void CreateStates(BoxCollider2D initialCollider, BoxCollider2D finalCollider) {
            this.expandingState = new ExpandingState(this, initialCollider, speed);
            this.flyingState = new FlyingState(this, finalCollider, speed);
            this.fadingAwayState = new FadingAwayState(this);
        }

        private void UpdateState() {
            int returnCode = curState.Update();
            if (returnCode != 0) {
                if (curState is ExpandingState) {
                    GetComponent<Animator>().enabled = false;       // Stop playing animation
                    ProcessExpandingStateReturnCode(returnCode);
                } else if (curState is FlyingState) {
                    EnterState(fadingAwayState);
                } else if (curState is FadingAwayState) {
                    Destroy(gameObject);
                }
            }
        }

        private int EnterState(IState state) {
            this.curState = state;
            return state.OnEnter();
        }

        private void ProcessExpandingStateReturnCode(int code) {
            Assert.IsTrue(code != 0);
            switch (code) {
                case 1:
                    // Animation finished playing
                    EnterState(flyingState);
                    break;
                case 2:
                    // Player was hit
                    MainGameManager.GetPlayer().GetComponent<PlayerGeneral>().TakeDamage(damage);
                    EnterState(fadingAwayState);
                    break;
                case 3:
                    // Wall was hit
                    EnterState(fadingAwayState);
                    break;
            }
        }

        private void ProcessFlyingStateReturnCode(int code) {
            Assert.IsTrue(code != 0);
            if (code == 1) {
                // Player was hit
                MainGameManager.GetPlayer().GetComponent<PlayerGeneral>().TakeDamage(damage);
            }
            // Wall or player was hit
            EnterState(fadingAwayState);
        }
    }
}