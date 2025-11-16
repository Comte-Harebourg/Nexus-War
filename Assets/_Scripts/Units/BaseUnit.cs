using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    public string UnitID;
    public string UnitName;
    public Tile OccupiedTile;
    public Faction Faction;
}

public enum Faction
{
    Aberrion = 0,
    Oromound = 1,
    Seranna = 2
}