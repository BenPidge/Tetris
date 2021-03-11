using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LandedItems : MonoBehaviour
{
    public static event Action<float> TetrominoPlaced;
    
    [SerializeField] private GameObject prefab;
    private SpriteRenderer _sprite;
    private List<GameObject> landedSquares = new List<GameObject>();

    private void Awake()
    {
        _sprite = prefab.GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        TetrominoBehaviour.Landed += AddSquares;
    }

    private void OnDisable()
    {
        TetrominoBehaviour.Landed -= AddSquares;
    }

    public void AddSquares(Vector2[] positions, Sprite colour)
    {
        _sprite.sprite = colour;
        float highestY = -200;
        
        foreach (var position in positions)
        {
            var nextSquare = Instantiate(prefab,
                position, Quaternion.identity);
            nextSquare.transform.SetParent(gameObject.transform);
            landedSquares.Append(nextSquare);

            var collider = nextSquare.GetComponent<BoxCollider2D>();
            if (collider.bounds.max.y > highestY && Math.Abs(collider.transform.position.x) < 1)
            {
                highestY = collider.bounds.max.y;
            }
        }
        
        TetrominoPlaced?.Invoke(highestY);
    }
}
