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
    private Tile MoveTile;
    private Tile AttackTile;

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
                Cancel();
            }
            else if (UnitManager.Instance.SelectedUnit != null)
            {
                ArrowManager.Instance.ClearArrow();
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
        else
        {
            _tileUnitObject.SetActive(false);
        }
    }

    public void ShowActionUI(Tile MTile, Tile ATile)
    {
        ActionMenue.transform.position = new Vector3(2f + ATile.Position.x, 0.5f + ATile.Position.y, 0);
        MoveTile = MTile;
        AttackTile = ATile;
        ActionMenue.SetActive(true);
    }

    public void Attack()
    {
        if (MoveTile != AttackTile)
        {
            MoveTile.SetUnit(UnitManager.Instance.SelectedUnit);
            UnitManager.Instance.Fight(UnitManager.Instance.SelectedUnit, AttackTile.OccupiedUnit);
            AttackTile.Highlight.SetActive(false);
        }
        else
        {

        }
        //Animation déplacement
        //Animation attaque
        //Épuisement unité
        Cancel();
        ArrowManager.Instance.ClearArrow();
        UnitManager.Instance.UnSelectUnit();
    }

    public void Wait()
    {
        MoveTile.SetUnit(UnitManager.Instance.SelectedUnit);
        if (MoveTile != AttackTile) AttackTile.Highlight.SetActive(false);
        //Animation déplacement
        //Épuisement unité
        Cancel();
        ArrowManager.Instance.ClearArrow();
        UnitManager.Instance.UnSelectUnit();
    }

    public void Cancel()
    {
        GridManager.Instance.MenueDisplay = false;
        ActionMenue.SetActive(false);
        if (HighlightedTile != null)
        {
            HighlightedTile.HideRange();
            HighlightedTile.Highlight.SetActive(false);
            HighlightedTile = null;
        }
        if (UnitManager.Instance.SelectedUnit != null)
        {
            UnitManager.Instance.SelectedUnit.OccupiedTile.HideRange();
            UnitManager.Instance.SelectedUnit.OccupiedTile.ShowRange(UnitManager.Instance.SelectedUnit);
        }
        if (GridManager.Instance.GetTileUnderMouse() != null)
        {
            GridManager.Instance.GetTileUnderMouse().Highlight.SetActive(true);
            GridManager.Instance.GetTileUnderMouse().OnMouseEnter();
        }
        MoveTile = null;
        AttackTile = null;
        ShowTileInfo(GridManager.Instance.GetTileUnderMouse());
    }

    public void EndTurn()
    {
        GameManager.Instance.NextTurn();
    }
}
