using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState GameState;
    public static event Action<GameState> OnGameStateChanged; //S'active si la phase change
    public int PlayerFaction; //Faction du joueur

    void Awake()
    {
        Instance = this;
        TileMapManager.Instance.LoadMap(); //Lance la map sélectionné dans le TileMapManager, si on met un int ça chargera toujours cette map
        ChangeState((GameState)PlayerFaction);
    }

    public void ChangeState(GameState newState)
    {
        GameState = newState;
        switch (newState)
        {
            case GameState.AberrionTurn:
                //Remet toutes les unités en actif
                //Consomme les rations des unités
                //Modifie les ressources de la faction
                //Check la production d'unité
                //Joue animation début tour Aberrion, se termine si clic gauche
                //Passe à Oromound si fin du tour ou si plus d'unités actives
                break;
            case GameState.OromoundTurn:
                //Remet toutes les unités en actif
                //Consomme les rations des unités
                //Modifie les ressources de la faction
                //Check la production d'unité
                //Joue animation début tour Oromound, se termine si clic gauche
                //Passe à Seranna si fin du tour
                break;
            case GameState.SerannaTurn:
                //Remet toutes les unités en actif
                //Consomme les rations des unités
                //Modifie les ressources de la faction
                //Check la production d'unité
                //Joue animation début tour Seranna, se termine si clic gauche
                //Passe à Aberrion si fin du tour
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