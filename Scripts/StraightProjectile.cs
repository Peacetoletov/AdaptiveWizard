using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StraightProjectile : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private float damage;
    // possibly add acceleration

    protected void Init(Vector2 direction, float speed, float damage) {
        this.direction = direction;
        this.speed = speed;
        this.damage = damage;
    }

    public Vector2 GetDirection() {
        return direction;
    }

    public float GetSpeed() {
        return speed;
    }

    public float GetDamage() {
        return damage;
    }
}
