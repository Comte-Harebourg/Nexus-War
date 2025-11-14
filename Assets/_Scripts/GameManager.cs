using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState GameState;
    public static event Action<GameState> OnGameStateChanged; ///S'active si la phase change

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ChangeState(GameState.GenerateGrid);
    }

    public void ChangeState(GameState newState)
    {
        GameState = newState;
        switch (newState)
        {
            case GameState.GenerateGrid: ///Génération de la grille
                GridManager.Instance.GenerateGrid();
                break;
            case GameState.SpawnPlayer:
                ///UnitManager.Instance.SpawnHeroes();
                break;
            case GameState.SpawnEnemie:
                ///UnitManager.Instance.SpawnEnemies();
                break;
            case GameState.PlayerTurn:
                break;
            case GameState.EnemieTurn:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
        OnGameStateChanged?.Invoke(newState); ///Active le changement de phase
    }
}

public enum GameState ///Initialisation des différentes phases
{
    GenerateGrid = 0,
    SpawnPlayer = 1,
    SpawnEnemie = 2,
    PlayerTurn = 3,
    EnemieTurn = 4
}