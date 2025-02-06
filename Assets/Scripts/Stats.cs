using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    #region Stats
    public int level = 1;
    public int attackAmplify = 0;
    public int manaGainAmplify = 0;
    public int evasion = 5;
    public float moveSpeed = 100f;
    public float XP = 0f;
    public float levelUpXP = 100f; // Starting max health value
    public float initialMaxHealth = 100f; 
    public float maxHealth = 100f; // Maximum health
    public float magicDurationModifier = 0f;
    public float chainCountModifier = 0f;
    public float rangeModifier = 0f;
    public float sizeModifier = 0f;
    public float critRate = 5f;
    public float critStrikeModifier = 100f;
    public float cooldownModifier = 0f;
    private float healthRegen = 0.1f;
    private float currentHealth;
    private float chanceModifier = 0f;
    #endregion

    #region Power Ups
    private Coroutine speedBoostCoroutine;
    private Coroutine damageBoostCoroutine;
    private Coroutine cooldownReductionCoroutine;
    private Coroutine shieldCoroutine;

    public bool hasShield = false; // Temporary shield status

    private GameObject player;

    public void ApplyPowerUp(PowerUp.PowerUpType type, float duration)
    {
        switch (type)
        {
            case PowerUp.PowerUpType.SpeedBoost:
                if (speedBoostCoroutine != null) StopCoroutine(speedBoostCoroutine);
                speedBoostCoroutine = StartCoroutine(ApplySpeedBoost(duration));
                break;

            case PowerUp.PowerUpType.DamageAmplifier:
                if (damageBoostCoroutine != null) StopCoroutine(damageBoostCoroutine);
                damageBoostCoroutine = StartCoroutine(ApplyDamageBoost(duration));
                break;

            case PowerUp.PowerUpType.CooldownReduction:
                if (cooldownReductionCoroutine != null) StopCoroutine(cooldownReductionCoroutine);
                cooldownReductionCoroutine = StartCoroutine(ApplyCooldownReduction(duration));
                break;

            case PowerUp.PowerUpType.Shield:
                if (shieldCoroutine != null) StopCoroutine(shieldCoroutine);
                shieldCoroutine = StartCoroutine(ApplyShield(duration));
                break;
        }

        GameStateManager.Instance.UpdateStatText();
    }

    private IEnumerator ApplySpeedBoost(float duration)
    {
        float originalSpeed = moveSpeed;
        moveSpeed *= 1.5f; // Increase speed by 50%
        yield return new WaitForSeconds(duration);
        moveSpeed = originalSpeed;
    }

    private IEnumerator ApplyDamageBoost(float duration)
    {
        int originalDamage = attackAmplify;

        attackAmplify = (int)((float)attackAmplify * 1.5f); // Increase damage by 50%
        yield return new WaitForSeconds(duration);
        attackAmplify = originalDamage;
        
    }

    private IEnumerator ApplyCooldownReduction(float duration)
    {
        float originalCooldown = cooldownModifier;
        cooldownModifier *= 1.3f; // Reduce cooldowns by 30%
        yield return new WaitForSeconds(duration);
        cooldownModifier = originalCooldown;
    }

    private IEnumerator ApplyShield(float duration)
    {
        hasShield = true;
        player.gameObject.GetComponent<SpriteRenderer>().color = new Color(1,0,0);
        yield return new WaitForSeconds(duration);
        hasShield = false;
        player.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
    }
    #endregion

    #region Boss Wave Logic
    public bool bIsBossWaveEnemy = false; // Flag for boss wave enemy logic
    #endregion

    #region References
    // Ref to the game manager
    public GameStateManager gameManager;

    // Ref to the level ui UI
    public LevelUpUI levelUpUI;
    #endregion

    void Start()
    {
        currentHealth = maxHealth; // Initialize health
        gameManager = FindFirstObjectByType<GameStateManager>();
        player = FindFirstObjectByType<Player>(FindObjectsInactive.Include).gameObject;
    }

    public void UpdateStats(List<string> statsToUp, List<int> values)
    {
        if (statsToUp.Count != values.Count)
        {
            Debug.LogError("Mismatch between statsToUp and values lists.");
            return;
        }

        for (int i = 0; i < statsToUp.Count; i++)
        {
            string stat = statsToUp[i];
            int value = values[i];

            switch (stat)
            {
                case "attackAmplify":
                    attackAmplify += value;
                    break;

                case "maxHealthModifier":
                    initialMaxHealth += (initialMaxHealth * (1+(value/100))); 
                    break;

                case "manaGainAmplify":
                    manaGainAmplify += value;
                    break;

                case "evasion":
                    evasion += value;
                    break;

                case "healthRegen":
                    AddHealthRegen(value * 0.01f); // Convert to proper scaling
                    break;

                case "magicDurationModifier":
                    magicDurationModifier += value;
                    break;

                case "critRate":
                    critRate += value;
                    break;

                case "moveSpeed":
                    moveSpeed += value;
                    break;

                case "cooldownModifier":
                    cooldownModifier += value;
                    break;

                case "rangeModifier":
                    rangeModifier += value;
                    break;

                case "sizeModifier":
                    sizeModifier += value;
                    break;

                case "chanceModifier":
                    chanceModifier += value;
                    break;

                default:
                    Debug.LogWarning($"Stat '{stat}' is not recognized.");
                    break;
            }
        }
    }

    public void ResetStats(Stats playerStats)
    {
        moveSpeed = playerStats.moveSpeed;
        XP = playerStats.XP;
        level = playerStats.level;
        levelUpXP = playerStats.levelUpXP;
        manaGainAmplify = playerStats.manaGainAmplify;
        maxHealth = playerStats.initialMaxHealth;
        currentHealth = maxHealth;
        evasion = playerStats.evasion;
        healthRegen = playerStats.healthRegen;
        magicDurationModifier = playerStats.magicDurationModifier;
        chainCountModifier = playerStats.chainCountModifier;
        rangeModifier = playerStats.rangeModifier;
        sizeModifier = playerStats.sizeModifier;
        critRate = playerStats.critRate;
        critStrikeModifier = playerStats.critStrikeModifier;
        cooldownModifier = playerStats.cooldownModifier;

        Debug.Log("Size is : " + playerStats.sizeModifier + "\nMana Gain is : " + playerStats.manaGainAmplify);
        Debug.Log("Health Regen is : " + playerStats.healthRegen);
    }

    public void LevelUp()
    {
        XP = XP - levelUpXP;
        level++;
        levelUpXP = levelUpXP * 1.25f;
        levelUpUI.ShowLevelUpUI();
        Debug.Log("Leveled UP ! The player is now level : " + level);
    }

    public void CheckForLevelUp()
    {
        if (XP >= levelUpXP)
        {
            LevelUp();
        }
    }

    public void GainXP(float amount)
    {
        XP += amount;
    }
    public void TakeDamage(float damage)
    {
        if (gameObject.CompareTag("Enemy"))
        {
            currentHealth -= damage * (1 + (attackAmplify / 100)); // Reduce health by damage value increased by the attack amplify
            // Debug.Log($"{gameObject.name} took {damage} damage. Remaining health: {currentHealth}");
        }
        else if (gameObject.CompareTag("Player"))
        {
            if(hasShield == false)
            currentHealth -= damage; // Reduce health by damage value
            // Debug.Log($"{gameObject.name} took {damage} damage. Remaining health: {currentHealth}");
        }


        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Debug.Log($"{gameObject.name} has died.");
        // Example: Destroy the GameObject or trigger death animations/effects
        if (gameObject.CompareTag("Player"))
        {
            gameManager.GameOver();
        }
        else
        {
            if (bIsBossWaveEnemy)
            {
                if (gameManager != null)
                {
                    gameManager.OnBossEnemyKilled();
                }
            }
            Destroy(gameObject);
        }
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount; // Increase health
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Clamp health to max value
        // Debug.Log($"{gameObject.name} healed {healAmount}. Current health: {currentHealth}");
    }

    public float GetCurrentHealth()
    {
        return currentHealth; // Return current health value
    }

    public float GetMaxHealth() { return maxHealth; }

    public float GetAttackAmp() { return attackAmplify; }

    public float GetManaGain() { return manaGainAmplify; }

    public float GetCurrentRegen() { return healthRegen; }

    public float GetEvasion() { return evasion; }

    public float GetCritRate() { return critRate; } // critRate

    public float GetCritStrike() { return critStrikeModifier; } // critStrikeModifier

    public float GetDurationModifier() { return magicDurationModifier; }

    public float GetRangeModifier() { return rangeModifier; }

    public float GetSpeedModifier() { return moveSpeed; }

    public float GetCountModifier() { return chainCountModifier; }

    public float GetCooldownModifier() { return cooldownModifier; }

    public float GetSizeModifier() { return sizeModifier; }

    public void AddHealthRegen(float amount)
    {
        healthRegen += amount;
    }

    public float GetChanceModifier() { return chanceModifier; }

    public void AddChanceModifier(float amount)
    {
        chanceModifier += amount;
    }

}