using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonballManager : AbstractSpellManager
{
    private GameObject cannonballObj;
    private const float manaCost = 50f;
    private const float cooldown = 4f;


    public override void Init() {
        this.cannonballObj = Resources.Load("Prefabs/Other/cannonballPrefab") as GameObject;
        base.Init(manaCost, cooldown);
    }

    public override void CastSpell(AbstractPlayer player) {
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - player.transform.position;
        float offset = 1.5f;       // how far away from the player will the fireball spawn 
        Vector2 spawnPos = (Vector2) player.transform.position + direction.normalized * offset;
        GameObject cannonball = Object.Instantiate(cannonballObj, spawnPos, Quaternion.identity) as GameObject;
        Cannonball cannonballScript = cannonball.GetComponent<Cannonball>();
        cannonballScript.Start(direction);
    }
}
