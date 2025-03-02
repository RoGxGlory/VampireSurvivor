using UnityEngine;

public class TowerBullet : MonoBehaviour
{
    public float speed = 8f;
    public int damage = 10;
    private Transform target;

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject); // Destroy bullet if the target is gone
            return;
        }

        // Move toward the target
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // Check if the bullet has reached the target
        if (Vector3.Distance(transform.position, target.position) < 0.1f) HitTarget();
    }

    public void SetTarget(Transform enemyTarget)
    {
        target = enemyTarget;
    }

    private void HitTarget()
    {
        var enemy = target.GetComponent<TDEnemy>();
        if (enemy != null) enemy.TakeDamage(damage);

        Destroy(gameObject); // Destroy the bullet after impact
    }
}