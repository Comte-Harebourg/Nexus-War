using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState GameState;
    public static event Action<GameState> OnGameStateChanged; ///S'active si la phase change
    public Faction PlayerFaction; ///Faction du joueur

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
            case GameState.Spawn:
                UnitManager.Instance.Spawn();
                break;
            case GameState.PlayerTurn:
                break;
            case GameState.Enemy1Turn:
                break;
            case GameState.Enemy2Turn:
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
    Spawn = 1,
    PlayerTurn = 2,
    Enemy1Turn = 3,
    Enemy2Turn = 4
}