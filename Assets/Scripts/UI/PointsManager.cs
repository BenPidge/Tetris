using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class PointsManager : MonoBehaviour
{
    private int _score;
    private TextMeshProUGUI _pointsText;
    private TetrominoManager _manager;

    private void Awake()
    {
        _score = 0;
        _pointsText = gameObject.GetComponent<TextMeshProUGUI>();
        _manager = FindObjectOfType<TetrominoManager>();
    }
    
    private void OnEnable()
    {
        TetrominoManager.RowCleared += IncrementScore;
        GameOverManager.GameRestarted += ResetPoints;
    }
    
    private void OnDisable()
    {
        TetrominoManager.RowCleared -= IncrementScore;
        GameOverManager.GameRestarted -= ResetPoints;
    }



    private void ResetPoints(float timeCalled)
    {
        _score = 0;
        SetPointsText();
    }
    
    private void SetPointsText()
    {
        _pointsText.text = "Points: " + _score;
    }

    private void IncrementScore(int increment)
    {
        _score += increment;
        SetPointsText();
        if (_score % 1000 == 0 && _score > 0)
        {
            _manager.IncreaseFallSpeed();
        }
    }
    
}
