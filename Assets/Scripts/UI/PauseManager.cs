using System;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static event Action<float> GameRestarted;
    public static event Action Unpaused;
    
    public void Restart()
    {
        CommandController.Empty();
        GameRestarted?.Invoke(Time.timeSinceLevelLoad);
    }

    public void Resume()
    {
        Unpaused?.Invoke();
        gameObject.SetActive(false);
    }
    
    public void ExitGame()
    {
        Save();
        Application.Quit();
    }

    private void Save()
    {
        List<(Vector2, Sprite)> blocks = new List<(Vector2, Sprite)>();
        List<GameObject> landedItems = FindObjectOfType<LandedItems>().landedSquares;
        for (int i = 0; i < landedItems.Count; i++)
        {
            blocks.Add((landedItems[i].transform.position, landedItems[i].GetComponent<SpriteRenderer>().sprite));
        }
        
        GameSave save = new GameSave(PointsManager.GetPoints(), blocks, 
            FindObjectOfType<TetrominoManager>().currentTetromino);
        SaveSystem.SaveGame(save);
    }
}