using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using AdaptiveWizard.Assets.Scripts.Enemies.AbstractClasses;


namespace AdaptiveWizard.Assets.Scripts.Enemies.Enemies.WalkingEyeball
{
    public class RangedAttackPositionFinder : AbstractRangedAttackPositionFinder
    {
        

        protected override bool CanHit(Vector2Int position, Vector2 projectileBoundingBoxSize, float projectileMaxTravelDistance) {
            // TODO
            //return position.y <= 5;

            return CanHitFromDirection(new Vector2(-1, 0), position, projectileBoundingBoxSize, projectileMaxTravelDistance) ||
                    CanHitFromDirection(new Vector2(1, 0), position, projectileBoundingBoxSize, projectileMaxTravelDistance);
        }
    }
}