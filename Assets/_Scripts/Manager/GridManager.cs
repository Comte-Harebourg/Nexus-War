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

    public Tile GetTileAtPosition(Vector2Int Pos)
    {
        _tiles.TryGetValue(Pos, out var tile);
        return tile;
    }
}