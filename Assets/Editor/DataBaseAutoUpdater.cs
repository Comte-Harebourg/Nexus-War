using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class DatabaseAutoUpdater : AssetPostprocessor
{
    private const string TileFolder = "Tiles";
    private const string UnitFolder = "Units";

    static void OnPostprocessAllAssets(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths)
    {
        UpdateTileDatabase();
        UpdateUnitDatabase();
    }

    private static void UpdateTileDatabase()
    {
        TileDatabase db = LoadDatabase<TileDatabase>();
        if (db == null) return;
        Tile[] tiles = Resources.LoadAll<Tile>(TileFolder);
        List<TileDatabase.TileEntry> entries = new List<TileDatabase.TileEntry>();
        foreach (var tile in tiles)
        {
            if (string.IsNullOrEmpty(tile.TileID))
            {
                Debug.LogWarning($"Tile '{tile.name}' n'a pas de TileID, il a été ignoré.");
                continue;
            }
            entries.Add(new TileDatabase.TileEntry
            {
                TileID = tile.TileID,
                Prefab = tile
            });
        }
        Undo.RecordObject(db, "Auto Update Tile Database");
        db.Tiles = entries.ToArray();
        EditorUtility.SetDirty(db);
        Debug.Log($"TileDatabase mis à jour ({entries.Count} entrées).");
    }

    private static void UpdateUnitDatabase()
    {
        UnitDatabase db = LoadDatabase<UnitDatabase>();
        if (db == null) return;
        BaseUnit[] units = Resources.LoadAll<BaseUnit>(UnitFolder);
        List<UnitDatabase.UnitEntry> entries = new List<UnitDatabase.UnitEntry>();
        foreach (var unit in units)
        {
            if (string.IsNullOrEmpty(unit.UnitID))
            {
                Debug.LogWarning($"Unit '{unit.name}' n'a pas d'UnitID, ignorée.");
                continue;
            }
            entries.Add(new UnitDatabase.UnitEntry
            {
                UnitID = unit.UnitID,
                Prefab = unit
            });
        }
        Undo.RecordObject(db, "Auto Update Unit Database");
        db.Units = entries.ToArray();
        EditorUtility.SetDirty(db);
        Debug.Log($"UnitDatabase mis à jour ({entries.Count} entrées).");
    }

    private static T LoadDatabase<T>() where T : ScriptableObject
    {
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
        if (guids.Length == 0) return null;
        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        return AssetDatabase.LoadAssetAtPath<T>(path);
    }
}