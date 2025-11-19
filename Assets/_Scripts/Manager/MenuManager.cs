using UnityEngine;
using TMPro;

public class MenuManager : MonoBehaviour //Gère l'affichage de l'UI
{
    public static MenuManager Instance;
    [SerializeField] private GameObject _tileObject,_tileUnitObject,_background;
    public GameObject AberrionInfo,SerannaInfo,OromoundInfo;

    private void Awake()
    {
        Instance = this;
        _background.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (UnitManager.Instance.SelectedUnit != null)
            {
                UnitManager.Instance.UnSelectUnit();
            }
            else
            {
                //Montre l'inventaire de _tileUnit
            }
        }
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
