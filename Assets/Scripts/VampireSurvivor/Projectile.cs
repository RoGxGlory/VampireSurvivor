using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 direction;
    private float damage;
    public float speed = 10f; // Speed of the projectile

    public void SetDirection(Vector3 dir, float dmg)
    {
        direction = dir.normalized;
        damage = dmg;
    }

    void Update()
    {
        // Move the projectile in the set direction
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // Debug.Log($"Projectile hit: {collision.gameObject.name}");
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
