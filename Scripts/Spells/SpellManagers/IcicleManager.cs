using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcicleManager : AbstractSpellManager
{
    private GameObject icicleObj;
    private const float manaCost = 10f;
    private const float cooldown = 0.4f;

    
    public override void Init() {
        this.icicleObj = Resources.Load("Prefabs/Other/iciclePrefab") as GameObject;
        base.Init(manaCost, cooldown);
    }

    public override void CastSpell(AbstractPlayer player) {
        // Create 8 icicles (one for each direction), each one with a slight offset and corresponding sprite and rotation
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x == 0 && y == 0) {
                    continue;
                }
                Vector2 direction = new Vector2(x, y).normalized;
                Vector2 offset = direction * 1.2f;
                Vector2 spawnPos = (Vector2) player.transform.position + offset;
                GameObject icicle = Object.Instantiate(icicleObj, spawnPos, Quaternion.identity) as GameObject;
                Icicle icicleScript = icicle.GetComponent<Icicle>();
                icicleScript.Start(direction, GetIcicleRotation(x, y));
            }
        }
    }

    private float GetIcicleRotation(int x, int y) {
        // Returns by how many degrees the sprite needs to by rotated for an icicle, position of which is given by x, y
        if (y == 0) {
            if (x == -1) {
                return 0f;
            }
            // x == 1
            return 180;
        }
        // y == -1 || y == 1 
        return 180 + (45 * (2 - x) * y);
    }
}
