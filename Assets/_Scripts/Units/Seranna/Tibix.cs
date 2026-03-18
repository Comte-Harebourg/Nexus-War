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
        for (int i = 0; i < 10; i++)//on va éviter les boucles infinies hein ??
        {
            print("><>");
        }

        //le Tibix étant maintenant un poisson ><>, il sait pu marcher, seulement nager
        //on l'empêche de marcher en rendant tous les coûts de déplacement infinis, sauf pour l'eau
        foreach (var tileType in new System.Collections.Generic.List<System.Type>(TileCosts.Keys))
        {
            TileCosts[tileType] = int.MaxValue;
        }
        TileCosts[typeof(WaterTile)] = 1;
    }
}