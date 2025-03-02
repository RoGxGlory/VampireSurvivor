using System.Collections.Generic;
using UnityEngine;

public class TDEnemy : MonoBehaviour
{
    public float speed = 3f;
    public int maxHealth = 50; // Enemy health
    private int currentHealth;
    
    private List<Transform> waypoints;
    private int waypointIndex;

    void Start()
    {
        currentHealth = maxHealth; // Initialize health
    }

    public void SetWaypoints(List<Transform> waypoints)
    {
        this.waypoints = waypoints;
        transform.position = waypoints[0].position;
    }

    void Update()
    {
        if (waypoints == null || waypointIndex >= waypoints.Count) return;

        Vector3 targetPosition = waypoints[waypointIndex].position;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            waypointIndex++;
        }
    }

    // **🔴 New Function: Take Damage**
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage! HP left: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // **☠️ Enemy Dies**
    void Die()
    {
        Debug.Log($"{gameObject.name} has been destroyed!");
        Destroy(gameObject); // Remove enemy from the game
    }
}