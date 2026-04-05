using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class UnitManager : MonoBehaviour //Permet de gérer les unités sélectionnées
{
    public static UnitManager Instance;
    private List<BaseUnit> _units;
    public BaseUnit SelectedUnit;
    public List<BaseUnit> DangerUnits = new List<BaseUnit>();

    private void Awake()
    {
        Instance = this;
    }

    public void SelectUnit(BaseUnit Unit)
    {
        SelectedUnit = Unit;
        Debug.Log($"Unité {Unit.name} sélectionnée");
        SelectedUnit.OccupiedTile.ShowRange(SelectedUnit);
    }

    public void UnSelectUnit()
    {
        SelectedUnit.OccupiedTile.HideRange();
        Debug.Log($"Unité {SelectedUnit.name} désélectionnée");
        LookTo(SelectedUnit, SelectedUnit.OccupiedTile, true);
        SelectedUnit = null;
    }

    public void UpdateDanger()
    {
        foreach (BaseUnit Unit in DangerUnits)
        {
            Unit.OccupiedTile.HideDanger();
            Unit.OccupiedTile.ShowDanger();
        }
    }

    public void ResetDanger()
    {
        foreach (BaseUnit Unit in DangerUnits)
        {
            Unit.OccupiedTile.HideDanger();
        }
        DangerUnits.Clear();
    }

    public void Fight(BaseUnit Attacker, BaseUnit Defenser)
    {
        Debug.Log(Attacker.UnitName + " a attaqué " + Defenser.UnitName);
        
        for (int i = 0; i < (Attacker.MemberCount - Attacker.demoralizedCount); i++)//pour chaque membre de l'unité attaquante, moins le nombre de membres démoralisés
        {
            float roll = UnityEngine.Random.value; // Renvoie un float entre 0.0 et 1.0

            if (roll <= Attacker.precision)
            {
                //l'attaque réussit
                if (Defenser.Armor > 0)
                {
                    Defenser.Armor -= Mathf.CeilToInt(Attacker.damage * Attacker.penetration * (1 - Defenser.OccupiedTile.cover));
                }
                else
                {
                    Defenser.Health -= Mathf.CeilToInt(Attacker.damage * (1 - Defenser.OccupiedTile.cover));
                }
            }
            else
            {
                //lorsque l'attaque échoue on fait perdre de la morale à l'adversaire
                Defenser.Morale -= Attacker.damage;
            }

            // Checks et bornage des valeurs
            if (Defenser.Health < 0) Defenser.Health = 0;
            if (Defenser.Armor < 0) Defenser.Armor = 0;
            if (Defenser.Morale < 0) Defenser.Morale = 0;

            if (Defenser.Health == 0)
            {
                Defenser.MemberCount--;
                if (Defenser.MemberCount <= 0)
                {
                    Kill(Defenser);
                    return;//il n'y a plus rien à attaquer, on sort de la boucle
                }
                else
                {
                    Defenser.NewMember();
                }
            }

            if (Defenser.Morale == 0)
            {
                Defenser.demoralizedCount++;
            }
        }
    }

    public void LookTo(BaseUnit Unit, Tile Tile, bool IsMoving) //Anime Unit pour regarder vers Tile et IsMoving s'il doit bougé et non etre figé
    {
        float x0 = Unit.OccupiedTile.transform.position.x;
        float y0 = Unit.OccupiedTile.transform.position.y;
        float x = Tile.transform.position.x;
        float y = Tile.transform.position.y;
        float Timing = (Time.time % Unit.Animator.GetCurrentAnimatorStateInfo(0).length) / Unit.Animator.GetCurrentAnimatorStateInfo(0).length; //Permet de synchroniser les animations
        if (!Unit.isActive) Unit.Animator.Play("Down", 0, Timing); //Si l'unité est épuisée on reset son animation
        else if (y <= y0 && math.abs(x - x0) <= math.abs(y - y0))
        {
            if (IsMoving) Unit.Animator.Play("Down", 0, Timing);
            else Unit.Animator.Play("SelectDown", 0, Timing);
        }
        else if (y > y0 && math.abs(x - x0) < math.abs(y - y0))
        {
            if (IsMoving) Unit.Animator.Play("Up", 0, Timing);
            else Unit.Animator.Play("SelectUp", 0, Timing);
        }
        else if (x < x0)
        {
            if (IsMoving) Unit.Animator.Play("Left", 0, Timing);
            else Unit.Animator.Play("SelectLeft", 0, Timing);
        }
        else
        {
            if (IsMoving) Unit.Animator.Play("Right", 0, Timing);
            else Unit.Animator.Play("SelectRight", 0, Timing);
        }
    }

    public void SetActive(BaseUnit Unit)
    {
        Unit.isActive = true;
        Unit.Sprite.color = new Color32(255, 255, 255, 255);
    }

    public void Exhaustion(BaseUnit Unit)
    {
        Unit.isActive = false;
        Unit.Sprite.color = new Color32(128, 128, 128, 255);
    }

    public void Kill(BaseUnit Unit) //Tue l'unité sélectionnée
    {
        Unit.OccupiedTile.HideDanger();
        //Joue animation mort
        if (Unit.Faction == (Faction)0) GameManager.Instance.AberrionUnits.Remove(Unit);
        else if (Unit.Faction == (Faction)1) GameManager.Instance.OromoundUnits.Remove(Unit);
        else if (Unit.Faction == (Faction)2) GameManager.Instance.SerannaUnits.Remove(Unit);
        Destroy(Unit.gameObject);
    }
}