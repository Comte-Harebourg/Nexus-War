using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour //A servi pour la génération procédurale des maps, maintenant quasiment inutile
{
    public static GridManager Instance;
    [SerializeField] private Tile _grassTile,_waterTile,_roadTile,_mountainTile,_holeTile,_forestTile;
    [SerializeField] private Transform _cam;
    private Dictionary<Vector2, Tile> _tiles;

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