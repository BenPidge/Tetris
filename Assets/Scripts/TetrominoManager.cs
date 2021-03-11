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

    private static bool _gameOver;
    
    private void Start()
    {
        _gameOver = false;
        NewTetromino(null, null);
    }

    private void OnEnable()
    {
        TetrominoBehaviour.Landed += NewTetromino;
        LandedItems.TetrominoPlaced += CheckProgress;
    }

    private void OnDisable()
    {
        TetrominoBehaviour.Landed -= NewTetromino;
        LandedItems.TetrominoPlaced += CheckProgress;
    }

    private void NewTetromino(Vector2[] vectors, Sprite sprite)
    {
        if (_gameOver) return;
        
        var prefabPos = Random.Range(0, 7);
        Instantiate(prefabs[prefabPos],
            spawnPoint, Quaternion.identity);
    }

    private void CheckProgress(float highestBlockY)
    {
        if (highestBlockY > highestY)
        {
            _gameOver = true;
        }
    }
}
