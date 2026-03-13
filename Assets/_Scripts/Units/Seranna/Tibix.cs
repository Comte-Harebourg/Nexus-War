using UnityEngine;

public class Tibix : BaseUnit
{
    protected override void InitializeStats()
    {
        UnitID = "003";
        UnitName = "Tibix";
        Faction = Faction.Seranna;
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