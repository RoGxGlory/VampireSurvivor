using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChestUI : MonoBehaviour
{
    [System.Serializable]
    public class ArtifactChoice
    {
        public string name; // Name of the artifact
        public string description; // Description to show in the UI
        public Sprite icon; // Icon for the artifact
    }

    public List<ArtifactChoice> artifactChoices; // List of all possible artifacts
    private List<ArtifactChoice> randomChoices;
    public GameObject chestUIPanel; // The UI panel for Chest opening
    public Button[] choiceButtons; // The 3 buttons to show artifacts
    public TextMeshProUGUI[] buttonTexts; // Text components for the buttons
    public TextMeshProUGUI[] artifactNameTexts; // Text components for the buttons
    public GameObject[] artifactIcons; // Icon components for the buttons

    private HashSet<string> obtainedArtifacts = new HashSet<string>(); // Track obtained artifacts in current run

    // REF to the Game State Manager
    public GameStateManager stateManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stateManager = FindFirstObjectByType<GameStateManager>();
        HideChestUI(); // Hide the UI initially
    }

    public void ShowChestUI()
    {
        Debug.Log("Chest UI Shown");
        chestUIPanel.SetActive(true);
        randomChoices = GetFilteredArtifactChoices(3);

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < randomChoices.Count)
            {
                ArtifactChoice choice = randomChoices[i];

                artifactNameTexts[i].text = choice.name;
                buttonTexts[i].text = choice.description;
                artifactIcons[i].GetComponent<Image>().sprite = choice.icon;

                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => SelectArtifact(choice));
                choiceButtons[i].gameObject.SetActive(true);
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
        stateManager.ChangeGameState(GameState.Pause);
    }

    public void HideChestUI()
    {
        chestUIPanel.SetActive(false);
        stateManager.ChangeGameState(GameState.InGame);
    }

    private List<ArtifactChoice> GetFilteredArtifactChoices(int count)
    {
        List<ArtifactChoice> validChoices = artifactChoices.FindAll(choice => !obtainedArtifacts.Contains(choice.name));
        validChoices.Shuffle();
        return validChoices.GetRange(0, Mathf.Min(count, validChoices.Count));
    }

    private void SelectArtifact(ArtifactChoice choice)
    {
        obtainedArtifacts.Add(choice.name);
        Debug.Log("Selected artifact: " + choice.name);
        stateManager.GetAttackHandler().SetArtifactOn(choice.name);
        HideChestUI();
    }

    public void ResetArtifacts()
    {
        obtainedArtifacts.Clear();
    }

}


