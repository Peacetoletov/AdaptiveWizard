using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using AdaptiveWizard.Assets.Scripts.Enemies.General.AbstractClasses;


namespace AdaptiveWizard.Assets.Scripts.Enemies.Enemies.WalkingEyeball.WalkingEyeball
{
    public class RangedAttackPositionFinder : AbstractRangedAttackPositionFinder
    {

        public override bool CanHit(Vector2 position, Vector2 projectileBoundingBoxSize, float projectileMaxTravelDistance) {
            
            return CanHitFromDirection(new Vector2(-1, 0), position, projectileBoundingBoxSize, projectileMaxTravelDistance) ||
                    CanHitFromDirection(new Vector2(1, 0), position, projectileBoundingBoxSize, projectileMaxTravelDistance);
        }
    }
}