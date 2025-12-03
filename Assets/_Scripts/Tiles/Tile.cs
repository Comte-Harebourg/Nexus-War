using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Tile : MonoBehaviour
{
    public string TileName;
    public string TileID;
    [SerializeField] protected SpriteRenderer _renderer;
    public GameObject Highlight;
    [SerializeField] private GameObject _blue;
    [SerializeField] private GameObject _darkBlue;
    [SerializeField] private GameObject _red;
    [SerializeField] private GameObject _darkRed;
    [SerializeField] private GameObject _orange;
    [SerializeField] private GameObject _darkOrange;
    [SerializeField] private GameObject _green;
    [SerializeField] private GameObject _darkGreen;
    public bool Walkable;
    public bool Roadable;
    [SerializeField] private int cover;
    public float cost;
    public BaseUnit OccupiedUnit;
    public List<Tile> BlueTiles = new List<Tile>();
    private List<Tile> DarkBlueTiles = new List<Tile>();
    private List<Tile> RedTiles = new List<Tile>();
    private List<Tile> DarkRedTiles = new List<Tile>();
    private List<Tile> OrangeTiles = new List<Tile>();
    private List<Tile> DarkOrangeTiles = new List<Tile>();
    private List<Tile> GreenTiles = new List<Tile>();
    private List<Tile> DarkGreenTiles = new List<Tile>();
    public List<Tile> Neighbors = new List<Tile>();
    public Vector2Int Position { get; set; }
    public float g_cost = 0;//Pour le pathfinding
    public float h_cost = 0;//Pour le pathfinding
    public float f_cost = 0;//Pour le pathfinding
    public Tile PrecedentTile = null;//Pour le pathfinding

    public virtual void Init(int x, int y) //Vide mais indispensable au chargement de la map
    {
        
    }

    private void OnMouseDown()
    {
        if (GridManager.Instance.MenueDisplay) return;
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
                    if (UnitManager.Instance.SelectedUnit.OccupiedTile.RedTiles.Contains(this))//Action UI si cible un ennemi
                    {
                        UnitManager.Instance.SelectedUnit.OccupiedTile.HideRange();
                        GridManager.Instance.MenueDisplay = true;
                        _red.SetActive(true);
                        RedTiles.Clear();
                        RedTiles.Add(this);
                        MenuManager.Instance.ShowActionUI(this);
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
                    if (OccupiedUnit.isActive == true) UnitManager.Instance.SelectUnit(OccupiedUnit);
                }
                else if (UnitManager.Instance.SelectedUnit == OccupiedUnit)//Action UI si cible soi-même
                {
                    HideRange();
                    GridManager.Instance.MenueDisplay = true;
                    ShowAttackRange(UnitManager.Instance.SelectedUnit);
                    MenuManager.Instance.ShowActionUI(this);
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
                if (UnitManager.Instance.SelectedUnit.OccupiedTile.BlueTiles.Contains(this))//ActionUI si cible une case vide
                {
                    MenuManager.Instance.HighlightedTile = this;
                    UnitManager.Instance.SelectedUnit.OccupiedTile.HideRange();
                    GridManager.Instance.MenueDisplay = true;
                    ShowAttackRange(UnitManager.Instance.SelectedUnit);
                    MenuManager.Instance.ShowActionUI(this);
                }
                else
                {
                    UnitManager.Instance.UnSelectUnit();
                }
            }
            else
            {
                //MenueDisplay = true;
                //Montre l'UI du sous-menu (Géré par MenuManager)
            }
        }
    }

    public void OnMouseEnter()
    {
        if (GridManager.Instance.MenueDisplay) return;
        Highlight.SetActive(true);
        MenuManager.Instance.ShowTileInfo(this);
        if (UnitManager.Instance.SelectedUnit != null)
        {
            if (UnitManager.Instance.SelectedUnit.OccupiedTile.BlueTiles.Contains(this))
            {
                if (UnitManager.Instance.SelectedUnit.OccupiedTile != this)
                {
                    PathfindingManager.Instance.ShowPath(UnitManager.Instance.SelectedUnit.OccupiedTile, this, UnitManager.Instance.SelectedUnit);
                }
                else PathfindingManager.Instance.ClearArrow();
            }
        }
        Debug.Log($"{TileName} at {Position} neighbors: {Neighbors.Count}");
    }

    void OnMouseExit()
    {
        if (GridManager.Instance.MenueDisplay) return;
        Highlight.SetActive(false);
        MenuManager.Instance.ShowTileInfo(null);
    }

    public void SetUnit(BaseUnit Unit)//Fonction pour la génération de l'unité à ne pas utiliser pour les déplacements
    {
        if (Unit.OccupiedTile != null) Unit.OccupiedTile = null;
        Unit.transform.position = transform.position;
        OccupiedUnit = Unit;
        Unit.OccupiedTile = this;
    }

    public void ShowRange(BaseUnit Unit)
    {
        _blue.SetActive(true);
        BlueTiles.Add(this);
        SearchMovementRange(this, Neighbors, Unit, Unit.speed);
        SearchAttackRange(this, Unit);
    }

    public void ShowAttackRange(BaseUnit Unit)
    {
        _blue.SetActive(true);
        BlueTiles.Add(this);
        SearchMovementRange(this, Neighbors, Unit, 0);
        SearchAttackRange(this, Unit);
    }

    public void SearchMovementRange(Tile origin, List<Tile> List, BaseUnit Unit, float Speed)
    {
        if (Speed < 0) return;
        foreach (Tile Tile in List)
        {
            if (Tile != origin && Speed - Tile.cost >= 0 && ((Unit.Infantry && Tile.Walkable) || (Unit.Vehicle && Tile.Roadable)))
            {
                if (Tile.OccupiedUnit == null)
                {
                    Tile._blue.SetActive(true);
                    origin.BlueTiles.Add(Tile);
                    SearchMovementRange(origin, Tile.Neighbors, Unit, Speed - Tile.cost);
                }
                else if (Tile.OccupiedUnit.Faction == (Faction)GameManager.Instance.PlayerFaction)
                {
                    Tile._darkBlue.SetActive(true);
                    origin.DarkBlueTiles.Add(Tile);
                    SearchMovementRange(origin, Tile.Neighbors, Unit, Speed - Tile.cost);
                }
            }
        }
    }

    public void SearchAttackRange(Tile origin, BaseUnit Unit)
    {
        foreach (Tile Tile in origin.BlueTiles)
        {
            for (int dx = -Unit.maxAttackRange; dx <= Unit.maxAttackRange; dx++)
            {
                for (int dy = -Unit.maxAttackRange; dy <= Unit.maxAttackRange; dy++)
                {
                    int dist = Mathf.Abs(dx) + Mathf.Abs(dy);
                    if (dist < Unit.minAttackRange || dist > Unit.maxAttackRange) continue;
                    Vector2Int targetPos = Tile.Position + new Vector2Int(dx, dy);
                    Tile targetTile = GridManager.Instance.GetTileAtPosition(targetPos);
                    if (targetTile == null) continue;
                    if (targetTile.OccupiedUnit == null || Unit.OccupiedTile == targetTile)
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
            if (Tile != origin && Speed - Tile.cost >= 0 && ((Infantry && Tile.Walkable) || (Vehicle && Tile.Roadable)))
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