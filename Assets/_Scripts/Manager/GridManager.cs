using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour //Gère la grille
{
    public static GridManager Instance;
    [SerializeField] private Tile _grassTile,_waterTile,_roadTile,_mountainTile,_holeTile,_forestTile;
    [SerializeField] private Transform _cam;
    private Dictionary<Vector2Int, Tile> _tiles = new Dictionary<Vector2Int, Tile>();
    private Dictionary<Vector2Int, DualTile> _dualTiles = new Dictionary<Vector2Int, DualTile>();
    public Vector2Int Dimension;

    void Awake()
    {
        Instance = this;
    }

    public void RegisterTile(Tile tile)
    {
        if (_tiles.ContainsKey(tile.Position))
            Debug.LogWarning($"Une tuile existe déjà à {tile.Position}");
        _tiles[tile.Position] = tile;
    }

    public void RegisterDualTile(DualTile tile)
    {
        if (!_dualTiles.ContainsKey(tile.Position)) _dualTiles[tile.Position] = tile;
    }

    public Tile GetTileAtPosition(Vector2Int Pos)
    {
        /// <summary>
        /// GetTileAtPosition(Vector2Int Pos) Renvoie la Tile à la position Pos qui est un Vector2Int
        /// <summary>
        _tiles.TryGetValue(Pos, out var tile);
        return tile;
    }

    public DualTile GetDualTileAtPosition(Vector2Int Pos)
    {
        _dualTiles.TryGetValue(Pos, out var tile);
        return tile;
    }

    public Tile GetTileUnderMouse()
    {
        int gx = Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).x - 0.5f);
        int gy = Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).y - 0.5f);
        Vector2Int gridPos = new Vector2Int(gx, gy);
        return GetTileAtPosition(gridPos);
    }

    internal void UpdateDualTileMap()
    {
        foreach (KeyValuePair<Vector2Int, DualTile> tile in _dualTiles)
        {
            tile.Value.UpdateSprite();
        }
    }
}