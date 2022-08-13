using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


namespace AdaptiveWizard.Assets.Scripts.Other.Other
{
    public class FixedTimer : AbstractTimer
    {
        public FixedTimer(float basePeriod, float periodVariance=0f, float initialDelay=0f) : 
                base(basePeriod, periodVariance, initialDelay) {}


        protected override float GetDeltaTime() {
            return Time.fixedDeltaTime;
        }
    }
}