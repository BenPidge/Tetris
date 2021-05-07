using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static event Action<float> GameRestarted;
    public static event Action<float> GameReplayed;
    public void Restart()
    {
        CommandController.Empty();
        GameRestarted?.Invoke(Time.timeSinceLevelLoad);
    }

    public void Replay()
    {
        GameReplayed?.Invoke(Time.timeSinceLevelLoad);
    }
    
    public void ExitGame()
    {
        if (SaveSystem.CurrentAccount != null)
        {
            SaveSystem.CurrentAccount.UpdateHighScore(PointsManager.GetPoints());
        }
        StartCoroutine(LoadMainMenu());
    }
    
    IEnumerator LoadMainMenu()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("MainMenu");
        
        // returns control back to the caller until it's complete
        while (!asyncOperation.isDone)
        {
            yield return null;
        }
    }
}