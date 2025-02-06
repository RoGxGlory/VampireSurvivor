using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestSpawner : MonoBehaviour
{
    public GameObject chestPrefab; // Chest prefab
    public Rect spawnArea; // Defines the area where chests can spawn
    public float spawnInterval = 300f; // Time before next chest spawns
    public int enemyKillRequirement = 250; // Enemies needed before next chest spawns
    private int enemiesKilled = 0;
    private GameObject activeChest;
    private Player player;

    void Start()
    {
        StartCoroutine(ChestSpawnTimer());
        player = FindFirstObjectByType<Player>();
    }

    private void Update()
    {
        spawnArea = new Rect(player.transform.position.x - 20, player.transform.position.y - 20, player.transform.position.x + 20, player.transform.position.y + 20);
    }

    private IEnumerator ChestSpawnTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (enemiesKilled >= enemyKillRequirement)
            {
                Vector2 spawnPosition = GetRandomSpawnPosition();
                activeChest = Instantiate(chestPrefab, spawnPosition, Quaternion.identity);
                enemiesKilled = 0; // Reset kill counter
                Debug.Log("Chest Spawned !");
            }
        }
    }

    private Vector2 GetRandomSpawnPosition()
    {
        return new Vector2(
            Random.Range(spawnArea.xMin, spawnArea.xMax),
            Random.Range(spawnArea.yMin, spawnArea.yMax)
        );
    }

    public void OnEnemyKilled()
    {
        enemiesKilled++;
    }
}