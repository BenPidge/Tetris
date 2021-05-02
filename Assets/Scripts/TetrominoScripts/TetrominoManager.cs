using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class TetrominoManager : MonoBehaviour
{
    public static event Action<int> RowCleared;
    public static event Action ReplayBegun;

    [SerializeField] private float highestY;
    [SerializeField] public Vector2 spawnPoint;
    [SerializeField] private List<GameObject> prefabs;
    [SerializeField] private GameObject specialPrefab;
    [SerializeField] public float fallSpeed;
    [SerializeField] public float fallSpeedIncrease;
    [SerializeField] public int lineWidth;
    [SerializeField] public int rowScoreInc;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private int transformCost;

    private readonly Queue<GameObject> _usedPrefabs = new Queue<GameObject>();
    public GameObject currentTetromino;
    private TetrominoBehaviour _currentTetrominoCode;
    private LandedItems _landedItems;
    private Dictionary<int, int> _rows;
    private static bool _gameOver;
    private static bool _isReplay;
    private bool _isTransformed;

    private void Start()
    {
        _gameOver = false;
        _isReplay = false;
        _landedItems = FindObjectOfType<LandedItems>();
        NewTetromino();
        _rows = new Dictionary<int, int>();
        gameOverPanel.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        LandedItems.TetrominoPlaced += CheckProgress;
        GameOverManager.GameRestarted += ResetGame;
        GameOverManager.GameReplayed += BeginReplay;
        TransformCommand.Transformed += CreateTransformedTetromino;
    }

    private void OnDisable()
    {
        LandedItems.TetrominoPlaced -= CheckProgress;
        GameOverManager.GameRestarted -= ResetGame;
        GameOverManager.GameReplayed -= BeginReplay;
        TransformCommand.Transformed -= CreateTransformedTetromino;
    }

    
    
    public void IncreaseFallSpeed()
    {
        fallSpeed += fallSpeedIncrease;
    }

    public void TransformTetromino()
    {
        if (PointsManager.GetPoints() >= 150 && !_isTransformed)
        {
            _isTransformed = true;
            CommandController.ExecuteCommand(new TransformCommand(Time.timeSinceLevelLoad, transformCost));
        }
    }
    
    
    
    private async void BeginReplay(float startTime)
    {
        await new WaitUntil(() => _landedItems.landedSquares.Count == 0);
        gameOverPanel.gameObject.SetActive(false);
        _gameOver = false;
        _isReplay = true;
        NewTetromino();
        ReplayBegun?.Invoke();
    }

    private async void ResetGame(float startTime)
    {
        await new WaitUntil(() => _landedItems.landedSquares.Count == 0);
        gameOverPanel.gameObject.SetActive(false);
        _gameOver = false;
        _isReplay = false;
        NewTetromino();
    }
    
    private void GameOver()
    {
        _gameOver = true;
        gameOverPanel.gameObject.SetActive(true);
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
        
        currentTetromino = Instantiate(nextPrefab, spawnPoint, Quaternion.identity);
        _currentTetrominoCode = currentTetromino.GetComponent<TetrominoBehaviour>();
        _currentTetrominoCode.SetReplay(_isReplay);
        Transform[] children = _currentTetrominoCode.children;
            
        for (int i = 0; i < children.Length; i++)
        {
            if (_landedItems.checkSquare(children[i].position))
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
        
        for (int i = 0; i < _landedItems.landedSquares.Count; i++)
        {
            int pos = (int) Math.Floor(_landedItems.landedSquares[i].transform.position.y);
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
        _landedItems.RemoveRow(row);
        int highestPotentialRow = (int) Math.Floor(highestY);
        for (int i = row + 1; i <= highestPotentialRow; i++)
        {
            _landedItems.LowerRow(i);
        }
        RowCleared?.Invoke(rowScoreInc);
    }
}
