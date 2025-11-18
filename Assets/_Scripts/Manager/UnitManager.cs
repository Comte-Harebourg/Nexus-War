using System.Collections.Generic;
using System.Linq;
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
        SelectedUnit.OccupiedTile.ShowRange();
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
}