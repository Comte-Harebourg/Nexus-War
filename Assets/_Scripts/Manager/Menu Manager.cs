using UnityEngine;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    [SerializeField] private GameObject _selectedUnitObject,_tileObject,_tileUnitObject;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowTileInfo(Tile Tile)
    {
        if (Tile == null)
        {
            _tileObject.SetActive(false);
            _tileUnitObject.SetActive(false);
            return;
        }
        _tileObject.GetComponentInChildren<TMP_Text>().text = Tile.TileName;
        _tileObject.SetActive(true);
        if (Tile.OccupiedUnit)
        {
            _tileUnitObject.GetComponentInChildren<TMP_Text>().text = Tile.OccupiedUnit.UnitName;
            _tileUnitObject.SetActive(true);
        }
    }

    public void ShowSelectedUnit(BasePlayer Unit)
    {
        if (Unit == null)
        {
            _selectedUnitObject.SetActive(false);
            return;
        }
        _selectedUnitObject.GetComponentInChildren<TMP_Text>().text = Unit.UnitName;
        _selectedUnitObject.SetActive(true);
    }
}
