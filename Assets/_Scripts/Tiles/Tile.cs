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


    public virtual void Init(int x, int y)
    {
        /// <summary>
        /// Init(int x, int y) initialise une case à la position (x,y)
        /// <summary>
    }

    private void OnMouseDown()
    {
        ///if (OccupiedUnit.Faction==) Logique des clics à faire
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

    public void SetUnit(BaseUnit Unit)
    {
        if (Unit.OccupiedTile != null) Unit.OccupiedTile = null;
        Unit.transform.position = transform.position;
        OccupiedUnit = Unit;
        Unit.OccupiedTile = this;
    }
}