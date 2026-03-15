using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    public string UnitID;
    public string UnitName;
    public Tile OccupiedTile;
    public Faction Faction;
    public int speed;
    public int minAttackRange;
    public int maxAttackRange;
    public bool isActive = true;
    public Animator Animator;
    public SpriteRenderer Sprite;

    [Header("Stats")]
    public int Health;
    public int MaxHealth;
    public int Armor;
    public int MaxArmor;
    public int Morale;
    public int MaxMorale;
    public int damage;
    public float precision;
    public float penetration;
    public int MemberCount;
    public int MaxMemberCount;

    public Dictionary<Type, int> TileCosts = new Dictionary<Type, int>();

    protected virtual void Awake()
    {
        InitializeStats();
        Health = MaxHealth;
        Armor = MaxArmor;
        Morale = MaxMorale;
        MemberCount = MaxMemberCount;
        Animator = GetComponent<Animator>();
        Sprite = GetComponent<SpriteRenderer>();
    }

    protected virtual void InitializeStats()
    {
        // Potentiellement mettre des stats par défaut ici, mais les unités spécifiques devraient les override
    }

    public void NewMember()
    // Utilisé lorsqu'un membre meurt pour réinitialiser les stats
    {
        Health = MaxHealth;
        Armor = MaxArmor;
        Morale = MaxMorale;
    }

    public int GetCost(Type tileType)
    // Retourne le coût de déplacement pour ce type de tile, ou l'infini si aucune valeur n'est définie
    {
        if (TileCosts.TryGetValue(tileType, out int cost))
        {
            return cost;
        }
        return int.MaxValue;
    }
}

public enum Faction
{
    Aberrion = 0,
    Oromound = 1,
    Seranna = 2
}