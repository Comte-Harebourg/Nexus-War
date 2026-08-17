using System;
using System.Collections.Generic;
using UnityEngine;

public class DualTile : MonoBehaviour
{
    public Vector2Int Position { get; set; }
    [SerializeField] private Animator BackgroundSprite;
    [SerializeField] private RuntimeAnimatorController BackgroundSpriteController;
    [SerializeField] private Animator BorderSprite;
    [SerializeField] private RuntimeAnimatorController BorderSpriteController;
    private List<Animator> PropSprites;
    private List<Tile> Neighbors;
    private Dictionary<Type, Tile> NeighborsType;

    public void UpdateSprite()
    {
        foreach (Transform child in transform) Destroy(child.gameObject);

        Neighbors = new List<Tile>
        {
            GridManager.Instance.GetTileAtPosition(Position + Vector2Int.left),
            GridManager.Instance.GetTileAtPosition(Position),
            GridManager.Instance.GetTileAtPosition(Position + Vector2Int.down + Vector2Int.left),
            GridManager.Instance.GetTileAtPosition(Position + Vector2Int.down)
        };

        NeighborsType = new Dictionary<Type, Tile>();

        foreach (Tile Tile in Neighbors)
        {
            if (Tile) NeighborsType[Tile.GetType()] = Tile;
        }

        ChildBirth(BackgroundSprite, BackgroundSpriteController, "", 1);

        if (Neighbors.Contains(null))
        {
            string bitmap = "";
            for (int i = 0; i < 4; i++)
                bitmap += (Neighbors[i] == null) ? "1" : "0";
            ChildBirth(BorderSprite, BorderSpriteController, bitmap, 2);
        }

        foreach (KeyValuePair<Type, Tile> entry in NeighborsType)
        {
            if (entry.Value.DualSprite && entry.Value.DualSpriteController)
            {
                string bitmap = "";
                for (int i = 0; i < 4; i++)
                    bitmap += (Neighbors[i].GetType() == entry.Key) ? "1" : "0";
                ChildBirth(entry.Value.DualSprite, entry.Value.DualSpriteController, bitmap, 3);
            }
        }
    }

    private GameObject ChildBirth(Animator Animator, RuntimeAnimatorController Controller, string bitmap, int layer)
    {
        GameObject child = new GameObject("DualGridAnimator");
        child.transform.SetParent(this.transform);
        child.transform.localPosition = Vector3.zero;
        child.transform.localRotation = Quaternion.identity;
        child.transform.localScale = Vector3.one;
        SpriteRenderer Rend = child.AddComponent<SpriteRenderer>();
        Rend.sortingLayerName = this.GetComponent<SpriteRenderer>().sortingLayerName;
        Rend.sortingOrder = layer;
        Animator = child.AddComponent<Animator>();
        Animator.runtimeAnimatorController = Controller;
        if (bitmap.Length == 4) Animator.Play(bitmap);
        else Animator.Play(RandomBitmap());
        return child;
    }

    private string RandomBitmap()
    {
        System.Random random = new System.Random();
        string resultat = "";
        for (int i = 0; i < 4; i++)
            resultat += random.Next(0, 2);
        return resultat;
    }
}