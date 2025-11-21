using System;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingManager : MonoBehaviour
{
    public static PathfindingManager Instance;
    private List<Tile> PathTiles = new List<Tile>();
    private List<Tile> PossibleTiles = new List<Tile>();
    private List<Tile> VisitedTiles = new List<Tile>();

    void Awake()
    {
        Instance = this;
    }

    public void ShowPath(Tile Start, Tile End, BaseUnit Unit)
    {
        if (FindPath(Start, End, Unit))
        {
            PathTiles.Clear();
            Tile current = End;
            while (current != null)
            {
                //Activer flèche de la case
                PathTiles.Add(current);
                if (current == Start) break;
                current = current.PrecedentTile;
            }
            PathTiles.Reverse();
        }
    }

    public bool FindPath(Tile Start, Tile End, BaseUnit Unit)
    {
        Tile ChosenTile = Start;
        ChosenTile.g_cost = 0;
        PathTiles.Clear();
        PossibleTiles.Clear();
        VisitedTiles.Clear();
        PossibleTiles.Add(Start);
        while (PossibleTiles.Count > 0)
        {
            ChosenTile = PossibleTiles[0];
            foreach (Tile t in PossibleTiles)
            {
                if (t.f_cost < ChosenTile.f_cost || (t.f_cost == ChosenTile.f_cost && t.h_cost < ChosenTile.h_cost))  ChosenTile = t;
            }
            if (ChosenTile == End) return true;
            PossibleTiles.Remove(ChosenTile);
            VisitedTiles.Add(ChosenTile);
            foreach (Tile neighbor in ChosenTile.Neighbors)
            {
                if (VisitedTiles.Contains(neighbor)) continue;
                if (!((Unit.Infantry && neighbor.Walkable) || (Unit.Vehicle && neighbor.Roadable))) continue;
                if (neighbor.OccupiedUnit != null) if (neighbor.OccupiedUnit.Faction != Unit.Faction) continue;
                float tentative_g = ChosenTile.g_cost + neighbor.cost;
                if (!PossibleTiles.Contains(neighbor) || tentative_g < neighbor.g_cost)
                {
                    neighbor.g_cost = tentative_g;
                    neighbor.h_cost = Mathf.Abs(neighbor.Position.x - End.Position.x) + Mathf.Abs(neighbor.Position.y - End.Position.y);
                    neighbor.f_cost = neighbor.g_cost + neighbor.h_cost;
                    neighbor.PrecedentTile = ChosenTile;
                    if (!PossibleTiles.Contains(neighbor))
                        PossibleTiles.Add(neighbor);
                }
            }
        }
        return false;
    }
}
