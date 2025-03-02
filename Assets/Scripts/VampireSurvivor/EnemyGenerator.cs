using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawn
{
    public GameObject enemyPrefab; // Enemy prefab to spawn
    [Range(0, 100)] public float spawnChance; // Spawn chance in percentage
}

public class EnemyGenerator : MonoBehaviour
{
    public List<EnemySpawn> enemySpawns; // List of enemies with spawn chances
    public GameObject bossPrefab; // Boss enemy prefab
    public Rect spawnArea; // Rectangular area for spawning enemies
    public float spawnInterval = 2f; // Time interval between spawns
    public int maxEnemies = 20; // Maximum number of enemies allowed at a time
    public LayerMask enemyLayerMask; // Layer mask for enemies to prevent overlap
    public float enemyCollisionRadius = 1f; // Radius used to check for overlap

    private List<GameObject> spawnedEnemies = new List<GameObject>(); // List of currently spawned enemies
    private bool bIsBossWave = false; // Tracks if it's currently a boss wave

    // REF to the Game State Manager
    public GameStateManager stateManager;

    private Transform playerTransform; // Reference to the player's transform

    void Start()
    {
        stateManager = FindFirstObjectByType<GameStateManager>();
        playerTransform = FindFirstObjectByType<Player>(FindObjectsInactive.Include).transform; // Get player reference
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        Debug.Log("SpawnEnemies() Coroutine Started.");

        while (true)
        {
            spawnedEnemies.RemoveAll(enemy => enemy == null);

            if (!bIsBossWave && spawnedEnemies.Count < maxEnemies)
            {
                SpawnEnemy(false);
            }
            else
            {
                Debug.Log("Skipping spawn due to Boss Wave or Max Enemy Limit.");
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy(bool isBossWave)
    {
        if (stateManager.CurrentState == GameState.InGame)
        {
            if (isBossWave)
            {
                SpawnBoss(1);
            }
            else
            {
                SpawnNormalEnemy();
            }
        }
    }

    private void SpawnNormalEnemy()
    {
        if (stateManager.CurrentState == GameState.InGame)
        {
            if (enemySpawns.Count == 0) return;

            Vector2 spawnPosition;
            int attempts = 0;
            const int maxAttempts = 100;

            do
            {
                spawnPosition = new Vector2(
                    Random.Range(spawnArea.xMin, spawnArea.xMax),
                    Random.Range(spawnArea.yMin, spawnArea.yMax)
                );

                attempts++;

                // Ensure the spawn position is at least 10 units away from the player
                if (playerTransform != null && Vector2.Distance(spawnPosition, playerTransform.position) < 10f)
                {
                    continue;
                }

                if (attempts >= maxAttempts)
                {
                    Debug.LogWarning("Could not find a non-overlapping position for the enemy after many attempts.");
                    return;
                }

            } while (Physics2D.OverlapCircle(spawnPosition, enemyCollisionRadius, enemyLayerMask));

            GameObject selectedEnemy = SelectEnemyBasedOnChance();
            if (selectedEnemy != null)
            {
                GameObject enemyInstance = Instantiate(selectedEnemy, spawnPosition, Quaternion.identity);
                spawnedEnemies.Add(enemyInstance);
                if (stateManager.bIsBossWave)
                {
                    enemyInstance.GetComponent<Stats>().bIsBossWaveEnemy = true;
                }
                enemyInstance.GetComponent<Enemy>()?.StartCoroutine(RemoveEnemyOnDestroy(enemyInstance));
            }
        }
    }

    private GameObject SelectEnemyBasedOnChance()
    {
        float totalChance = 0f;
        foreach (var enemySpawn in enemySpawns)
        {
            totalChance += enemySpawn.spawnChance;
        }

        float randomValue = Random.Range(0f, totalChance);
        float cumulativeChance = 0f;

        foreach (var enemySpawn in enemySpawns)
        {
            cumulativeChance += enemySpawn.spawnChance;
            if (randomValue <= cumulativeChance)
            {
                return enemySpawn.enemyPrefab;
            }
        }

        return null;
    }

    private void SpawnBoss(int n)
    {
        for (int i = 0; i < n; i++)
        {
            Vector2 spawnPosition;
            int attempts = 0;
            const int maxAttempts = 100;

            do
            {
                spawnPosition = new Vector2(
                    Random.Range(spawnArea.xMin, spawnArea.xMax),
                    Random.Range(spawnArea.yMin, spawnArea.yMax)
                );

                attempts++;

                // Ensure the boss spawns at least 10 units away from the player
                if (playerTransform != null && Vector2.Distance(spawnPosition, playerTransform.position) < 10f)
                {
                    continue;
                }

                if (attempts >= maxAttempts)
                {
                    Debug.LogWarning("Could not find a suitable boss spawn position after many attempts.");
                    return;
                }

            } while (Physics2D.OverlapCircle(spawnPosition, enemyCollisionRadius, enemyLayerMask));

            GameObject bossInstance = Instantiate(bossPrefab, spawnPosition, Quaternion.identity);
            if (stateManager.bIsBossWave)
            {
                bossInstance.GetComponent<Stats>().bIsBossWaveEnemy = true;
            }
            spawnedEnemies.Add(bossInstance);
        }
    }

    public void SpawnBossWaveEnemies(int count)
    {
        bIsBossWave = true;
        SpawnBoss(1);
        StartCoroutine(SpawnBossEnemies(count));
    }

    private IEnumerator SpawnBossEnemies(int count)
    {
        int spawnedCount = 0;

        while (stateManager.bIsBossWave && stateManager.slider.value > 0)
        {
            if (GameObject.FindGameObjectsWithTag("Enemy").Length < maxEnemies)
            {
                SpawnEnemy(false);
                spawnedCount++;
            }

            yield return new WaitForSeconds(spawnInterval / 1.5f);
        }

        yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("Enemy").Length == 0);
        stateManager.EndBossWave();
    }

    private IEnumerator RemoveEnemyOnDestroy(GameObject enemy)
    {
        yield return new WaitUntil(() => enemy == null);
        spawnedEnemies.Remove(enemy);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(spawnArea.center, new Vector3(spawnArea.width, spawnArea.height, 0));
    }

    public void SetBossWave(bool state)
    {
        bIsBossWave = state;
    }

    public void EnableStart()
    {
        Debug.Log("Resuming enemy spawning...");
        StopAllCoroutines();
        bIsBossWave = false;
        StartCoroutine(SpawnEnemies());
    }

    public void StopCoroutine()
    {
        StopAllCoroutines();
    }
}