using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Items;

namespace Items {
    public interface GoldModifier
    {
        float GoldMultiplier(float gold);
    }
}
