using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

public class BotManager : MonoBehaviour
{
    public static BotManager Instance;

    void Awake()
    {
        Instance = this;
    }


    public IEnumerator Play(List<BaseUnit> Units)
    {
        yield return null; //Évite les bugs de première itération
        while (Units.Any(go => go.isActive))
        {
            BaseUnit Unit = Units.First(go => go.isActive); //On prend la première unité active
            Unit.OccupiedTile.PerformGenericSearch(Unit, Unit.speed, false); //On regarde ce qu'elle peut faire
            if (Unit.OccupiedTile.RedTiles.Count() != 0) //On attaque si possible
            {
                Tile Tile = Unit.OccupiedTile.RedTiles[Random.Range(0, Unit.OccupiedTile.RedTiles.Count)];//choix aléatoire
                ArrowManager.Instance.ShowPath(Unit.OccupiedTile, Tile.SearchNearestTile(Tile, Unit), false);
                Unit.OccupiedTile.HideRange();
                yield return StartCoroutine(UnitManager.Instance.MoveUnit(Unit, ArrowManager.Instance.PathTiles));
                if (ArrowManager.Instance.PathTiles.Count() != 0) ArrowManager.Instance.PathTiles.Last().SetUnit(Unit);
                yield return StartCoroutine(UnitManager.Instance.FightRoutine(Unit, Tile.OccupiedUnit));
            }
            else //Sinon on se déplace
            {
                Tile Tile = Unit.OccupiedTile.BlueTiles[Random.Range(0, Unit.OccupiedTile.BlueTiles.Count)];//choix aléatoire
                Unit.OccupiedTile.HideRange();
                ArrowManager.Instance.ShowPath(Unit.OccupiedTile, Tile, false);
                yield return StartCoroutine(UnitManager.Instance.MoveUnit(Unit, ArrowManager.Instance.PathTiles));
                Tile.SetUnit(Unit);
            }
            UnitManager.Instance.Exhaustion(Unit);
            UnitManager.Instance.LookTo(Unit, Unit.OccupiedTile, true);
        }
        GameManager.Instance.NextTurn();
    }
}
