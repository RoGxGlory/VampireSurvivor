using System.Collections.Generic;

[System.Serializable]
public class GridSaveData
{
    public List<GridCellData> cells = new List<GridCellData>();
}

[System.Serializable]
public class GridCellData
{
    public int x, y;
    public bool isPath;
    public bool canPlaceTower;
}