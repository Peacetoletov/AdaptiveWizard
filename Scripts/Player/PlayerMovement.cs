using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    private PlayerGeneral playerGeneral;
    private Vector2 start;
    private Vector2 target;
    private float speed = 3.5f;     // was 5.0
    private bool isMoving = false;

    private void Start() {
        this.boxCollider = GetComponent<BoxCollider2D>();
        this.playerGeneral = GetComponent<PlayerGeneral>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0)) {      // 0 = left click, 1 = right click
            this.start = transform.position;
            this.target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            this.isMoving = true;
        }
    }

    private void FixedUpdate() {
        if (isMoving) {
            SmoothMove(5);      // I might need to scale this number in the future, based on the logarithm of movement speed. For now, it's constant.
        }
        playerGeneral.CheckCollisionWithEnemies();
    }

    private void SmoothMove(int precision) {
        float deltaX = (target - start).normalized.x * speed * Time.deltaTime;
        float deltaY = (target - start).normalized.y * speed * Time.deltaTime;
        // delta represents signed distance

        bool canMoveOnX = SmoothMoveOneAxis(precision, deltaX, new Vector2(deltaX, 0));
        bool canMoveOnY = SmoothMoveOneAxis(precision, deltaY, new Vector2(0, deltaY));
        this.isMoving = canMoveOnX || canMoveOnY;
        // print("time = " + Time.deltaTime * 1000);
    }

    private bool SmoothMoveOneAxis(int precision, float delta, Vector2 axisVector) {
        // returns false if player cannot move (anymore) in this axis; true otherwise
        float HighestNonCollidingDelta = GetHighestNonCollidingDelta(precision, delta, axisVector);
        bool canMoveFarther = true;        

        if (Mathf.Abs(HighestNonCollidingDelta) < 0.00001) {      // == 0, floating point arithmetic
            return false;
        }

        // check if player should stop here
        if (axisVector.x != 0) {
            float remainingDelta = target.x - transform.position.x;
            if (Mathf.Abs(remainingDelta) < Mathf.Abs(HighestNonCollidingDelta)) {
                HighestNonCollidingDelta = remainingDelta;
                canMoveFarther = false;
            }
            transform.Translate(HighestNonCollidingDelta, 0, 0);
        }
        else {
            float remainingDelta = target.y - transform.position.y;
            if (Mathf.Abs(remainingDelta) < Mathf.Abs(HighestNonCollidingDelta)) {
                HighestNonCollidingDelta = remainingDelta;
                canMoveFarther = false;
            }
            transform.Translate(0, HighestNonCollidingDelta, 0);
        }
        return canMoveFarther;
    }

    private float GetHighestNonCollidingDelta(int precision, float delta, Vector2 axisVector) {
        float higherBound = delta * 2;
        float lowerBound = 0f;
        float HighestNonCollidingDelta = 0f;
        float extraBoxDistance = 0.001f;      // without this buffer, player could get stuck in a wall on rare occasions (presumably due to floating point errors)
        RaycastHit2D hit;
        for (int i = 0; i < precision; i++) {
            hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, axisVector, 
                                    Mathf.Abs((higherBound + lowerBound) / 2) + extraBoxDistance, LayerMask.GetMask("Wall"));
            if (hit.collider == null) {
                HighestNonCollidingDelta = (higherBound + lowerBound) / 2;
                lowerBound = HighestNonCollidingDelta;
                if (i == 0) {
                    break;
                }
            }
            else {
                higherBound = (higherBound + lowerBound) / 2;
            }
        }
        return HighestNonCollidingDelta;
    }
}
