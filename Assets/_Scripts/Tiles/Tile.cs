using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tile : MonoBehaviour
{
    public string TileName;
    [SerializeField] protected SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight;
    [SerializeField] private bool _isWalkable;
    [SerializeField] private bool _isRoadable;

    public BaseUnit OccupiedUnit;
    public bool Walkable => _isWalkable && OccupiedUnit == null;
    public bool Roadable => _isRoadable && OccupiedUnit == null;


    public virtual void Init(int x,int y)
    {

    }

    private void OnMouseDown()
    {
        if (GameManager.Instance.GameState != GameState.PlayerTurn) return;
        if (OccupiedUnit != null)
        {
            if (OccupiedUnit.Faction == Faction.Player) UnitManager.Instance.SetSelectedUnit((BasePlayer)OccupiedUnit);
            else if(UnitManager.Instance.SelectedUnit != null)
            {
                    var enemy = (BaseEnemy)OccupiedUnit; ///attaque de la cible
                    Destroy(enemy.gameObject);
                    UnitManager.Instance.SetSelectedUnit(null);
            }  
        else if (UnitManager.Instance.SelectedUnit != null)
        {
                SetUnit(UnitManager.Instance.SelectedUnit);
                UnitManager.Instance.SetSelectedUnit(null);
        }
        }
    }

    void OnMouseEnter()
    {
        _highlight.SetActive(true);
        MenuManager.Instance.ShowTileInfo(this);
    }

    void OnMouseExit()
    {
        _highlight.SetActive(false);
        MenuManager.Instance.ShowTileInfo(null);
    }

    public void SetUnit(BaseUnit unit)
    {
        if (unit.OccupiedTile != null) unit.OccupiedTile = null;
        unit.transform.position = transform.position;
        OccupiedUnit = unit;
        unit.OccupiedTile = this;
    }
}