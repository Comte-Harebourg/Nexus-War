using System;
using System.Collections.Generic;
using UnityEngine;

public class ArrowManager : MonoBehaviour
{
    public static ArrowManager Instance;
    public List<Tile> PathTiles = new List<Tile>();

    // Helpers pour le direction des flèches
    private bool IsUp(Vector2Int v) => v == Vector2Int.up;
    private bool IsDown(Vector2Int v) => v == Vector2Int.down;
    private bool IsLeft(Vector2Int v) => v == Vector2Int.left;
    private bool IsRight(Vector2Int v) => v == Vector2Int.right;

    void Awake() => Instance = this;

    public void ShowPath(Tile start, Tile end)
    {
        ClearArrow();
        PathTiles.Clear();

        // On retourne sur nos pas en utilisant ParentTile définie pendant la recherche du mouvement
        Tile current = end;

        // Check de securite: On verifie que la tuile de fin fait bien partie du chemin atteignable
        while (current != null)
        {
            PathTiles.Add(current);
            if (current == start) break;
            current = current.ParentTile;
        }

        // On n'affiche que si on a bien atteint le départ
        if (PathTiles.Contains(start))
        {
            PathTiles.Reverse();
            RenderArrowSprites();
        }
    }

    public void ClearArrow()
    {
        foreach (var tile in PathTiles)
        {
            var renderer = tile.GetComponent<ArrowTileRenderer>();
            if (renderer != null) renderer.Clear();
        }
        PathTiles.Clear();
    }

    private void RenderArrowSprites()
    {
        for (int i = 0; i < PathTiles.Count; i++)
        {
            var renderer = PathTiles[i].GetComponent<ArrowTileRenderer>();
            if (renderer == null) continue;

            Vector2Int? fromDir = null;
            Vector2Int? toDir = null;

            if (i > 0)
                fromDir = PathTiles[i].Position - PathTiles[i - 1].Position;
            if (i < PathTiles.Count - 1)
                toDir = PathTiles[i + 1].Position - PathTiles[i].Position;

            if (i == 0) // Début de la flèche
            {
                renderer.SetSprite(GetStartSprite(renderer, toDir.Value));
            }
            else if (i == PathTiles.Count - 1) // Bout de la flèche
            {
                renderer.SetSprite(GetEndSprite(renderer, fromDir.Value));
            }
            else // Corps de la flèche
            {
                renderer.SetSprite(GetBodySprite(renderer, fromDir.Value, toDir.Value));
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
