using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LightningAttack : MonoBehaviour
{
    public float chainCount = 3; // Number of enemies to chain to
    public float damage = 10f; // Damage dealt by the lightning
    public float chainRange = 6f; // Maximum range for chaining lightning
    public GameObject lightningEffectPrefab; // Prefab for visual lightning effect

    private List<GameObject> chainedEnemies = new List<GameObject>(); // Track enemies hit by the lightning

    public void Initialize(Vector3 startPosition, float damage, int chainCount, float rangeModifier)
    {
        this.damage = damage;
        this.chainCount = chainCount;
        this.chainRange = rangeModifier;

        // Start the lightning chain
        StartCoroutine(ChainLightning(startPosition));
    }

    private IEnumerator ChainLightning(Vector3 startPosition)
    {
        Vector3 currentPosition = startPosition;

        for (int i = 0; i < chainCount; i++)
        {
            // Find the closest enemy within range that hasn't been hit yet
            GameObject closestEnemy = FindClosestEnemy(currentPosition);

            if (closestEnemy == null) break; // Stop if no more valid targets are found

            // Apply damage to the enemy
            var enemyStats = closestEnemy.GetComponent<Stats>();
            if (enemyStats != null)
            {
                enemyStats.TakeDamage(damage);
            }

            // Create a lightning effect
            if (lightningEffectPrefab != null)
            {
                GameObject lightningEffect = Instantiate(lightningEffectPrefab);
                LineRenderer lr = lightningEffect.GetComponent<LineRenderer>();
                if (lr != null)
                {
                    lr.SetPosition(0, currentPosition);
                    lr.SetPosition(1, closestEnemy.transform.position);
                }

                Destroy(lightningEffect, 0.5f); // Destroy the effect after a short time
            }

            // Update the current position and add the enemy to the chain list
            currentPosition = closestEnemy.transform.position;
            chainedEnemies.Add(closestEnemy);

            yield return new WaitForSeconds(0.1f); // Small delay between chain jumps
        }

        Destroy(gameObject); // Destroy the lightning attack object after chaining is complete
    }

    private GameObject FindClosestEnemy(Vector3 position)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            if (chainedEnemies.Contains(enemy)) continue; // Skip already chained enemies

            float distance = Vector3.Distance(position, enemy.transform.position);
            if (distance < shortestDistance && distance <= chainRange) // Use the updated range
            {
                shortestDistance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // Apply damage to the enemy
            Stats enemyStats = collision.GetComponent<Stats>();
            if (enemyStats != null)
            {
                enemyStats.TakeDamage(damage);
            }

            // Optional: Add to chained enemies or destroy the object if the chain is complete
            chainedEnemies.Add(collision.gameObject);
        }
    }
}
