using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; } // Singleton instance

    TMP_InputField inputField;
    public TMP_Text leaderboardText;

    public GameObject gameOverCanvas;

    public string PlayerName = ""; // Current player's name

    public float Currency;
    public float CurrentScore { get; private set; } = 0; // Current score
    public float HighScore { get; private set; } = 0; // High score
    public string HighScoreName { get; private set; } = "Player 1"; // High score holder name

    private const int LeaderboardSize = 10; // Max number of scores in the leaderboard
    private List<float> leaderboardScores = new List<float>(); // Leaderboard scores
    private List<string> leaderboardNames = new List<string>(); // Leaderboard player names

    public bool bIsScoreSubmitted = false; // Tracks if the score has been submitted

    public float scoreMultiplier = 1.0f;

    void Awake()
    {
        // Ensure singleton instance
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        inputField = gameOverCanvas.transform.Find("NameInputField").GetComponent<TMP_InputField>();

        LoadScores();
        LoadCurrency();
        UpdateLeaderboardUI(); // Update UI with leaderboard
    }

    public void AddScore(float points)
    {
        CurrentScore += points * scoreMultiplier;
        if (CurrentScore > HighScore)
        {
            HighScore = CurrentScore;
            HighScoreName = PlayerName;
        }
    }

    public void ResetScore()
    {
        CurrentScore = 0;
    }

    public void InputYourName()
    {
        if (bIsScoreSubmitted)
        {
            Debug.LogWarning("Score has already been submitted!");
            return;
        }

        PlayerName = inputField.text.Trim();
        if (string.IsNullOrEmpty(PlayerName))
        {
            PlayerName = "Anonymous";
        }

        Debug.Log($"Player name set to: {PlayerName}");

        // Save the score and update leaderboard
        SaveScore();
        //ScoreManager.Instance.UpdateHighScoreUI();
        ScoreManager.Instance.UpdateLeaderboardUI();

        // Mark the score as submitted
        bIsScoreSubmitted = true;
    }

    public void SaveScore()
    {
        // Add the current score and name to the leaderboard and sort it
        leaderboardScores.Add(CurrentScore);
        leaderboardNames.Add(PlayerName);

        // Sort leaderboard by scores in descending order
        for (int i = 0; i < leaderboardScores.Count; i++)
        {
            for (int j = i + 1; j < leaderboardScores.Count; j++)
            {
                if (leaderboardScores[j] > leaderboardScores[i])
                {
                    // Swap scores
                    float tempScore = leaderboardScores[i];
                    leaderboardScores[i] = leaderboardScores[j];
                    leaderboardScores[j] = tempScore;

                    // Swap names
                    string tempName = leaderboardNames[i];
                    leaderboardNames[i] = leaderboardNames[j];
                    leaderboardNames[j] = tempName;
                }
            }
        }

        // Trim to leaderboard size
        if (leaderboardScores.Count > LeaderboardSize)
        {
            leaderboardScores.RemoveAt(LeaderboardSize);
            leaderboardNames.RemoveAt(LeaderboardSize);
        }

        SaveScores();
    }

    public void SaveCurrency()
    {
        Currency += CurrentScore;
        PlayerPrefs.SetFloat("Currency", Currency);
        PlayerPrefs.Save();
    }

    private void LoadCurrency()
    {
        Currency = PlayerPrefs.GetFloat("Currency");
        ShopManager.Instance.UpdateCurrencyUI();
        Debug.Log("Currency Loaded. It is : " + Currency);
        
    }
    private void SaveScores()
    {
        // Save high score and name
        PlayerPrefs.SetInt("HighScore", (int)HighScore);
        PlayerPrefs.SetString("HighScoreName", HighScoreName);

        // Save leaderboard scores and names
        for (int i = 0; i < leaderboardScores.Count; i++)
        {
            PlayerPrefs.SetInt($"Leaderboard_Score_{i}", (int)leaderboardScores[i]);
            PlayerPrefs.SetString($"Leaderboard_Name_{i}", leaderboardNames[i]);
        }

        PlayerPrefs.Save();
    }

    private void LoadScores()
    {
        // Load high score and name
        HighScore = PlayerPrefs.GetInt("HighScore", 0);
        HighScoreName = PlayerPrefs.GetString("HighScoreName", "Player 1");

        // Load leaderboard scores and names
        leaderboardScores.Clear();
        leaderboardNames.Clear();

        for (int i = 0; i < LeaderboardSize; i++)
        {
            if (PlayerPrefs.HasKey($"Leaderboard_Score_{i}") && PlayerPrefs.HasKey($"Leaderboard_Name_{i}"))
            {
                leaderboardScores.Add(PlayerPrefs.GetInt($"Leaderboard_Score_{i}"));
                leaderboardNames.Add(PlayerPrefs.GetString($"Leaderboard_Name_{i}"));
            }
        }
    }

    public List<(string playerName, float score)> GetLeaderboard()
    {
        List<(string playerName, float score)> leaderboard = new List<(string, float)>();

        for (int i = 0; i < leaderboardScores.Count; i++)
        {
            leaderboard.Add((leaderboardNames[i], (int)leaderboardScores[i]));
        }

        return leaderboard;
    }

    public void ResetScores()
    {
        PlayerPrefs.DeleteAll();
        HighScore = 0;
        HighScoreName = "Player 1";
        leaderboardScores.Clear();
        leaderboardNames.Clear();
        UpdateLeaderboardUI();
    }

    public void ResetScoreMultiplier()
    {
        scoreMultiplier = 1f;
    }

    public void UpdateLeaderboardUI()
    {
        if (leaderboardText != null)
        {
            var leaderboard = GetLeaderboard();
            leaderboardText.text = ""; // Clears the text
            for (int i = 0; i < leaderboard.Count; i++)
            {
                leaderboardText.text += $"{i + 1}. {leaderboard[i].playerName} : {leaderboard[i].score}\n";
            }
        }
    }
}
