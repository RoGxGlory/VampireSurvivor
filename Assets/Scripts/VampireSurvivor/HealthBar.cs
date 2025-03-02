using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Stats Stats; // Reference to entity's stats
    public Image healthBarImage; // Reference to the health bar fill image
    public GameObject healthBarBG; // Reference to the background of the health bar
    public Canvas healthBarCanvas; // Reference to the canvas

    public GameObject player;

    private Transform Transform; // Reference to the entity's transform
    private Transform healthBarSocket; // Reference to the health bar socket

    [SerializeField] private bool isOnEnemy = false;

    private void Start()
    {
        player = FindFirstObjectByType<Player>(FindObjectsInactive.Include).gameObject;
        Stats = player.GetComponent<Stats>();
        if (isOnEnemy == false)
        {
            // Find the player and set references
            if (player != null)
            {
                player = FindFirstObjectByType<Player>(FindObjectsInactive.Include).gameObject;
                Transform = player.transform;
                Stats = player.GetComponent<Stats>();
                healthBarSocket = player.transform.Find("HealthBarSocket");

                if (Stats == null)
                {
                    Debug.LogError("PlayerStats component not found on the player object.");
                }
            }
            else
            {
                Debug.LogError("Player object not found in the scene! Make sure it has the 'Player' tag.");
            }
        }
        else
        {
            // Find the enemy and set references
            GameObject enemy = gameObject.transform.parent.parent.gameObject;
            if (enemy != null)
            {
                Transform = enemy.transform;
                Stats = enemy.GetComponent<Stats>();
                healthBarSocket = enemy.transform.GetChild(0).GetChild(0);
                if (healthBarSocket != null)
                {
                    Debug.Log(healthBarSocket.gameObject.name);
                }
                healthBarBG.transform.position = Transform.position;
                healthBarImage.transform.position = Transform.position;
                healthBarCanvas.worldCamera = Camera.main;

                if (Stats == null)
                {
                    Debug.LogError("EnemyStats component not found on the game object.");
                }
            }
            else
            {
                Debug.LogError("Enemy object not found in the scene!");
            }
        }

    }

    private void Update()
    {
        if (Transform != null)
        {
            // Anchor the health bar to the player's health bar socket
            if (!isOnEnemy)
            {
                transform.position = Camera.main.WorldToScreenPoint(healthBarSocket.transform.position); // Offset below the player
            }
            else
            {
                healthBarBG.transform.position = Transform.position - new Vector3(0, 3.5f, 0);
                healthBarImage.transform.position = Transform.position - new Vector3(0, 3.5f, 0);
            }
        }

        if (Stats != null && healthBarImage != null)
        {
            // Update the health bar fill based on health
            healthBarImage.fillAmount = Mathf.Clamp01((float)Stats.GetCurrentHealth() / (float)Stats.maxHealth);
        }
    }

    public void ResetBar()
    {
        healthBarImage.fillAmount = 1;
    }
}
