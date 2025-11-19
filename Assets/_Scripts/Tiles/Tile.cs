using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tile : MonoBehaviour
{
    public string TileName;
    public string TileID;
    [SerializeField] protected SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight;
    [SerializeField] private GameObject _blue;
    [SerializeField] private GameObject _darkBlue;
    [SerializeField] private GameObject _red;
    [SerializeField] private GameObject _darkRed;
    [SerializeField] private GameObject _orange;
    [SerializeField] private GameObject _darkOrange;
    [SerializeField] private GameObject _green;
    [SerializeField] private GameObject _darkGreen;
    [SerializeField] private bool _isWalkable;
    [SerializeField] private bool _isRoadable;
    [SerializeField] private int cover;
    [SerializeField] private float cost;
    public BaseUnit OccupiedUnit;
    private List<Tile> BlueTiles = new List<Tile>();
    private List<Tile> DarkBlueTiles = new List<Tile>();
    private List<Tile> RedTiles = new List<Tile>();
    private List<Tile> DarkRedTiles = new List<Tile>();
    private List<Tile> OrangeTiles = new List<Tile>();
    private List<Tile> DarkOrangeTiles = new List<Tile>();
    private List<Tile> GreenTiles = new List<Tile>();
    private List<Tile> DarkGreenTiles = new List<Tile>();
    private List<Tile> Neighbors = new List<Tile>();
    public Vector2Int Position { get; set; }

    public virtual void Init(int x, int y) //Vide mais indispensable au chargement de la map
    {
        
    }

    private void OnMouseDown()
    {
        if (OccupiedUnit != null)
        {
            if (OccupiedUnit.Faction != (Faction)GameManager.Instance.PlayerFaction)
            {
                if (UnitManager.Instance.SelectedUnit == null)
                {
                    if (UnitManager.Instance.DangerUnits.Contains(OccupiedUnit))
                    {
                        HideDanger();
                        UnitManager.Instance.DangerUnits.Remove(OccupiedUnit);
                        UnitManager.Instance.UpdateDanger();
                    }
                    else
                    {
                        ShowDanger();
                        UnitManager.Instance.DangerUnits.Add(OccupiedUnit);
                    }
                }
                else
                {
                    if (UnitManager.Instance.SelectedUnit.OccupiedTile.RedTiles.Contains(this))
                    {
                        //Montre l'UI des actions (Géré par un autre script) avec l'unité de ciblée
                    }
                    else
                    {
                        UnitManager.Instance.UnSelectUnit();
                    }
                }
            }
            else
            {
                if (UnitManager.Instance.SelectedUnit == null)
                {
                    UnitManager.Instance.SelectUnit(OccupiedUnit);
                }
                else if (UnitManager.Instance.SelectedUnit == OccupiedUnit)
                {
                    //Montre l'UI des actions (Géré par un autre script)     
                }
                //Exceptions à faire pour certains cas comme les médecins et les structures
                else
                {
                    UnitManager.Instance.UnSelectUnit();
                }
            }
        }
        else
        {
            if (UnitManager.Instance.SelectedUnit != null)
            {
                //Si la case ciblée est à portée de déplacement
                //Montre l'UI des actions à la case ciblée (Géré par MenuManager)
                //Sinon
                //Déselectionne l'unité sélectionnée
            }
            else
            {
                //Montre l'UI du sous-menu (Géré par MenuManager)
            }
        }
    }

    void OnMouseEnter()
    {
        Debug.Log($"{TileName} at {Position} neighbors: {Neighbors.Count}"); //Debug
        _highlight.SetActive(true);
        MenuManager.Instance.ShowTileInfo(this);
    }

    void OnMouseExit()
    {
        _highlight.SetActive(false);
        MenuManager.Instance.ShowTileInfo(null);
    }

    public void SetUnit(BaseUnit Unit)//Rajouter évolution ShowDanger
    {
        if (Unit.OccupiedTile != null) Unit.OccupiedTile = null;
        Unit.transform.position = transform.position;
        OccupiedUnit = Unit;
        Unit.OccupiedTile = this;
    }

    public void ShowRange()
    {
        _blue.SetActive(true);
        BlueTiles.Add(this);
        SearchMovementRange(this, OccupiedUnit.speed, Neighbors, OccupiedUnit.Infantry, OccupiedUnit.Vehicle);
        SearchAttackRange(this, OccupiedUnit.minAttackRange, OccupiedUnit.maxAttackRange);
    }

    public void SearchMovementRange(Tile origin, float Speed, List<Tile> List, bool Infantry, bool Vehicle)
    {
        if (Speed < 0) return;
        foreach (Tile Tile in List)
        {
            if (Tile != origin && Speed - Tile.cost >= 0 && ((Infantry && Tile._isWalkable) || (Vehicle && Tile._isRoadable)))
            {
                if (Tile.OccupiedUnit == null)
                {
                    Tile._blue.SetActive(true);
                    origin.BlueTiles.Add(Tile);
                    SearchMovementRange(origin, Speed - Tile.cost, Tile.Neighbors, Infantry, Vehicle);
                }
                else if (Tile.OccupiedUnit.Faction == (Faction)GameManager.Instance.PlayerFaction)
                {
                    Tile._darkBlue.SetActive(true);
                    origin.DarkBlueTiles.Add(Tile);
                    SearchMovementRange(origin, Speed - Tile.cost, Tile.Neighbors, Infantry, Vehicle);
                }
            }
        }
    }

    public void SearchAttackRange(Tile origin, int minAttackRange, int maxAttackRange)
    {
        foreach (Tile Tile in origin.BlueTiles)
        {
            for (int dx = -maxAttackRange; dx <= maxAttackRange; dx++)
            {
                for (int dy = -maxAttackRange; dy <= maxAttackRange; dy++)
                {
                    int dist = Mathf.Abs(dx) + Mathf.Abs(dy);
                    if (dist < minAttackRange || dist > maxAttackRange) continue;
                    Vector2Int targetPos = Tile.Position + new Vector2Int(dx, dy);
                    Tile targetTile = GridManager.Instance.GetTileAtPosition(targetPos);
                    if (targetTile == null) continue;
                    if (targetTile.OccupiedUnit == null)
                    {
                        if (!origin.DarkRedTiles.Contains(targetTile))
                        {
                            targetTile._darkRed.SetActive(true);
                            origin.DarkRedTiles.Add(targetTile);
                        }
                    }
                    else if (targetTile.OccupiedUnit.Faction != (Faction)GameManager.Instance.PlayerFaction)
                    {
                        if (!origin.RedTiles.Contains(targetTile))
                        {
                            targetTile._red.SetActive(true);
                            origin.RedTiles.Add(targetTile);
                        }
                    }
                }
            }
        }
    }

    public void HideRange()
    {
        foreach (Tile Tile in BlueTiles)
        {
            Tile._blue.SetActive(false);
        }
        BlueTiles.Clear();
        foreach (Tile Tile in DarkBlueTiles)
        {
            Tile._darkBlue.SetActive(false);
        }
        DarkBlueTiles.Clear();
        foreach (Tile Tile in RedTiles)
        {
            Tile._red.SetActive(false);
        }
        RedTiles.Clear();
        foreach (Tile Tile in DarkRedTiles)
        {
            Tile._darkRed.SetActive(false);
        }
        DarkRedTiles.Clear();
    }

    public void ShowDanger()//Similaire à ShowRange mais pour les ennemis
    {
        _orange.SetActive(true);
        OrangeTiles.Add(this);
        SearchDangerMovementRange(this, OccupiedUnit.speed, Neighbors, OccupiedUnit.Infantry, OccupiedUnit.Vehicle);
        SearchDangerAttackRange(this, OccupiedUnit.minAttackRange, OccupiedUnit.maxAttackRange);
    }

    public void SearchDangerMovementRange(Tile origin, float Speed, List<Tile> List, bool Infantry, bool Vehicle)//Similaire à SearchMovementRange mais pour les ennemis
    {
        if (Speed < 0) return;
        foreach (Tile Tile in List)
        {
            if (Tile != origin && Speed - Tile.cost >= 0 && ((Infantry && Tile._isWalkable) || (Vehicle && Tile._isRoadable)))
            {
                if (Tile.OccupiedUnit == null)
                {
                    Tile._orange.SetActive(true);
                    origin.OrangeTiles.Add(Tile);
                    SearchDangerMovementRange(origin, Speed - Tile.cost, Tile.Neighbors, Infantry, Vehicle);
                }
                else if (Tile.OccupiedUnit.Faction == origin.OccupiedUnit.Faction)
                {
                    Tile._orange.SetActive(true);
                    origin.OrangeTiles.Add(Tile);
                    SearchDangerMovementRange(origin, Speed - Tile.cost, Tile.Neighbors, Infantry, Vehicle);
                }
            }
        }
    }

    public void SearchDangerAttackRange(Tile origin, int minAttackRange, int maxAttackRange)//Similaire à SearchAttackRange mais pour les ennemis
    {
        foreach (Tile Tile in origin.OrangeTiles)
        {
            for (int dx = -maxAttackRange; dx <= maxAttackRange; dx++)
            {
                for (int dy = -maxAttackRange; dy <= maxAttackRange; dy++)
                {
                    int dist = Mathf.Abs(dx) + Mathf.Abs(dy);
                    if (dist < minAttackRange || dist > maxAttackRange) continue;
                    Vector2Int targetPos = Tile.Position + new Vector2Int(dx, dy);
                    Tile targetTile = GridManager.Instance.GetTileAtPosition(targetPos);
                    if (targetTile == null) continue;
                    if (origin.OrangeTiles.Contains(targetTile)) continue;
                    if (targetTile.OccupiedUnit == null)
                    {
                        targetTile._darkOrange.SetActive(true);
                        origin.DarkOrangeTiles.Add(targetTile);
                    }
                    else if (targetTile.OccupiedUnit.Faction != origin.OccupiedUnit.Faction)
                    {
                        targetTile._darkOrange.SetActive(true);
                        origin.DarkOrangeTiles.Add(targetTile);
                    }
                }
            }
        }
    }

    public void HideDanger()//Similaire à ShowRange mais pour les ennemis
    {
        foreach (Tile Tile in OrangeTiles)
        {
            Tile._orange.SetActive(false);
        }
        OrangeTiles.Clear();
        foreach (Tile Tile in DarkOrangeTiles)
        {
            Tile._darkOrange.SetActive(false);
        }
        DarkOrangeTiles.Clear();
    }

    public void FindNeighbors()
    {
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        Neighbors.Clear();
        foreach (var dir in directions)
        {
            Tile neighbor = GridManager.Instance.GetTileAtPosition(Position + dir);
            if (neighbor != null)
                Neighbors.Add(neighbor);
        }
    }
}