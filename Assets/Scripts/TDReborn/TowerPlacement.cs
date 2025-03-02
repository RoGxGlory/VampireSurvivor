using UnityEngine;

public class TowerPlacement : MonoBehaviour
{
    public GridManager gridManager;
    private GridCell hoveredCell;
    private GameObject previewTower; // Temporary preview

    private void Start()
    {
        if (gridManager == null)
        {
            gridManager = FindFirstObjectByType<GridManager>(FindObjectsInactive.Include);
            if (gridManager == null) Debug.LogError("GridManager not found in scene!");
        }
    }

    private void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var cell = gridManager.GetCell(mousePosition);

        if (cell != hoveredCell)
        {
            RemovePreview();
            hoveredCell = cell;
            ShowPreview();
        }
    }

    private void ShowPreview()
    {
        if (hoveredCell == null || hoveredCell.isPath || !hoveredCell.canPlaceTower ||
            TowerManager.Instance.selectedTowerPrefab == null)
            return;

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