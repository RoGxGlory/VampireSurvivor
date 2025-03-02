using System.Collections;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType { SpeedBoost, DamageAmplifier, CooldownReduction, Shield }
    public PowerUpType type;
    public float duration = 10f; // Default duration

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Stats playerStats = other.GetComponent<Stats>();
            if (playerStats != null)
            {
                playerStats.ApplyPowerUp(type, duration);
                Destroy(gameObject);
            }
        }
    }
}