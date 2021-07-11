using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGeneral : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    private float maxHealth;
    private float curHealth;
    private bool meleeInvulnerability = false;

    private void Start() {
        if (!IsInitialized()) {
            Initialize();
        }
    }

    private bool IsInitialized() {
        return !(boxCollider == null);
    }

    private void Initialize() {
        this.boxCollider = GetComponent<BoxCollider2D>();
        this.maxHealth = 100f;
        this.curHealth = maxHealth;
    }

    public void CheckCollisionWithEnemies() {
        if (!IsInitialized()) {
            // this function can be called from other scripts (before Start happens), therefore I need to check this here
            Initialize();
        }
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, Vector2.zero, 0, LayerMask.GetMask("Enemy"));
        if (hit.collider != null) {
            float meleeDamage = 10.0f;      // currently constant damage for all enemy types; might change later
            float invulnerabilitySeconds = 0.5f;
            if (!meleeInvulnerability) {
                StartCoroutine(SetMeleeInvulnerability(invulnerabilitySeconds));
                TakeDamage(meleeDamage);
            }
        }
    }

    private IEnumerator SetMeleeInvulnerability(float seconds) {
        this.meleeInvulnerability = true;
        yield return new WaitForSeconds(seconds);
        this.meleeInvulnerability = false;
    }

    public void TakeDamage(float damage) {
        this.curHealth -= damage;
        // print("Took " + damage + " damage. Current health: " + curHealth);
        CheckDeath();
    }

    private void CheckDeath() {
        if (curHealth <= 0) {
            print("You died!");
            RestartLevel();
        }
    }

    private void RestartLevel() {
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("GameController");
        if (objectsWithTag.Length != 1) {
            string message = "ERROR! Incorrect number of \"GameController\" objects found when attempting to spawn an enemy. " +
                            "Number of GameController objects found: " + objectsWithTag.Length;
            throw new System.InvalidOperationException(message);
        }
        TestRoomManager managerScript = objectsWithTag[0].GetComponent<TestRoomManager>();
        managerScript.RestartLevel();
    }

    public float GetCurHealth() {
        return curHealth;
    }
}
