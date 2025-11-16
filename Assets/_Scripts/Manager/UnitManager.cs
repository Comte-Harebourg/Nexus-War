using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;
    private List<ScriptableUnit> _units;
    public BaseUnit SelectedUnit;

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

    public void Spawn(int IntFaction) ///Fonction à refaire intégralement lorsqu'on saura faire des maps préfaites
    {
        GameManager.Instance.PlayerFaction = IntFaction;
        var PlayerCount = 1; ///Nombre de Serge à faire spawn
        for (int i = 0; i < PlayerCount; i++)
        { 
            var SpawnedPlayer = Instantiate(ChoosePlayerUnit<BaseUnit>(Faction.Aberrion));
            var SpawnTile = GridManager.Instance.GetPlayerSpawn();
            SpawnTile.SetUnit(SpawnedPlayer);
        }
        var EnemyCount = 1; ///Nombre de Tibix à faire spawn
        for (int i = 0; i < EnemyCount; i++)
        {
            var SpawnedEnemy = Instantiate(ChooseEnemyUnit<BaseUnit>(Faction.Seranna));
            var SpawnTile = GridManager.Instance.GetEnemySpawn();
            SpawnTile.SetUnit(SpawnedEnemy);
        }
    }
}