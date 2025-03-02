using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDEnemySpawner : MonoBehaviour
{
    [Header("Wave Settings")] public List<WaveConfig> waves; // List of waves, editable in Unity Inspector

    public int maxWaves = 25; // Can be changed per difficulty
    public float waveInterval = 5f; // Time between waves

    [Header("Difficulty Scaling")]
    public float spawnRateScaler = 0.9f; // Reduces spawn time per wave (higher values = slower reduction)

    public float enemyCountScaler = 1.2f; // Increases enemy count per wave (1.2 = 20% increase)

    [Header("Final Wave")] public WaveConfig finalWave; // Custom final boss wave setup

    [Header("Waypoint System")] public WaypointManager waypointManager; // Reference to waypoint system

    private int currentWave;
    private bool isSpawning;
    public static TDEnemySpawner Instance { get; private set; } // Singleton instance

    private void Awake()
    {
        // Ensure only one instance of TDEnemySpawner exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple TDEnemySpawner instances found! Destroying extra.");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (waypointManager == null)
        {
            Debug.LogError("No WaypointManager assigned to TDEnemySpawner!");
            return;
        }

        StartCoroutine(SpawnWaves());
    }

    private IEnumerator SpawnWaves()
    {
        while (currentWave < maxWaves)
            if (!isSpawning)
            {
                isSpawning = true;
                yield return StartCoroutine(SpawnWave(currentWave));
                isSpawning = false;
                currentWave++;

                yield return new WaitForSeconds(waveInterval);
            }

        // Final Boss Wave
        if (currentWave == maxWaves)
        {
            Debug.Log("Starting Final Wave!");
            yield return StartCoroutine(SpawnWave(maxWaves, finalWave));
        }
    }

    private IEnumerator SpawnWave(int waveIndex, WaveConfig overrideWave = null)
    {
        var wave = overrideWave ??
                   waves
                       [Mathf.Min(waveIndex, waves.Count - 1)]; // Get wave config, fallback to last wave if index exceeds
        var enemyAmount = Mathf.RoundToInt(wave.enemyCount * Mathf.Pow(enemyCountScaler, waveIndex));
        var spawnDelay = wave.spawnRate * Mathf.Pow(spawnRateScaler, waveIndex);

        Debug.Log($"Starting {wave.waveName}: {enemyAmount} enemies with {spawnDelay}s spawn rate.");

        for (var i = 0; i < enemyAmount; i++)
        {
            SpawnEnemy(wave);
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void SpawnEnemy(WaveConfig wave)
    {
        if (waypointManager == null || waypointManager.waypoints.Count == 0)
        {
            Debug.LogError("No valid waypoints! Make sure to set up WaypointManager.");
            return;
        }

        // Choose a random enemy prefab from the wave list
        var enemyPrefab = wave.enemyPrefabs[Random.Range(0, wave.enemyPrefabs.Count)];
        var spawnPoint = waypointManager.GetWaypoint(0);

        var enemyInstance = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

        var enemyScript = enemyInstance.GetComponent<TDEnemy>();
        if (enemyScript != null) enemyScript.SetWaypoints(waypointManager.waypoints);
    }

    public int GetCurrentWave()
    {
        return currentWave + 1; // Since waves start from 0 internally
    }
}