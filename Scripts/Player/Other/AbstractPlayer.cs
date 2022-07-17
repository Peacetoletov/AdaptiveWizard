using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AdaptiveWizard.Assets.Scripts.Player.Other
{
    public abstract class AbstractPlayer : MonoBehaviour
    {
        public void ResetPlayer() {
            gameObject.GetComponent<PlayerGeneral>().Reset();
            gameObject.GetComponent<PlayerMovement>().Reset();
            gameObject.GetComponent<PlayerCastSpell>().Reset();
        }

        public abstract void Reset();
    }
}
