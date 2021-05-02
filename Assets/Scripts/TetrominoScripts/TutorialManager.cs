using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class TutorialManager : MonoBehaviour
{
    public static event Action<int> RowCleared;
    
    //queues are not supported as serializable, so these must be lists
    [SerializeField] private float highestY;
    [SerializeField] private List<GameObject> prefabs;
    [SerializeField] private Vector2 spawnPoint;
    [SerializeField] private List<string> promptText;
    [SerializeField] private TextMeshProUGUI promptTextBox;
    [SerializeField] private List<TutorialStep> commands;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject specialPrefab;
    
    public GameObject currentTetromino;
    private Queue<Action> _callOrder = new Queue<Action>();
    private bool _keepExecuting;
    private LandedItems _landedItems;

    [Serializable]
    public class TutorialStep
    {
        public string stepType;
        public List<float> inputs;
    }
    
    
    
    private void Start()
    {
        _landedItems = FindObjectOfType<LandedItems>();
        SetupPriorSquares();

        gameOverPanel.gameObject.SetActive(false);
        for (int i = 0; i < commands.Count; i++)
        {
            ExtractStep(commands[i]);
        }
    }

    private void OnEnable()
    {
        TransformCommand.Transformed += CreateTransformedTetromino;
    }

    private void OnDisable()
    {
        TransformCommand.Transformed -= CreateTransformedTetromino;
    }
    


    private void SetupPriorSquares()
    {
        _landedItems.AddSquares(new []{new Vector2(2.5f, -3.5f), new Vector2(3.5f, -3.5f), 
                new Vector2(3.5f, -2.5f), new Vector2(4.5f, -2.5f)}, 
            Resources.Load<Sprite>("Sprites/Squares/GreenSquare"));
        _landedItems.AddSquares(new []{new Vector2(7.5f, -3.5f), new Vector2(6.5f, -3.5f), 
                new Vector2(5.5f, -3.5f), new Vector2(4.5f, -3.5f)}, 
            Resources.Load<Sprite>("Sprites/Squares/BlueSquare"));
        _landedItems.AddSquares(new []{new Vector2(9.5f, -3.5f), new Vector2(8.5f, -3.5f), 
                new Vector2(9.5f, -2.5f), new Vector2(8.5f, -2.5f)}, 
            Resources.Load<Sprite>("Sprites/Squares/YellowSquare"));
        _landedItems.AddSquares(new []{new Vector2(-3.5f, -3.5f), new Vector2(-4.5f, -3.5f), 
                new Vector2(-5.5f, -3.5f), new Vector2(-5.5f, -2.5f)}, 
            Resources.Load<Sprite>("Sprites/Squares/OrangeSquare"));
        _landedItems.AddSquares(new []{new Vector2(-6.5f, -3.5f), new Vector2(-7.5f, -3.5f), 
                new Vector2(-8.5f, -3.5f), new Vector2(-8.5f, -2.5f)}, 
            Resources.Load<Sprite>("Sprites/Squares/LightBlueSquare"));
        _landedItems.AddSquares(new []{new Vector2(-3.5f, -2.5f), 
                new Vector2(-2.5f, -2.5f), new Vector2(-1.5f, -3.5f), new Vector2(-2.5f, -3.5f)}, 
            Resources.Load<Sprite>("Sprites/Squares/RedSquare"));
    }
    
    public void ExecuteNextStep()
    {
        if (!_keepExecuting && promptText.Count > 0)
        {
            _keepExecuting = true;
            promptTextBox.text = promptText[0];
            promptText.RemoveAt(0);
            StartCoroutine(ExecuteStepWithPause());
        }
    }

    private void ExtractStep(TutorialStep step)
    {
        Action action;
        switch (step.stepType)
        {
            case "MoveCommand":
                action = () => StartCoroutine(MoveCommand(step.inputs));
                break;
            case "RotateCommand":
                action = () => StartCoroutine(RotateCommand());
                break;
            case "RemoveRow":
                action = () => StartCoroutine(RemoveRow((int) step.inputs[0]));
                break;
            case "LoadMenu":
                action = () => StartCoroutine(LoadMenu());
                break;
            default:
                action = () => Invoke(step.stepType, 0);
                break;
        }
        _callOrder.Enqueue(action);
    }

    private void Break()
    {
        _keepExecuting = false;
    }
    
    private void NextTetromino()
    {
        GameObject nextPrefab = prefabs[0];
        prefabs.RemoveAt(0);
        currentTetromino = Instantiate(nextPrefab, spawnPoint, Quaternion.identity);
        currentTetromino.GetComponent<TetrominoBehaviour>().SetReplay(true);
    }

    public void TransformTetromino()
    {
        RowCleared?.Invoke(100);
        CommandController.ExecuteCommand(new TransformCommand(Time.timeSinceLevelLoad, 150));
        currentTetromino.GetComponent<TetrominoBehaviour>().SetReplay(true);
    }
    
    private void CreateTransformedTetromino(int cost)
    {
        Vector2 pos = currentTetromino.GetComponent<TetrominoBehaviour>().wholeRigidbody.position;
        Destroy(currentTetromino);
        currentTetromino = Instantiate(specialPrefab, pos, Quaternion.identity);
    }
    
    
    
    IEnumerator RemoveRow(int row)
    {
        yield return new WaitForSeconds(0.5f);
        _landedItems.RemoveRow(row);
        int highestPotentialRow = (int) Math.Floor(highestY);
        for (int i = row + 1; i <= highestPotentialRow; i++)
        {
            _landedItems.LowerRow(i);
        }
        RowCleared?.Invoke(100);
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator ExecuteStepWithPause()
    {
        while (_keepExecuting && _callOrder.Count > 0)
        {
            yield return new WaitForSeconds(0.5f);
            _callOrder.Dequeue()();
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    IEnumerator MoveCommand(List<float> inputs)
    {
        yield return new WaitForSeconds(0.1f);
        CommandController.ExecuteCommand(new MoveCommand(0, inputs[0], 
            inputs[1], (int) inputs[2]));
    }
    
    IEnumerator RotateCommand()
    {
        yield return new WaitForSeconds(0.1f);
        CommandController.ExecuteCommand(new RotateCommand(0));
    }
    
    IEnumerator LoadMenu()
    {
        Break();
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("MainMenu");
        
        // returns control back to the caller until it's complete
        while (!asyncOperation.isDone)
        {
            yield return null;
        }
    }
}