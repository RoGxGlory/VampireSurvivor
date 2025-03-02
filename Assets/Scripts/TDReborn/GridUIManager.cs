using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GridUIManager : MonoBehaviour
{
    public GridManager gridManager;
    public TMP_Dropdown difficultyDropdown;
    public Button saveButton;
    public Button loadButton;
    public Button clearButton;

    void Start()
    {
        difficultyDropdown.onValueChanged.AddListener(SetDifficulty);
        saveButton.onClick.AddListener(SaveGrid);
        loadButton.onClick.AddListener(LoadGrid);
        clearButton.onClick.AddListener(ClearGrid);
    }

    void SetDifficulty(int index)
    {
        gridManager.selectedDifficulty = (GridManager.Difficulty)index;
        Debug.Log($"Selected Difficulty: {gridManager.selectedDifficulty}");
    }

    void SaveGrid()
    {
        gridManager.SaveGrid();
    }

    void LoadGrid()
    {
        gridManager.LoadGrid();
    }

    void ClearGrid()
    {
        gridManager.ClearGrid();
    }
}