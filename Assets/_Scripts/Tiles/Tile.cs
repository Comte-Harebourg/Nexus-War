using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private int cover;
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
        if (MenuManager.Instance.AttackDisplay && _red.activeSelf) //Sélection d'une cible pour l'attaque
        {
            MenuManager.Instance.TryAttack(this);
        }
        else if (MenuManager.Instance.MenueDisplay || MenuManager.Instance.AttackDisplay) return;
        else if (OccupiedUnit != null)
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
                    MenuManager.Instance.MenueDisplay = true;
                    _red.SetActive(true);
                    RedTiles.Clear();
                    RedTiles.Add(this);
                    if (ArrowManager.Instance.PathTiles.Count != 0)
                    {
                        MenuManager.Instance.ShowActionUI(ArrowManager.Instance.PathTiles.Last(), this);
                    }
                    else
                    {
                        MenuManager.Instance.ShowActionUI(UnitManager.Instance.SelectedUnit.OccupiedTile, this);
                    }
                    
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
                    if (OccupiedUnit.isActive)
                    {
                    UnitManager.Instance.SelectUnit(OccupiedUnit);
                    UnitManager.Instance.LookTo(UnitManager.Instance.SelectedUnit, this, false);
                    }
                }
                else if (UnitManager.Instance.SelectedUnit == OccupiedUnit)
                {
                    // Clic sur soi-meme: on montre le menu d'action a l'endroit actuel
                    HideRange();
                    MenuManager.Instance.MenueDisplay = true;
                    ShowAttackRange(UnitManager.Instance.SelectedUnit);
                    MenuManager.Instance.ShowActionUI(this,this);
                }
                else
                {
                    ArrowManager.Instance.ClearArrow();
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
                    MenuManager.Instance.MenueDisplay = true;
                    ShowAttackRange(UnitManager.Instance.SelectedUnit);
                    MenuManager.Instance.ShowActionUI(this,this);
                }
                else
                {
                    ArrowManager.Instance.ClearArrow();
                    UnitManager.Instance.UnSelectUnit();
                }
            }
            else //Montre le menu pour passer son tour
            {
                MenuManager.Instance.HighlightedTile = this;
                MenuManager.Instance.MenueDisplay = true;
                MenuManager.Instance.ShowActionUI(this, this);
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
        if (MenuManager.Instance.MenueDisplay || MenuManager.Instance.AttackDisplay) return;
        Highlight.SetActive(true);
        MenuManager.Instance.ShowTileInfo(this);

        if (UnitManager.Instance.SelectedUnit != null)
        {
            var selectedTile = UnitManager.Instance.SelectedUnit.OccupiedTile;
            UnitManager.Instance.LookTo(UnitManager.Instance.SelectedUnit, this, false);
            if (selectedTile.BlueTiles.Contains(this) && selectedTile != this)
            {
                ArrowManager.Instance.ShowPath(selectedTile, this);
            }
            else if (selectedTile.RedTiles.Contains(this))
            {
                ArrowManager.Instance.ShowPath(selectedTile, SearchNearestTile(this, UnitManager.Instance.SelectedUnit.OccupiedTile.BlueTiles, UnitManager.Instance.SelectedUnit.minAttackRange, UnitManager.Instance.SelectedUnit.maxAttackRange));
            }
            else
            {
                ArrowManager.Instance.ClearArrow();
                ArrowManager.Instance.PathTiles.Clear();
            }
        }
    }

    private void OnMouseExit()
    {
        if (MenuManager.Instance.MenueDisplay || MenuManager.Instance.AttackDisplay) return;
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
        ParentTile = null; // On supprime le parent de la tuile de départ
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

    private void CalculateMovement(Tile startTile, float maxSpeed, BaseUnit unit, bool isDanger, Dictionary<Tile, float> dist)
    {
        List<Tile> S = new List<Tile>();
        
        dist[startTile] = 0;
        // La convention de parent[départ] = départ a été remplacée par null (déjà fait dans PerformGenericSearch)
        // pour empêcher d'éventuelles boucles infinies de chemin avec un theStart.ParentTile = theStart.
        S.Add(startTile);

        while (S.Count > 0)
        {
            // Trouver et retirer de S l'élément u dont la valeur dist[u] est minimale
            Tile u = S[0];
            for (int i = 1; i < S.Count; i++)
            {
                if (dist[S[i]] < dist[u])
                {
                    u = S[i];
                }
            }
            S.Remove(u);

            foreach (Tile v in u.Neighbors)
            {
                // Contraintes de mouvement des Unités
                float movementCost = unit.GetCost(v.GetType());
                if (float.IsInfinity(movementCost)) continue;

                // Contraintes de faction (Blocage si un ennemi est sur la tuile)
                bool hasEnemy = v.OccupiedUnit != null && v.OccupiedUnit.Faction != unit.Faction;
                if (hasEnemy) continue;

                float alt = dist[u] + movementCost;

                // La portée de mouvement ne doit pas être dépassée
                if (alt <= maxSpeed)
                {
                    if (!dist.ContainsKey(v) || alt < dist[v])
                    {
                        dist[v] = alt;
                        v.ParentTile = u;

                        bool isAlly = v.OccupiedUnit != null && v.OccupiedUnit.Faction == unit.Faction;
                        ApplyMovementVisuals(v, isAlly, isDanger);

                        if (!S.Contains(v)) // Si v n'est pas dans S
                        {
                            S.Add(v);
                        }
                    }
                }
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

    public Tile SearchNearestTile(Tile Start, List<Tile> Liste, int MinRange, int MaxRange)
    //Cherche la case la plus proche de Start dans la liste Liste à une portée maximale de MaxRange et une portée minimale de MinRange sinon renvoit null
    {
        Tile End = null;
        if (Liste.Count() != 0)
        {
            foreach (Tile Tile in Liste)
            {
                if (Mathf.Abs(Tile.Position.x-Start.Position.x) + Mathf.Abs(Tile.Position.y - Start.Position.y)<=MaxRange && Mathf.Abs(Tile.Position.x - Start.Position.x) + Mathf.Abs(Tile.Position.y - Start.Position.y) >= MinRange)
                //On vérifie que la distance entre Tile et Start est comprise entre MinRnage et MaxRange
                {
                    End = Tile;
                    MaxRange = Mathf.Abs(Tile.Position.x - Start.Position.x) + Mathf.Abs(Tile.Position.y - Start.Position.y);
                    //MaxRange devient la distance entre Start et Tile
                }
            }
        }
        return (End);
    }
    #endregion
}