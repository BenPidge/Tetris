using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LandedItems : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    private SpriteRenderer _sprite;
    private List<GameObject> landedSquares = new List<GameObject>();

    private void OnEnable()
    {
        _sprite = prefab.GetComponent<SpriteRenderer>();
        TetrominoBehaviour.Landed += AddSquares;
    }

    private void OnDisable()
    {
        TetrominoBehaviour.Landed -= AddSquares;
    }

    public void AddSquares(Vector2[] positions, Sprite colour)
    {
        _sprite.sprite = colour;
        foreach (var position in positions)
        {
            var nextSquare = Instantiate(prefab,
                position, Quaternion.identity);
            nextSquare.transform.SetParent(gameObject.transform);
            landedSquares.Append(nextSquare);
        }
    }
}
