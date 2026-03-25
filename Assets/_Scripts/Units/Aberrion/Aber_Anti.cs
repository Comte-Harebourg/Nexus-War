using UnityEngine;

public class Aber_Anti : BaseUnit
{
    protected override void InitializeStats()
    {
        UnitID = "0016";
        UnitName = "Antichar d'Aberrion";
        Faction = Faction.Aberrion;
        speed = 6;
        minAttackRange = 1;
        maxAttackRange = 2;
        MaxHealth = 100;
        MaxArmor = 0;
        MaxMorale = 100;
        damage = 200;
        precision = 0.3f;
        penetration = 0.5f;
        MaxMemberCount = 4;

        TileCosts[typeof(ForestTile)] = 4;
        TileCosts[typeof(GrassTile)] = 2;
        TileCosts[typeof(HoleTile)] = 3;
        TileCosts[typeof(MountainTile)] = 6;
        TileCosts[typeof(RoadTile)] = 1;
        TileCosts[typeof(WaterTile)] = int.MaxValue;
    }
}