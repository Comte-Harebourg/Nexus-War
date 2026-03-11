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

    [Header("Stats")]
    public float Health;
    public float MaxHealth;
    public float Armor;
    public float MaxArmor;
    public float Morale;
    public float MaxMorale;
    public float damage;
    public float precision;
    public float penetration;
    public float Member;
    public float MaxMember;

    public Dictionary<Type, float> TileCosts = new Dictionary<Type, float>();

    protected virtual void Awake()
    {
        InitializeStats();
        Health = MaxHealth;
        Armor = MaxArmor;
        Morale = MaxMorale;
        Member = MaxMember;
        Animator = GetComponent<Animator>();
    }

    protected virtual void InitializeStats()
    {
        // Potentiellement mettre des stats par défaut ici, mais les unités spécifiques devraient les override
    }

    public float GetCost(Type tileType)
    // Retourne le coût de déplacement pour ce type de tile, ou l'infini si aucune valeur n'est définie
    {
        if (TileCosts.TryGetValue(tileType, out float cost))
        {
            return cost;
        }
        return float.PositiveInfinity;
    }
}

public enum Faction
{
    Aberrion = 0,
    Oromound = 1,
    Seranna = 2
}