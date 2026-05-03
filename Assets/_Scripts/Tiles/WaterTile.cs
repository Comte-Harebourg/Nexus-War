using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaterTile : Tile
{
    [Header("Sprites")]
    public SpriteRenderer SpriteRenderer;
    public Sprite Up;
    public Sprite Right;
    public Sprite Down;
    public Sprite Left;
    public Sprite UpxRight;
    public Sprite DownxRight;
    public Sprite DownxLeft;
    public Sprite UpxLeft;
    public Sprite UpxDownxRight;
    public Sprite DownxRightxLeft;
    public Sprite UpxDownxLeft;
    public Sprite UpxRightxLeft;
    public Sprite UpxDown;
    public Sprite RightxLeft;
    public Sprite UpxDownxRightxLeft;
    public Sprite UpLeft;
    public Sprite UpRight;
    public Sprite DownRight;
    public Sprite DownLeft;
    public Sprite DownLeftxDownRight;
    public Sprite DownRightxUpRight;
    public Sprite UpRightxUpLeft;
    public Sprite UpLeftxDownLeft;
    public Sprite DownLeftxDownRightxUpRight;
    public Sprite DownRightxUpRightxUpLeft;
    public Sprite UpRightxUpLeftxDownLeft;
    public Sprite UpLeftxDownLeftxDownRight;
    public Sprite UpLeftxDownRight;
    public Sprite UpRightxDownLeft;
    public Sprite DownLeftxDownRightxUpRightxUpLeft;
    public Sprite DownxUpLeftxUpRight;
    public Sprite DownxUpLeft;
    public Sprite DownxUpRight;
    public Sprite RightxDownLeftxUpLeft;
    public Sprite RightxDownLeft;
    public Sprite RightxUpLeft;
    public Sprite UpxDownRightxDownLeft;
    public Sprite UpxDownLeft;
    public Sprite UpxDownRight;
    public Sprite LeftxDownRightxUpRight;
    public Sprite LeftxDownRight;
    public Sprite LeftxUpRight;
    public Sprite UpxRightxDownLeft;
    public Sprite DownxRightxUpLeft;
    public Sprite DownxLeftxUpRight;
    public Sprite UpxLeftxDownRight;
    private List<bool> Voisins = new List<bool>();

    public void Start()
    {
        UpdateSprite();
    }

    public void UpdateSprite()
    {
        FindWaterNeighbors(this);

        bool up = Voisins[0];
        bool down = Voisins[1];
        bool left = Voisins[2];
        bool right = Voisins[3];
        bool upLeft = Voisins[4];
        bool upRight = Voisins[5];
        bool downLeft = Voisins[6];
        bool downRight = Voisins[7];

        if (Voisins.All(x => !x)) return;
        if (up)
        {
            if (right)
            {
                if (down)
                {
                    if (left)
                    {
                        SpriteRenderer.sprite = UpxDownxRightxLeft;
                    }
                    else SpriteRenderer.sprite = UpxDownxRight;
                }
                else if (left) SpriteRenderer.sprite = UpxRightxLeft;
                else if (downLeft) SpriteRenderer.sprite = UpxRightxDownLeft;
                else SpriteRenderer.sprite = UpxRight;
            }
            else if (down)
            {
                if (left) SpriteRenderer.sprite = UpxDownxLeft;
                else SpriteRenderer.sprite = UpxDown;
            }
            else if (left)
            {
                if (downRight) SpriteRenderer.sprite = UpxLeftxDownRight;
                else SpriteRenderer.sprite = UpxLeft;
            }
            else if (downLeft)
            {
                if (downRight) SpriteRenderer.sprite = UpxDownRightxDownLeft;
                else SpriteRenderer.sprite = UpxDownLeft;
            }
            else if (downRight) SpriteRenderer.sprite = UpxDownRight;
            else SpriteRenderer.sprite = Up;
        }
        else if (right)
        {
            if (down)
            {
                if (left)
                {
                    SpriteRenderer.sprite = DownxRightxLeft;
                }
                else if (upLeft) SpriteRenderer.sprite = DownxRightxUpLeft;
                else SpriteRenderer.sprite = DownxRight;
            }
            else if (left) SpriteRenderer.sprite = RightxLeft;
            else if (upLeft)
            {
                if (downLeft) SpriteRenderer.sprite = RightxDownLeftxUpLeft;
                else SpriteRenderer.sprite = RightxUpLeft;
            }
            else if (downLeft) SpriteRenderer.sprite = RightxDownLeft;
            else SpriteRenderer.sprite = Right;
        }
        else if (down)
        {
            if (left)
            {
                if (upRight) SpriteRenderer.sprite = DownxLeftxUpRight;
                else SpriteRenderer.sprite = DownxLeft;
            }
            else if (upLeft)
            {
                if (upRight) SpriteRenderer.sprite = DownxUpLeftxUpRight;
                else SpriteRenderer.sprite = DownxUpLeft;
            }
            else if (upRight) SpriteRenderer.sprite = DownxUpRight;
            else SpriteRenderer.sprite = Down;
        }
        else if (left)
        {
            if (upRight)
            {
                if (downRight) SpriteRenderer.sprite = LeftxDownRightxUpRight;
                else SpriteRenderer.sprite = LeftxUpRight;
            }
            else if (downRight) SpriteRenderer.sprite = LeftxDownRight;
            else SpriteRenderer.sprite = Left;
        }
        else if (upLeft)
        {
            if (upRight)
            {
                if (downLeft)
                {
                    if (downRight) SpriteRenderer.sprite = DownLeftxDownRightxUpRightxUpLeft;
                    else SpriteRenderer.sprite = UpRightxUpLeftxDownLeft;
                }
                else if (downRight) SpriteRenderer.sprite = DownRightxUpRightxUpLeft;
                else SpriteRenderer.sprite = UpRightxUpLeft;
            }
            else if (downLeft)
            {
                if (downRight) SpriteRenderer.sprite = UpLeftxDownLeftxDownRight;
                else SpriteRenderer.sprite = UpLeftxDownLeft;
            }
            else if (downRight) SpriteRenderer.sprite = UpLeftxDownRight;
            else SpriteRenderer.sprite = UpLeft;
        }
        else if (upRight)
        {
            if (downLeft)
            {
                if (downRight) SpriteRenderer.sprite = DownLeftxDownRightxUpRight;
                else SpriteRenderer.sprite = UpRightxDownLeft;
            }
            else if (downRight) SpriteRenderer.sprite = DownRightxUpRight;
            else SpriteRenderer.sprite = UpRight;

        }
        else if (downLeft)
        {
            if (downRight) SpriteRenderer.sprite = DownLeftxDownRight;
            else SpriteRenderer.sprite = DownLeft;
        }
        else SpriteRenderer.sprite = DownRight;
    }

    private void FindWaterNeighbors(Tile Tile)
    {
        Vector2Int[] directions = {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right,
        Vector2Int.up + Vector2Int.left,
        Vector2Int.up + Vector2Int.right,
        Vector2Int.down + Vector2Int.left,
        Vector2Int.down + Vector2Int.right};

        Voisins.Clear();
        foreach (var dir in directions)
        {
            Tile neighbor = GridManager.Instance.GetTileAtPosition(Tile.Position + dir);
            if (neighbor != null) Voisins.Add(neighbor.TileName != "Riviere");
            else Voisins.Add(false);
        }
    }
}