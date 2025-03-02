using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerSelectionUI : MonoBehaviour
{
    public GameObject[] towerPrefabs; // Different towers in the UI
    public Button[] towerButtons; // Buttons linked to tower selection
    public TMP_Text currencyText; // UI text for displaying currency
    public TMP_Text waveText; // UI text for displaying current wave

    private void Start()
    {
        for (var i = 0; i < towerButtons.Length; i++)
        {
            var index = i;
            towerButtons[i].onClick.AddListener(() => SelectTower(index));
        }
    }

    private void Update()
    {
        currencyText.text = $"Currency: {TowerManager.Instance.playerCurrency}";
        waveText.text = $"Wave: {TDEnemySpawner.Instance.GetCurrentWave()}";
    }

    private void SelectTower(int index)
    {
        TowerManager.Instance.SetSelectedTower(towerPrefabs[index]);
        Debug.Log($"Selected tower: {towerPrefabs[index].name}");
    }
}