using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    public string UnitID;
    public string UnitName;
    public Tile OccupiedTile;
    public Faction Faction;
    public int speed;
    public bool Infantry;
    public bool Vehicle;
    public int minAttackRange;
    public int maxAttackRange;
}

public enum Faction
{
    Aberrion = 0,
    Oromound = 1,
    Seranna = 2
}