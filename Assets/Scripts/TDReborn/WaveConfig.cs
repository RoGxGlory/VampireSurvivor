using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveConfig
{
    [Header("Wave Settings")]
    public string waveName = "Wave 1";  // Custom name in editor
    public float spawnRate = 1f;        // Time between spawns (decreases with difficulty)
    public int enemyCount = 5;          // Number of enemies in this wave
    public List<GameObject> enemyPrefabs; // Enemies that spawn in this wave
}