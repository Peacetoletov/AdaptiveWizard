using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    private PlayerGeneral playerGeneral;

    // moving
    private float movementSpeed = 4.5f;     // was 3.5
    private Vector2 movementDir;

    // dashing
    private Vector2 lastNonzeroMovementDir;
    private float dashSpeed = 20f;
    private float dashDuration = 0.15f;      // 20 speed and 0.15 duration works well
    private bool isDashing = false;

    private void Start() {
        this.boxCollider = GetComponent<BoxCollider2D>();
        this.playerGeneral = GetComponent<PlayerGeneral>();
        this.lastNonzeroMovementDir = new Vector2(1, 0);        // arbitrarily chosen default dashing direction
    }

    void Update() {
        this.movementDir = GetInputWASD();
        if (movementDir != Vector2.zero) {
            this.lastNonzeroMovementDir = movementDir;
        }

        if (Input.GetKeyDown("space")) {
            if (!isDashing) {
                this.isDashing = true;
                StartCoroutine(Dashing());
            }
        }
    }

    private Vector2 GetInputWASD() {
        Vector2 dir = new Vector2(0, 0);
        if (Input.GetKey(KeyCode.W)) {
            dir.y += 1;
        }
        if (Input.GetKey(KeyCode.A)) {
            dir.x += -1;
        }
        if (Input.GetKey(KeyCode.S)) {
            dir.y += -1;
        }
        if (Input.GetKey(KeyCode.D)) {
            dir.x += 1;
        }
        return dir;
    }

    private IEnumerator Dashing() {
        yield return new WaitForSeconds(dashDuration);
        this.isDashing = false;
    }

    private void FixedUpdate() {
        if (isDashing) {
            SmoothMove(lastNonzeroMovementDir, dashSpeed);
        }
        else if (this.movementDir != Vector2.zero) {
            SmoothMove(movementDir, movementSpeed);
        }
        playerGeneral.CheckCollisionWithEnemies();
    }

    private void SmoothMove(Vector2 direction, float speed, int precision=5) {
        // I might need to scale the precision number in the future, based on the logarithm of movementSpeed or dashSpeed. For now, it's constant.
        float deltaX = direction.normalized.x * speed * Time.deltaTime;
        float deltaY = direction.normalized.y * speed * Time.deltaTime;
        // delta represents signed distance

        SmoothMoveOneAxis(precision, deltaX, new Vector2(deltaX, 0));
        SmoothMoveOneAxis(precision, deltaY, new Vector2(0, deltaY));
        // print("time = " + Time.deltaTime * 1000);
    }

    private void SmoothMoveOneAxis(int precision, float delta, Vector2 axisVector) {
        // returns false if player cannot move (anymore) in this axis; true otherwise
        float HighestNonCollidingDelta = GetHighestNonCollidingDelta(precision, delta, axisVector); 

        if (Mathf.Abs(HighestNonCollidingDelta) < 0.00001) {      // == 0, floating point arithmetic
            return;
        }

        // check if player should stop here
        /*
        float remainingDelta = axisVector.x == 0 ? target.y - transform.position.y : target.x - transform.position.x;
        if (Mathf.Abs(remainingDelta) < Mathf.Abs(HighestNonCollidingDelta)) {
            HighestNonCollidingDelta = remainingDelta;
        }
        */
        if (axisVector.x == 0) {
            transform.Translate(0, HighestNonCollidingDelta, 0);
        }
        else {
            transform.Translate(HighestNonCollidingDelta, 0, 0);
        }
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
