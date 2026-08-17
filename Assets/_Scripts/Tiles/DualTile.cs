using System;
using System.Collections.Generic;
using UnityEngine;

public class DualTile : MonoBehaviour
{
    public Vector2Int Position { get; set; }
    public GameObject BackgroundSprite;
    public GameObject BorderSprite;
    private List<Tile> Neighbors;

    public void UpdateSprite()//récupère le type des tuils entourant la dualtile, renvoit null si c'est une bordure
    {
        Neighbors = new List<Tile>
        {
            GridManager.Instance.GetTileAtPosition(Position + Vector2Int.left),
            GridManager.Instance.GetTileAtPosition(Position),
            GridManager.Instance.GetTileAtPosition(Position + Vector2Int.down + Vector2Int.left),
            GridManager.Instance.GetTileAtPosition(Position + Vector2Int.down)
        };
        if (Neighbors.Contains(null))
        {
            string bitmap = "";
            for (int i = 0; i < 4; i++)
            {
                if (Neighbors[i] == null) bitmap += "1";
                else bitmap += "0";
            }
        }
    }
}