using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    public List<GameObject> powerUpPrefabs; // List of different power-up prefabs
    public float spawnIntervalMin = 90f; // Min time between spawns
    public float spawnIntervalMax = 180f; // Max time between spawns
    public int maxPowerUpsOnMap = 3; // Limits the number of active power-ups
    public float spawnRadius = 30f; // Radius around the player to spawn power-ups

    private List<GameObject> activePowerUps = new List<GameObject>();
    private Transform player; // Reference to the player

    void Start()
    {
        player = FindFirstObjectByType<Player>(FindObjectsInactive.Include).gameObject.transform; // Make sure your player has the "Player" tag
        if (player == null)
        {
            Debug.LogError("Player not found! Please assign the Player tag.");
        }
        StartCoroutine(SpawnPowerUps());
    }

    private IEnumerator SpawnPowerUps()
    {
        while (true)
        {

                yield return new WaitForSeconds(Random.Range(spawnIntervalMin, spawnIntervalMax));

                // Ensure we do not exceed the max power-ups allowed on the map
                activePowerUps.RemoveAll(p => p == null);
                if (activePowerUps.Count >= maxPowerUpsOnMap) continue;

                // Spawn a random power-up around the player
                Vector2 spawnPosition = GetRandomSpawnPositionAroundPlayer();
                GameObject powerUpPrefab = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Count)];

            if (GameStateManager.Instance.bIsPlaying)
            {
                GameObject powerUpInstance = Instantiate(powerUpPrefab, spawnPosition, Quaternion.identity);
                activePowerUps.Add(powerUpInstance);
            }
        }
    }

    private Vector2 GetRandomSpawnPositionAroundPlayer()
    {
        // Random angle and distance within the spawn radius
        float angle = Random.Range(0f, 360f);
        float distance = Random.Range(5f, spawnRadius); // Minimum distance of 5 to avoid spawning too close

        Vector2 offset = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * distance;
        return (Vector2)player.position + offset;
    }

    private void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(player.position, spawnRadius);
        }
    }
}