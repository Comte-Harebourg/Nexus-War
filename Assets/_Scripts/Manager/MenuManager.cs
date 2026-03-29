using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class MenuManager : MonoBehaviour //Gère l'affichage de l'UI
{
    public static MenuManager Instance;
    [SerializeField] private GameObject _tileObject,_tileUnitObject,_background,_attackMenu,_waitMenu,_cancelMenu,_endTurnMenu;
    [SerializeField] private Image _healthBar, _armorBar, _moraleBar;
    [SerializeField] private TMP_Text _healthNumber, _armorNumber, _moraleNumber, _turnNumberAberrion, _turnNumberSeranna, _turnNumberOromound, _memberNumber;
    public GameObject AberrionInfo,SerannaInfo,OromoundInfo;
    public GameObject ActionMenue;
    public Tile HighlightedTile;
    private Tile MoveTile; //Case où l'unité se déplace pour attaquer
    private Tile AttackTile; //Case de la cible de l'attaque
    public bool MenueDisplay = false;
    public bool AttackDisplay = false;

    private void Awake()
    {
        Instance = this;
        _background.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) //Click logic
        {
            if (AttackDisplay == true)
            {
                AttackDisplay = false;
                MenueDisplay = true;
                ActionMenue.SetActive(true);
            }
            else if (MenueDisplay == true) Cancel();
            else if (UnitManager.Instance.SelectedUnit != null)
            {
                ArrowManager.Instance.ClearArrow();
                UnitManager.Instance.SelectedUnit.Animator.Play("Down");
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
            if (Tile.OccupiedUnit.MaxHealth != 0) _healthBar.fillAmount = (float)(Tile.OccupiedUnit.Health + Tile.OccupiedUnit.MaxHealth * (Tile.OccupiedUnit.MemberCount - 1)) / (float)(Tile.OccupiedUnit.MaxHealth * Tile.OccupiedUnit.MaxMemberCount);
            else _healthBar.fillAmount = 0;
            if (Tile.OccupiedUnit.MaxArmor != 0) _armorBar.fillAmount = (float)(Tile.OccupiedUnit.Armor + Tile.OccupiedUnit.MaxArmor * (Tile.OccupiedUnit.MemberCount - 1)) / (float)(Tile.OccupiedUnit.MaxArmor * Tile.OccupiedUnit.MaxMemberCount);
            else _armorBar.fillAmount = 0;
            if (Tile.OccupiedUnit.MaxMorale != 0) _moraleBar.fillAmount = (float)(Tile.OccupiedUnit.Morale + Tile.OccupiedUnit.MaxMorale * (Tile.OccupiedUnit.MemberCount - 1)) / (float)(Tile.OccupiedUnit.MaxMorale * Tile.OccupiedUnit.MaxMemberCount);
            else _moraleBar.fillAmount = 0;
            _healthNumber.text = (Tile.OccupiedUnit.Health + Tile.OccupiedUnit.MaxHealth * (Tile.OccupiedUnit.MemberCount - 1)).ToString()+"/"+ (Tile.OccupiedUnit.MaxHealth * Tile.OccupiedUnit.MaxMemberCount).ToString();
            _armorNumber.text = (Tile.OccupiedUnit.Armor + Tile.OccupiedUnit.MaxArmor * (Tile.OccupiedUnit.MemberCount - 1)).ToString() + "/" + (Tile.OccupiedUnit.MaxArmor * Tile.OccupiedUnit.MaxMemberCount).ToString();
            _moraleNumber.text = (Tile.OccupiedUnit.Morale + Tile.OccupiedUnit.MaxMorale * (Tile.OccupiedUnit.MemberCount - 1)).ToString() + "/" + (Tile.OccupiedUnit.MaxMorale * Tile.OccupiedUnit.MaxMemberCount).ToString();
            _memberNumber.text = (Tile.OccupiedUnit.MemberCount).ToString() + "/" + (Tile.OccupiedUnit.MaxMemberCount).ToString();
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
        if (UnitManager.Instance.SelectedUnit != null) UnitManager.Instance.LookTo(UnitManager.Instance.SelectedUnit, MTile, true);
        MoveTile = MTile;
        AttackTile = ATile;
        _cancelMenu.SetActive(true); //Cette partie regarde les menus cohérents à activer
        if (UnitManager.Instance.SelectedUnit != null)
        {
            _waitMenu.SetActive(true);
            if (ATile.RedTiles.Count() != 0)
            {
                _attackMenu.SetActive(true);
            }
        }
        else _endTurnMenu.SetActive(true);
        ActionMenue.SetActive(true);
    }

    public void Attack()
    {
        if (MoveTile != AttackTile)
        {
            UnitManager.Instance.SelectedUnit.OccupiedTile.Highlight.SetActive(false); //Debug affichage curseur sur case de l'unité
            MoveTile.SetUnit(UnitManager.Instance.SelectedUnit);
            UnitManager.Instance.Fight(UnitManager.Instance.SelectedUnit, AttackTile.OccupiedUnit);
            AttackTile.Highlight.SetActive(false);
            MoveTile.Highlight.SetActive(false);
            //Animation déplacement
            //Animation attaque
            UnitManager.Instance.Exhaustion(UnitManager.Instance.SelectedUnit); //Épuisement unité
            Cancel();
            ArrowManager.Instance.ClearArrow();
            UnitManager.Instance.UnSelectUnit();
            AttackTile = null;
            MoveTile = null;
        }
        else if (AttackTile.RedTiles.Count != 0)
        {
            AttackDisplay = true;
            ActionMenue.SetActive(false);
            MenueDisplay = false;
        }
    }

    public void TryAttack(Tile Tile)
    {
        AttackDisplay = false;
        MoveTile.SetUnit(UnitManager.Instance.SelectedUnit);
        UnitManager.Instance.Fight(UnitManager.Instance.SelectedUnit, Tile.OccupiedUnit);
        AttackTile.Highlight.SetActive(false);
        //Animation déplacement
        //Animation attaque
        UnitManager.Instance.Exhaustion(UnitManager.Instance.SelectedUnit); //Épuisement unité
        Cancel();
        ArrowManager.Instance.ClearArrow();
        UnitManager.Instance.UnSelectUnit();
        AttackTile = null;
        MoveTile = null;
    }

    public void Wait()
    {
        UnitManager.Instance.SelectedUnit.OccupiedTile.Highlight.SetActive(false); //Debug affichage curseur sur case de l'unité
        MoveTile.SetUnit(UnitManager.Instance.SelectedUnit);
        if (MoveTile != AttackTile) AttackTile.Highlight.SetActive(false);
        //Animation déplacement
        UnitManager.Instance.Exhaustion(UnitManager.Instance.SelectedUnit); //Épuisement unité
        Cancel();
        ArrowManager.Instance.ClearArrow();
        UnitManager.Instance.UnSelectUnit();
    }

    public void Cancel()
    {
        MenueDisplay = false;
        ActionMenue.SetActive(false);
        _cancelMenu.SetActive(false);
        _attackMenu.SetActive(false);
        _waitMenu.SetActive(false);
        _endTurnMenu.SetActive(false);
        if (HighlightedTile != null)
        {
            HighlightedTile.HideRange();
            HighlightedTile.Highlight.SetActive(false);
            HighlightedTile = null;
        }
        if (UnitManager.Instance.SelectedUnit != null)
        {
            UnitManager.Instance.SelectedUnit.OccupiedTile.Highlight.SetActive(false); //Debug affichage curseur sur case de l'unité
            UnitManager.Instance.SelectedUnit.OccupiedTile.HideRange();
            UnitManager.Instance.SelectedUnit.OccupiedTile.ShowRange(UnitManager.Instance.SelectedUnit); //Je crois que c'est pour refresh les couleurs des tuiles mais je suis pas sur
        }
        if (GridManager.Instance.GetTileUnderMouse() != null)
        {
            GridManager.Instance.GetTileUnderMouse().Highlight.SetActive(true);
            ArrowManager.Instance.ClearArrow(); //Pour éviter des des bugs visuels
            ArrowManager.Instance.PathTiles.Clear(); //Pour éviter des des bugs de Pathfinding
            GridManager.Instance.GetTileUnderMouse().OnMouseEnter();
        }
        MoveTile = null;
        AttackTile = null;
        ShowTileInfo(GridManager.Instance.GetTileUnderMouse());
    }

    public void EndTurn()
    {
        GameManager.Instance.NextTurn();
        Cancel();
    }

    public void UpdateTurn(int Turn) //Mise à jour affichage nombre tour pour chaque faction
    {
        _turnNumberAberrion.text = "Tour: " + Turn.ToString();
        _turnNumberSeranna.text = "Tour: " + Turn.ToString();
        _turnNumberOromound.text = "Tour: " + Turn.ToString();
    }
}
