using UnityEngine;

public class Aber_Mortar : BaseUnit
{
    protected override void InitializeStats()
    {
        UnitID = "019";
        UnitName = "Mortier d'Aberrion";
        Faction = Faction.Aberrion;
        speed = 4;
        minAttackRange = 3;
        maxAttackRange = 6;
        MaxHealth = 100;
        MaxArmor = 0;
        MaxMorale = 100;
        damage = 150;
        precision = 0.3f;
        penetration = 0.10f;
        MaxMemberCount = 3;
        TriggerDevastation = true;

        TileCosts[typeof(ForestTile)] = 4;
        TileCosts[typeof(GrassTile)] = 2;
        TileCosts[typeof(HoleTile)] = 3;
        TileCosts[typeof(MountainTile)] = 6;
        TileCosts[typeof(RoadTile)] = 1;
        TileCosts[typeof(WaterTile)] = int.MaxValue;
    }
}
