using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using AdaptiveWizard.Assets.Scripts.Enemies.General.Interfaces;
using AdaptiveWizard.Assets.Scripts.Other.GameManagers;
using AdaptiveWizard.Assets.Scripts.Player.Other;


/*
Web spawns after a certain frame of WalkingEyeball's ranged attack animation plays out. It is spawned next to the Eyeball,
based on whichever way it is oriented.
Web starts expanding after is is spawned. When it is fully expanded, it starts rotating in one direction. The rotation speed
linearly decreases until it hits 0.
Web stops and starts fading away after it hits the player or a wall. If it hits the player, it also deals damage.
*/
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

        private Vector2 direction;
        private const float damage = 15f;
        private const float speed = 6.5f;
        private float rotateSpeed;

        
        void Start() {
            int rand = UnityEngine.Random.Range(150, 200);
            this.rotateSpeed = rand % 2 == 0 ? rand : -rand;
            Debug.Log($"Rotate speed: {rotateSpeed}");
            CreateStates(initialCollider, finalCollider);
            EnterState(expandingState);
        }

        void FixedUpdate() {
            if (MainGameManager.IsGameActive()) {
                UpdateState();
            }
        }

        private void CreateStates(BoxCollider2D initialCollider, BoxCollider2D finalCollider) {
            this.expandingState = new ExpandingState(this, initialCollider, direction, speed, rotateSpeed);
            this.flyingState = new FlyingState(this, finalCollider, direction, speed, rotateSpeed);
            this.fadingAwayState = new FadingAwayState(this);
        }

        private void UpdateState() {
            int returnCode = curState.StateUpdate();
            if (returnCode != 0) {
                if (curState is ExpandingState) {
                    GetComponent<Animator>().enabled = false;       // Stop playing animation
                    ProcessExpandingStateReturnCode(returnCode);
                } else if (curState is FlyingState) {
                    ProcessFlyingStateReturnCode(returnCode);
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

        public void SetDirection(Vector2 direction) {
            this.direction = direction;
        }
    }
}