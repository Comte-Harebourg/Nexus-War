using System.Collections.Generic;
using UnityEngine;

public class MountainTile : Tile
{
    [Header("Sprites")]
    public SpriteRenderer SpriteRenderer;
    public Sprite Mountain1;
    public Sprite Mountain2;
    public Sprite Mountain3;
    public Sprite Mountain4;
    public Sprite Mountain5;
    public Sprite Mountain6;

    public void Start()
    {
        UpdateSprite();
    }

    public void UpdateSprite()
    {
        List<Sprite> Liste = new List<Sprite>();
        Liste.Add(Mountain1);
        Liste.Add(Mountain2);
        Liste.Add(Mountain3);
        Liste.Add(Mountain4);
        Liste.Add(Mountain5);
        Liste.Add(Mountain6);
        SpriteRenderer.sprite = Liste[Random.Range(0, Liste.Count)];
    }
}