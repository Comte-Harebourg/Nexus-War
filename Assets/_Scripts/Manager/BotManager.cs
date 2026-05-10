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

            // Priorité 1: Attaquer si un ennemi est à portée
            if (Unit.OccupiedTile.RedTiles.Any())
            {
                Tile enemyTile = Unit.OccupiedTile.RedTiles[Random.Range(0, Unit.OccupiedTile.RedTiles.Count)]; //choix aléatoire
                Tile destinationTile = enemyTile.SearchNearestTile(enemyTile, Unit);
                if (destinationTile == null) destinationTile = Unit.OccupiedTile; //Si on ne peut pas bouger on reste sur place

                ArrowManager.Instance.ShowPath(Unit.OccupiedTile, destinationTile, false); //Calcul du chemin
                Unit.OccupiedTile.HideRange(); //On cache sa portee
                yield return StartCoroutine(UnitManager.Instance.MoveUnit(Unit, ArrowManager.Instance.PathTiles)); //Animation de deplacement
                if (ArrowManager.Instance.PathTiles.Any()) ArrowManager.Instance.PathTiles.Last().SetUnit(Unit); //Si on a pas deplacement pas besoin de changer de case
                yield return StartCoroutine(UnitManager.Instance.FightRoutine(Unit, enemyTile.OccupiedUnit)); //Animation de combat
            }
            else // Priorité 2: Se déplacer vers l'ennemi le plus proche
            {
                List<Tile> pathToEnemy = Unit.OccupiedTile.FindNearestEnemy(Unit);
                if (pathToEnemy != null && pathToEnemy.Count > 1)
                {
                    // Trouver la tuile la plus éloignée sur le chemin qu'on peut atteindre
                    Tile destinationTile = null;
                    for (int i = pathToEnemy.Count - 1; i >= 0; i--)
                    {
                        if (Unit.OccupiedTile.BlueTiles.Contains(pathToEnemy[i]))
                        {
                            destinationTile = pathToEnemy[i];
                            break;
                        }
                    }

                    if (destinationTile != null)
                    {
                        Unit.OccupiedTile.HideRange();
                        ArrowManager.Instance.ShowPath(Unit.OccupiedTile, destinationTile, false);
                        yield return StartCoroutine(UnitManager.Instance.MoveUnit(Unit, ArrowManager.Instance.PathTiles));
                        destinationTile.SetUnit(Unit);
                    }
                    else
                    {
                        // Ne peut atteindre aucune tuile sur le chemin, mouvement aléatoire par défaut
                        MoveRandomly(Unit);
                    }
                }
                else
                {
                    // Pas d'ennemi trouvé, mouvement aléatoire par défaut
                    yield return StartCoroutine(MoveRandomly(Unit));
                }
            }

            if (GameManager.Instance.GameOver) yield break; // On arrête tout si la partie est finie
            UnitManager.Instance.Exhaustion(Unit); //On epuise l'unite
            UnitManager.Instance.LookTo(Unit, Unit.OccupiedTile, true); //Reinitialise l'animation de l'unité
        }
        _isPlaying = false;
        GameManager.Instance.NextTurn(); //On commence le prochain tour quand on a terminé
    }

    private IEnumerator MoveRandomly(BaseUnit Unit)
    {
        if (Unit.OccupiedTile.BlueTiles.Any())
        {
            Tile Tile = Unit.OccupiedTile.BlueTiles[Random.Range(0, Unit.OccupiedTile.BlueTiles.Count)]; //choix aléatoire
            Unit.OccupiedTile.HideRange(); //On cache sa portee
            ArrowManager.Instance.ShowPath(Unit.OccupiedTile, Tile, false); //Calcul du chemin
            yield return StartCoroutine(UnitManager.Instance.MoveUnit(Unit, ArrowManager.Instance.PathTiles)); //Animation de deplacement
            Tile.SetUnit(Unit); //On change sa case
        }
        else
        {
            Unit.OccupiedTile.HideRange(); // Cache la portée même si on ne bouge pas
        }
    }
}
