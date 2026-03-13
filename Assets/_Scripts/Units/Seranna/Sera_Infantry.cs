using UnityEngine;

public class Sera_Infantry : BaseUnit
{
    protected override void InitializeStats()
    {
        UnitID = "006";
        UnitName = "Infanterie de Seranna";
        Faction = Faction.Seranna;
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
