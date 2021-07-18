using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionManager : AbstractSpellManager
{
    private GameObject explosionObj;
    private const float manaCost = 20f;

    
    public ExplosionManager() : base(manaCost) {
        this.explosionObj = Resources.Load("Prefabs/explosionPrefab") as GameObject;
    }

    public override void CastSpell(AbstractPlayer player) {
        GameObject newExplosion = Object.Instantiate(explosionObj, player.transform.position, Quaternion.identity) as GameObject;
    }
}
