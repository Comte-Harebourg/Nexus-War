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
    [SerializeField] private bool _isWalkable;
    [SerializeField] private bool _isRoadable;
    [SerializeField] private int cover;
    [SerializeField] private float cost;
    public BaseUnit OccupiedUnit;
    public bool Walkable => _isWalkable && OccupiedUnit == null;
    public bool Roadable => _isRoadable && OccupiedUnit == null;
    private List<Tile> BlueTiles = new List<Tile>();
    private List<Tile> DarkBlueTiles = new List<Tile>();
    private List<Tile> Neighbors = new List<Tile>();
    public Vector2Int Position { get; set; }


    public virtual void Init(int x, int y)
    {
        /// <summary>
        /// Init(int x, int y) initialise une case à la position (x,y)
        /// <summary>
    }

    public void Start()//Cherche toutes les tuiles voisines
    {

    }

    private void OnMouseDown()
    {
        if (OccupiedUnit != null)
        {
            if (OccupiedUnit.Faction != (Faction)GameManager.Instance.PlayerFaction)
            {
                if (UnitManager.Instance.SelectedUnit != null)
                {
                    //Montre la portée de l'unité ennemie
                }
                else
                {
                    if (false)//Si l'unité ciblée est à portée d'attaque
                    {
                        //Déplace lunité sélectionnée à portée d'attaque
                        //Attaque l'unité ciblée avec l'unité sélectionnée
                        //Déselectionne l'unité sélectionnée
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
                //Montre l'UI des actions (Géré par MenuManager)
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
        Debug.Log($"{TileName} at {Position} neighbors: {Neighbors.Count}");
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

    public void ShowRange()
    {
        _blue.SetActive(true);
        SearchRange(this, OccupiedUnit.speed, Neighbors, OccupiedUnit.Infantry, OccupiedUnit.Vehicle);
    }

    public void SearchRange(Tile FirstTile, float Speed, List<Tile> List, bool Infantry, bool Vehicle)
    {
        if (Speed < 0) return;
        foreach (Tile Tile in List)
        {
            if (Tile != FirstTile && Speed - Tile.cost >= 0 && ((Infantry && Tile._isWalkable) || (Vehicle && Tile._isRoadable)))
            {
                if (Tile.OccupiedUnit == null)
                {
                    Tile._blue.SetActive(true);
                    FirstTile.BlueTiles.Add(Tile);
                    SearchRange(FirstTile, Speed - Tile.cost, Tile.Neighbors, Infantry, Vehicle);
                }
                else if (Tile.OccupiedUnit.Faction == (Faction)GameManager.Instance.PlayerFaction)
                {
                    Tile._darkBlue.SetActive(true);
                    FirstTile.DarkBlueTiles.Add(Tile);
                    SearchRange(FirstTile, Speed - Tile.cost, Tile.Neighbors, Infantry, Vehicle);
                }
            }
        }
    }

    public void HideRange()
    {
        _blue.SetActive(false);
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
    }

    public void InitPosition() // On arrondit la position du monde pour obtenir les coordonnées de grille
    {
        Position = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
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