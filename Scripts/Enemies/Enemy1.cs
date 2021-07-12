using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : AbstractEnemy
{
    private float speed = 2.0f;

    // Start is called before the first frame update
    private void Start() {
        base.Start(30f);
    }

    private void FixedUpdate() {
        if (TestRoomManager.IsGameActive()) {
            MoveTowardsPlayer();
        }
    }

    private void MoveTowardsPlayer() {
        transform.position += (Vector3) DirectionToPlayer().normalized * speed * Time.deltaTime; 
    }
}
