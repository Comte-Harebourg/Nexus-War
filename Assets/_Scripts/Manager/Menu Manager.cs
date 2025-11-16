using UnityEngine;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    [SerializeField] private GameObject _tileObject,_tileUnitObject;

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
}
