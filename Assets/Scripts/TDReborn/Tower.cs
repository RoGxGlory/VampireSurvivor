using UnityEngine;

public class Tower : MonoBehaviour
{
    public int cost = 50; // Cost of the tower
    public float range = 5f; // Attack range
    public float fireRate = 1f; // Shots per second
    public GameObject bulletPrefab;

    public bool canFire;
    [SerializeField] private GameObject topPart;

    private float nextFireTime;
    private Transform targetEnemy;

    private void Update()
    {
        if (canFire)
        {
            FindTarget();
            if (targetEnemy != null)
            {
                RotateTopPart(); // Rotate turret to face the target
                if (Time.time >= nextFireTime)
                {
                    FireAtEnemy();
                    nextFireTime = Time.time + 1f / fireRate;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range); // Shows attack range in the editor
    }

    private void FindTarget()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, range); // Detects 2D Enemies

        var closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var col in colliders)
            if (col.CompareTag("Enemy")) // Ensure enemies have the "Enemy" tag
            {
                var distance = Vector3.Distance(transform.position, col.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = col.transform;
                }
            }

        targetEnemy = closestEnemy;
    }

    private void FireAtEnemy()
    {
        if (targetEnemy == null) return;
        Shoot(targetEnemy);
    }

    private void Shoot(Transform target)
    {
        var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, 0)); // Fix bullet rotation
        var bulletScript = bullet.GetComponent<TowerBullet>();
        if (bulletScript != null) bulletScript.SetTarget(target);
    }

    private void RotateTopPart()
    {
        if (targetEnemy == null || topPart == null) return;

        // Get direction to target
        var direction = targetEnemy.position - topPart.transform.position;

        // Lock X and Z, only affect Y
        direction.y = 0f;

        // Ensure there's an actual direction
        if (direction == Vector3.zero) return;

        // Compute target Y rotation
        var targetRotation = Quaternion.LookRotation(direction);

        // Extract ONLY the Y rotation while preserving original X and Z
        var currentEuler = topPart.transform.eulerAngles;
        topPart.transform.rotation = Quaternion.Euler(1, 2, 3);
    }
}