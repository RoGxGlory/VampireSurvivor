using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilingBackground : MonoBehaviour
{
    public Transform player; // Reference to the player
    public GameObject tilePrefab; // Prefab of the background tile
    public float tileSizeX, tileSizeY; // Size of a single tile
    public int tilesX = 5, tilesY = 5; // Number of tiles in X and Y (covers screen + buffer)

    private Vector3 lastPlayerPosition;
    private Dictionary<Vector2Int, GameObject> activeTiles = new Dictionary<Vector2Int, GameObject>();

    void Start()
    {
        player = FindFirstObjectByType<Player>(FindObjectsInactive.Include).gameObject.transform;

        if (player == null)
        {
            Debug.LogError("Player transform is not assigned!");
            return;
        }

        // Get tile size automatically if not set
        if (tileSizeX == 0 || tileSizeY == 0)
        {
            SpriteRenderer sr = tilePrefab.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                tileSizeX = sr.bounds.size.x;
                tileSizeY = sr.bounds.size.y;
            }
            else
            {
                Debug.LogError("Tile prefab does not have a SpriteRenderer!");
                return;
            }
        }

        lastPlayerPosition = player.position;

        GenerateInitialTiles();
    }

    void Update()
    {
        Vector2Int playerGridPos = GetGridPosition(player.position);
        Vector2Int lastGridPos = GetGridPosition(lastPlayerPosition);

        if (playerGridPos != lastGridPos)
        {
            UpdateTiles(playerGridPos);
        }

        lastPlayerPosition = player.position;
    }

    private void GenerateInitialTiles()
    {
        Vector2Int centerPos = GetGridPosition(player.position);

        for (int x = -tilesX / 2; x <= tilesX / 2; x++)
        {
            for (int y = -tilesY / 2; y <= tilesY / 2; y++)
            {
                Vector2Int gridPos = new Vector2Int(centerPos.x + x, centerPos.y + y);
                SpawnTile(gridPos);
            }
        }
    }

    private void UpdateTiles(Vector2Int playerGridPos)
    {
        List<Vector2Int> positionsToKeep = new List<Vector2Int>();

        for (int x = -tilesX / 2; x <= tilesX / 2; x++)
        {
            for (int y = -tilesY / 2; y <= tilesY / 2; y++)
            {
                Vector2Int gridPos = new Vector2Int(playerGridPos.x + x, playerGridPos.y + y);
                positionsToKeep.Add(gridPos);

                if (!activeTiles.ContainsKey(gridPos))
                {
                    SpawnTile(gridPos);
                }
            }
        }

        RemoveOldTiles(positionsToKeep);
    }

    private void SpawnTile(Vector2Int gridPos)
    {
        Vector3 worldPosition = new Vector3(gridPos.x * tileSizeX, gridPos.y * tileSizeY, 1);
        GameObject newTile = Instantiate(tilePrefab, worldPosition, Quaternion.identity, transform);
        activeTiles.Add(gridPos, newTile);
    }

    private void RemoveOldTiles(List<Vector2Int> positionsToKeep)
    {
        List<Vector2Int> toRemove = new List<Vector2Int>();

        foreach (var tile in activeTiles)
        {
            if (!positionsToKeep.Contains(tile.Key))
            {
                Destroy(tile.Value);
                toRemove.Add(tile.Key);
            }
        }

        foreach (var key in toRemove)
        {
            activeTiles.Remove(key);
        }
    }

    private Vector2Int GetGridPosition(Vector3 position)
    {
        return new Vector2Int(Mathf.FloorToInt(position.x / tileSizeX), Mathf.FloorToInt(position.y / tileSizeY));
    }
}