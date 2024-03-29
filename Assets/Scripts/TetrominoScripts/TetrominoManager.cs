﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class TetrominoManager : GameManager
{
    public static event Action<int> RowCleared;
    public static event Action ReplayBegun;
    public static event Action TetrominoTransformed;
    
    [SerializeField] public float fallSpeed;
    [SerializeField] public float fallSpeedIncrease;
    [SerializeField] public int lineWidth;
    [SerializeField] public int rowScoreInc;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private int transformCost;

    private readonly Queue<GameObject> _usedPrefabs = new Queue<GameObject>();
    public GameObject currentTetrominoPrefab;
    private TetrominoBehaviour _currentTetrominoCode;
    private Dictionary<int, int> _rows;
    private static bool _gameOver;
    private static bool _isReplay;
    private bool _isTransformed;

    private void Start()
    {
        _gameOver = false;
        _isReplay = false;
        LandedItems = FindObjectOfType<LandedItems>();
        if (MainMenuManager.isLoadingResume)
        {
            ResumeGame();
        }
        else
        {
            NewTetromino();
        }
        _rows = new Dictionary<int, int>();
        gameOverPanel.gameObject.SetActive(false);
        pausePanel.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        LandedItems.TetrominoPlaced += CheckProgress;
        GameOverManager.GameRestarted += ResetGame;
        GameOverManager.GameReplayed += BeginReplay;
        PauseManager.GameRestarted += ResetGame;
        PauseManager.Unpaused += UnpauseGame;
        TransformCommand.Transformed += CreateTransformedTetromino;
    }

    private void OnDisable()
    {
        LandedItems.TetrominoPlaced -= CheckProgress;
        GameOverManager.GameRestarted -= ResetGame;
        GameOverManager.GameReplayed -= BeginReplay;
        PauseManager.GameRestarted -= ResetGame;
        PauseManager.Unpaused -= UnpauseGame;
        TransformCommand.Transformed -= CreateTransformedTetromino;
    }

    
    
    public void IncreaseFallSpeed()
    {
        fallSpeed += fallSpeedIncrease;
    }

    public override void TransformTetromino()
    {
        if (PointsManager.GetPoints() >= 150 && !_isTransformed && !_currentTetrominoCode.paused)
        {
            _isTransformed = true;
            CommandController.ExecuteCommand(new TransformCommand(Time.timeSinceLevelLoad, transformCost));
            TetrominoTransformed?.Invoke();
        }
    }

    
    
    private void ResumeGame()
    {
        GameSave save = SaveSystem.CurrentAccount.Save;
        LandedItems.ResumeGame(save.GETBlocks());
        RowCleared?.Invoke(save.points);
        currentTetrominoPrefab = save.GETTetromino();
        currentTetromino = Instantiate(currentTetrominoPrefab, 
            save.GETTetrominoPos(), Quaternion.identity);
        _currentTetrominoCode = currentTetromino.GetComponent<TetrominoBehaviour>();
    }
    
    private async void BeginReplay(float startTime)
    {
        await new WaitUntil(() => LandedItems.landedSquares.Count == 0);
        gameOverPanel.gameObject.SetActive(false);
        _gameOver = false;
        _isReplay = true;
        NewTetromino();
        ReplayBegun?.Invoke();
    }

    private async void ResetGame(float startTime)
    {
        await new WaitUntil(() => LandedItems.landedSquares.Count == 0);
        gameOverPanel.gameObject.SetActive(false);
        pausePanel.gameObject.SetActive(false);
        _gameOver = false;
        _isReplay = false;
        Destroy(currentTetromino);
        NewTetromino();
    }
    
    private void GameOver()
    {
        _gameOver = true;
        gameOverPanel.gameObject.SetActive(true);
        if (SaveSystem.CurrentAccount != null)
        {
            SaveSystem.CurrentAccount.UpdateHighScore(PointsManager.GetPoints());
        }
    }

    public void PauseGame()
    {
        _currentTetrominoCode.paused = true;
        pausePanel.gameObject.SetActive(true);
    }

    private void UnpauseGame()
    {
        _currentTetrominoCode.paused = false;
    }

    private void NewTetromino()
    {
        _isTransformed = false;
        if (_gameOver) return;
        GameObject nextPrefab;
        
        // gets the appropriate prefab if it's a replay, or randomly selects one otherwise
        if (_isReplay)
        {
            nextPrefab = _usedPrefabs.Dequeue();
        }
        else
        {
            int prefabPos = Random.Range(0, prefabs.Count);
            nextPrefab = prefabs[prefabPos];
            _usedPrefabs.Enqueue(nextPrefab);
        }

        currentTetrominoPrefab = nextPrefab;
        currentTetromino = Instantiate(nextPrefab, spawnPoint, Quaternion.identity);
        _currentTetrominoCode = currentTetromino.GetComponent<TetrominoBehaviour>();
        _currentTetrominoCode.SetReplay(_isReplay);
        Transform[] children = _currentTetrominoCode.children;
            
        for (int i = 0; i < children.Length; i++)
        {
            if (LandedItems.checkSquare(children[i].position))
            {
                Destroy(currentTetromino);
                GameOver();
                return;
            }
        }
    }

    private void CreateTransformedTetromino(int cost)
    {
        Vector2 pos = currentTetromino.GetComponent<TetrominoBehaviour>().wholeRigidbody.position;
        Destroy(currentTetromino);
        currentTetromino = Instantiate(specialPrefab, pos, Quaternion.identity);
    }
    
    private void CheckProgress(float highestBlockY)
    {
        if (highestBlockY > highestY)
        {
            _gameOver = true;
            GameOver();
        }
        CheckLines();
        NewTetromino();
    }

    private void CheckLines()
    {
        // rows uses the y coordinate key to link to a counter of items in that row
        // the y coordinate is rounded to avoid minor inaccuracies
        _rows.Clear();
        List<int> fullRows = new List<int>();
        
        for (int i = 0; i < LandedItems.landedSquares.Count; i++)
        {
            int pos = (int) Math.Floor(LandedItems.landedSquares[i].transform.position.y);
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
    }

    private void ClearRow(int row)
    {
        LandedItems.RemoveRow(row);
        int highestPotentialRow = (int) Math.Floor(highestY);
        for (int i = row + 1; i <= highestPotentialRow; i++)
        {
            LandedItems.LowerRow(i);
        }
        RowCleared?.Invoke(rowScoreInc);
    }
}
