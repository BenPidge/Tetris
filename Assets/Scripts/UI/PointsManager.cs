using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(TextMeshProUGUI))]
public class PointsManager : MonoBehaviour
{
    private static int _score;
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
        TutorialManager.RowCleared += IncrementScore;
        PauseManager.GameRestarted += ResetPoints;
        GameOverManager.GameRestarted += ResetPoints;
        GameOverManager.GameReplayed += ResetPoints;
        TransformCommand.Transformed += DecreaseScore;
    }
    
    private void OnDisable()
    {
        TetrominoManager.RowCleared -= IncrementScore;
        TutorialManager.RowCleared -= IncrementScore;
        PauseManager.GameRestarted -= ResetPoints;
        GameOverManager.GameRestarted -= ResetPoints;
        GameOverManager.GameReplayed -= ResetPoints;
        TransformCommand.Transformed -= DecreaseScore;
    }



    public static int GetPoints()
    {
        return _score;
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

    private void DecreaseScore(int amnt)
    {
        _score -= amnt;
        SetPointsText();
    }
}
