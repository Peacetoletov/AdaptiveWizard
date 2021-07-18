using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGeneral : AbstractPlayer
{
    private BoxCollider2D boxCollider;
    private float maxHealth;
    private float curHealth;
    private bool meleeInvulnerability;
    private Timer meleeInvulnerabilityTimer;
    private float maxMana;
    private float curMana;

    private void Start() {
        if (!IsInitialized()) {
            Initialize();
        }
    }

    private void Update() {
        if (TestRoomManager.IsGameActive()) {
            if (meleeInvulnerability && meleeInvulnerabilityTimer.UpdateAndCheck()) {
                this.meleeInvulnerability = false;
            }
        }
    }

    public void CheckCollisionWithEnemies() {
        if (!IsInitialized()) {
            // this function can be called from other scripts (before Start happens), therefore I need to check this here
            Initialize();
        }
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, Vector2.zero, 0, LayerMask.GetMask("Enemy"));
        if (hit.collider != null) {
            if (!meleeInvulnerability) {
                float meleeDamage = 10.0f;      // currently constant damage for all enemy types; might change later
                TakeDamage(meleeDamage);
                this.meleeInvulnerability = true;
            }
        }
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

    private bool IsInitialized() {
        return !(boxCollider == null);
    }

    private void Initialize() {
        Reset();
    }

    public override void Reset() {
        this.boxCollider = GetComponent<BoxCollider2D>();
        this.maxHealth = 100000f;
        this.curHealth = maxHealth;
        this.meleeInvulnerability = false;
        float invulnerabilitySeconds = 0.5f;
        this.meleeInvulnerabilityTimer = new Timer(invulnerabilitySeconds);
        this.maxMana = 100000f;
        this.curMana = maxMana;
    }

    public float GetCurHealth() {
        return curHealth;
    }

    public float GetCurMana() {
        return curMana;
    }

    public void AddMana(float amount) {
        this.curMana = Mathf.Min(curMana + amount, maxMana);    
    }

    public bool CheckAndSpendMana(float amount) {
        // returns false if player doesn't have enough mana
        if (amount > curMana) {
            return false;
        }
        this.curMana -= amount;
        return true;
    }
}
