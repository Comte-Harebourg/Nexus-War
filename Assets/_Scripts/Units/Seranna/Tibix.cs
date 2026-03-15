using UnityEngine;

public class Tibix : BaseUnit
{
    protected override void InitializeStats()
    {
        UnitID = "003";
        UnitName = "Tibix";
        Faction = Faction.Seranna;
        speed = 8;
        minAttackRange = 1;
        maxAttackRange = 2;
        MaxHealth = 100;
        MaxArmor = 100;
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

    public void BeFish()
    {
        print(">be me");
        print(">fish");
        while (true)
        {
            print("><>");
        }
    }
}