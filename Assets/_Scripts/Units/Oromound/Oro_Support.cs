using UnityEngine;

public class Oro_Support : BaseUnit
{
    protected override void InitializeStats()
    {
        UnitID = "011";
        UnitName = "Support d'Oromound";
        Faction = Faction.Oromound;
        speed = 3;
        minAttackRange = 1;
        maxAttackRange = 2;
        MaxHealth = 800;
        MaxArmor = 0;
        MaxMorale = 800;
        damage = 90;
        precision = 0.6f;
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
