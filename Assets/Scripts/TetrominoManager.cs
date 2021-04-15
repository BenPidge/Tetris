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

    public bool _linesInProgress;
    private static bool _gameOver;
    private LandedItems _landedItems;
    private Dictionary<int, int> _rows;
    
    private void Start()
    {
        _linesInProgress = false;
        _gameOver = false;
        NewTetromino(null, null);
        _landedItems = FindObjectOfType<LandedItems>();
        _rows = new Dictionary<int, int>();
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

        int prefabPos = Random.Range(0, prefabs.Count);
        Instantiate(prefabs[prefabPos], spawnPoint, Quaternion.identity);
    }

    private void CheckProgress(float highestBlockY)
    {
        if (highestBlockY > highestY)
        {
            _gameOver = true;
        }
        CheckLines();
    }

    private void CheckLines()
    {
        // rows uses the y coordinate key to link to a counter of items in that row
        // the y coordinate is rounded to avoid minor inaccuracies
        _linesInProgress = true;
        _rows.Clear();
        List<int> fullRows = new List<int>();
        
        for (int i = 0; i < _landedItems.landedSquares.Count; i++)
        {
            int pos = (int) Math.Round(_landedItems.landedSquares[i].transform.position.y);
            if (_rows.ContainsKey(pos))
            {
                _rows[pos] += 1;
            } 
            else
            {
                _rows.Add(pos, 1);
            }

            if (_rows[pos] == lineWidth && !fullRows.Contains(pos))
            {
                fullRows.Add(pos);
            }
        }

        for (int i = 0; i < fullRows.Count; i++)
        {
            ClearRow(fullRows[i]);
        }

        _linesInProgress = false;
    }

    private void ClearRow(int row)
    {
        _landedItems.RemoveRow(row);
        int highestPotentialRow = (int) Math.Round(highestY);
        if (row < highestPotentialRow)
        {
            for (int i = row + 1; i <= highestPotentialRow; i++)
            {
                _landedItems.LowerRow(i);
            }
        }
    }
}
