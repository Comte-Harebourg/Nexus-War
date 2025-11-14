using UnityEngine;

public class GrassTile : Tile
{
    [SerializeField] private Color _baseColor, _offsetColor;

    public override void Init(int x, int y)
    {
        var isOffset = (x + y) % 2 == 1; ///Vérifie si la case est décalée par rapport à l'origine
        _renderer.color = isOffset ? _offsetColor : _baseColor;
    }
}
