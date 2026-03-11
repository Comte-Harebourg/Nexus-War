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

    public void Fight(BaseUnit Attacker, BaseUnit Defenser)
    {
        Debug.Log(Attacker.UnitName + " a attaqué " + Defenser.UnitName);
    }

    public void LookTo(BaseUnit Unit, Tile Tile)
    {
        float x0 = Unit.OccupiedTile.transform.position.x;
        float y0 = Unit.OccupiedTile.transform.position.y;
        float x = Tile.transform.position.x;
        float y = Tile.transform.position.y;
        if (y <= y0 && math.abs(x - x0) <= math.abs(y - y0))
        {
            Unit.Animator.Play("Down");
        }
        else if (y > y0 && math.abs(x - x0) < math.abs(y - y0))
        {
            Unit.Animator.Play("Up");
        }
        else if (x < x0)
        {
            Unit.Animator.Play("Left");
        }
        else
        {
            Unit.Animator.Play("Right");
        }
    }

    public void LookReset(BaseUnit Unit)
    {
        Unit.Animator.Play("Down");
    }
}