﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class CommandController : MonoBehaviour
{
    private static bool _isTutorial;
    
    private static List<Command> _commands = new List<Command>();
    private static GameManager _manager;
    private static float _timer;

    private void Start()
    {
        _manager = FindObjectOfType<TutorialManager>();
        _isTutorial = _manager != null;
        if (!_isTutorial)
        {
            _manager = FindObjectOfType<TetrominoManager>();
        }
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
        command.Execute(_manager.currentTetromino.GetComponent<TetrominoBehaviour>());
    }

    private void RunCommands()
    {
        for (int i = 0; i < _commands.Count; i++)
        {
            Invoke(nameof(RunOldestCommand), _commands[i].TimeExecuted);
        }
    }
    
    public void RunOldestCommand()
    {
        if (_manager.currentTetromino == null) return;
        _commands[0].Execute(_manager.currentTetromino.GetComponent<TetrominoBehaviour>());
        _commands.RemoveAt(0);
    }

    public static void Empty()
    {
        _commands.Clear();
    }
}