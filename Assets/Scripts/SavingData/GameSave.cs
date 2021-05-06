using System.Collections.Generic;
using UnityEngine;

public class GameSave
{
    public int Points;
    public List<(Vector2, Sprite)> Blocks;
    public GameObject ActiveTetromino;

    public GameSave(int points, List<(Vector2, Sprite)> blocks, GameObject tetromino)
    {
        Points = points;
        Blocks = blocks;
        ActiveTetromino = tetromino;
    }
}
