using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LandedItems : MonoBehaviour
{
    public static event Action<float> TetrominoPlaced;
    public List<GameObject> landedSquares = new List<GameObject>();

    [SerializeField] private GameObject prefab;
    private SpriteRenderer _sprite;

    private void Awake()
    {
        _sprite = prefab.GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        GameOverManager.GameRestarted += ResetBoard;
        GameOverManager.GameReplayed += ResetBoard;
        TetrominoBehaviour.Landed += AddSquares;
    }

    private void OnDisable()
    {
        GameOverManager.GameRestarted -= ResetBoard;
        GameOverManager.GameReplayed -= ResetBoard;
        TetrominoBehaviour.Landed -= AddSquares;
    }

    public void AddSquares(Vector2[] positions, Sprite colour)
    {
        _sprite.sprite = colour;
        float highestY = -200;
        BoxCollider2D squareCollider;
        
        for (int i = 0; i < positions.Length; i++)
        {
            // rounds the positions to the nearest 0.5
            // the y value is done in a different way to ensure consistency
            Vector2 position = new Vector2((float) Math.Round(2*positions[i].x)/2, 
                (float) (Math.Floor(positions[i].y) + 0.5));
            GameObject nextSquare = Instantiate(prefab,
                position, Quaternion.identity);
            nextSquare.transform.SetParent(gameObject.transform);
            landedSquares.Add(nextSquare);

            squareCollider = nextSquare.GetComponent<BoxCollider2D>();
            if (squareCollider.bounds.max.y > highestY && Math.Abs(squareCollider.transform.position.x) < 1)
            {
                highestY = squareCollider.bounds.max.y;
            }
        }
        
        TetrominoPlaced?.Invoke(highestY);
    }

    private void ResetBoard(float timeStarted)
    {
        for (int i = 0; i < landedSquares.Count; i++)
        {
            Destroy(landedSquares[i]);
        }
        landedSquares.Clear();
    }
    
    public bool checkSquare(Vector2 position)
    {
        bool isFull = false;
        for (int i = 0; i < landedSquares.Count; i++)
        {
            if ((Vector2) landedSquares[i].transform.position == position)
            {
                isFull = true;
            }
        }

        return isFull;
    }
    
    public void RemoveRow(int yVal)
    {
        List<Tuple<GameObject, int>> row = RowContents(yVal);
        List<int> indexesToRemove = new List<int>();
        
        for (int i = 0; i < row.Count; i++)
        {
            GameObject tetromino = row[i].Item1;
            indexesToRemove.Add(row[i].Item2);
            Destroy(tetromino);
        }
        indexesToRemove.Sort((a, b) => b.CompareTo(a));
        
        for (int i = 0; i < indexesToRemove.Count; i++)
        {
            landedSquares.RemoveAt(indexesToRemove[i]);
        }
    }
    
    public void LowerRow(int yVal)
    {
        List<Tuple<GameObject, int>> row = RowContents(yVal);
        for (int i = 0; i < row.Count; i++)
        {
            row[i].Item1.transform.Translate(Vector3.down);
        }
    }

    private List<Tuple<GameObject, int>> RowContents(int yVal)
    {
        List<Tuple<GameObject, int>> contents = new List<Tuple<GameObject, int>>();
        for (int i = 0; i < landedSquares.Count; i++)
        {
            if ((int) Math.Floor(landedSquares[i].transform.position.y) == yVal)
            {
                contents.Add(new Tuple<GameObject, int>(landedSquares[i], i));
            }
        }
        return contents;
    }
}
