using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

public class BotManager : MonoBehaviour
{
    public static BotManager Instance;
    private bool _isPlaying = false; //Empêche qu'un autre bot ne joue simultanement

    void Awake()
    {
        Instance = this;
    }

    public IEnumerator Play(List<BaseUnit> Units)
    {
        if (_isPlaying) yield break; // Sécurité anti-double exécution
        _isPlaying = true;
        yield return null; //Évite les bugs de première itération
        while (Units.Any(go => go.isActive))
        {
            while (GameManager.Instance.InAnimation) yield return null; //Le bot attend la fin des animations en cours pour jouer
            BaseUnit Unit = Units.First(go => go.isActive); //On prend la première unité active
            Unit.OccupiedTile.PerformGenericSearch(Unit, Unit.speed, false); //On regarde ce qu'elle peut faire
            if (Unit.OccupiedTile.RedTiles.Count() != 0) //On attaque si possible
            {
                Tile Tile = Unit.OccupiedTile.RedTiles[Random.Range(0, Unit.OccupiedTile.RedTiles.Count)]; //choix aléatoire
                ArrowManager.Instance.ShowPath(Unit.OccupiedTile, Tile.SearchNearestTile(Tile, Unit), false); //Calcul du chemin
                Unit.OccupiedTile.HideRange(); //On cache sa portee
                yield return StartCoroutine(UnitManager.Instance.MoveUnit(Unit, ArrowManager.Instance.PathTiles)); //Animation de deplacement
                if (ArrowManager.Instance.PathTiles.Count() != 0) ArrowManager.Instance.PathTiles.Last().SetUnit(Unit); //Si on a pas deplacement pas besoin de changer de case
                yield return StartCoroutine(UnitManager.Instance.FightRoutine(Unit, Tile.OccupiedUnit)); //Animation de combat
            }
            else //Sinon on se déplace
            {
                Tile Tile = Unit.OccupiedTile.BlueTiles[Random.Range(0, Unit.OccupiedTile.BlueTiles.Count)]; //choix aléatoire
                Unit.OccupiedTile.HideRange(); //On cache sa portee
                ArrowManager.Instance.ShowPath(Unit.OccupiedTile, Tile, false); //Calcul du chemin
                yield return StartCoroutine(UnitManager.Instance.MoveUnit(Unit, ArrowManager.Instance.PathTiles)); //Animation de deplacement
                Tile.SetUnit(Unit); //On change sa case
            }
            if (GameManager.Instance.GameOver) yield break; // On arrête tout si la partie est finie
            UnitManager.Instance.Exhaustion(Unit); //On epuise l'unite
            UnitManager.Instance.LookTo(Unit, Unit.OccupiedTile, true); //Reinitialise l'animation de l'unité
        }
        _isPlaying = false;
        GameManager.Instance.NextTurn(); //On commence le prochan tour quand on a termine
    }
}
