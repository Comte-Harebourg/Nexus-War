using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState GameState;
    public static event Action<GameState> OnGameStateChanged; //S'active si la phase change
    public int PlayerFaction; //Faction du joueur
    public GameObject UnitMap;
    public List<BaseUnit> AberrionUnits = new List<BaseUnit>();
    public List<BaseUnit> OromoundUnits = new List<BaseUnit>();
    public List<BaseUnit> SerannaUnits = new List<BaseUnit>();
    private int Turn = 0;

    void Awake()
    {
        Instance = this;
        TileMapManager.Instance.LoadMap(); //Lance la map sélectionné dans le TileMapManager, si on met un int ça chargera toujours cette map
    }

    private void Start()
    {
        ChangeState((GameState)PlayerFaction); //Ne pas mettre dans Awake sous peine de bug
    }

    public void ChangeState(GameState newState)
    {
        GameState = newState;
        SetAllActive();
        switch (newState)
        {
            case GameState.AberrionTurn:
                if (AberrionUnits.Count() != 0)
                {
                    Turn += 1;
                    MenuManager.Instance.UpdateTurn(Turn);
                    MenuManager.Instance.OromoundInfo.SetActive(false);
                    MenuManager.Instance.SerannaInfo.SetActive(false);
                    MenuManager.Instance.AberrionInfo.SetActive(true);
                    //Consomme les rations des unités
                    //Modifie les ressources de la faction
                    //Check la production d'unité
                    //Joue animation début tour Aberrion, se termine si clic gauche
                }
                else //Skip le tour si pas d'unité
                {
                    NextTurn();
                }
                break;
            case GameState.OromoundTurn:
                if (OromoundUnits.Count() != 0)
                {
                    Turn += 1;
                    MenuManager.Instance.UpdateTurn(Turn);
                    MenuManager.Instance.AberrionInfo.SetActive(false);
                    MenuManager.Instance.SerannaInfo.SetActive(false);
                    MenuManager.Instance.OromoundInfo.SetActive(true);
                    //Consomme les rations des unités
                    //Modifie les ressources de la faction
                    //Check la production d'unité
                    //Joue animation début tour Oromound, se termine si clic gauche
                }
                else //Skip le tour si pas d'unité
                {
                    NextTurn();
                }
                break;
            case GameState.SerannaTurn:
                if (SerannaUnits.Count() != 0)
                {
                    Turn += 1;
                    MenuManager.Instance.UpdateTurn(Turn);
                    MenuManager.Instance.AberrionInfo.SetActive(false);
                    MenuManager.Instance.OromoundInfo.SetActive(false);
                    MenuManager.Instance.SerannaInfo.SetActive(true);
                    //Consomme les rations des unités
                    //Modifie les ressources de la faction
                    //Check la production d'unité
                    //Joue animation début tour Seranna, se termine si clic gauche
                }
                else //Skip le tour si pas d'unité
                {
                    NextTurn();
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
        OnGameStateChanged?.Invoke(newState); //Active le changement de phase
    }

    public void UpdateUnits()
    {
        AberrionUnits.Clear();
        OromoundUnits.Clear();
        SerannaUnits.Clear();
        foreach (Transform child in UnitMap.transform)
        {
            BaseUnit unit = child.GetComponent<BaseUnit>();
            if (unit != null)
            {
                if (unit.Faction == (Faction)0)
                {
                    AberrionUnits.Add(unit);
                    Debug.Log(unit.name + " ajouté à la liste d'Aberrion.");
                }
                else if (unit.Faction == (Faction)1)
                {
                    OromoundUnits.Add(unit);
                    Debug.Log(unit.name + " ajouté à la liste d'Oromound.");
                }
                else if (unit.Faction == (Faction)2)
                {
                    SerannaUnits.Add(unit);
                    Debug.Log(unit.name + " ajouté à la liste de Seranna.");
                }
                else
                {
                    Debug.Log(unit.name + " n'a pas de faction.");
                }
            }
        }
    }

    public void NextTurn()
    {
        ChangeState((GameState)(((int)GameState + 1) % Enum.GetValues(typeof(GameState)).Length)); //Passe au prochain enum du tour
        PlayerFaction = (int)GameState; //Change la faction du joueur car par d'IA, à suppprimer plus tard
        UnitManager.Instance.ResetDanger(); //Comme le joueur change de faction il faut enlever le danger
    }

    public void SetAllActive()
    {
        foreach(BaseUnit Unit in AberrionUnits)
        {
            UnitManager.Instance.SetActive(Unit);
        }
        foreach (BaseUnit Unit in OromoundUnits)
        {
            UnitManager.Instance.SetActive(Unit);
        }
        foreach (BaseUnit Unit in SerannaUnits)
        {
            UnitManager.Instance.SetActive(Unit);
        }
    }
}

public enum GameState //Initialisation des différentes phases
{
    AberrionTurn = 0,
    OromoundTurn = 1,
    SerannaTurn = 2
}