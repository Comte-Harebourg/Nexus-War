using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState GameState;
    public static event Action<GameState> OnGameStateChanged; ///S'active si la phase change
    public int PlayerFaction; ///Faction du joueur

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GridManager.Instance.GenerateGrid(16,9);
        UnitManager.Instance.Spawn(0); ///La faction est définie sur Aberrion pour l'instant
        ChangeState((GameState)PlayerFaction);
    }

    public void ChangeState(GameState newState)
    {
        GameState = newState;
        switch (newState)
        {
            case GameState.AberrionTurn:
                break;
            case GameState.OromoundTurn:
                break;
            case GameState.SerannaTurn:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
        OnGameStateChanged?.Invoke(newState); ///Active le changement de phase
    }
}

public enum GameState ///Initialisation des différentes phases
{
    AberrionTurn = 0,
    OromoundTurn = 1,
    SerannaTurn = 2
}