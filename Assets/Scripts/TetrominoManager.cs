using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TetrominoManager : MonoBehaviour
{
    [SerializeField] private float highestY;
    [SerializeField] private Vector2 spawnPoint;
    [SerializeField] private List<GameObject> prefabs;
    [SerializeField] public float fallSpeed;
    [SerializeField] public int lineWidth;
    
    private static bool _gameOver;
    private LandedItems _landedItems;
    
    private void Start()
    {
        _gameOver = false;
        NewTetromino(null, null);
        _landedItems = FindObjectOfType<LandedItems>();
    }

    private void OnEnable()
    {
        TetrominoBehaviour.Landed += NewTetromino;
        LandedItems.TetrominoPlaced += CheckProgress;
    }

    private void OnDisable()
    {
        TetrominoBehaviour.Landed -= NewTetromino;
        LandedItems.TetrominoPlaced -= CheckProgress;
    }

    private void NewTetromino(Vector2[] vectors, Sprite sprite)
    {
        if (_gameOver) return;

        var prefabPos = Random.Range(0, 7);
        Instantiate(prefabs[prefabPos], spawnPoint, Quaternion.identity);
    }

    private void CheckProgress(float highestBlockY)
    {
        if (highestBlockY > highestY)
        {
            _gameOver = true;
        }
        checkLines();
    }

    private void checkLines()
    {
        // rows uses the y coordinate key to link to a counter of items in that row
        var rows = new Dictionary<float, int>();
        var fullRows = new List<float>();
        
        foreach (var item in _landedItems.landedSquares)
        {
            var pos = item.transform.position.y;
            if (rows.ContainsKey(pos))
            {
                rows[pos] += 1;
            } 
            else
            {
                rows.Add(pos, 1);
            }

            if (rows[pos] >= lineWidth && !fullRows.Contains(pos))
            {
                fullRows.Add(pos);
            }
        }

        foreach (var row in fullRows)
        {
            clearRow(row);
        }
    }

    private void clearRow(float row)
    {
        Debug.Log(row);
        //implement me
    }
}
