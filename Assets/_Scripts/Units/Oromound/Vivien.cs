using UnityEngine;

public class Vivien : BaseUnit
{
    protected override void InitializeStats()
    {
        UnitID = "002";
        UnitName = "Vivien";
        Faction = Faction.Oromound;
        speed = 8;
        minAttackRange = 1;
        maxAttackRange = 2;
        MaxHealth = 100;
        MaxArmor = 50;
        MaxMorale = 100;
        damage = 20;
        precision = 0.8f;
        penetration = 0.01f;
        MaxMemberCount = 2;

        TileCosts[typeof(ForestTile)] = 4;
        TileCosts[typeof(GrassTile)] = 2;
        TileCosts[typeof(HoleTile)] = 3;
        TileCosts[typeof(MountainTile)] = 6;
        TileCosts[typeof(RoadTile)] = 1;
        TileCosts[typeof(WaterTile)] = int.MaxValue;
    }
}