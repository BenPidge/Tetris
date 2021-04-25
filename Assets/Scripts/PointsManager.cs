using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class UIManager : MonoBehaviour
{
    private int _score;
    private TextMeshProUGUI _pointsText;

    private void Awake()
    {
        _score = 0;
        _pointsText = GetComponent<TextMeshProUGUI>();
    }

    private void SetPointsText()
    {
        _pointsText.text = "Points: " + _score;
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
        _score += increment;
        SetPointsText();
    }
    
}
