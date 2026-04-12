using System.Collections.Generic;
using UnityEngine;

public class ForestTile : Tile
{
    [Header("Sprites")]
    public SpriteRenderer SpriteRenderer;
    public Sprite Forest1;
    public Sprite Forest2;
    public Sprite Forest3;
    public Sprite Forest4;
    public Sprite Forest5;
    public Sprite Forest6;

    public void Start()
    {
        UpdateSprite();
        //DevastationTile = "010";
    }

    public void UpdateSprite()
    {
        List<Sprite> Liste = new List<Sprite>();
        Liste.Add(Forest1);
        Liste.Add(Forest2);
        Liste.Add(Forest3);
        Liste.Add(Forest4);
        Liste.Add(Forest5);
        Liste.Add(Forest6);
        SpriteRenderer.sprite = Liste[Random.Range(0, Liste.Count)];
    }
}
