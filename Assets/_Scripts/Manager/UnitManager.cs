using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitManager : MonoBehaviour //Permet de gérer les unités sélectionnées
{
    public static UnitManager Instance;
    private List<ScriptableUnit> _units;
    public BaseUnit SelectedUnit;

    private void Awake()
    {
        Instance = this;
    }
}