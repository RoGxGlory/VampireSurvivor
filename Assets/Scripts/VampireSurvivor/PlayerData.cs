using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int level;
    public int attackAmplify;
    public int manaGainAmplify;
    public int evasion;
    public float xp;
    public float levelUpXP;
    public float moveSpeed;
    public float initialMaxHealth;
    public float magicDurationModifier;
    public float chainCountModifier;
    public float rangeModifier;
    public float sizeModifier;
    public float critRate;
    public float critStrikeModifier;
    public float cooldownModifier;
    public float healthRegen;
    public float chanceModifier;

    public PlayerData(Stats stats)
    {
        level = stats.level;
        xp = stats.XP;
        levelUpXP = stats.levelUpXP;
        attackAmplify = stats.attackAmplify;
        manaGainAmplify = stats.manaGainAmplify;
        evasion = stats.evasion;
        moveSpeed = stats.moveSpeed;
        initialMaxHealth = stats.initialMaxHealth;
        magicDurationModifier = stats.magicDurationModifier;
        chainCountModifier = stats.chainCountModifier;
        rangeModifier = stats.rangeModifier;
        sizeModifier = stats.sizeModifier;
        critRate = stats.critRate;
        critStrikeModifier = stats.critStrikeModifier;
        cooldownModifier = stats.cooldownModifier;
        healthRegen = stats.GetCurrentRegen();
        chanceModifier = stats.GetChanceModifier();
    }
}