using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;
    private List<ScriptableUnit> _units;

    private void Awake()
    {
        Instance = this;
        _units = Resources.LoadAll<ScriptableUnit>("Units").ToList();
    }

    private Unit ChoosePlayerUnit<Unit>(Faction faction) where Unit : BaseUnit
    {
        return (Unit)_units[0].UnitPrefab; ///Sélection de Serge comme unité donc à refaire lorsqu'il y aura d'autre unités
    }

    private Unit ChooseEnemyUnit<Unit>(Faction faction) where Unit : BaseUnit
    {
        return (Unit)_units[1].UnitPrefab; ///Sélection de Tibi comme unité donc à refaire lorsqu'il y aura d'autre unités
    }

    ///J'ai fait quatres fonctions qui pourrait tenir en une donc il faudrait revoir tout ça pour l'optimiser, surtout lorsqu'il y aura les trois factions à gérer

    public void SpawnPlayer()
    {
        var PlayerCount = 1; ///Nombre de Serge à faire spawn
        for (int i = 0; i < PlayerCount; i++)
        { 
            var SpawnedPlayer = Instantiate(ChoosePlayerUnit<BasePlayer>(Faction.Player));
            var SpawnTile = GridManager.Instance.GetPlayerSpawn();
            SpawnTile.SetUnit(SpawnedPlayer);
        }
        GameManager.Instance.ChangeState(GameState.SpawnEnemy);
    }

    public void SpawnEnemy()
    {
        var EnemyCount = 1; ///Nombre de Tibix à faire spawn
        for (int i = 0; i < EnemyCount; i++)
        {
            var SpawnedEnemy = Instantiate(ChooseEnemyUnit<BaseEnemy>(Faction.Enemy));
            var SpawnTile = GridManager.Instance.GetEnemySpawn();
            SpawnTile.SetUnit(SpawnedEnemy);
        }
        GameManager.Instance.ChangeState(GameState.PlayerTurn);
    }
}