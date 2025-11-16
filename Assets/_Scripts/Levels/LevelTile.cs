using System;
using UnityEngine;

public class LevelTile : Tile
{
    public TileType Type;
}

[Serializable]
public enum TileType
{
    //Ground
    Grass = 0,
    Forest = 1,
    Road = 2,
    Mountain = 3,
    Water = 4,
    Hole = 5,
    //Unit
    Serge = 100,
    Tibix = 101
}