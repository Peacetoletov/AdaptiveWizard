using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObjectAfterAnimation : StateMachineBehaviour
{
    /*
    Notes for myself when working with animations:
    This script needs to be attached to a single state in the animator. When the state's animation finishes playing,
    the object is destroyed. Why the is function called OnStateEnter but runs when the state finishes, is beyond me.
    For this to work properly, I also apparently need to uncheck a box in Assets->Animations->[ANIMATION_NAME].anim->Loop Time
    If the box remains checked, it would play the first animation frame just before the object gets destroyed.
    */

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        Destroy(animator.gameObject, stateInfo.length);
    }
}
