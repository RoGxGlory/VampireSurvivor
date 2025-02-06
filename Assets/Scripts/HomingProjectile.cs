using UnityEngine;

public class HomingProjectile : MonoBehaviour
{
    public float speed = 5f; // Speed of the projectile
    public float rotationSpeed = 200f; // Rotation speed to face the target
    private Transform target; // Target to home in on
    private float damage; // Damage dealt by the projectile

    public void Initialize(Transform targetTransform, float damageAmount)
    {
        if (targetTransform == null)
        {
            Debug.LogError("HomingProjectile: No valid target provided! Destroying projectile.");
            Destroy(gameObject);
            return;
        }

        target = targetTransform;
        damage = damageAmount;
    }

    void Update()
    {
        if (target == null)
        {
            Debug.LogWarning("HomingProjectile: Target lost! Destroying projectile.");
            Destroy(gameObject); // Destroy the projectile if the target is null
            return;
        }

        // Calculate direction to the target
        Vector2 direction = (target.position - transform.position).normalized;

        // Rotate smoothly toward the target
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Move the projectile forward
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // Apply damage to the enemy
            Stats enemyHealth = collision.GetComponent<Stats>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }

            // Destroy the projectile after hitting the enemy
            Destroy(gameObject);
        }
    }
}