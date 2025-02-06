using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Orbs
{
    public GameObject orbPrefab; // The orb prefab
    public float spawnChance; // The spawn chance (in percentage)
}

public class Enemy : MonoBehaviour
{
    public string enemyName = "Enemy"; // Name of the enemy
    public float moveSpeed = 3f; // Speed at which the enemy moves toward the player
    public int damage = 10; // Damage dealt by the enemy

    public bool bIsLargeEnemy = false;
    public bool bIsBossEnemy = false;
    public GameObject deathEffectPrefab; // Assign this in the Inspector

    public event Action OnEnemyKilled;

    public ChestSpawner chestSpawner;

    public GameStateManager gameManager;

    private Transform playerTransform; // Reference to the player's transform

    public List<Orbs> orbs = new List<Orbs>(); // List of orb prefabs with spawn chances

    private Time time;

    public AudioClip deathSound;
    public float soundVolume = 1f;

    void Start()
    {
        // Gets a reference to the game state manager
        gameManager = FindFirstObjectByType<GameStateManager>();

        // Gets a reference to the chest spawner
        chestSpawner = FindFirstObjectByType<ChestSpawner>();

        // Find the player GameObject in the scene
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player object not found in the scene! Make sure it has the 'Player' tag.");
        }
    }

    void Update()
    {
        if (playerTransform != null && gameManager.CurrentState == GameState.InGame)
        {
            // Move toward the player
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }

    private void SpawnDeathEffect()
    {
        if (deathEffectPrefab != null)
        {
            GameObject effect = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            if (bIsLargeEnemy)
            {
                effect.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
            }
            if (bIsBossEnemy)
            {
                effect.transform.localScale = new Vector3(5, 5, 5);
            }
            Destroy(effect, 2f); // Destroy effect after 2 seconds to avoid clutter

        }
        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position, soundVolume);
        }
    }

    private void OnDestroy()
    {
        OnEnemyKilled?.Invoke();

        chestSpawner.OnEnemyKilled();

        if (orbs.Count == 0)
        {
            Debug.LogWarning("No orbs assigned to the EnemyOrbSpawner.");
            return;
        }

        float randomValue = UnityEngine.Random.Range(0f, 100f);
        float cumulativeChance = 0f;

        foreach (Orbs orb in orbs)
        {
            cumulativeChance += orb.spawnChance;

            if (randomValue <= cumulativeChance)
            {
                // Calculate spawn position
                Vector3 spawnPosition = transform.position;

                // Instantiate the selected orb prefab
                if (orb.orbPrefab != null)
                {
                    Instantiate(orb.orbPrefab, spawnPosition, Quaternion.identity);
                    SpawnDeathEffect();
                }
                else
                {
                    Debug.LogWarning("Orb prefab is not assigned for one of the entries in the orbs list.");
                }
                break;
            }
        }
    }
}
