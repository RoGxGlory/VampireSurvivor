using System.IO;
using UnityEngine;
#if UNITY_EDITOR
#endif

[ExecuteInEditMode]
public class GridManager : MonoBehaviour
{
    public enum Difficulty
    {
        Easy,
        Medium,
        Hard,
        Impossible
    }

    [Header("Grid Settings")] public int width = 10;

    public int height = 10;
    public float cellSize = 1f;
    public GameObject gridCellPrefab;

    [Header("Path Settings")] public GridCell startCell; // Manually assign in editor

    public GridCell endCell; // Manually assign in editor
    public Difficulty selectedDifficulty = Difficulty.Easy;

    public Pathfinding pathfinding;

    private GridCell[,] grid;

    private bool hasLoadedGrid = false; // Prevent multiple loads

    private void Start()
    {
        if (grid == null)
        {
            Debug.Log("Generating grid on start...");
            CreateGrid();
        }

        LoadGrid(); // Automatically load the saved grid on start
        UpdateGridVisuals(); // Refresh all visuals
        // Invoke("InitializePathfinding", 0.1f); // Small delay to ensure grid is set
    }

    private void Update()
    {
        if (Application.isPlaying) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var clickedCell = GetCell(mousePos);

            if (clickedCell != null)
            {
                clickedCell.isPath = !clickedCell.isPath;
                clickedCell.UpdateVisual();
            }
        }
    }

    private string GetSavePath()
    {
        return Path.Combine(Application.persistentDataPath, $"{selectedDifficulty}_grid.json");
    }

    public void GenerateGridInEditor()
    {
        CreateGrid();
        MarkTowerPlacementAreas();
        UpdateGridVisuals();
    }

    public void SaveGrid()
    {
        if (grid == null)
        {
            Debug.LogError("Cannot save: Grid is null!");
            return;
        }

        var saveData = new GridSaveData();

        for (var x = 0; x < width; x++)
        for (var y = 0; y < height; y++)
        {
            var cell = grid[x, y];
            var cellData = new GridCellData
            {
                x = x,
                y = y,
                isPath = cell.isPath,
                canPlaceTower = cell.canPlaceTower
            };
            saveData.cells.Add(cellData);
        }

        var json = JsonUtility.ToJson(saveData);
        var key = $"GridData_{selectedDifficulty}";

        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();

        Debug.Log($"Grid layout saved for {selectedDifficulty}");
    }


    public void LoadGrid()
    {
        var key = $"GridData_{selectedDifficulty}";

        if (!PlayerPrefs.HasKey(key))
        {
            Debug.LogWarning($"No saved grid data found for {selectedDifficulty}");
            return;
        }

        var json = PlayerPrefs.GetString(key);
        var saveData = JsonUtility.FromJson<GridSaveData>(json);

        if (saveData == null || saveData.cells.Count == 0)
        {
            Debug.LogWarning("Loaded grid data is empty!");
            return;
        }

        Debug.Log($"Loading grid data for {selectedDifficulty}");

        // Update existing grid instead of regenerating it
        foreach (var cellData in saveData.cells)
            if (IsInBounds(cellData.x, cellData.y))
            {
                var cell = grid[cellData.x, cellData.y];
                cell.isPath = cellData.isPath;
                cell.canPlaceTower = cellData.canPlaceTower;
                cell.UpdateVisual(); // Update the color immediately
            }
    }


    public void ClearGrid()
    {
        foreach (var cell in grid)
            if (cell != null)
            {
                cell.isPath = false;
                cell.canPlaceTower = true;
                cell.UpdateVisual(); // Reset visuals
            }

        Debug.Log("Grid cleared, but saved data remains.");
    }

    private void CreateGrid()
    {
        Debug.Log("Creating the grid...");

        grid = new GridCell[width, height];
        var bottomLeft = transform.position - new Vector3(width * cellSize / 2, height * cellSize / 2, 0);

        for (var x = 0; x < width; x++)
        for (var y = 0; y < height; y++)
        {
            var cellPos = bottomLeft + new Vector3(x * cellSize + cellSize / 2, y * cellSize + cellSize / 2, 0);
            var cellObj = Instantiate(gridCellPrefab, cellPos, Quaternion.identity, transform);

            var cell = cellObj.GetComponent<GridCell>();
            if (cell != null)
            {
                cell.Initialize(x, y);
                grid[x, y] = cell;
                Debug.Log($"Created GridCell at ({x},{y})"); // Log every cell
            }
            else
            {
                Debug.LogError("GridCell Prefab missing GridCell component!");
            }
        }
    }


    private void MarkTowerPlacementAreas()
    {
        for (var x = 0; x < width; x++)
        for (var y = 0; y < height; y++)
        {
            var currentCell = grid[x, y];
            if (!currentCell.isPath) currentCell.canPlaceTower = true;
        }
    }

    private void UpdateGridVisuals()
    {
        foreach (var cell in grid) cell.UpdateVisual();
    }

    public GridCell GetCell(Vector2Int coords)
    {
        if (IsInBounds(coords.x, coords.y)) return grid[coords.x, coords.y];
        return null;
    }

    public GridCell GetCell(Vector2 position)
    {
        var x = Mathf.FloorToInt((position.x + width * cellSize / 2) / cellSize);
        var y = Mathf.FloorToInt((position.y + height * cellSize / 2) / cellSize);

        return IsInBounds(x, y) ? grid[x, y] : null;
    }

    public GridCell GetCell(int x, int y)
    {
        return IsInBounds(x, y) ? grid[x, y] : null;
    }

    public bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    public GridCell[,] GetGrid()
    {
        return grid;
    }
}