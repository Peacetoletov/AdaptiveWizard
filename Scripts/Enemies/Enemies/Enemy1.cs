using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy1 : MovingEnemy
{
    protected void Start() {
        base.Start(30f, 3.5f);
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();
    }
}
