using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractSpellManager 
{
    private float manaCost;


    public AbstractSpellManager(float manaCost) {
        this.manaCost = manaCost;
    }

    public void TryToCast(PlayerGeneral playerGeneral) {
        if (playerGeneral.CheckAndSpendMana(manaCost)) {
            CastSpell(playerGeneral);
        }
    }

    public abstract void CastSpell(AbstractPlayer player);
}
