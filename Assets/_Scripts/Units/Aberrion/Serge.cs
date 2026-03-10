using UnityEngine;

public class Serge : BaseUnit
{
    protected override void InitializeStats()
    {
        UnitID = "001";
        UnitName = "Serge";
        Faction = Faction.Aberrion;
        speed = 4;
        minAttackRange = 1;
        maxAttackRange = 2;

        MaxHealth = 100;
        MaxArmor = 100;
        MaxMorale = 100;
        damage = 20;
        precision = 0.8f;
        penetration = 0.01f;
        MaxMember = 2;

        TileCosts[typeof(ForestTile)] = 2f;
        TileCosts[typeof(GrassTile)] = 1f;
        TileCosts[typeof(HoleTile)] = 1.5f;
        TileCosts[typeof(MountainTile)] = 3f;
        TileCosts[typeof(RoadTile)] = 0.5f;
        TileCosts[typeof(WaterTile)] = float.PositiveInfinity;
    }
}