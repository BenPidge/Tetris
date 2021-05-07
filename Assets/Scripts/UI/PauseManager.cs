using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static event Action<float> GameRestarted;
    public static event Action Unpaused;
    [SerializeField] private TransitionManager transition;
    
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
        StartCoroutine(transition.LoadSceneWithAnim("MainMenu"));
    }

    private void Save()
    {
        List<(Vector2, Sprite)> blocks = new List<(Vector2, Sprite)>();
        List<GameObject> landedItems = FindObjectOfType<LandedItems>().landedSquares;
        for (int i = 0; i < landedItems.Count; i++)
        {
            blocks.Add((landedItems[i].transform.position, landedItems[i].GetComponent<SpriteRenderer>().sprite));
        }

        TetrominoManager manager = FindObjectOfType<TetrominoManager>();
        GameSave save = new GameSave(PointsManager.GetPoints(), blocks, manager.currentTetrominoPrefab, 
            manager.currentTetromino.transform.position);
        SaveSystem.SaveGame(save);
    }
}