using UnityEngine;

public class Aber_Tank : BaseUnit
{
    protected override void InitializeStats()
    {
        UnitID = "013";
        UnitName = "Char leger d'Aberrion";
        Faction = Faction.Aberrion;
        speed = 12;
        minAttackRange = 1;
        maxAttackRange = 2;
        MaxHealth = 500;
        MaxArmor = 50;
        MaxMorale = 500;
        damage = 200;
        precision = 0.5f;
        penetration = 0.25f;
        MaxMemberCount = 3;
        Vision = 6;
        Camo = 0;

        TileCosts[typeof(ForestTile)] = 4;
        TileCosts[typeof(GrassTile)] = 2;
        TileCosts[typeof(HoleTile)] = 3;
        TileCosts[typeof(MountainTile)] = int.MaxValue;
        TileCosts[typeof(RoadTile)] = 1;
        TileCosts[typeof(WaterTile)] = int.MaxValue;
    }
}
