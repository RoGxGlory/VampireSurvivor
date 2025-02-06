using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class XPBar : MonoBehaviour
{
    [SerializeField] private Image xpFillImage; // The UI image for the XP bar (fill)
    [SerializeField] private TextMeshProUGUI levelText; // Text to display the current level

    private Stats playerStats; // Reference to the player's Stats script

    void Start()
    {
        // Find the player's Stats component
        playerStats = FindFirstObjectByType<Player>().GetComponent<Stats>();
        if (playerStats == null)
        {
            Debug.LogError("Stats component not found on the player.");
            return;
        }

        // Initialize the XP bar
        UpdateXPBar();
    }

    void Update()
    {
        // Continuously update the XP bar
        UpdateXPBar();
    }

    private void UpdateXPBar()
    {
        if (playerStats == null) return;

        // Update the fill amount and level text
        float fillAmount = (float)playerStats.XP / playerStats.levelUpXP;
        xpFillImage.fillAmount = Mathf.Clamp01(fillAmount);
        levelText.text = "Level: " + playerStats.level;
    }
}