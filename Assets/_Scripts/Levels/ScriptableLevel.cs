using System;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableLevel : ScriptableObject
{
    public int LevelIndex;
    public List<SavedTile> GroundTiles;
    public List<SavedUnits> UnitTiles;
}

[Serializable]

public class SavedTile
{
    public Vector3Int Position;
    public string TileID;
}

[Serializable]

public class SavedUnits
{
    public Vector3Int Position;
    public string UnitID;
}