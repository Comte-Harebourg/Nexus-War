using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileDatabase", menuName = "Database/TileDatabase")] //Base d'ID des tuiles
public class TileDatabase : ScriptableObject
{
    public TileEntry[] Tiles;
    [System.Serializable]
    public class TileEntry
    {
        public string TileID;
        public Tile Prefab;
    }
    private Dictionary<string, Tile> _lookup;
    public void Init()
    {
        _lookup = new Dictionary<string, Tile>();
        foreach (var entry in Tiles)
        {
            _lookup[entry.TileID] = entry.Prefab;
        }
    }
    public Tile GetTile(string id)
    {
        if (_lookup == null) Init();
        _lookup.TryGetValue(id, out Tile prefab);
        return prefab;
    }
}