using UnityEngine;

[CreateAssetMenu(fileName="New Unit",menuName ="Scriptable Unit")]

public class ScriptableUnit : ScriptableObject
{
    public Faction Faction;
    public BaseUnit UnitPrefab;
}

public enum Faction
{
    Aberrion=0,
    Oromound=1,
    Seranna=2
}