using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tile : MonoBehaviour
{
    [Header("Infos sur la Tuile")]
    public string TileName;
    public string TileID;
    [SerializeField] protected SpriteRenderer _renderer;
    public GameObject Highlight;

    [Header("Overlays Visuels")]
    [SerializeField] private GameObject _blue;       // Portée de deplacement (Vide)
    [SerializeField] private GameObject _darkBlue;   // Portee de deplacement (Allie present)
    [SerializeField] private GameObject _red;        // Portee d'attaque (Ennemis)
    [SerializeField] private GameObject _darkRed;    // Portee d'attaque (Vide/Allie)
    [SerializeField] private GameObject _orange;     // Portee de deplacement des ennemis
    [SerializeField] private GameObject _darkOrange; // Portee d'attaque des ennemis

    [Header("Stats et Statuts")]
    public bool Walkable;
    public bool Roadable;
    [SerializeField] private int cover;
    public float cost;
    public BaseUnit OccupiedUnit;

    [Header("Listes pour le nettoyage")]
    public List<Tile> BlueTiles = new List<Tile>();
    private List<Tile> DarkBlueTiles = new List<Tile>();
    public List<Tile> RedTiles = new List<Tile>();
    private List<Tile> DarkRedTiles = new List<Tile>();
    public List<Tile> OrangeTiles = new List<Tile>();
    private List<Tile> DarkOrangeTiles = new List<Tile>();
    private List<Tile> GreenTiles = new List<Tile>();
    private List<Tile> DarkGreenTiles = new List<Tile>();

    [Header("Pathfinding et Grille")]
    public List<Tile> Neighbors = new List<Tile>();
    public Vector2Int Position { get; set; }
    [HideInInspector] public Tile ParentTile = null;

    public virtual void Init(int x, int y) { }

    #region Logique d'interaction

    private void OnMouseDown()
    {
        if (GridManager.Instance.MenueDisplay) return;

        if (OccupiedUnit != null)
        {
            // TUILE OCCUPEE PAR UN ENNEMI
            if (OccupiedUnit.Faction != (Faction)GameManager.Instance.PlayerFaction)
            {
                if (UnitManager.Instance.SelectedUnit == null)
                {
                    HandleDangerToggle();
                }
                else if (UnitManager.Instance.SelectedUnit.OccupiedTile.RedTiles.Contains(this))
                {
                    // SELECTION D'UN ENNEMI POUR UNE ACTION
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
            // TUILE OCCUPEE PAR UN ALLIE
            else
            {
                if (UnitManager.Instance.SelectedUnit == null)
                {
                    if (OccupiedUnit.isActive) UnitManager.Instance.SelectUnit(OccupiedUnit);
                }
                else if (UnitManager.Instance.SelectedUnit == OccupiedUnit)
                {
                    // Clic sur soi-meme: on montre le menu d'action a l'endroit actuel
                    HideRange();
                    GridManager.Instance.MenueDisplay = true;
                    ShowAttackRange(UnitManager.Instance.SelectedUnit);
                    MenuManager.Instance.ShowActionUI(this);
                }
                else
                {
                    UnitManager.Instance.UnSelectUnit();
                }
            }
        }
        else // TUILE VIDE
        {
            if (UnitManager.Instance.SelectedUnit != null)
            {
                if (UnitManager.Instance.SelectedUnit.OccupiedTile.BlueTiles.Contains(this))
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
        }
    }

    private void HandleDangerToggle()
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

    public void OnMouseEnter()
    {
        if (GridManager.Instance.MenueDisplay) return;
        Highlight.SetActive(true);
        MenuManager.Instance.ShowTileInfo(this);

        if (UnitManager.Instance.SelectedUnit != null)
        {
            var selectedTile = UnitManager.Instance.SelectedUnit.OccupiedTile;
            // Dans Tile.OnMouseEnter()
            if (selectedTile.BlueTiles.Contains(this))
            {
                if (selectedTile != this)
                {
                    // Plus besoin de passer Unit anymore, le chemin est déja dans les Tuiles
                    ArrowManager.Instance.ShowPath(selectedTile, this);
                }
                else ArrowManager.Instance.ClearArrow();
            }
        }
    }

    private void OnMouseExit()
    {
        if (GridManager.Instance.MenueDisplay) return;
        Highlight.SetActive(false);
        MenuManager.Instance.ShowTileInfo(null);
    }

    #endregion

    // Une seule fonction pour tout les types de recherches différents (porte de deplacement, d'attaque, de danger)
    #region Recherche Unifiée

    public void ShowRange(BaseUnit unit) => PerformGenericSearch(unit, unit.speed, false);
    public void ShowAttackRange(BaseUnit unit) => PerformGenericSearch(unit, 0, false);
    public void ShowDanger() => PerformGenericSearch(OccupiedUnit, OccupiedUnit.speed, true);

    private void PerformGenericSearch(BaseUnit unit, float speed, bool isDanger)
{
        this.ParentTile = null; // On supprime le parent de la tuile de départ
        SetOverlayState(isDanger ? _orange : _blue, true);
        
        // Tuile de depart
        SetOverlayState(isDanger ? _orange : _blue, true);
        (isDanger ? OrangeTiles : BlueTiles).Add(this);

        // 1. Recherche des tuiles accessibles
        Dictionary<Tile, float> visited = new Dictionary<Tile, float>();
        CalculateMovement(this, speed, unit, isDanger, visited);

        // 2. Recherche de l'attaque (calculée à partir de toutes les tuiles accessibles)
        CalculateAttack(unit, isDanger, isDanger ? OrangeTiles : BlueTiles);
    }

    private void CalculateMovement(Tile current, float remainingSpeed, BaseUnit unit, bool isDanger, Dictionary<Tile, float> visited)
    {
        visited[current] = remainingSpeed;

        foreach (Tile neighbor in current.Neighbors)
        {
            float nextSpeed = remainingSpeed - neighbor.cost;
            if (nextSpeed < 0) continue;

            // Contraintes de mouvement des Unités
            bool canEnter = (unit.Infantry && neighbor.Walkable) || (unit.Vehicle && neighbor.Roadable);
            if (!canEnter) continue;

            // Contraintes de faciton (Blocage si un ennemi est sur la tuile)
            bool hasEnemy = neighbor.OccupiedUnit != null && neighbor.OccupiedUnit.Faction != unit.Faction;
            if (hasEnemy) continue;

            // Optimisation: Recursion seulement si ce chemin est moins couteux qu'un déja trouve
            if (!visited.ContainsKey(neighbor) || visited[neighbor] < nextSpeed)
            {
                // ON ENREGISTRE LE CHEMIN
                neighbor.ParentTile = current;

                bool isAlly = neighbor.OccupiedUnit != null && neighbor.OccupiedUnit.Faction == unit.Faction;
                ApplyMovementVisuals(neighbor, isAlly, isDanger);
                CalculateMovement(neighbor, nextSpeed, unit, isDanger, visited);
            }
        }
    }

    private void ApplyMovementVisuals(Tile target, bool isAlly, bool isDanger)
    {
        if (isDanger)
        {
            if (!OrangeTiles.Contains(target))
            {
                target._orange.SetActive(true);
                OrangeTiles.Add(target);
            }
        }
        else
        {
            if (isAlly)
            {
                if (!DarkBlueTiles.Contains(target)) { target._darkBlue.SetActive(true); DarkBlueTiles.Add(target); }
            }
            else
            {
                if (!BlueTiles.Contains(target)) { target._blue.SetActive(true); BlueTiles.Add(target); }
            }
        }
    }

    private void CalculateAttack(BaseUnit unit, bool isDanger, List<Tile> moveRange)
    {
        foreach (Tile moveTile in moveRange)
        {
            for (int dx = -unit.maxAttackRange; dx <= unit.maxAttackRange; dx++)
            {
                for (int dy = -unit.maxAttackRange; dy <= unit.maxAttackRange; dy++)
                {
                    int dist = Mathf.Abs(dx) + Mathf.Abs(dy);
                    if (dist < unit.minAttackRange || dist > unit.maxAttackRange) continue;

                    Tile target = GridManager.Instance.GetTileAtPosition(moveTile.Position + new Vector2Int(dx, dy));
                    if (target == null) continue;

                    ApplyAttackVisuals(target, unit, isDanger);
                }
            }
        }
    }

    private void ApplyAttackVisuals(Tile target, BaseUnit unit, bool isDanger)
    {
        if (isDanger)
        {
            // Le danger n'est affiché que sur les tuiles qui ne sont PAS marquées pour le mouvement
            if (!OrangeTiles.Contains(target) && !DarkOrangeTiles.Contains(target))
            {
                target._darkOrange.SetActive(true);
                DarkOrangeTiles.Add(target);
            }
        }
        else
        {
            bool isEnemy = target.OccupiedUnit != null && target.OccupiedUnit.Faction != unit.Faction;
            if (isEnemy)
            {
                if (!RedTiles.Contains(target)) { target._red.SetActive(true); RedTiles.Add(target); }
            }
            else if (target.OccupiedUnit != unit) // Ne surligne pas sa propre case comme portée d'attaque
            {
                if (!DarkRedTiles.Contains(target)) { target._darkRed.SetActive(true); DarkRedTiles.Add(target); }
            }
        }
    }

    #endregion

    #region Helper Methods

    public void SetUnit(BaseUnit Unit)
    {
        if (Unit.OccupiedTile != null) Unit.OccupiedTile.OccupiedUnit = null;
        Unit.transform.position = transform.position;
        OccupiedUnit = Unit;
        Unit.OccupiedTile = this;
    }

    public void HideRange()
    {
        ClearList(BlueTiles, "_blue");
        ClearList(DarkBlueTiles, "_darkBlue");
        ClearList(RedTiles, "_red");
        ClearList(DarkRedTiles, "_darkRed");
    }

    public void HideDanger()
    {
        ClearList(OrangeTiles, "_orange");
        ClearList(DarkOrangeTiles, "_darkOrange");
    }

    private void ClearList(List<Tile> list, string fieldName)
    {
        foreach (Tile t in list)
        {
            if (fieldName == "_blue") t._blue.SetActive(false);
            else if (fieldName == "_darkBlue") t._darkBlue.SetActive(false);
            else if (fieldName == "_red") t._red.SetActive(false);
            else if (fieldName == "_darkRed") t._darkRed.SetActive(false);
            else if (fieldName == "_orange") t._orange.SetActive(false);
            else if (fieldName == "_darkOrange") t._darkOrange.SetActive(false);
        }
        list.Clear();
    }

    private void SetOverlayState(GameObject overlay, bool state)
    {
        if (overlay != null) overlay.SetActive(state);
    }

    public void FindNeighbors()
    {
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        Neighbors.Clear();
        foreach (var dir in directions)
        {
            Tile neighbor = GridManager.Instance.GetTileAtPosition(Position + dir);
            if (neighbor != null) Neighbors.Add(neighbor);
        }
    }
    #endregion
}