using UnityEngine;

public class GridCell : MonoBehaviour
{
    public bool isPath;
    public bool canPlaceTower;
    public bool isOccupied; // New state: A tower is placed here
    public Vector3 worldPosition;
    public int gridX, gridY;

    public Color pathColor = Color.magenta;
    public Color towerPlacementColor = Color.green;
    public Color occupiedColor = Color.gray; // New color for occupied areas
    public Color previewColor = new(1f, 1f, 1f, 0.5f); // Transparent white

    private GridCell hoveredCell;
    private GameObject previewTower; // Temporary preview

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    private void OnMouseDown()
    {
        if (canPlaceTower && !isOccupied)
        {
            var selectedTower = TowerManager.Instance.selectedTowerPrefab;

            if (selectedTower == null)
            {
                Debug.LogWarning("No tower selected!");
                return;
            }

            var towerScript = selectedTower.GetComponent<Tower>();
            if (towerScript == null)
            {
                Debug.LogError("Selected tower prefab does not have a Tower script!");
                return;
            }

            if (!TowerManager.Instance.CanAffordTower())
            {
                Debug.LogWarning("Not enough currency to place tower!");
                return;
            }

            // Purchase and Place Tower
            TowerManager.Instance.PurchaseTower();
            var towerInstance =
                Instantiate(selectedTower, transform.position, Quaternion.Euler(-90f, 0f, 0f), transform);
            PlaceTower(); // Mark the cell as occupied
            towerInstance.GetComponent<Tower>().canFire = true;

            Debug.Log($"Tower placed at ({gridX}, {gridY})");
        }
    }

    private void OnMouseEnter()
    {
        hoveredCell = this;
        if (canPlaceTower)
        {
            spriteRenderer.color = previewColor; // Show tower preview color
            ShowPreview();
        }
    }

    private void OnMouseExit()
    {
        UpdateVisual(); // Restore original state when not hovering
        if (canPlaceTower)
            RemovePreview();
    }

    public void Initialize(int x, int y)
    {
        gridX = x;
        gridY = y;
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        if (spriteRenderer == null) return;

        if (isOccupied)
            spriteRenderer.color = occupiedColor;
        else if (isPath)
            spriteRenderer.color = pathColor;
        else if (canPlaceTower)
            spriteRenderer.color = towerPlacementColor;
        else
            spriteRenderer.color = Color.white;
    }


    public void PlaceTower()
    {
        if (isOccupied || !canPlaceTower) return; // Can't place if already occupied

        isOccupied = true;
        canPlaceTower = false; // Mark the cell as occupied
        UpdateVisual();
        RemovePreview();
    }

    private void ShowPreview()
    {
        if (hoveredCell == null || hoveredCell.isPath || !hoveredCell.canPlaceTower ||
            TowerManager.Instance.selectedTowerPrefab == null)
        {
            Debug.LogWarning(hoveredCell);
            Debug.LogWarning(hoveredCell.isPath);
            Debug.LogWarning(hoveredCell.canPlaceTower);
            Debug.LogWarning(TowerManager.Instance.selectedTowerPrefab);
            Debug.LogWarning("No tower selected!");
            return;
        }


        // Instantiate preview tower
        previewTower = Instantiate(TowerManager.Instance.selectedTowerPrefab, hoveredCell.transform.position,
            Quaternion.Euler(-90, 0, 0));

        // Adjust transparency for preview
        var renderer = previewTower.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            var previewMaterial = new Material(renderer.material);
            var color = previewMaterial.color;
            color.a = 0.5f; // Set transparency
            previewMaterial.color = color;
            renderer.material = previewMaterial;
        }
    }

    private void RemovePreview()
    {
        if (previewTower != null) Destroy(previewTower);
    }
}