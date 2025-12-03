using System;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingManager : MonoBehaviour
{
    public static PathfindingManager Instance;
    private List<Tile> PathTiles = new List<Tile>();
    private List<Tile> PossibleTiles = new List<Tile>();
    private List<Tile> VisitedTiles = new List<Tile>();
    private bool IsUp(Vector2Int v) => v == Vector2Int.up; //Helper pour l'affichage des flèches
    private bool IsDown(Vector2Int v) => v == Vector2Int.down; //Helper pour l'affichage des flèches
    private bool IsLeft(Vector2Int v) => v == Vector2Int.left; //Helper pour l'affichage des flèches
    private bool IsRight(Vector2Int v) => v == Vector2Int.right; //Helper pour l'affichage des flèches

    void Awake()
    {
        Instance = this;
    }

    public void ShowPath(Tile Start, Tile End, BaseUnit Unit)
    {
        ClearArrow();
        if (FindPath(Start, End, Unit))
        {
            PathTiles.Clear();
            Tile current = End;
            while (current != null)
            {
                PathTiles.Add(current);
                if (current == Start) break;
                current = current.PrecedentTile;
            }
            PathTiles.Reverse();
            RenderArrowSprites();
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

    public void ClearArrow()
    {
        foreach (var tile in PathTiles)
        {
            var renderer = tile.GetComponent<ArrowTileRenderer>();
            if (renderer != null)
                renderer.Clear();
        }
    }

    private void RenderArrowSprites()
    {
        for (int i = 0; i < PathTiles.Count; i++)
        {
            Tile tile = PathTiles[i];
            var renderer = tile.GetComponent<ArrowTileRenderer>();
            if (renderer == null) continue;
            Vector2Int? From = null;
            Vector2Int? To = null;
            if (i > 0)
                From = PathTiles[i].Position - PathTiles[i - 1].Position;
            if (i < PathTiles.Count - 1)
                To = PathTiles[i + 1].Position - PathTiles[i].Position;
            if (i == 0)
            {
                renderer.SetSprite(GetStartSprite(renderer, To.Value));
            }
            else if (i == PathTiles.Count - 1)
            {
                renderer.SetSprite(GetEndSprite(renderer, From.Value));
            }
            else
            {
                renderer.SetSprite(GetBodySprite(renderer, From.Value, To.Value));
            }
        }
    }

    private Sprite GetStartSprite(ArrowTileRenderer r, Vector2Int dir)
    {
        if (IsUp(dir)) return r.startUp;
        if (IsDown(dir)) return r.startDown;
        if (IsLeft(dir)) return r.startLeft;
        return r.startRight;
    }

    private Sprite GetEndSprite(ArrowTileRenderer r, Vector2Int dir)
    {
        if (IsUp(dir)) return r.endUp;
        if (IsDown(dir)) return r.endDown;
        if (IsLeft(dir)) return r.endLeft;
        return r.endRight;
    }

    private Sprite GetBodySprite(ArrowTileRenderer r, Vector2Int from, Vector2Int to)
    {
        if ((IsUp(from) && IsUp(to)) || (IsDown(from) && IsDown(to))) return r.straightVertical;
        if ((IsLeft(from) && IsLeft(to)) || (IsRight(from) && IsRight(to))) return r.straightHorizontal;
        if (IsUp(from) && IsRight(to) || IsLeft(from) && IsDown(to)) return r.turnDownRight;
        if (IsUp(from) && IsLeft(to) || IsRight(from) && IsDown(to)) return r.turnDownLeft;
        if (IsDown(from) && IsRight(to) || IsLeft(from) && IsUp(to)) return r.turnUpRight;
        return r.turnUpLeft;
    }
}
