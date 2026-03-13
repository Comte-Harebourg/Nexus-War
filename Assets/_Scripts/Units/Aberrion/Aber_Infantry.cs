using UnityEngine;

public class Aber_Infantry : BaseUnit
{
    protected override void InitializeStats()
    {
        UnitID = "004";
        UnitName = "Infanterie d'Aberrion";
        Faction = Faction.Aberrion;
        speed = 4;
        minAttackRange = 1;
        maxAttackRange = 2;
        MaxHealth = 1200;
        MaxArmor = 0;
        MaxMorale = 1200;
        damage = 50;
        precision = 0.8f;
        penetration = 0.01f;
        MaxMember = 12;

        TileCosts[typeof(ForestTile)] = 2f;
        TileCosts[typeof(GrassTile)] = 1f;
        TileCosts[typeof(HoleTile)] = 1.5f;
        TileCosts[typeof(MountainTile)] = 3f;
        TileCosts[typeof(RoadTile)] = 0.5f;
        TileCosts[typeof(WaterTile)] = float.PositiveInfinity;
    }
}
