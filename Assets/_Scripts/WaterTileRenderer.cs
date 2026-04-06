using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaterTileRenderer : MonoBehaviour
{
    public SpriteRenderer Sprite;
    public Sprite Up;
    public Sprite Right;
    public Sprite Down;
    public Sprite Left;
    public Sprite UpxRight;
    public Sprite UpxLeft;
    public Sprite DownxRight;
    public Sprite DownxLeft;
    public Sprite UpxRightxLeft;
    public Sprite UpxDownxRight;
    public Sprite UpxDownxLeft;
    public Sprite DownxRightxLeft;
    public Sprite UpxDownxRightxLeft;
    public Sprite UpxDown;
    public Sprite RightxLeft;
    public Sprite UpRight;
    public Sprite UpLeft;
    public Sprite DownRight;
    public Sprite DownLeft;
    public Sprite DownLeftxDownRight;
    public Sprite DownRightxUpRight;
    public Sprite UpRightxUpLeft;
    public Sprite UpLeftxDownLeft;
    public Sprite UpRightxDownLeft;
    public Sprite UpLeftxDownRight;
    public Sprite DownLeftxDownRightxUpRight;
    public Sprite DownRightxUpRightxUpLeft;
    public Sprite UpRightxUpLeftxDownLeft;
    public Sprite UpLeftxDownLeftxDownRight;
    public Sprite UpxDownRight;
    public Sprite UpxDownLeft;
    public Sprite UpxDownRightxDownLeft;
    public Sprite RightxDownLeft;
    public Sprite RightxUpLeft;
    public Sprite RightxDownLeftxUpLeft;
    public Sprite DownxUpLeft;
    public Sprite DownxUpRight;
    public Sprite DownxUpLeftxUpRight;
    public Sprite LeftxDownRight;
    public Sprite LeftxUpRight;
    public Sprite LeftxDownRightxUpRight;
    public Sprite UpxRightxDownLeft;
    public Sprite UpxLeftxDownRight;
    public Sprite DownxLeftxUpRight;
    public Sprite DownxRightxUpLeft;
    public Sprite DownLeftxDownRightxUpRightxUpLeft;
    public Sprite Neutral;
    private List<bool> Voisins = new List<bool>();

    public void UpdateSprite(Tile Tile)
    {
        FindWaterNeighbors(Tile);

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
                        Sprite.sprite = UpxDownxRightxLeft;
                    }   
                    else Sprite.sprite = UpxDownxRight;
                }
                else if (left) Sprite.sprite = UpxRightxLeft;
                else if (downLeft) Sprite.sprite = UpxRightxDownLeft;
                else Sprite.sprite = UpxRight;
            }
            else if (down)
            {
                if (left) Sprite.sprite = UpxDownxLeft;
                else Sprite.sprite = UpxDown;
            }
            else if (left)
            {
                if (downRight) Sprite.sprite = UpxLeftxDownRight;
                else Sprite.sprite = UpxLeft;
            }
            else if (downLeft)
            {
                if (downRight) Sprite.sprite = UpxDownRightxDownLeft;
                else Sprite.sprite = UpxDownLeft;
            }
            else if (downRight) Sprite.sprite = UpxDownRight;
            else Sprite.sprite = Up;
        }
        else if (right)
        {
            if (down)
            {
                if (left)
                {
                    Sprite.sprite = DownxRightxLeft;
                }
                else if (upLeft) Sprite.sprite = DownxRightxUpLeft;
                else Sprite.sprite = DownxRight;
            }
            else if (left) Sprite.sprite = RightxLeft;
            else if (upLeft)
            {
                if (downLeft) Sprite.sprite = RightxDownLeftxUpLeft;
                else Sprite.sprite = RightxUpLeft;
            }
            else if (downLeft) Sprite.sprite = RightxDownLeft;
            else Sprite.sprite = Right;
        }
        else if (down)
        {
            if (left)
            {
                if (upRight) Sprite.sprite = DownxLeftxUpRight;
                else Sprite.sprite = DownxLeft;
            }
            else if (upLeft)
            {
                if (upRight) Sprite.sprite = DownxUpLeftxUpRight;
                else Sprite.sprite = DownxUpLeft;
            }
            else if (upRight) Sprite.sprite = DownxUpRight;
            else Sprite.sprite = Down;
        }
        else if (left)
        {
            if (upRight)
            {
                if (downRight) Sprite.sprite = LeftxDownRightxUpRight;
                else Sprite.sprite = LeftxUpRight;
            }
            else if (downRight) Sprite.sprite = LeftxDownRight;
            else Sprite.sprite = Left;
        }
        else if (upLeft)
        {
            if (upRight)
            {
                if (downLeft)
                {
                    if (downRight) Sprite.sprite = DownLeftxDownRightxUpRightxUpLeft;
                    else Sprite.sprite = UpRightxUpLeftxDownLeft;
                }
                else if (downRight) Sprite.sprite = DownRightxUpRightxUpLeft;
                else Sprite.sprite = UpRightxUpLeft;
            }
            else if (downLeft)
            {
                if (downRight) Sprite.sprite = UpLeftxDownLeftxDownRight;
                else Sprite.sprite = UpLeftxDownLeft;
            }
            else if (downRight) Sprite.sprite = UpLeftxDownRight;
            else Sprite.sprite = UpLeft;
        }
        else if (upRight)
        {
            if (downLeft)
            {
                if (downRight) Sprite.sprite = DownLeftxDownRightxUpRight;
                else Sprite.sprite = UpRightxDownLeft;
            }
            else if (downRight) Sprite.sprite = DownRightxUpRight;
            else Sprite.sprite = UpRight;

        }
        else if (downLeft)
        {
            if (downRight) Sprite.sprite = DownLeftxDownRight;
            else Sprite.sprite = DownLeft;
        }
        else Sprite.sprite = DownRight;
    }

    public void FindWaterNeighbors(Tile Tile)
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
            if (neighbor != null) Voisins.Add(neighbor.TileName!="Riviere");
            else Voisins.Add(false);
        }
    }
}