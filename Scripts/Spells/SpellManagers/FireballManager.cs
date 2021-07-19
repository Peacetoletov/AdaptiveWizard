using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballManager : AbstractSpellManager
{
    private GameObject fireballObj;
    private const float manaCost = 0f;
    private const float cooldown = 0.1f;        // balanced cooldown is 0.4f

    
    public override void Init() {
        this.fireballObj = Resources.Load("Prefabs/fireballPrefab") as GameObject;
        base.Init(manaCost, cooldown);
    }

    public override void CastSpell(AbstractPlayer player) {
        // player argument must be of a stricter type PlayerGeneral in this case
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - player.transform.position;
        float offset = 0.8f;       // how far away from the player will the fireball spawn 
        Vector2 spawnPos = (Vector2) player.transform.position + direction.normalized * offset;
        GameObject fireball = Object.Instantiate(fireballObj, spawnPos, Quaternion.identity) as GameObject;
        Fireball fireballScript = fireball.GetComponent<Fireball>();
        fireballScript.Start(direction, (PlayerGeneral) player);
    }
}
