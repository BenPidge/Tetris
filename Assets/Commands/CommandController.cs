using System;
using System.Collections.Generic;
using UnityEngine;

public class CommandController : MonoBehaviour
{
    private static bool _isTutorial;
    
    private static List<Command> _commands = new List<Command>();
    private static TetrominoManager _tetrominoManager;
    private static TutorialManager _tutorialManager;
    private static int _activeManager;
    private static float _timer;

    private void Start()
    {
        _tetrominoManager = FindObjectOfType<TetrominoManager>();
        _tutorialManager = FindObjectOfType<TutorialManager>();
        _isTutorial = _tutorialManager != null;
    }

    private void OnEnable()
    {
        TetrominoManager.ReplayBegun += RunCommands;
    }

    private void OnDisable()
    {
        TetrominoManager.ReplayBegun -= RunCommands;
    }
    
    
    
    public static void ExecuteCommand(Command command)
    {
        _commands.Add(command);
        command.Execute(GetBehaviour());
    }

    public void RunOldestCommand()
    {
        if (_tetrominoManager.currentTetromino == null) return;
        _commands[0].Execute(GetBehaviour());
        _commands.RemoveAt(0);
    }

    public static void Empty()
    {
        _commands.Clear();
    }

    private void RunCommands()
    {
        for (int i = 0; i < _commands.Count; i++)
        {
            Invoke(nameof(RunOldestCommand), _commands[i].TimeExecuted);
        }
    }

    private static TetrominoBehaviour GetBehaviour()
    {
        return (_isTutorial
            ? _tutorialManager.currentTetromino.GetComponent<TetrominoBehaviour>()
            : _tetrominoManager.currentTetromino.GetComponent<TetrominoBehaviour>());
    }
}