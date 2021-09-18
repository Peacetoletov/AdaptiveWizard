using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Items;

namespace Items {
    public interface MaxManaModifier
    {
        float ModifyMana(float health);
    }
}
