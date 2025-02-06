using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameStateManager : MonoBehaviour
{
    #region References
    // REF to the player
    [SerializeField] private Player player;
    public GameObject Player;
    private Stats playerStats;
    private Stats initialPlayerStats;
    public Stats GetInitialPlayerStats()
    {
        return initialPlayerStats;
    }
    private AttackHandler attackHandler;

    // UI Panels
    public GameObject homeUI;
    public GameObject inGameUI;
    public GameObject gameOverUI;
    public GameObject inGamePauseUI;
    public GameObject leaderboardUI;
    public GameObject statsPanelUI;
    public GameObject artifactUI;
    public GameObject shopUI;

    // UI Elements
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI timerText;

    // Stats Panel
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI healthregenText;
    public TextMeshProUGUI evasionText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI manaGainText;
    public TextMeshProUGUI critRateText;
    public TextMeshProUGUI critStrikeText;
    public TextMeshProUGUI countText;
    public TextMeshProUGUI cooldowntText;
    public TextMeshProUGUI durationText;
    public TextMeshProUGUI sizeText;
    public TextMeshProUGUI amplifyText;
    public TextMeshProUGUI rangeText;

    // REF to the enemy generator
    [SerializeField] private EnemyGenerator enemyGenerator;

    // REF to the score manager
    [SerializeField] private ScoreManager scoreManager;

    // REF to the transform resetter
    public TransformResetter transformResetter;

    // REF to the level up UI
    public LevelUpUI levelUpUI;

    // REF to the artifact UI
    public ChestUI chestUI;

    bool bIsActive = false;
    #endregion

    #region Timer
    // Timer to track spawn intervals
    public float timer;
    public int timePlayed;
    int seconds, minutes, dozens, minuteDozens;
    #endregion

    #region Boss Wave
    // Boss wave logic
    [SerializeField] public bool bIsBossWave = false;
    [SerializeField] private int timeUntilBossWave = 150;
    [SerializeField] private int delayBetweenBossWaves = 150; // Time until the next boss wave
    private int bossEnemiesToSpawn = 1; // Total number of enemies for the boss wave
    private int bossEnemiesRemaining; // Enemies left to kill in the boss wave
    public UnityEngine.UI.Slider slider;

    public GameObject chestPrefab;
    #endregion

    #region State Management
    private static GameStateManager _instance;
    public static GameStateManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject singletonObject = new GameObject("GameStateManager");
                _instance = singletonObject.AddComponent<GameStateManager>();
                DontDestroyOnLoad(singletonObject);
            }
            return _instance;
        }
    }

    [SerializeField] public GameState CurrentState { get; private set; } = GameState.Menu;

    // Event for notifying subscribers of game state changes
    public event Action<GameState> OnGameStateChanged;

    public bool bIsPlaying = false;
    #endregion

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        ShowHomeUI();
        playerStats = Player.GetComponent<Stats>();
        attackHandler = Player.GetComponent<AttackHandler>();
        initialPlayerStats = playerStats;
        ShopManager.Instance.SavePlayerData();
    }

    private void Update()
    {
        // Input for pausing the game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (CurrentState == GameState.InGame)
            {
                PauseGame();
            }
            else if (CurrentState == GameState.Pause)
            {
                ResumeGame();
            }
        }

        if (CurrentState == GameState.InGame)
        {
            // Update the timer
            timer += Time.deltaTime;
            if (timer >= 1f)
            {
                scoreManager.AddScore(1);
                UpdateCurrentScore(scoreManager.CurrentScore);
                // levelGenerator.moveSpeedMultiplier += 0.01f;
                // levelGenerator.UpdateSpeed();
                timer = 0f;
                playerStats.Heal((playerStats.GetCurrentRegen()/100));
                if (!bIsBossWave)
                {
                    UpdateTimer();
                }
            }
            enemyGenerator.spawnArea.xMin = Player.transform.position.x - 12;
            enemyGenerator.spawnArea.xMax = Player.transform.position.x + 24;
            enemyGenerator.spawnArea.yMin = Player.transform.position.y - 9;
            enemyGenerator.spawnArea.yMax = Player.transform.position.y + 18;

            if (timePlayed == timeUntilBossWave)
            {
                SetBossWave();
                timeUntilBossWave += delayBetweenBossWaves; // Increments the time until the next boss wave by 2 minutes and 30 seconds
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.C)) 
        {
            bIsActive = !bIsActive;
            UpdateStatText();
            statsPanelUI.SetActive(bIsActive);
        }
    }

    // Home logic
    public void ShowHomeUI()
    {
        bIsPlaying = false;
        homeUI.SetActive(true);
        inGameUI.SetActive(false);
        gameOverUI.SetActive(false);
        Player.SetActive(false);
        inGamePauseUI.SetActive(false);
        leaderboardUI.SetActive(false);
        statsPanelUI.SetActive(false);
        shopUI.SetActive(false);
    }

    public void StartGame()
    {
        // Reset the player's position
        transformResetter.ResetPlayerPosition();
        Player.SetActive(true);
        player.bIsPlaying = true;
        attackHandler.ResetAttacks();
        chestUI.ResetArtifacts();


        ResetTimer();

        CurrentState = GameState.InGame;
        Time.timeScale = 1f;
        Debug.Log("Game Started");

        // Makes sure the game objects are cleanly deleted
        ClearGameObjects();


        // UI logic
        homeUI.SetActive(false);
        inGameUI.SetActive(true);
        gameOverUI.SetActive(false);
        inGamePauseUI.SetActive(false);
        statsPanelUI.SetActive(false);
        shopUI.SetActive(false);
        ScoreManager.Instance.ResetScore();
        UpdateCurrentScore(0);
        levelUpUI.ResetUnlockedAttacks();

        // Game logic
        scoreManager.bIsScoreSubmitted = false;
        scoreManager.ResetScoreMultiplier();
        enemyGenerator.EnableStart();
        bIsBossWave = false;
        ShopManager.Instance.LoadPlayerData();
        playerStats.ResetStats(initialPlayerStats);
        UpdateStatText();
        bIsPlaying = true;
    }

    public void GameOver()
    {
        CurrentState = GameState.GameOver;
        bIsPlaying = false;
        enemyGenerator.StopCoroutine();
        Time.timeScale = 0f;
        Debug.Log("Game Over!");
        player.bIsPlaying = false;
        // Show Game Over UI or handle Game Over logic

        // Display final score and high score
        if (finalScoreText != null)
        {
            finalScoreText.text = "Your Score: " + ScoreManager.Instance.CurrentScore;
        }

        if (highScoreText != null)
        {
            highScoreText.text = "High Score: " + ScoreManager.Instance.HighScore;
            if (ScoreManager.Instance.CurrentScore > ScoreManager.Instance.HighScore)
            {
                ScoreManager.Instance.SaveScore();
            }
        }

        ScoreManager.Instance.SaveCurrency();

        ShowGameOverUI();
    }

    public void ShowGameOverUI()
    {
        homeUI.SetActive(false);
        inGameUI.SetActive(false);
        gameOverUI.SetActive(true);
        inGamePauseUI.SetActive(false);
        Player.SetActive(false);
        statsPanelUI.SetActive(false);
        ShopManager.Instance.UpdateCurrency();
    }
    public void PauseGame()
    {
        CurrentState = GameState.Pause;
        Time.timeScale = 0f;
        Debug.Log("Game Paused");

        // Show Pause UI
        inGamePauseUI.SetActive(true);
        player.bIsPlaying = false;
    }

    public void ResumeGame()
    {
        CurrentState = GameState.InGame;
        Time.timeScale = 1f;
        Debug.Log("Game Resumed");

        // Hide Pause UI
        inGamePauseUI.SetActive(false);
        player.bIsPlaying = true;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Resume the game
        transformResetter.ResetPlayerPosition();
        ClearGameObjects();
        StartGame();
        player.bIsPlaying = true;
        
    }

    public void ReturnToMenu()
    {
        CurrentState = GameState.Menu;
        Time.timeScale = 1f;
        Debug.Log("Returning to Menu");

        // Load Menu Scene
        ShowHomeUI();
        player.bIsPlaying = false;
    }

    public void ShowLeaderboardUI()
    {
        homeUI.SetActive(false);
        leaderboardUI.SetActive(true);
    }

    public void ShowShopUI()
    {
        ShopManager.Instance.UpdateCurrencyUI();
        ShopManager.Instance.SetUpgradeIconAlpha(0);
        shopUI.SetActive(true);
    }

    public void ChangeGameState(GameState newState)
    {
        if (CurrentState == newState)
        {
            Debug.LogWarning("Game state is already " + newState);
            return;
        }

        CurrentState = newState;
        Debug.Log("Game state changed to: " + newState);

        // Notify subscribers about the state change
        OnGameStateChanged?.Invoke(newState);
        player.HandleGameStateChanged(newState);

        // Additional logic for state transitions can be added here
    }

    public bool IsGameState(GameState state)
    {
        return CurrentState == state;
    }

    // In-Game logic
    public void UpdateCurrentScore(float score)
    {
        if (currentScoreText != null)
        {
            currentScoreText.text = "Score: " + score;
        }
    }

    public void ClearGameObjects()
    {
        GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject Enemy in Enemies)
        {
            Destroy(Enemy);
        }
        GameObject[] Obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach (GameObject Obstacle in Obstacles)
        {
            Destroy(Obstacle);
        }
        
        GameObject[] Orbs = GameObject.FindGameObjectsWithTag("Orb");
        foreach (GameObject Orb in Orbs)
        {
            Destroy(Orb);
        }
        
        GameObject[] Chests = GameObject.FindGameObjectsWithTag("Chest");
        foreach (GameObject Chest in Chests)
        {
            Destroy(Chest);
        }
        GameObject[] PowerUps = GameObject.FindGameObjectsWithTag("PowerUp");
        foreach (GameObject PowerUp in PowerUps)
        {
            Destroy(PowerUp);
        }
        GameObject[] SCMultipliers = GameObject.FindGameObjectsWithTag("ScoreMultiplier");
        foreach (GameObject SCMultiplier in SCMultipliers)
        {
            Destroy(SCMultiplier);
        }
    }

    void UpdateTimer()
    {
        timePlayed++;
        seconds++;
        if (seconds == 10)
        {
            seconds = 0;
            dozens++;
        }
        if (dozens == 6)
        {
            dozens = 0;
            minutes++;
        }
        if (minutes == 10)
        {
            minutes = 0;
            minuteDozens++;
        }
        timerText.SetText(minuteDozens.ToString() + minutes.ToString() + ":" + dozens.ToString() + seconds.ToString());
    }

    void ResetTimer()
    {
        timer = 0;
        timePlayed = 0;
        seconds = 0;
        dozens = 0;
        minutes = 0;
        minuteDozens = 0;
        timerText.SetText(minuteDozens.ToString() + minutes.ToString() + ":" + dozens.ToString() + seconds.ToString());
    }

    void SetBossWave()
    {
        bIsBossWave = !bIsBossWave;
        slider.gameObject.SetActive(bIsBossWave);

        // Configure slider
        bossEnemiesToSpawn = 50; 
        bossEnemiesRemaining = bossEnemiesToSpawn;
        slider.maxValue = bossEnemiesToSpawn;
        slider.value = bossEnemiesRemaining;

        // Stop normal enemy spawning and start boss wave enemies
        enemyGenerator.StopCoroutine();
        enemyGenerator.SpawnBossWaveEnemies(bossEnemiesToSpawn);
    }

    public void OnBossEnemyKilled()
    {
        bossEnemiesRemaining--;
        slider.value = bossEnemiesRemaining;

        if (bossEnemiesRemaining == 0)
        {
            EndBossWave();
        }
    }

    public void EndBossWave()
    {
        Debug.Log("Ending Boss Wave...");

        bIsBossWave = false; // Makes sure normal enemies can spawn
        enemyGenerator.SetBossWave(false); // Also reset inside the EnemyGenerator
        slider.gameObject.SetActive(false); // Hides progress bar

        enemyGenerator.StopAllCoroutines();
        enemyGenerator.EnableStart(); // Resume normal spawning

        SpawnChest(chestPrefab, player.transform.position + new Vector3(5,0,0), Quaternion.identity);
    }

    public void SpawnChest(GameObject gameObject, Vector3 position, Quaternion rotation)
    {
        GameObject chest = Instantiate(gameObject, position, rotation);
        chest.SetActive(true);
        Debug.Log("Spawned Chest at: " + position);
    }

    public void UpdateStatText()
    {
        hpText.text = "HP : " + playerStats.GetMaxHealth();
        healthregenText.text = "REG : " + playerStats.GetCurrentRegen();
        evasionText.text = "EVA : " + playerStats.GetEvasion();
        speedText.text = "SPD : " + player.moveSpeed * 20;
        manaGainText.text = "MGN : " + playerStats.GetManaGain();
        critRateText.text = "CS : " + playerStats.GetCritRate();
        critStrikeText.text = "CM : " + playerStats.GetCritStrike();
        countText.text = "CMD : " + playerStats.GetCountModifier();
        cooldowntText.text = "CDR : " + playerStats.GetCooldownModifier();
        durationText.text = "DUR : " + playerStats.GetDurationModifier();
        sizeText.text = "SIZE : " + playerStats.GetSizeModifier();
        amplifyText.text = "AMP : " + playerStats.GetAttackAmp();
        rangeText.text = "RAN : " + playerStats.GetRangeModifier();
    }

    public AttackHandler GetAttackHandler()
    {
        return attackHandler;
    }
}
public enum GameState
{
    Menu,
    InGame,
    GameOver,
    Pause
}