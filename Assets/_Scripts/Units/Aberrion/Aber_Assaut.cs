using UnityEngine;

public class Aber_Assaut : BaseUnit
{
    protected override void InitializeStats()
    {
        UnitID = "007";
        UnitName = "Assaut d'Aberrion";
        Faction = Faction.Aberrion;
        speed = 10;
        minAttackRange = 1;
        maxAttackRange = 1;
        MaxHealth = 100;
        MaxArmor = 0;
        MaxMorale = 200;
        damage = 140;
        precision = 0.9f;
        penetration = 0.01f;
        MaxMemberCount = 8;

        TileCosts[typeof(ForestTile)] = 4;
        TileCosts[typeof(GrassTile)] = 2;
        TileCosts[typeof(HoleTile)] = 3;
        TileCosts[typeof(MountainTile)] = 6;
        TileCosts[typeof(RoadTile)] = 1;
        TileCosts[typeof(WaterTile)] = int.MaxValue;
    }
}
