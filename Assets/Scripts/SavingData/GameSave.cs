using System.Collections.Generic;
using UnityEngine;

public class GameSave
{
    private int _points;
    private List<(Vector2, string)> _blocks;
    private GameObject _activeTetromino;

    public GameSave(int points, List<(Vector2, string)> blocks, GameObject tetromino)
    {
        _points = points;
        _blocks = blocks;
        _activeTetromino = tetromino;
    }
}
