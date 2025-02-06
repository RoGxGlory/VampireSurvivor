using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionHandler : MonoBehaviour
{
    private float constantPushForce = 20f; // Constant push force applied regardless of player speed

    private Rigidbody2D rb;

    // REF to the score manager
    ScoreManager scoreManager;

    // REF to the game manager
    GameStateManager gameManager;

    // REF to the player stats
    Stats playerStats;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody not found on the player object.");
        }
        scoreManager = FindFirstObjectByType<ScoreManager>();
        if (scoreManager == null)
        {
            Debug.LogError("ScoreManager not found !");
        }
        gameManager = FindFirstObjectByType<GameStateManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found !");
        }
        playerStats = GetComponent<Stats>();
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats not found !");
        }
        /*
        levelGenerator = FindObjectOfType<LevelGenerator>();
        if (levelGenerator == null)
        {
            Debug.LogError("LevelGenerator not found !");
        }
        */
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check for collision with objects tagged as "Obstacle"
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            // Debug.Log("Player hit an obstacle.");
            // Call a game manager or handle the game over logic
            GameStateManager.Instance.GameOver();
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Player got hit by an enemy");
            playerStats.TakeDamage(collision.gameObject.GetComponent<Enemy>().damage);
        }
        else
        {
            Debug.Log("Collided with a non-obstacle object.");
            // Collisions with other objects (non-obstacle) are handled naturally by Rigidbody physics
            HandleNonObstacleCollision(collision.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        // Check for collision with objects classified as "Bonuses"
        if (collider.gameObject.CompareTag("Coin"))
        {
            Debug.Log("Player took a coin.");
            scoreManager.AddScore(5);
            gameManager.UpdateCurrentScore(scoreManager.CurrentScore);
            Destroy(collider.gameObject);
        }
        if (collider.gameObject.CompareTag("ScoreMultiplier"))
        {
            Debug.Log("Player took a score multiplier.");
            scoreManager.scoreMultiplier += 0.2f;
            Destroy(collider.gameObject);
            Debug.Log(scoreManager.scoreMultiplier);
        }
        if (collider.gameObject.CompareTag("Chest"))
        {
            Debug.Log("Player opened a chest !");
            Chest chest = collider.gameObject.GetComponent<Chest>();
            chest.OpenChest();
        }
        if (collider.gameObject.CompareTag("Orb"))
        {
            // Debug.Log("Player took an XP orb.");
            //levelGenerator.UpdateSpeed();
            playerStats.GainXP(collider.gameObject.GetComponent<Orb>().amount);
            playerStats.CheckForLevelUp();
            Destroy(collider.gameObject);
            //Debug.Log(levelGenerator.moveSpeedMultiplier);
        }
    }

    private void HandleNonObstacleCollision(GameObject nonObstacle)
    {
        // Apply a strong, constant push force to the non-obstacle object
        Rigidbody objectRb = nonObstacle.GetComponent<Rigidbody>();
        if (objectRb != null)
        {
            Vector3 pushDirection = (nonObstacle.transform.position - transform.position).normalized;
            objectRb.AddForce(pushDirection * constantPushForce, ForceMode.Impulse);
        }
    }
}
