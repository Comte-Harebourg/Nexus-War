using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapManager : MonoBehaviour //J'arrivais pas à faire cette classe donc j'ai utilisé ChatGPT, il faudra probablement la revoir plus tard
{
    public static TileMapManager Instance;
    [SerializeField] private Tilemap _groundMap;
    [SerializeField] private Tilemap _unitMap;
    [SerializeField] private int _levelIndex;
    [SerializeField] private TileDatabase _tileDatabase;
    [SerializeField] private UnitDatabase _unitDatabase;

    void Awake()
    {
        Instance = this;
    }

    public Vector2Int GetLevelDimensions(ScriptableLevel level)
    {
        int minX = level.GroundTiles.Min(t => t.Position.x);
        int maxX = level.GroundTiles.Max(t => t.Position.x);
        int minY = level.GroundTiles.Min(t => t.Position.y);
        int maxY = level.GroundTiles.Max(t => t.Position.y);
        int width = (maxX - minX) + 1;
        int height = (maxY - minY) + 1;
        return new Vector2Int(width, height);
    }

    public void SaveMap(int? index = null)
    {
        int levelIndex = index ?? _levelIndex;
        var level = ScriptableObject.CreateInstance<ScriptableLevel>();
        level.LevelIndex = levelIndex;
        level.name = $"Level_{levelIndex}";
        level.GroundTiles = GetTiles(_groundMap).ToList();
        level.UnitTiles = GetUnits(_unitMap).ToList();
        ScriptableObjectUtility.SaveLevelFile(level);
        Debug.Log($"Level {level.LevelIndex} sauvegardé correctement.");
    }

    private IEnumerable<SavedTile> GetTiles(Tilemap map)
    {
        foreach (var tile in map.GetComponentsInChildren<Tile>())
        {
            Vector3Int pos = map.WorldToCell(tile.transform.position);
            yield return new SavedTile()
            {
                Position = pos,
                TileID = tile.TileID
            };
        }
    }

    private IEnumerable<SavedUnits> GetUnits(Tilemap map)
    {
        foreach (var unit in map.GetComponentsInChildren<BaseUnit>())
        {
            Vector3Int pos = map.WorldToCell(unit.transform.position);
            yield return new SavedUnits()
            {
                Position = pos,
                UnitID = unit.UnitID
            };
        }
    }

    public void ClearMap()
    {
        foreach (var unit in _unitMap.GetComponentsInChildren<BaseUnit>().ToList())
        {
            DestroyImmediate(unit.gameObject);
        }
        foreach (var tile in _groundMap.GetComponentsInChildren<Tile>().ToList())
        {
            DestroyImmediate(tile.gameObject);
        }
        _groundMap.ClearAllTiles();
        _unitMap.ClearAllTiles();
    }

    public void LoadMap(int? index = null) //Charge la map dont l'index correspond au paramètre ou à l'index du manager dans Unity
    {
        int levelIndex = index ?? _levelIndex;
        var level = Resources.Load<ScriptableLevel>($"Levels/Level_{_levelIndex}");
        if (level == null)
        {
            Debug.LogError($"Level { _levelIndex } introuvable.");
            return;
        }
        ClearMap();
        Dictionary<Vector3Int, Tile> tileLookup = new Dictionary<Vector3Int, Tile>();
        foreach (var savedTile in level.GroundTiles)
        {
            Tile prefab = _tileDatabase.GetTile(savedTile.TileID);
            if (prefab == null)
            {
                Debug.LogError($"Tile '{savedTile.TileID}' introuvable dans la base.");
                continue;
            }
            var tileInstance = Instantiate(prefab, _groundMap.transform);
            tileInstance.transform.position = _groundMap.GetCellCenterWorld(savedTile.Position);
            tileLookup[savedTile.Position] = tileInstance;
        }
        foreach (var savedUnit in level.UnitTiles)
        {
            BaseUnit prefab = _unitDatabase.GetUnit(savedUnit.UnitID);
            if (prefab == null)
            {
                Debug.LogError($"Unit '{savedUnit.UnitID}' introuvable dans la base.");
                continue;
            }
            var unitInstance = Instantiate(prefab, _unitMap.transform);
            if (tileLookup.TryGetValue(savedUnit.Position, out Tile tile))
                tile.SetUnit(unitInstance);
        }
        GridManager.Instance.Dimension = GetLevelDimensions(level);
        CameraController.Instance.SetCameraBounds(level);
        print($"Les dimensions du level sont ({GridManager.Instance.Dimension.x}:{GridManager.Instance.Dimension.y})");
        Debug.Log($"Level {level.LevelIndex} chargé correctement.");
    }
}

#if UNITY_EDITOR //Se compile seulement dans l'éditeur

public static class ScriptableObjectUtility
{
    public static void SaveLevelFile(ScriptableLevel level)
    {
        AssetDatabase.CreateAsset(level, $"Assets/Resources/Levels/{level.name}.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}

#endif