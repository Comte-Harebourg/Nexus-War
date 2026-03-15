using UnityEngine;

public class Aber_Tank : BaseUnit
{
    protected override void InitializeStats()
    {
        UnitID = "013";
        UnitName = "Char léger d'Aberrion";
        Faction = Faction.Aberrion;
        speed = 12;
        minAttackRange = 1;
        maxAttackRange = 2;
        MaxHealth = 500;
        MaxArmor = 150;
        MaxMorale = 500;
        damage = 600;
        precision = 0.5f;
        penetration = 0.25f;
        MaxMemberCount = 1;

        TileCosts[typeof(ForestTile)] = 4;
        TileCosts[typeof(GrassTile)] = 2;
        TileCosts[typeof(HoleTile)] = 3;
        TileCosts[typeof(MountainTile)] = int.MaxValue;
        TileCosts[typeof(RoadTile)] = 1;
        TileCosts[typeof(WaterTile)] = int.MaxValue;
    }
}
