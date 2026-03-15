using UnityEngine;

public class Sera_Assaut : BaseUnit
{
    protected override void InitializeStats()
    {
        UnitID = "009";
        UnitName = "Assaut de Seranna";
        Faction = Faction.Seranna;
        speed = 10;
        minAttackRange = 1;
        maxAttackRange = 1;
        MaxHealth = 800;
        MaxArmor = 0;
        MaxMorale = 1600;
        damage = 140;
        precision = 0.9f;
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
