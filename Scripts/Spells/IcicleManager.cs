using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcicleManager : AbstractSpellManager
{
    private GameObject icicleObj;

    
    public IcicleManager() : base(10f) {
        this.icicleObj = Resources.Load("Prefabs/iciclePrefab") as GameObject;
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
        if (y == -1) {
            if (x == -1) {
                return 45f;
            }
            if (x == 0) {
                return 90f;
            }
            // x == 1
            return 135f;
        }
        if (y == 0) {
            if (x == -1) {
                return 0f;
            }
            // x == 1
            return 180;
        }
        // y == 1
        if (x == -1) {
            return 315f;
        }
        if (x == 0) {
            return 270f;
        }
        // x == 1
        return 225f;
    }
}
