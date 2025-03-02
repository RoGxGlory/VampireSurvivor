using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public static TowerManager Instance;
    public GameObject selectedTowerPrefab; // The currently selected tower
    public int playerCurrency = 100; // Starting currency

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void SetSelectedTower(GameObject towerPrefab)
    {
        selectedTowerPrefab = towerPrefab;
    }

    public bool CanAffordTower()
    {
        if (selectedTowerPrefab == null) return false;

        var towerScript = selectedTowerPrefab.GetComponent<Tower>();
        return towerScript != null && playerCurrency >= towerScript.cost;
    }

    public void PurchaseTower()
    {
        if (selectedTowerPrefab == null) return;

        var towerScript = selectedTowerPrefab.GetComponent<Tower>();
        if (towerScript != null && playerCurrency >= towerScript.cost)
        {
            playerCurrency -= towerScript.cost;
            Debug.Log($"Tower purchased! Remaining currency: {playerCurrency}");
        }
        else
        {
            Debug.Log("Not enough currency to place tower.");
        }
    }
}