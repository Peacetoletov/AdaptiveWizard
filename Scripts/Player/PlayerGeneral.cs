using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGeneral : AbstractPlayer
{
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;
    private float maxHealth;
    private float curHealth;
    private bool meleeInvulnerability;
    private Timer meleeInvulnerabilityTimer;
    private float maxMana;
    private float curMana;
    private Timer manaRegenTimer;
    

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

            if (manaRegenTimer.UpdateAndCheck()) {
                AddMana(1f);
            }

            IncreaseAlphaIfBelowOne();
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

        // temporarily decrease alpha to signal taking damage
        Color tmp = spriteRenderer.color;
        tmp.a = 0.1f;
        spriteRenderer.color = tmp;

        CheckDeath();
    }

    private void CheckDeath() {
        if (curHealth <= 0) {
            print("You died!");
            RestartLevel();
        }
    }

    private void IncreaseAlphaIfBelowOne() {
        // if player sprite's alpha is below 1, slightly increase it
        Color tmp = spriteRenderer.color;
        tmp.a = Mathf.Min(1, tmp.a + 1.4f * Time.deltaTime);
        spriteRenderer.color = tmp;
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
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        SetAlphaToOne();
        this.maxHealth = 100f;
        this.curHealth = maxHealth;
        this.meleeInvulnerability = false;
        float invulnerabilitySeconds = 0.5f;
        this.meleeInvulnerabilityTimer = new Timer(invulnerabilitySeconds);
        this.maxMana = 100f;
        this.curMana = maxMana;
        float manaRegenPerSec = 3f;
        this.manaRegenTimer = new Timer(1 / manaRegenPerSec);
    }

    private void SetAlphaToOne() {
        Color tmp = spriteRenderer.color;
        tmp.a = 1f;
        spriteRenderer.color = tmp;
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
