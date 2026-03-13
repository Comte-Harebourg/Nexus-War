using UnityEngine;

public class Oro_Assaut : BaseUnit
{
    protected override void InitializeStats()
    {
        UnitID = "008";
        UnitName = "Assaut d'Oromound";
        Faction = Faction.Oromound;
        speed = 5;
        minAttackRange = 1;
        maxAttackRange = 1;
        MaxHealth = 800;
        MaxArmor = 0;
        MaxMorale = 1600;
        damage = 140;
        precision = 0.9f;
        penetration = 0.01f;
        MaxMember = 8;

        TileCosts[typeof(ForestTile)] = 2f;
        TileCosts[typeof(GrassTile)] = 1f;
        TileCosts[typeof(HoleTile)] = 1.5f;
        TileCosts[typeof(MountainTile)] = 3f;
        TileCosts[typeof(RoadTile)] = 0.5f;
        TileCosts[typeof(WaterTile)] = float.PositiveInfinity;
    }
}
