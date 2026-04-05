using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BotManager : MonoBehaviour
{
    public static BotManager Instance;

    void Awake()
    {
        Instance = this;
    }


    public void Play(List<BaseUnit> Units)
    {
        while (Units.Any(go => go.isActive))
        {
            BaseUnit Unit = Units.First(go => go.isActive);
            Unit.OccupiedTile.PerformGenericSearch(Unit, Unit.speed, false);
            Tile Tile = Unit.OccupiedTile.BlueTiles[Random.Range(0, Unit.OccupiedTile.BlueTiles.Count)];
            Unit.OccupiedTile.HideRange();
            Tile.SetUnit(Unit);
            UnitManager.Instance.Exhaustion(Unit);
        }
        GameManager.Instance.NextTurn();
    }
}
