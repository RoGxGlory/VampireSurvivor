using UnityEngine;

public class AOEAttack : MonoBehaviour
{
    public float areaOfEffect = 5f; // Radius of the AoE effect
    public float damage = 10f; // Damage dealt by the AoE
    public float duration = 2f; // Lifetime of the AoE attack

    private void Start()
    {
        // Automatically destroy the AoE object after its duration
        Destroy(gameObject, duration);

        // Apply damage immediately upon creation
        ApplyAoEDamage();
    }

    private void ApplyAoEDamage()
    {
        // Find all colliders within the area of effect
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, areaOfEffect);

        foreach (var hit in hits)
        {
            // Ignore self or non-enemy objects
            if (hit.gameObject != gameObject && hit.CompareTag("Enemy"))
            {
                // Apply damage to the enemy
                var enemyStats = hit.GetComponent<Stats>();
                if (enemyStats != null)
                {
                    enemyStats.TakeDamage(damage);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // Debug.Log($"Lava Zone hit: {collision.gameObject.name}");
            // Apply damage to the enemy
            Stats enemyHealth = collision.GetComponent<Stats>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // Debug.Log($"Lava Zone hit: {collision.gameObject.name}");
            // Apply damage to the enemy
            Stats enemyHealth = collision.GetComponent<Stats>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the AoE radius in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, areaOfEffect);
    }
}
