using UnityEngine;

public class Sera_Support : BaseUnit
{
    protected override void InitializeStats()
    {
        UnitID = "012";
        UnitName = "Support de Seranna";
        Faction = Faction.Seranna;
        speed = 6;
        minAttackRange = 1;
        maxAttackRange = 2;
        MaxHealth = 800;
        MaxArmor = 0;
        MaxMorale = 800;
        damage = 90;
        precision = 0.6f;
        penetration = 0.01f;
        MaxMemberCount = 8;

        TileCosts[typeof(ForestTile)] = 4;
        TileCosts[typeof(GrassTile)] = 2;
        TileCosts[typeof(HoleTile)] = 3;
        TileCosts[typeof(MountainTile)] = 6;
        TileCosts[typeof(RoadTile)] = 1;
        TileCosts[typeof(WaterTile)] = int.MaxValue;
    }
}
