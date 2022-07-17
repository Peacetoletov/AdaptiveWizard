using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AdaptiveWizard.Assets.Scripts.Items.Passive.Interfaces
{
    public interface PlayerHealthModifier
    {
        float ModifyHealth(float health);
    }
}
