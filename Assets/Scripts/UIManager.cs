using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public int Score { private set; get; }

    private void Awake()
    {
        Score = 0;
    }

    private void OnEnable()
    {
        TetrominoManager.RowCleared += IncrementScore;
    }
    
    private void OnDisable()
    {
        TetrominoManager.RowCleared -= IncrementScore;
    }
    
    private void IncrementScore(int increment)
    {
        Score += increment;
    }
}
