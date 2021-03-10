using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TetrominoManager : MonoBehaviour
{
    [SerializeField] private Vector2 spawnPoint;
    [SerializeField] private List<GameObject> prefabs;

    private void OnEnable()
    {
        TetrominoBehaviour.Landed += NewTetromino;
    }

    private void OnDisable()
    {
        TetrominoBehaviour.Landed -= NewTetromino;
    }

    private void NewTetromino(Vector2[] vectors, Sprite sprite)
    {
        var prefabPos = Random.Range(0, 7);
        Instantiate(prefabs[prefabPos],
            spawnPoint, Quaternion.identity);
    }
}
