using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(TextMeshProUGUI))]
public class PointsManager : MonoBehaviour
{
    private int _score;
    [SerializeField] private TextMeshProUGUI pointsText;
    private TetrominoManager _manager;

    private void Awake()
    {
        _score = 0;
        pointsText = gameObject.GetComponent<TextMeshProUGUI>();
        _manager = FindObjectOfType<TetrominoManager>();
    }
    
    private void OnEnable()
    {
        TetrominoManager.RowCleared += IncrementScore;
        GameOverManager.GameRestarted += ResetPoints;
        GameOverManager.GameReplayed += ResetPoints;
    }
    
    private void OnDisable()
    {
        TetrominoManager.RowCleared -= IncrementScore;
        GameOverManager.GameRestarted -= ResetPoints;
        GameOverManager.GameReplayed -= ResetPoints;
    }



    private void ResetPoints(float timeCalled)
    {
        _score = 0;
        SetPointsText();
    }
    
    private void SetPointsText()
    {
        pointsText.text = "Points: " + _score;
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
