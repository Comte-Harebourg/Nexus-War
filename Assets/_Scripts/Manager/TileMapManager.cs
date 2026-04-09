using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapManager : MonoBehaviour
{
    public static TileMapManager Instance;
    [SerializeField] private Tilemap _groundMap;
    [SerializeField] private Tilemap _unitMap;
    [SerializeField] private int _levelIndex = 1;
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
#if UNITY_EDITOR
        int levelIndex = index ?? _levelIndex;
        var level = ScriptableObject.CreateInstance<ScriptableLevel>();
        level.LevelIndex = levelIndex;
        level.name = $"Level_{levelIndex}";
        level.GroundTiles = GetTiles(_groundMap).ToList();
        level.UnitTiles = GetUnits(_unitMap).ToList();
        ScriptableObjectUtility.SaveLevelFile(level);
        Debug.Log($"Level {level.LevelIndex} sauvegardé correctement.");
#else
        Debug.LogError("SaveMap() ne peut être appelé qu’en Éditeur.");
#endif
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

    public void LoadMap(int? index = null)
    {
        int levelIndex = index ?? _levelIndex;
        var level = Resources.Load<ScriptableLevel>($"Levels/Level_{_levelIndex}");
        if (level == null)
        {
            Debug.LogError($"Level {_levelIndex} introuvable.");
            return;
        }
        ClearMap();// On supprime les anciennes tuiles et unités
        Dictionary<Vector2Int, Tile> tileLookup = new Dictionary<Vector2Int, Tile>();// Dictionnaire pour retrouver les tuiles par position de grille
        foreach (var savedTile in level.GroundTiles)//Instanciation des tuiles
        {
            Tile prefab = _tileDatabase.GetTile(savedTile.TileID);
            if (prefab == null)
            {
                Debug.LogError($"Tile '{savedTile.TileID}' introuvable dans la base.");
                continue;
            } 
            var tileInstance = Instantiate(prefab, _groundMap.transform);// Instanciation
            Vector2Int Pos = new Vector2Int(savedTile.Position.x, savedTile.Position.y);// Affectation de la position de grille unique
            tileInstance.Position = Pos;
            GridManager.Instance.RegisterTile(tileInstance);// Enregistrement dans GridManager
            tileInstance.transform.position = _groundMap.GetCellCenterWorld(new Vector3Int(Pos.x, Pos.y, 0));// Position dans le monde pour Unity
            tileLookup[Pos] = tileInstance;// Ajout au dictionnaire local
        } 
        foreach (var tileInstance in tileLookup.Values)//Calcul des voisins pour toutes les tuiles
        {
            tileInstance.FindNeighbors();
        }
        foreach (var savedUnit in level.UnitTiles)//Instanciation des unités
        {
            BaseUnit prefab = _unitDatabase.GetUnit(savedUnit.UnitID);
            if (prefab == null)
            {
                Debug.LogError($"Unit '{savedUnit.UnitID}' introuvable dans la base.");
                continue;
            }
            var unitInstance = Instantiate(prefab, _unitMap.transform);
            Vector2Int unitGridPos = new Vector2Int(savedUnit.Position.x, savedUnit.Position.y);
            if (tileLookup.TryGetValue(unitGridPos, out Tile tile))// Placer l'unité sur la tuile correspondante
                tile.SetUnit(unitInstance);
            unitInstance.transform.position = _unitMap.GetCellCenterWorld(new Vector3Int(unitGridPos.x, unitGridPos.y, 0));// Position dans le monde
        }
        GridManager.Instance.Dimension = GetLevelDimensions(level);// Mettre à jour les dimensions de la grille et la caméra
        CameraController.Instance.SetCameraBounds(level);
        GameManager.Instance.UpdateUnits();
        Debug.Log($"Level {level.LevelIndex} a été chargé correctement. Les dimensions sont : ({GridManager.Instance.Dimension.x}, {GridManager.Instance.Dimension.y})");
    }

}
#if UNITY_EDITOR
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