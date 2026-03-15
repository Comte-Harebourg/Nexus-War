using UnityEngine;

public class Aber_Tank : BaseUnit
{
    protected override void InitializeStats()
    {
        UnitID = "013";
        UnitName = "Char léger d'Aberrion";
        Faction = Faction.Aberrion;
        speed = 6;
        minAttackRange = 1;
        maxAttackRange = 2;
        MaxHealth = 500;
        MaxArmor = 150;
        MaxMorale = 500;
        damage = 600;
        precision = 0.5f;
        penetration = 0.25f;
        MaxMember = 1;

        TileCosts[typeof(ForestTile)] = 2f;
        TileCosts[typeof(GrassTile)] = 1f;
        TileCosts[typeof(HoleTile)] = 1.5f;
        TileCosts[typeof(MountainTile)] = float.PositiveInfinity;
        TileCosts[typeof(RoadTile)] = 0.5f;
        TileCosts[typeof(WaterTile)] = float.PositiveInfinity;
    }
}
