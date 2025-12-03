using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour //Gère l'affichage de l'UI
{
    public static MenuManager Instance;
    [SerializeField] private GameObject _tileObject,_tileUnitObject,_background;
    public GameObject AberrionInfo,SerannaInfo,OromoundInfo;
    public GameObject ActionMenue;
    public Tile HighlightedTile;

    private void Awake()
    {
        Instance = this;
        _background.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) //Click logic
        {
            if (GridManager.Instance.MenueDisplay == true)
            {
                GridManager.Instance.MenueDisplay = false;
                ActionMenue.SetActive(false);
                if (UnitManager.Instance.SelectedUnit != null)
                {
                    UnitManager.Instance.SelectedUnit.OccupiedTile.HideRange();
                    UnitManager.Instance.SelectedUnit.OccupiedTile.ShowRange(UnitManager.Instance.SelectedUnit);
                }
                if (HighlightedTile != null)
                {
                    HighlightedTile.Highlight.SetActive(false);
                    HighlightedTile = null;
                }
                if (GridManager.Instance.GetTileUnderMouse() != null)
                {
                    GridManager.Instance.GetTileUnderMouse().Highlight.SetActive(true);
                }
            }
            else if (UnitManager.Instance.SelectedUnit != null)
            {
                PathfindingManager.Instance.ClearArrow();
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

    public void ShowActionUI(Tile Tile)
    {
        ActionMenue.transform.position = new Vector3(2f + Tile.Position.x, 0.5f + Tile.Position.y, 0);
        ActionMenue.SetActive(true);
    }
}
