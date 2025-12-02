using UnityEngine;

public class ArrowTileRenderer : MonoBehaviour
{
    public SpriteRenderer arrowRenderer;

    [Header("Start Sprites")]
    public Sprite startUp;
    public Sprite startDown;
    public Sprite startLeft;
    public Sprite startRight;

    [Header("End Sprites")]
    public Sprite endUp;
    public Sprite endDown;
    public Sprite endLeft;
    public Sprite endRight;

    [Header("Body Sprites")]
    public Sprite straightVertical;
    public Sprite straightHorizontal;
    public Sprite turnUpRight;
    public Sprite turnUpLeft;
    public Sprite turnDownRight;
    public Sprite turnDownLeft;

    public void SetSprite(Sprite sprite)
    {
        arrowRenderer.sprite = sprite;
        arrowRenderer.enabled = true;
    }

    public void Clear()
    {
        arrowRenderer.enabled = false;
    }
}