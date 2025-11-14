using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    [SerializeField] private int _width, _height;
    [SerializeField] private Tile _grassTile, _waterTile;
    [SerializeField] private Transform _cam;
    private Dictionary<Vector2, Tile> _tiles;

    void Awake()
    {
        Instance = this;
    }

    public void GenerateGrid()
    {
        /// <summary>
        /// GenerateGrid() génère la grille de dimension (width,height)
        /// <summary>
        _tiles = new Dictionary<Vector2, Tile>();
        for (int x=0;x<_width;x++)
        {
            for (int y = 0; y < _height; y++)
            {///C'est ici qu'on génère la map si on veut faire de la génération procédurale
                var randomTile = UnityEngine.Random.Range(0, 5) == 0 ? _waterTile : _grassTile;
                var spawnedTile = Instantiate(randomTile, new Vector3(x, y), Quaternion.identity);
                spawnedTile.name = $"Tile[{x}:{y}]";
                spawnedTile.Init(x,y);
                _tiles[new Vector2(x, y)] = spawnedTile;
            }
        }
        _cam.transform.position = new Vector3((float)_width / 2 - 0.5f, (float)_height / 2 - 0.5f, -10);
        GameManager.Instance.ChangeState(GameState.SpawnPlayer);
    }

    public Tile GetPlayerSpawn()
    {
        return _tiles.Where(tile => tile.Key.x < _width / 2 && tile.Value.Walkable).OrderBy(tile => UnityEngine.Random.value).First().Value;
    }

    public Tile GetEnemySpawn()
    {
        return _tiles.Where(tile => tile.Key.x > _width / 2 && tile.Value.Walkable).OrderBy(tile => UnityEngine.Random.value).First().Value;
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