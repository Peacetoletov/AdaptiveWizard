using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using InventoryNS;
using Items;

// TODO: create subclasses and move some functionality there from here (PlayerHealthManager, PlayerManaManager, ...)
public class PlayerGeneral : AbstractPlayer
{
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;
    private const float baseMaxHealth = 100000f;
    private float maxHealth;
    private float curHealth;
    private bool meleeInvulnerability;
    private Timer meleeInvulnerabilityTimer;
    private const float baseMaxMana = 100000f;
    private float maxMana;
    private float curMana;
    private Timer manaRegenTimer;
    
    public void Initialize() {
        // Initialize() must be called as soon as player is created
        Reset();
    }

    private void Start() {
        UnityEngine.Assertions.Assert.IsTrue(IsInitialized());
        /*
        if (!IsInitialized()) {
            Initialize();
        }
        */
    }

    private void Update() {
        if (MainGameManager.IsGameActive()) {
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
        UnityEngine.Assertions.Assert.IsTrue(IsInitialized());
        /*
        if (!IsInitialized()) {
            // this function can be called from other scripts (before Start happens), therefore I need to check this here
            Initialize();
        }
        */
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
            RestartGame();
        }
    }

    private void IncreaseAlphaIfBelowOne() {
        // if player sprite's alpha is below 1, slightly increase it
        Color tmp = spriteRenderer.color;
        tmp.a = Mathf.Min(1, tmp.a + 1.4f * Time.deltaTime);
        spriteRenderer.color = tmp;
    }

    private void RestartGame() {
        /*
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("GameController");
        if (objectsWithTag.Length != 1) {
            string message = "ERROR! Incorrect number of \"GameController\" objects found when attempting to spawn an enemy. " +
                            "Number of GameController objects found: " + objectsWithTag.Length;
            throw new System.InvalidOperationException(message);
        }
        MainGameManager managerScript = objectsWithTag[0].GetComponent<MainGameManager>();
        */
        MainGameManager managerScript = (MainGameManager) FindObjectOfType(typeof(MainGameManager));
        managerScript.RestartGame();
    }

    private bool IsInitialized() {
        return !(boxCollider == null);
    }

    public override void Reset() {
        this.boxCollider = GetComponent<BoxCollider2D>();
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        SetAlphaToOne();
        this.maxHealth = baseMaxHealth;
        this.curHealth = maxHealth;
        this.meleeInvulnerability = false;
        float invulnerabilitySeconds = 0.5f;
        this.meleeInvulnerabilityTimer = new Timer(invulnerabilitySeconds);
        this.maxMana = baseMaxMana;
        this.curMana = maxMana;
        float manaRegenPerSec = 3f;
        this.manaRegenTimer = new Timer(1 / manaRegenPerSec);
        Inventory.Initialize();
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

    public void UpdateMaxHealth() {
        float oldMaxHealth = maxHealth;
        float newMaxHealth = baseMaxHealth;
        foreach (PlayerHealthModifier item in Inventory.passiveItemsManager.playerHealthModifiers) {
            newMaxHealth = item.ModifyHealth(newMaxHealth);
        }
        this.maxHealth = newMaxHealth;
        float maxHealthDelta = newMaxHealth - oldMaxHealth;
        if (maxHealthDelta > 0) {
            Heal(maxHealthDelta);
        }
        else if (maxHealthDelta < 0 && curHealth > maxHealth) {
            DecreaseCurHealth(maxHealth);
        }
        print("health: " + curHealth + "/" + maxHealth);
    }

    public void Heal(float amount) {
        // in distant future, add a condition to check if player is allowed to heal
        this.curHealth = Mathf.Min(curHealth + amount, maxHealth);
    }

    private void DecreaseCurHealth(float newCurHealth) {
        // This action is different from taking damage, player cannot die from decreasing health.
        // Current health is decreased when maxHealth drops below curHealth.
        UnityEngine.Assertions.Assert.IsTrue(newCurHealth > 0);
        this.curHealth = newCurHealth;
    }

    public void UpdateMaxMana() {
        float oldMaxMana = maxMana;
        float newMaxmana = baseMaxMana;
        foreach (MaxManaModifier item in Inventory.passiveItemsManager.maxManaModifiers) {
            newMaxmana = item.ModifyMana(newMaxmana);
        }
        this.maxMana = newMaxmana;
        float maxManaDelta = newMaxmana - oldMaxMana;
        if (maxManaDelta > 0) {
            AddMana(maxManaDelta);
        }
        else if (maxManaDelta < 0 && curMana > maxMana) {
            DecreaseCurMana(maxMana);
        }
        print("mana: " + curMana + "/" + maxMana);
    }

    private void DecreaseCurMana(float newCurMana) {
        // This action is different from spending mana, actions associated with spending mana do not apply here.
        // Current mana is decreased when maxMana drops below curMana.
        UnityEngine.Assertions.Assert.IsTrue(newCurMana >= 0);
        this.curMana = newCurMana;
    }

    public float GetMaxHealth() {
        return maxHealth;
    }

    public float GetMaxMana() {
        return maxMana;
    }
}
