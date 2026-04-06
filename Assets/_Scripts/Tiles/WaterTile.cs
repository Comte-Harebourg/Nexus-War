using UnityEngine;

public class WaterTile : Tile
{
    public void Start()
    {
        GetComponent<WaterTileRenderer>().UpdateSprite(this);
    }
}