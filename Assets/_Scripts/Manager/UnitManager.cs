using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class UnitManager : MonoBehaviour //Permet de gérer les unités sélectionnées
{
    public static UnitManager Instance;
    private List<BaseUnit> _units;
    public BaseUnit SelectedUnit;
    public List<BaseUnit> DangerUnits = new List<BaseUnit>();

    private void Awake()
    {
        Instance = this;
    }

    public void SelectUnit(BaseUnit Unit)
    {
        SelectedUnit = Unit;
        Debug.Log($"Unité {Unit.name} sélectionnée");
        SelectedUnit.OccupiedTile.ShowRange(SelectedUnit);
    }

    public void UnSelectUnit()
    {
        SelectedUnit.OccupiedTile.HideRange();
        Debug.Log($"Unité {SelectedUnit.name} désélectionnée");
        SelectedUnit.Animator.Play("Down");
        SelectedUnit = null;
    }

    public void UpdateDanger()
    {
        foreach (BaseUnit Unit in DangerUnits)
        {
            Unit.OccupiedTile.HideDanger();
            Unit.OccupiedTile.ShowDanger();
        }
    }

    public void ResetDanger()
    {
        foreach (BaseUnit Unit in DangerUnits)
        {
            Unit.OccupiedTile.HideDanger();
        }
        DangerUnits.Clear();
    }

    public void Fight(BaseUnit Attacker, BaseUnit Defenser)
    {
        Debug.Log(Attacker.UnitName + " a attaqué " + Defenser.UnitName);
        Kill(Defenser); //Tue le défenseur
    }

    public void LookTo(BaseUnit Unit, Tile Tile, bool IsMoving) //Anime Unit pour regarder vers Tile et IsMoving s'il doit bougé et non etre figé
    {
        float x0 = Unit.OccupiedTile.transform.position.x;
        float y0 = Unit.OccupiedTile.transform.position.y;
        float x = Tile.transform.position.x;
        float y = Tile.transform.position.y;
        if (!Unit.isActive) Unit.Animator.Play("Down"); //Si l'unité est épuisée on reset son animation
        else if (y <= y0 && math.abs(x - x0) <= math.abs(y - y0))
        {
            if (IsMoving) Unit.Animator.Play("Down");
            else Unit.Animator.Play("SelectDown");
        }
        else if (y > y0 && math.abs(x - x0) < math.abs(y - y0))
        {
            if (IsMoving) Unit.Animator.Play("Up");
            else Unit.Animator.Play("SelectUp");
        }
        else if (x < x0)
        {
            if (IsMoving) Unit.Animator.Play("Left");
            else Unit.Animator.Play("SelectLeft");
        }
        else
        {
            if (IsMoving) Unit.Animator.Play("Right");
            else Unit.Animator.Play("SelectRight");
        }
    }

    public void SetActive(BaseUnit Unit)
    {
        Unit.isActive = true;
        Unit.Sprite.color = new Color32(255, 255, 255, 255);
    }

    public void Exhaustion(BaseUnit Unit)
    {
        Unit.isActive = false;
        Unit.Sprite.color = new Color32(128, 128, 128, 255);
    }

    public void Kill(BaseUnit Unit) //Tue l'unité sélectionnée
    {
        Unit.OccupiedTile.HideDanger();
        //Joue animation mort
        if (Unit.Faction == (Faction)0) GameManager.Instance.AberrionUnits.Remove(Unit);
        else if (Unit.Faction == (Faction)1) GameManager.Instance.OromoundUnits.Remove(Unit);
        else if (Unit.Faction == (Faction)2) GameManager.Instance.SerannaUnits.Remove(Unit);
        Destroy(Unit.gameObject);
    }
}