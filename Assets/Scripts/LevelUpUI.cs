using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUpUI : MonoBehaviour
{
    [System.Serializable]
    public class UpgradeChoice
    {
        public string name; // Name of the upgrade
        public string description; // Description to show in the UI
        public Sprite icon; // Icon for the upgrade (optional)
        public AttackHandler.Attack associatedAttack; // The attack this upgrade affects
        public bool isUnlockable; // Can this unlock a new attack?
        public float cdModifier; // Cooldown modifier
        public float damageModifier; // Damage modifier
        public float AOEModifier; // Area of effect modifier
        public float durationModifier; // Duration modifier
        public float rangeModifier; // Range modifier
        public float chainModifier; // Chain count modifier
        public float countModifier; // Attack count modifier
        public float sizeModifier; // Size modifier
    }

    public List<UpgradeChoice> upgradeChoices; // List of all possible upgrades
    private List<UpgradeChoice> randomChoices;
    public GameObject levelUpPanel; // The UI panel for Level Up
    public Button[] choiceButtons; // The 3 buttons to show upgrades
    public TextMeshProUGUI[] buttonTexts; // Text components for the buttons
    public TextMeshProUGUI[] upgradeNameTexts; // Text components for the buttons
    public GameObject[] upgradeIcons; // Icon components for the buttons

    private AttackHandler attackHandler; // Reference to the player's AttackHandler
    public AttackHandler.MyAttacksSO playerAttacks;// List of the player's attacks

    private HashSet<string> unlockedThisRun = new HashSet<string>(); // Track unlocked attacks in current run

    // REF to the Game State Manager
    public GameStateManager stateManager;

    void Start()
    {
        stateManager = FindFirstObjectByType<GameStateManager>();
        attackHandler = FindFirstObjectByType<Player>(FindObjectsInactive.Include).GetComponent<AttackHandler>();
        if (attackHandler == null)
        {
            Debug.LogError("AttackHandler not found in the scene.");
        }

        HideLevelUpUI(); // Hide the UI initially
    }

    public void ShowLevelUpUI()
    {
        Debug.Log("UI Shown");
        levelUpPanel.SetActive(true);
        // Randomly select 3 choices from the filtered upgrade choices
        randomChoices = GetFilteredRandomChoices(3);

        // Disable all Enemy scripts
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (var enemy in enemies)
        {
            enemy.enabled = false;
        }

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < randomChoices.Count)
            {
                UpgradeChoice choice = randomChoices[i];

                // Update button text and functionality
                upgradeNameTexts[i].text = choice.name;
                buttonTexts[i].text = choice.description;
                upgradeIcons[i].GetComponent<Image>().sprite = choice.icon;

                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => ApplyUpgrade(choice));

                choiceButtons[i].gameObject.SetActive(true);
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
        stateManager.ChangeGameState(GameState.Pause);
    }

    public void HideLevelUpUI()
    {
        levelUpPanel.SetActive(false);
        // Enable all Enemy scripts
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (var enemy in enemies)
        {
            enemy.enabled = true;
        }
        stateManager.ChangeGameState(GameState.InGame);
    }

    private List<UpgradeChoice> GetFilteredRandomChoices(int count)
    {
        List<UpgradeChoice> validChoices = new List<UpgradeChoice>();

        // Filter out upgrades for attacks that are not enabled unless they are unlockable
        foreach (var choice in upgradeChoices)
        {
            var matchingAttack = playerAttacks.myAttacks.Find(a => a.name == choice.associatedAttack?.name);
            if (matchingAttack != null && (matchingAttack.isEnabled || (choice.isUnlockable && !unlockedThisRun.Contains(choice.associatedAttack.name))))
            {
                validChoices.Add(choice);
            }
        }

        validChoices.RemoveAll(choice => choice.isUnlockable && unlockedThisRun.Contains(choice.associatedAttack.name)); // Ensure unlockables don't appear after being taken

        validChoices.Shuffle(); // Shuffle the list randomly
        return validChoices.GetRange(0, Mathf.Min(count, validChoices.Count));
    }

    private void ApplyUpgrade(UpgradeChoice choice)
    {
        if (choice.isUnlockable && choice.associatedAttack != null)
        {
            var matchingAttack = playerAttacks.myAttacks.Find(a => a.name == choice.associatedAttack.name);
            if (matchingAttack != null)
            {
                matchingAttack.isEnabled = true;
                unlockedThisRun.Add(choice.associatedAttack.name); // Prevent it from appearing again this run
            }
        }
        else if (choice.associatedAttack != null)
        {
            var matchingAttack = playerAttacks.myAttacks.Find(a => a.name == choice.associatedAttack.name);
            if (matchingAttack != null)
            {
                attackHandler.LevelUpAttack(
                    matchingAttack,
                    choice.cdModifier,
                    choice.damageModifier,
                    choice.AOEModifier,
                    choice.durationModifier,
                    choice.rangeModifier,
                    choice.chainModifier,
                    choice.countModifier,
                    choice.sizeModifier
                );
            }
        }

        Debug.Log("Applied upgrade: " + choice.name);

        Time.timeScale = 1.0f;
        HideLevelUpUI();
    }

    public void ResetUnlockedAttacks()
    {
        unlockedThisRun.Clear(); // Reset when starting a new run
    }
}

