using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using AdaptiveWizard.Assets.Scripts.Enemies.General.AbstractClasses;


/*
This class is used to find a good position from which to shoot a ranged attack.
*/
namespace AdaptiveWizard.Assets.Scripts.Enemies.Enemies.WalkingEyeball.WalkingEyeball
{
    public class RangedAttackPositionFinder : AbstractRangedAttackPositionFinder
    {
        /*
        Valid angles (o = origin, x = valid, . = invalid):
        x...............x
        xxxx.........xxxx
        xxxxxxx...xxxxxxx
        xxxxxxxxoxxxxxxxx
        xxxxxxx...xxxxxxx
        xxxx........ xxxx
        x...............x

        Angle is valid, if it is in one of these ranges:
        1) Right arc: given by vectors (1, -0.5) and (1, 0.5).
        2) Left arc: given by vectors (-1, -0.5) and (-1, 0.5).
        Angle of both arcs is given by arcAngle.
        */
        const float arcAngle = 53f;         // manually calculated


        protected override bool IsAngleValid(float signedAngle) {
            return (Math.Abs(signedAngle) < arcAngle / 2) || (Math.Abs(signedAngle) > 180 - arcAngle / 2);
        }
    }
}