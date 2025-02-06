using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [System.Serializable]
    public class StatModifier
    {
        public string statName;   // The name of the stat (e.g., "attackAmplify", "moveSpeed")
        public int value;         // The value to modify the stat
    }

    [System.Serializable]
    public class Upgrade
    {
        public string name;
        public Sprite icon;
        public string description;
        public int cost;
        public bool isPurchased;
        public List<StatModifier> modifiers; // List of stat modifiers
    }

    public static ShopManager Instance { get; private set; }

    public List<Upgrade> availableUpgrades = new List<Upgrade>();

    public Button[] upgradeButtons;
    public Button buyButton;
    public Image upgradeIcon;
    public TextMeshProUGUI upgradeDescriptionText;
    public TextMeshProUGUI currencyText;

    private int playerCurrency;
    private Upgrade selectedUpgrade;
    private Stats playerStats; // Reference to the player's stats

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        playerStats = FindFirstObjectByType<Player>(FindObjectsInactive.Include).GetComponent<Stats>();
        UpdateCurrencyUI();
        AssignUpgradeButtons();
        buyButton.interactable = false;
        LoadCurrency();
        Debug.Log("Player's currency is : " + playerCurrency);
    }

    public void LoadCurrency()
    {
        playerCurrency = (int)PlayerPrefs.GetFloat("Currency");
        UpdateCurrencyUI();
    }

    public void UpdateCurrency()
    {
        int finalScore = (int)ScoreManager.Instance.CurrentScore;
        playerCurrency += finalScore;
        UpdateCurrencyUI();
    }

    private void AssignUpgradeButtons()
    {
        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            if (i < availableUpgrades.Count)
            {
                Upgrade upgrade = availableUpgrades[i];
                Button button = upgradeButtons[i];

                button.image.sprite = upgrade.icon;
                button.onClick.AddListener(() => SelectUpgrade(upgrade, button));

                if (upgrade.isPurchased)
                {
                    button.interactable = false;
                }
            }
            else
            {
                upgradeButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void SelectUpgrade(Upgrade upgrade, Button button)
    {
        selectedUpgrade = upgrade;
        upgradeIcon.sprite = upgrade.icon;
        upgradeDescriptionText.text =upgrade.name + "\n\n" + upgrade.description + "\n\nPrice : " + upgrade.cost;
        SetUpgradeIconAlpha(1);

        buyButton.interactable = !upgrade.isPurchased && playerCurrency >= upgrade.cost;
    }

    public void SetUpgradeIconAlpha(float alpha)
    {
        Color iconColor = upgradeIcon.color;
        iconColor.a = alpha;
        upgradeIcon.color = iconColor;
    }

    public void PurchaseUpgrade()
    {
        if (selectedUpgrade != null && playerCurrency >= selectedUpgrade.cost)
        {
            playerCurrency -= selectedUpgrade.cost;
            selectedUpgrade.isPurchased = true;
            ApplyUpgrade(selectedUpgrade);
            UpdateCurrencyUI();
            buyButton.interactable = false;

            foreach (Button button in upgradeButtons)
            {
                if (button.image.sprite == selectedUpgrade.icon)
                {
                    button.interactable = false;
                    break;
                }
            }

            SavePlayerData();
            Debug.Log($"Purchased: {selectedUpgrade.name}");
        }
    }

    private void ApplyUpgrade(Upgrade upgrade)
    {
        if (playerStats != null)
        {
            List<string> statsToUp = new List<string>();
            List<int> values = new List<int>();

            foreach (var modifier in upgrade.modifiers)
            {
                statsToUp.Add(modifier.statName);
                values.Add(modifier.value);
            }

            playerStats.UpdateStats(statsToUp, values);
        }
        else
        {
            Debug.LogError("Player stats not found!");
        }
    }

    public void UpdateCurrencyUI()
    {
        currencyText.text = $"Currency: {playerCurrency}";
    }

    #region Save/Load

    public void SavePlayerData()
    {
        PlayerData playerData = new PlayerData(GameStateManager.Instance.GetInitialPlayerStats());
        UpgradeData upgradeData = new UpgradeData(availableUpgrades);

        string playerJson = JsonUtility.ToJson(playerData);
        string upgradeJson = JsonUtility.ToJson(upgradeData);

        PlayerPrefs.SetString("PlayerData", playerJson);
        PlayerPrefs.SetString("UpgradeData", upgradeJson);
        PlayerPrefs.Save();

        Debug.Log("Player data saved.");
    }

    public void LoadPlayerData()
    {
        if (PlayerPrefs.HasKey("PlayerData"))
        {
            string playerJson = PlayerPrefs.GetString("PlayerData");
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(playerJson);

            playerStats.level = playerData.level;
            playerStats.XP = playerData.xp;
            playerStats.levelUpXP = playerData.levelUpXP;
            playerStats.attackAmplify = playerData.attackAmplify;
            playerStats.manaGainAmplify = playerData.manaGainAmplify;
            playerStats.initialMaxHealth = playerData.initialMaxHealth;
            playerStats.evasion = playerData.evasion;
            playerStats.moveSpeed = playerData.moveSpeed;
            playerStats.magicDurationModifier = playerData.magicDurationModifier;
            playerStats.chainCountModifier = playerData.chainCountModifier;
            playerStats.rangeModifier = playerData.rangeModifier;
            playerStats.sizeModifier = playerData.sizeModifier;
            playerStats.critRate = playerData.critRate;
            playerStats.critStrikeModifier = playerData.critStrikeModifier;
            playerStats.cooldownModifier = playerData.cooldownModifier;
            playerStats.AddHealthRegen(playerData.healthRegen);
            playerStats.AddChanceModifier(playerData.chanceModifier);
        }

        if (PlayerPrefs.HasKey("UpgradeData"))
        {
            string upgradeJson = PlayerPrefs.GetString("UpgradeData");
            UpgradeData upgradeData = JsonUtility.FromJson<UpgradeData>(upgradeJson);

            foreach (var upgradeName in upgradeData.purchasedUpgrades)
            {
                Upgrade upgrade = availableUpgrades.Find(u => u.name == upgradeName);
                if (upgrade != null)
                {
                    upgrade.isPurchased = true;
                    ApplyUpgrade(upgrade);
                }
            }
        }
    }

    public void ResetProgress()
    {
        UpgradeData upgradeData = new UpgradeData(availableUpgrades);
        upgradeData.ResetList();
        PlayerPrefs.DeleteKey("PlayerData");
        PlayerPrefs.DeleteKey("UpgradeData");
        PlayerPrefs.Save();
        Debug.Log("Progress reset.");
    }
    #endregion
}