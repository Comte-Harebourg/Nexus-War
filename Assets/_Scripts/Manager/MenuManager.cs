using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System;

public class MenuManager : MonoBehaviour //Gère l'affichage de l'UI
{
    public static MenuManager Instance;
    [SerializeField] private GameObject _tileObject,_tileUnitObject,_background,_attackMenu,_waitMenu,_cancelMenu, _endTurnMenu, _endMenu;
    [SerializeField] private Image _healthBar, _armorBar, _moraleBar;
    [SerializeField] private TMP_Text _healthNumber, _armorNumber, _moraleNumber, _turnNumberAberrion, _turnNumberSeranna, _turnNumberOromound, _damageNumber, _precisionNumber, _penetrationNumber, _endText;
    public Transform DamagePopUp;
    [SerializeField] private Image _cover1, _cover2, _cover3, _cover4;
    [SerializeField] private GameObject  _aberrionIcon, _oromoundIcon, _serannaIcon;
    [SerializeField] private GameObject _turnAnimation;
    [SerializeField] private TMP_Text _turnAmount;
    [SerializeField] private Image _banniere1, _banniere2;
    private List<Image> Covers = new List<Image>();
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
        Covers.Add(_cover1);
        Covers.Add(_cover2);
        Covers.Add(_cover3);
        Covers.Add(_cover4);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (!GameManager.Instance.InAnimation)
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
            else GameManager.Instance.InAnimation = false;
        }
    }

    public void ShowTileInfo(Tile Tile)
    {
        if (Tile == null)
        {
            _tileObject.SetActive(false);
            _tileUnitObject.SetActive(false);
        }
        else
        {
            _tileObject.GetComponentInChildren<TMP_Text>().text = Tile.TileName;
            _tileObject.SetActive(true);
            if (Tile.OccupiedUnit)
            {
                _tileObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(221.2f, 39.51001f);
                BaseUnit Unit = Tile.OccupiedUnit;
                ShowCover(Tile);
                _tileUnitObject.GetComponentInChildren<TMP_Text>().text = Unit.UnitName;
                if (Unit.MaxHealth != 0) _healthBar.fillAmount = (float)(Unit.Health + Unit.MaxHealth * (Unit.MemberCount - 1)) / (float)(Unit.MaxHealth * Unit.MaxMemberCount);
                else _healthBar.fillAmount = 0;
                if (Unit.MaxArmor != 0) _armorBar.fillAmount = (float)(Unit.Armor + Unit.MaxArmor * (Unit.MemberCount - 1)) / (float)(Unit.MaxArmor * Unit.MaxMemberCount);
                else _armorBar.fillAmount = 0;
                if (Unit.MaxMorale != 0) _moraleBar.fillAmount = (float)(MathF.Max(0, Unit.MaxMorale * (Unit.MemberCount - Unit.demoralizedCount - 1) + Unit.Morale)) / (float)(Unit.MaxMorale * Unit.MaxMemberCount);
                else _moraleBar.fillAmount = 0;
                _healthNumber.text = (Unit.Health + Unit.MaxHealth * (Unit.MemberCount - 1)).ToString() + "/" + (Unit.MaxHealth * Unit.MaxMemberCount).ToString();
                _armorNumber.text = (Unit.Armor + Unit.MaxArmor * (Unit.MemberCount - 1)).ToString() + "/" + (Unit.MaxArmor * Unit.MaxMemberCount).ToString();
                _moraleNumber.text = (MathF.Max(0, Unit.MaxMorale * (Unit.MemberCount - Unit.demoralizedCount - 1) + Unit.Morale)).ToString() + "/" + (Unit.MaxMorale * Unit.MaxMemberCount).ToString();
                _damageNumber.text = (Unit.damage).ToString();
                _precisionNumber.text = (Unit.precision).ToString();
                _penetrationNumber.text = (Unit.penetration).ToString();
                _tileUnitObject.SetActive(true);
            }
            else
            {
                _tileObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(58.9f, 39.51001f);
                ShowCover(Tile);
                _tileUnitObject.SetActive(false);
            }
        }
    }

    public void ShowCover(Tile Tile)
    {
        int cover = (int) (Tile.cover*5); //Affiche la cover de la case en colorant les boucliers de l'UI
        for (int i = 0; i < Covers.Count(); i++)
        {
            if (cover == 0)
            {
                Covers[i].color = new Color32(255, 255, 255, 255);
            }
            else if (cover > 0)
            {
                Covers[i].color = new Color32(0, 100, 255, 255);
                cover -= 1;
            }
            else
            {
                Covers[i].color = new Color32(255, 0, 0, 255);
                cover += 1;
            }
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
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        yield return null; //Évite les bugs de première itération
        if (MoveTile != AttackTile)
        {
            UnitManager.Instance.SelectedUnit.OccupiedTile.Highlight.SetActive(false);
            ArrowManager.Instance.ClearArrow();
            ActionMenue.SetActive(false);
            AttackTile.HideRange();
            yield return StartCoroutine(UnitManager.Instance.MoveUnit(UnitManager.Instance.SelectedUnit,ArrowManager.Instance.PathTiles));
            MoveTile.SetUnit(UnitManager.Instance.SelectedUnit);
            yield return StartCoroutine(UnitManager.Instance.FightRoutine(UnitManager.Instance.SelectedUnit, AttackTile.OccupiedUnit));
            AttackTile.Highlight.SetActive(false);
            MoveTile.Highlight.SetActive(false);
            UnitManager.Instance.Exhaustion(UnitManager.Instance.SelectedUnit);
            Cancel();
            UnitManager.Instance.UnSelectUnit();
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
        StartCoroutine(TryAttackRoutine(Tile));
    }

    private IEnumerator TryAttackRoutine(Tile Tile)
    {
        yield return null; //Évite les bugs de première itération
        AttackDisplay = false;
        ArrowManager.Instance.ClearArrow();
        ActionMenue.SetActive(false);
        AttackTile.HideRange();
        yield return StartCoroutine(UnitManager.Instance.MoveUnit(UnitManager.Instance.SelectedUnit,ArrowManager.Instance.PathTiles));
        MoveTile.SetUnit(UnitManager.Instance.SelectedUnit);
        yield return StartCoroutine(UnitManager.Instance.FightRoutine(UnitManager.Instance.SelectedUnit, Tile.OccupiedUnit));
        AttackTile.Highlight.SetActive(false);
        UnitManager.Instance.Exhaustion(UnitManager.Instance.SelectedUnit);
        Cancel();
        UnitManager.Instance.UnSelectUnit();
    }

    public void Wait()
    {
        StartCoroutine(WaitRoutine());
    }

    private IEnumerator WaitRoutine()
    {
        yield return null; //Évite les bugs de première itération
        UnitManager.Instance.SelectedUnit.OccupiedTile.Highlight.SetActive(false);
        ArrowManager.Instance.ClearArrow();
        ActionMenue.SetActive(false);
        AttackTile.HideRange();
        yield return StartCoroutine(UnitManager.Instance.MoveUnit(UnitManager.Instance.SelectedUnit,ArrowManager.Instance.PathTiles));
        MoveTile.SetUnit(UnitManager.Instance.SelectedUnit);
        if (MoveTile != AttackTile) AttackTile.Highlight.SetActive(false);
        UnitManager.Instance.Exhaustion(UnitManager.Instance.SelectedUnit);
        Cancel();
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
        ShowTileInfo(GridManager.Instance.GetTileUnderMouse());
        ArrowManager.Instance.ClearArrow();
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

    public void ShowEndMenu(List<BaseUnit> Winner)//Affiche le menu de fin, supprime l'UI, le plateau et désactive les bots
    {
        string faction = Winner.First().Faction.ToString();
        Debug.Log(string.Format("{0} a gagné", faction));
        TileMapManager.Instance.ClearMap();
        _tileObject.SetActive(false);
        _tileUnitObject.SetActive(false);
        OromoundInfo.SetActive(false);
        SerannaInfo.SetActive(false);
        AberrionInfo.SetActive(false);
        _endMenu.SetActive(true);
        GameManager.Instance.Bot = false;
        if (faction=="Aberrion") _aberrionIcon.SetActive(true);
        else if (faction == "Oromound") _oromoundIcon.SetActive(true);
        else if (faction == "Seranna") _serannaIcon.SetActive(true);
        _endText.text = string.Format("{0} a gagne avec {1} unites restantes en {2} tours.", faction, Winner.Count, GameManager.Instance.Turn);
    }

    public IEnumerator TurnAnimation(GameState Faction)//Gere l'animation du bandeau de nouveau tour
    {
        yield return null; //Évite les bugs de première itération
        if (GameManager.Instance.SkipAnimation) yield break;
        float duration = GameManager.Instance.AnimationSpeed*10;
        GameManager.Instance.InAnimation = true;
        _turnAmount.text = "Tour " + GameManager.Instance.Turn.ToString();
        if (Faction == (GameState)0)
        {
            _turnAnimation.GetComponent<Image>().color = new Color32(35, 79, 137, 255);
            _banniere1.color = new Color32(98, 122, 147, 255);
            _banniere2.color = new Color32(98, 122, 147, 255);
            _turnAmount.color = new Color32(98, 122, 147, 255);
        }
        else if (Faction == (GameState)1)
        {
            _turnAnimation.GetComponent<Image>().color = new Color32(54, 75, 40, 255);
            _banniere1.color = new Color32(215, 239, 184, 255);
            _banniere2.color = new Color32(215, 239, 184, 255);
            _turnAmount.color = new Color32(215, 239, 184, 255);
        }
        else if (Faction == (GameState)2)
        {
            _turnAnimation.GetComponent<Image>().color = new Color32(163, 144, 88, 255);
            _banniere1.color = new Color32(64, 61, 50, 255);
            _banniere2.color = new Color32(64, 61, 50, 255);
            _turnAmount.color = new Color32(64, 61, 50, 255);
        }
        _turnAnimation.SetActive(true);

        RectTransform rt = _turnAnimation.GetComponent<RectTransform>();
        float width = rt.rect.width;

        float slideInDuration = duration * 0.25f;
        float holdDuration = duration * 0.50f;
        float slideOutDuration = duration * 0.25f;

        Vector2 hiddenLeft = new Vector2(-width, rt.anchoredPosition.y);
        Vector2 centerPos = new Vector2(0f, rt.anchoredPosition.y);
        Vector2 hiddenRight = new Vector2(width, rt.anchoredPosition.y);

        float t = 0f;
        rt.anchoredPosition = hiddenLeft;
        while (t < slideInDuration && GameManager.Instance.InAnimation)
        {
            t += Time.deltaTime;
            float normalized = Mathf.Clamp01(t / slideInDuration);
            float eased = 1f - Mathf.Pow(1f - normalized, 3f);
            rt.anchoredPosition = Vector2.LerpUnclamped(hiddenLeft, centerPos, eased);
            yield return null;
        }
        rt.anchoredPosition = centerPos;

        t = 0f;
        while (t < holdDuration && GameManager.Instance.InAnimation)
        {
            t += Time.deltaTime;
            yield return null;
        }

        t = 0f;
        while (t < slideOutDuration && GameManager.Instance.InAnimation)
        {
            t += Time.deltaTime;
            float normalized = Mathf.Clamp01(t / slideOutDuration);
            float eased = Mathf.Pow(normalized, 3f);
            rt.anchoredPosition = Vector2.LerpUnclamped(centerPos, hiddenRight, eased);
            yield return null;
        }
        _turnAnimation.SetActive(false);
        GameManager.Instance.InAnimation = false;
    }
}
