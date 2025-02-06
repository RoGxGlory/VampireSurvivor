using System.Collections;
using System.IO.Compression;
using Unity.VisualScripting;
using UnityEngine;

public class Chest : MonoBehaviour
{

    // REF to the chest animator
    public Animator animator;
    // REF to the Game State Manager
    public GameStateManager stateManager;

    private Camera mainCamera;
    public GameObject chestIndicatorPrefab; // UI Arrow Indicator
    private GameObject chestIndicator; // Instantiated Arrow Indicator
    private GameObject chestIndicatorInstance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stateManager = FindFirstObjectByType<GameStateManager>();
        mainCamera = Camera.main;
        
        // chestIndicatorPrefab = GameObject.Find("ChestIndicator").transform.GetChild(0).transform.GetChild(0).gameObject;

        chestIndicatorInstance = Instantiate(chestIndicatorPrefab, stateManager.Player.transform);

        chestIndicator = chestIndicatorInstance.transform.GetChild(0).transform.GetChild(0).gameObject;

        if (chestIndicator != null)
        {
            chestIndicator.SetActive(true); // Hide indicator at start
        }
    }

    private void Update()
    {
        if (chestIndicator != null)
        {
            Vector3 screenPosition = mainCamera.WorldToViewportPoint(transform.position);
            bool isOffScreen = screenPosition.x < 0 || screenPosition.x > 1 || screenPosition.y < 0 || screenPosition.y > 1;

            if (isOffScreen)
            {
                chestIndicator.SetActive(true);
                UpdateIndicatorPosition();
            }
            else
            {
                chestIndicator.SetActive(false);
            }
        }
    }

    private void UpdateIndicatorPosition()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Vector3 chestScreenPosition = mainCamera.WorldToScreenPoint(transform.position);
        Vector3 direction = (chestScreenPosition - screenCenter).normalized;

        float maxDistance = Screen.width * 0.4f; // Keep the indicator within screen bounds
        Vector3 indicatorPosition = screenCenter + direction * maxDistance;

        chestIndicator.transform.position = indicatorPosition;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        chestIndicator.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void OpenChest()
    {
        animator.SetBool("open", true);
        if (chestIndicator != null)
        {
            chestIndicator.SetActive(false);
        }
        stateManager.chestUI.ShowChestUI();
        this.gameObject.SetActive(false);
    }
}


