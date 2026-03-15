using UnityEngine;

public class Aber_Infantry : BaseUnit
{
    protected override void InitializeStats()
    {
        UnitID = "004";
        UnitName = "Infanterie d'Aberrion";
        Faction = Faction.Aberrion;
        speed = 8;
        minAttackRange = 1;
        maxAttackRange = 2;
        MaxHealth = 1200;
        MaxArmor = 0;
        MaxMorale = 1200;
        damage = 50;
        precision = 0.8f;
        penetration = 0.01f;
        MaxMemberCount = 12;

        TileCosts[typeof(ForestTile)] = 4;
        TileCosts[typeof(GrassTile)] = 2;
        TileCosts[typeof(HoleTile)] = 3;
        TileCosts[typeof(MountainTile)] = 6;
        TileCosts[typeof(RoadTile)] = 1;
        TileCosts[typeof(WaterTile)] = int.MaxValue;
    }
}
