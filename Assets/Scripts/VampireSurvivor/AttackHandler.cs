using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class AttackHandler : MonoBehaviour
{
    [System.Serializable]
    public class Attack
    {
        public string name; // Name of the attack
        public bool isEnabled; // Whether the attack is enabled
        public bool allowWithoutTarget; // Allow the attack even if no target is found
        public float attackCooldown; // How frequently the attack can be used
        public float damage; // Damage dealt by the attack
        public float areaOfEffect; // Area of effect radius
        public float baseDuration = 2f; // Base duration of the attack effect
        public float attackLevel = 1f; // Level of the attack
        public float range; // How far can the attack be cast / chained
        public float chainCount; // Number of chains the attack can do
        public float attackCount = 1f; // Number of times the attack get shot per cast
        public float size = 1f; // Size of AOE attacks
        public GameObject attackPrefab; // Prefab for visual or physical representation of the attack
    }

    private Stats playerStats;

    public List<Attack> attacks = new List<Attack>(); // List of attacks available to the entity
    public List<AttackSO> attackSOs = new List<AttackSO>(); // List of attacks scriptable objects
    public MyAttacksSO myAttacksSO;

    private Dictionary<string, float> attackCooldowns = new Dictionary<string, float>(); // Track attack cooldowns

    // REF to the Game State Manager
    public GameStateManager stateManager;

    private float lightningDamageMultiplier = 1f;
    private float fireDamageMultiplier = 1f;

    private void Start()
    {
        playerStats = FindFirstObjectByType<Player>(FindObjectsInactive.Include)?.GetComponent<Stats>();
        if (playerStats == null) Debug.LogError("playerStats is NULL");

        stateManager = FindFirstObjectByType<GameStateManager>();
        if (stateManager == null) Debug.LogError("stateManager is NULL");

        foreach (AttackSO attackSO in attackSOs)
        {
            if (attackSO == null)
                Debug.LogError("AttackSO is NULL");
            if (attackSO.attack == null)
                Debug.LogError("Attack in AttackSO is NULL");
        }

        if (stateManager != null && playerStats != null)
        {
            foreach (AttackSO attackSO in attackSOs)
            {
                var CopiedAttack = new Attack
                {
                    name = attackSO.attack.name,
                    isEnabled = attackSO.attack.isEnabled,
                    allowWithoutTarget = attackSO.attack.allowWithoutTarget,
                    attackCooldown = attackSO.attack.attackCooldown,
                    damage = attackSO.attack.damage,
                    areaOfEffect = attackSO.attack.areaOfEffect,
                    baseDuration = attackSO.attack.baseDuration,
                    attackLevel = attackSO.attack.attackLevel,
                    attackPrefab = attackSO.attack.attackPrefab,
                    range = attackSO.attack.range,
                    chainCount = attackSO.attack.chainCount,
                    size = attackSO.attack.size,

                };
                if (myAttacksSO != null)
                    myAttacksSO.myAttacks.Add(CopiedAttack);
                else
                    Debug.LogError("AttacksSO not found !");
            }
        }
        else
        {
            playerStats = FindFirstObjectByType<Player>(FindObjectsInactive.Include).GetComponent<Stats>();
            stateManager = FindFirstObjectByType<GameStateManager>();
        }        
    }

    void Update()
    {
        if (stateManager.CurrentState == GameState.InGame && stateManager.bIsPlaying)
        {
            // Handle all attacks that are enabled
            foreach (var attack in myAttacksSO.myAttacks)
            {
                if (attack.isEnabled && CanAttack(attack.name))
                {
                    PerformAttack(attack);
                    attackCooldowns[attack.name] = Time.time + (attack.attackCooldown / (1 + (playerStats.cooldownModifier/100))); // Set cooldown
                }
            }
        }

    }

    private bool CanAttack(string attackName)
    {
        if (!attackCooldowns.ContainsKey(attackName)) return true;
        return Time.time >= attackCooldowns[attackName];
    }

    private void PerformAttack(Attack attack)
    {
        switch (attack.name)
        {
            case "Magic Arrow":
                ExecuteBasicAttack(attack);
                break;

            case "Lava Zone":
                ExecuteAoEAttack(attack);
                break;

            case "Withering Spirit":
                ExecuteHomingAttack(attack);
                break;

            case "Lightning Attack":
                ExecuteLightningAttack(attack);
                break;

            default:
                Debug.LogWarning($"Attack logic for {attack.name} not implemented.");
                break;
        }
    }

    private void ExecuteBasicAttack(Attack attack)
    {
        // Example: Perform a straight projectile attack
        GameObject attackInstance = Instantiate(attack.attackPrefab, transform.position, Quaternion.identity);
        attackInstance.transform.localScale *= attack.size * ( 1 + (playerStats.sizeModifier/ 100));

        GameObject closestEnemy = FindClosestEnemy();
        if (closestEnemy != null)
        {
            Vector3 direction = (closestEnemy.transform.position - transform.position).normalized;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            attackInstance.transform.rotation = Quaternion.Euler(0, 0, angle);

            Projectile projectile = attackInstance.AddComponent<Projectile>();
            projectile.SetDirection(direction, attack.damage);
        }

        Destroy(attackInstance, 5f);
    }

    private void ExecuteAoEAttack(Attack attack)
    {
        if (attack.attackPrefab != null)
        {
            // Instantiate the AoE prefab at the player's position
            GameObject attackInstance = Instantiate(attack.attackPrefab, transform.position, Quaternion.identity);

            // Apply size modifier
            attackInstance.transform.localScale *= attack.size * (1 + (playerStats.sizeModifier / 100));

            // Adjust duration based on player stats
            float adjustedDuration = attack.baseDuration * (1 + playerStats.magicDurationModifier / 100f);

            // Configure the AoE script
            AOEAttack aoeAttack = attackInstance.GetComponent<AOEAttack>();
            if (aoeAttack != null)
            {
                aoeAttack.areaOfEffect = attack.areaOfEffect;
                aoeAttack.damage = attack.damage * fireDamageMultiplier * (1 + (playerStats.attackAmplify / 100));
                aoeAttack.duration = adjustedDuration;
            }
        }

        // Debug.Log("Executed AoE Attack");
    }

    private void ExecuteHomingAttack(Attack attack)
    {
        // Spawn a homing projectile
        GameObject attackInstance = Instantiate(attack.attackPrefab, transform.position, Quaternion.identity);
        attackInstance.transform.localScale *= attack.size * (1 + (playerStats.sizeModifier / 100));

        GameObject closestEnemy = FindClosestEnemy();
        if (closestEnemy != null)
        {
            HomingProjectile homingProjectile = attackInstance.GetComponent<HomingProjectile>();
            if (homingProjectile != null)
            {
                homingProjectile.Initialize(closestEnemy.transform, attack.damage * (1 + (playerStats.attackAmplify / 100)));
                Debug.Log($"Homing attack initialized toward {closestEnemy.name}");
            }
        }
        else
        {
            Debug.LogWarning("No target found for homing attack!");
        }

        Destroy(attackInstance, 10f);
    }

    private void ExecuteLightningAttack(Attack attack)
    {
        if (attack.attackPrefab != null)
        {
            // Instantiate the lightning attack prefab at the player's position
            GameObject attackInstance = Instantiate(attack.attackPrefab, transform.position, Quaternion.identity);
            attackInstance.transform.localScale *= attack.size * (1 + (playerStats.sizeModifier / 100));

            // Get the LightningAttack script and initialize it
            LightningAttack lightning = attackInstance.GetComponent<LightningAttack>();
            if (lightning != null)
            {
                lightning.Initialize(transform.position, attack.damage * lightningDamageMultiplier * (1+(playerStats.attackAmplify/100)), (int)attack.chainCount + Mathf.FloorToInt(playerStats.chainCountModifier), attack.range + Mathf.FloorToInt(playerStats.rangeModifier));
            }
        }

        // Debug.Log("Executed Lightning Attack");
    }

    private GameObject FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }

    public void LevelUpAttack(Attack attack, float cdModifier, float damageModifier, float AOEModifier, float durationModifier, float rangeModifier, float chainModifier, float countModifier, float sizeModifier)
    {
        attack.attackCooldown += cdModifier;
        attack.damage += damageModifier;
        attack.areaOfEffect += AOEModifier;
        attack.baseDuration += durationModifier;
        attack.range += rangeModifier;
        attack.chainCount += chainModifier;
        attack.attackCount += countModifier;
        attack.size += sizeModifier;
        attack.attackLevel++;
    }

    public void ResetAttacks()
    {
        if(myAttacksSO.myAttacks != null)
        {
            myAttacksSO.myAttacks.Clear();
            foreach (AttackSO attackSO in attackSOs)
            {
                var CopiedAttack = new Attack
                {
                    name = attackSO.attack.name,
                    isEnabled = attackSO.attack.isEnabled,
                    allowWithoutTarget = attackSO.attack.allowWithoutTarget,
                    attackCooldown = attackSO.attack.attackCooldown,
                    damage = attackSO.attack.damage,
                    areaOfEffect = attackSO.attack.areaOfEffect,
                    baseDuration = attackSO.attack.baseDuration,
                    attackLevel = attackSO.attack.attackLevel,
                    attackPrefab = attackSO.attack.attackPrefab,
                    range = attackSO.attack.range,
                    chainCount = attackSO.attack.chainCount,
                    size = attackSO.attack.size,

                };
                myAttacksSO.myAttacks.Add(CopiedAttack);
            }
        }
        else
        {
            Debug.LogError("AttacksSO.MyAttacks is null");
        }

    }

    public void SetArtifactOn(string artifact)
    {
        switch(artifact)
        {
            case "Helionis":
                fireDamageMultiplier += 1;
                break;

            case "Astrapheon":
                lightningDamageMultiplier += 1;
                break;

            case "Blood Potion":
                playerStats.maxHealth += 50;
                break;

            case "Agile Cloak":
                playerStats.evasion += 10;
                break;

            case "Gunpowder":
                playerStats.sizeModifier += 10;
                break;

            case "Ruby":
                playerStats.attackAmplify += 12;
                break;

            case "Holy Grail":
                playerStats.AddHealthRegen(0.5f);
                break;

            case "Radar":
                playerStats.critRate += 9;
                break;

            case "Bomb":
                playerStats.attackAmplify += 8;
                playerStats.sizeModifier += 8;
                break;


            default:
                Debug.LogError("No such artifact exists : " + artifact);
                break;
        }

        playerStats.gameManager.UpdateStatText();
  
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize area of effect for debugging
        foreach (var attack in attacks)
        {
            if (attack.isEnabled && attack.areaOfEffect > 0)
            {
                Gizmos.color = UnityEngine.Color.red;
                Gizmos.DrawWireSphere(transform.position, attack.areaOfEffect);
            }
        }
    }
}
