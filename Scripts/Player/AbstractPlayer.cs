using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractPlayer : MonoBehaviour
{
    public void ResetPlayer() {
        gameObject.GetComponent<PlayerGeneral>().Reset();
        gameObject.GetComponent<PlayerMovement>().Reset();
        gameObject.GetComponent<PlayerCastSpell>().Reset();
    }

    public abstract void Reset();
}
