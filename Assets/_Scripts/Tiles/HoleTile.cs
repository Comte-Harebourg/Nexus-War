using System.Collections.Generic;
using UnityEngine;

public class HoleTile : Tile
{
    [Header("Sprites")]
    public SpriteRenderer SpriteRenderer;
    public Sprite Hole1;
    public Sprite Hole2;
    public Sprite Hole3;
    public Sprite Hole4;
    public Sprite Hole5;
    public Sprite Hole6;

    public void Start()
    {
        UpdateSprite();
        //DevastationTile = "010";
    }

    public void UpdateSprite()
    {
        List<Sprite> Liste = new List<Sprite>();
        Liste.Add(Hole1);
        Liste.Add(Hole2);
        Liste.Add(Hole3);
        Liste.Add(Hole4);
        Liste.Add(Hole5);
        Liste.Add(Hole6);
        SpriteRenderer.sprite = Liste[Random.Range(0, Liste.Count)];
    }
}
