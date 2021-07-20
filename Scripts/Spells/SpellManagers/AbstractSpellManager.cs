using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractSpellManager : MonoBehaviour
{
    private float manaCost;
    private bool isOnCooldown;
    private Timer cooldownTimer;
    

    protected void Init(float manaCost, float cooldown) {
        this.manaCost = manaCost;
        this.cooldownTimer = new Timer(cooldown);
    }

    public abstract void Init();

    private void Update() {
        if (TestRoomManager.IsGameActive() && isOnCooldown && cooldownTimer.UpdateAndCheck()) {
            this.isOnCooldown = false;
        }
    }

    public void TryToCast(PlayerGeneral playerGeneral) {
        // Order is important - cooldown must be checked first, otherwise mana could be spent with no effect.
        if (!isOnCooldown && playerGeneral.CheckAndSpendMana(manaCost)) {
            CastSpell(playerGeneral);
            this.isOnCooldown = true;
        }
    }

    public abstract void CastSpell(AbstractPlayer player);

    public Timer GetTimer() {
        // this method is necessary for displaying remaining cooldown in UI
        return cooldownTimer;
    }
}
