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
    private Dictionary<Vector2, Tile> _tiles;
    public Vector2Int Dimension;

    void Awake()
    {
        Instance = this;
    }

    public Tile GetTileAtPosition(Vector2 pos)
    {
        /// <summary>
        /// GetTileAtPosition((x,y)) récupère la tuile de calsse Tile à la postion (x,y)
        /// <summary>
        if (_tiles.TryGetValue(pos,out var tile))
        {
            return tile;
        }
        return null;
    }
}