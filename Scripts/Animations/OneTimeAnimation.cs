using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneTimeAnimation : AbstractAnimation
{
    private Timer animationTimer;

    protected override void Start() {
        base.Start();
        float animationLength = GetAnimator().GetCurrentAnimatorStateInfo(0).length;
        this.animationTimer = new Timer(animationLength);
        
        // animationTimer.UpdateAndCheck();
        /* ^ There's a problem that the animation jumps to the beginning for one frame, just before the object is destroyed.
        I could fix it by updating the timer one extra time, but this solution feels like a hack. Another way of solving this
        issue is by forbidding animation loops (unchecking Loop Time checkbox in the animation inspector). That might also cause
        slight timing problems but I will go with this solution, at least for now.
        */
    }

    protected override void Update() {
        base.Update();
        if (TestRoomManager.IsGameActive() && animationTimer.UpdateAndCheck()) {
            Destroy(gameObject);
            //print("destroying");
        }
    }
}
