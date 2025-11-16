using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitDatabase", menuName = "Database/UnitDatabase")] //Base d'ID des unités
public class UnitDatabase : ScriptableObject
{
    public UnitEntry[] Units;
    [System.Serializable]
    public class UnitEntry
    {
        public string UnitID;
        public BaseUnit Prefab;
    }
    private Dictionary<string, BaseUnit> _lookup;
    public void Init()
    {
        _lookup = new Dictionary<string, BaseUnit>();
        foreach (var entry in Units)
        {
            _lookup[entry.UnitID] = entry.Prefab;
        }
    }
    public BaseUnit GetUnit(string id)
    {
        if (_lookup == null) Init();
        _lookup.TryGetValue(id, out BaseUnit prefab);
        return prefab;
    }
}