using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]public class GameSave
{
    public int points;
    private List<(float, float, string)> _blocks = new List<(float, float, string)>();
    private string _activeTetromino;
    private (float, float) _tetrominoPos;

    public GameSave(int points, List<(Vector2, Sprite)> blocks, GameObject tetromino, Vector2 position)
    {
        this.points = points;
        _activeTetromino = tetromino.name;
        _tetrominoPos = (position.x, position.y);
        
        for (int i = 0; i < blocks.Count; i++)
        {
            _blocks.Add((blocks[i].Item1.x, blocks[i].Item1.y, blocks[i].Item2.name));
        }
    }

    public GameObject GETTetromino()
    {
        return Resources.Load("Prefabs/Tetrominoes/" + _activeTetromino) as GameObject;
    }
    
    public Vector2 GETTetrominoPos()
    {
        return new Vector2(_tetrominoPos.Item1, _tetrominoPos.Item2);
    }

    public List<(Vector2, Sprite)> GETBlocks()
    {
        List<(Vector2, Sprite)> convertedBlocks = new List<(Vector2, Sprite)>();
        for (int i = 0; i < _blocks.Count; i++)
        {
            convertedBlocks.Add((new Vector2(_blocks[i].Item1, _blocks[i].Item2), 
                SaveSystem.GetSprite(_blocks[i].Item3)));
        }

        return convertedBlocks;
    }
}
