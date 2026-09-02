using System.Collections;
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
    public List<Tile> PlayerVisibility = new List<Tile>();

    private void Awake()
    {
        Instance = this;
    }

    public void SelectUnit(BaseUnit Unit)
    {
        SelectedUnit = Unit;
        Debug.Log($"L'unité {Unit.UnitName} en {Unit.OccupiedTile.Position} sélectionnée");
        SelectedUnit.OccupiedTile.ShowRange(SelectedUnit);
    }

    public void UnSelectUnit()
    {
        SelectedUnit.OccupiedTile.HideRange();
        Debug.Log($"L'unité {SelectedUnit.UnitName} en {SelectedUnit.OccupiedTile.Position} désélectionnée");
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

    public IEnumerator FightRoutine(BaseUnit Attacker, BaseUnit Defenser)
    {
        yield return null; //Évite les bugs de première itération
        for (int i = 0; i < (Attacker.MemberCount - Attacker.demoralizedCount); i++)//pour chaque membre de l'unité attaquante, moins le nombre de membres démoralisés
        {
            yield return StartCoroutine(BounceUnit(Attacker, Defenser.OccupiedTile));
            float roll = UnityEngine.Random.value; // Renvoie un float entre 0.0 et 1.0
            //if (Attacker.Shooting) Attacker.Shooting.PlayOneShot(Attacker.Shooting.clip);
            if (roll <= Attacker.precision)
            {
                //l'attaque réussit
                if (Defenser.Armor > 0)
                {
                    int output = Mathf.CeilToInt(Attacker.damage * Attacker.penetration * (1 - Defenser.OccupiedTile.cover));
                    Defenser.Armor -= output;
                    Debug.Log(string.Format("{0} en {1} à fait perdre {2} Armure à {3} en {4}", Attacker.UnitName, Attacker.OccupiedTile.Position, output, Defenser.UnitName, Defenser.OccupiedTile.Position));
                    DamagePopUp.Create(new Vector3(0.75f + Defenser.OccupiedTile.Position.x, 1f + Defenser.OccupiedTile.Position.y, 0), output, new Color32(83, 183, 255, 255), 1);
                }
                else
                {
                    int output = Mathf.CeilToInt(Attacker.damage * (1 - Defenser.OccupiedTile.cover));
                    Defenser.Health -= output;
                    Debug.Log(string.Format("{0} en {1} à fait perdre {2} Santé à {3} en {4}", Attacker.UnitName, Attacker.OccupiedTile.Position, output, Defenser.UnitName, Defenser.OccupiedTile.Position));
                    DamagePopUp.Create(new Vector3(0.75f + Defenser.OccupiedTile.Position.x, 1f + Defenser.OccupiedTile.Position.y, 0), output, new Color32(253, 99, 99, 255), 1);
                }
            }
            else
            {
                Defenser.Morale -= Attacker.damage; //lorsque l'attaque échoue on fait perdre de la morale à l'adversaire
                Debug.Log(string.Format("{0} en {1} à fait perdre {2} Morale à {3} en {4}", Attacker.UnitName, Attacker.OccupiedTile.Position, Attacker.damage, Defenser.UnitName, Defenser.OccupiedTile.Position));
                DamagePopUp.Create(new Vector3(0.75f + Defenser.OccupiedTile.Position.x, 1f + Defenser.OccupiedTile.Position.y, 0), Attacker.damage, new Color32(221, 255, 121, 255), 1);
            }

            // Checks et bornage des valeurs
            if (Defenser.Health < 0) Defenser.Health = 0;
            if (Defenser.Armor < 0) Defenser.Armor = 0;
            if (Defenser.Morale < 0) Defenser.Morale = 0;

            if (Defenser.Health == 0)
            {
                Defenser.MemberCount--;
                Debug.Log(string.Format("{0} en {1} a perdu 1 Membre", Defenser.UnitName, Defenser.OccupiedTile.Position));
                //if (Defenser.Dying) Defenser.Dying.Play();
                Defenser.BadgeUpdate();
                if (Defenser.MemberCount <= 0)
                {
                    Kill(Defenser);
                    Debug.Log(string.Format("{0} en {1} a tué {2} en {3}", Attacker.UnitName, Attacker.OccupiedTile.Position, Defenser.UnitName, Defenser.OccupiedTile.Position));
                    if (!GameManager.Instance.GameOver) LookTo(Attacker, Attacker.OccupiedTile, true);
                    yield break;
                }
                else Defenser.NewMember();
            }

            if (Defenser.Morale == 0 && Defenser.MemberCount > Defenser.demoralizedCount)
            {
                Defenser.demoralizedCount++;
                Defenser.Morale = Defenser.MaxMorale;
                Defenser.BadgeUpdate();
            }
        }
        Debug.Log(string.Format("{0} en {1} a attaqué {2} en {3}", Attacker.UnitName, Attacker.OccupiedTile.Position, Defenser.UnitName, Defenser.OccupiedTile.Position));
        if (Attacker.TriggerDevastation) Defenser.OccupiedTile.ChangeTile(Defenser.OccupiedTile.DevastationTile); //Les mortiers font des trous
        LookTo(Attacker, Attacker.OccupiedTile, true);
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

    public void Kill(BaseUnit Unit)
    {
        if (Unit.Danger != null) Unit.OccupiedTile.HideDanger();
        if (Unit.OccupiedTile != null) Unit.OccupiedTile.OccupiedUnit = null;
        DangerUnits.Remove(Unit);
        if (Unit.Faction == (Faction)0) GameManager.Instance.AberrionUnits.Remove(Unit);
        else if (Unit.Faction == (Faction)1) GameManager.Instance.OromoundUnits.Remove(Unit);
        else if (Unit.Faction == (Faction)2) GameManager.Instance.SerannaUnits.Remove(Unit);
        Destroy(Unit.gameObject);
        UpdateDanger();
        GameManager.Instance.CheckVictoryCondition();
    }

    public IEnumerator MoveUnit(BaseUnit Unit, List<Tile> Path)
    {
        yield return null; //Évite les bugs de première itération
        if (GameManager.SkipAnimation) yield break;
        GameManager.Instance.InAnimation = true;
        float duration = GameManager.AnimationSpeed;
        //if (Unit.Walking) Unit.Walking.Play();
        for (int i = 1; i < Path.Count && GameManager.Instance.InAnimation; i++)
        {
            Vector3 startPos = Path[i - 1].transform.position;
            Vector3 endPos = Path[i].transform.position;
            Vector2 direction = Path[i].Position - Path[i - 1].Position;
            float Timing = (Time.time % Unit.Animator.GetCurrentAnimatorStateInfo(0).length) / Unit.Animator.GetCurrentAnimatorStateInfo(0).length; //Permet de synchroniser les animations
            // Animation selon direction
            if (direction == new Vector2(0, -1))
                Unit.Animator.Play("Down", 0, Timing);
            else if (direction == new Vector2(0, 1))
                Unit.Animator.Play("Up", 0, Timing);
            else if (direction == new Vector2(-1, 0))
                Unit.Animator.Play("Left", 0, Timing);
            else if (direction == new Vector2(1, 0))
                Unit.Animator.Play("Right", 0, Timing);
            else
                Debug.Log("Animation de déplacement impossible");
            float elapsed = 0f;
            while (elapsed < duration)
            {
                Unit.transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null; //Évite les bugs de première itération
            }
            Unit.transform.position = endPos; // Assure la position exacte à la fin
        }
        if (Unit.Walking) Unit.Walking.Stop();
        GameManager.Instance.InAnimation = false;
    }

    public IEnumerator BounceUnit(BaseUnit Unit, Tile Tile)//buggé au premier bounce jsp quoi y faire
    {
        yield return null; //Évite les bugs de première itération
        Vector3 startPos = Unit.transform.position;
        if (GameManager.SkipAnimation) yield break;
        GameManager.Instance.InAnimation = true;
        Vector3 targetPos = (startPos + Tile.transform.position) / 2f; //Valeur par défaut en cas de bug
        float x0 = startPos.x;
        float y0 = startPos.y;
        float x = Tile.transform.position.x;
        float y = Tile.transform.position.y;
        float Timing = (Time.time % Unit.Animator.GetCurrentAnimatorStateInfo(0).length) / Unit.Animator.GetCurrentAnimatorStateInfo(0).length; //Permet de synchroniser les animations
        if (y <= y0 && math.abs(x - x0) <= math.abs(y - y0))
        {
            Unit.Animator.Play("Down", 0, Timing);
            targetPos = (startPos + Vector3Int.down);
        }
        else if (y > y0 && math.abs(x - x0) < math.abs(y - y0))
        {
            Unit.Animator.Play("Up", 0, Timing);
            targetPos = (startPos + Vector3Int.up);
        }
        else if (x < x0)
        {
            Unit.Animator.Play("Left", 0, Timing);
            targetPos = (startPos + Vector3Int.left);
        }
        else
        {
            Unit.Animator.Play("Right", 0, Timing);
            targetPos = (startPos + Vector3Int.right);
        }
        float duration = GameManager.AnimationSpeed;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            Unit.transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null; //Évite les bugs de première itération
        }
        Unit.transform.position = targetPos;

        // Retour à la position de départ
        elapsed = 0f;
        while (elapsed < duration)
        {
            Unit.transform.position = Vector3.Lerp(targetPos, startPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null; //Évite les bugs de première itération
        }

        Unit.transform.position = startPos;
        GameManager.Instance.InAnimation = false;
    }
}