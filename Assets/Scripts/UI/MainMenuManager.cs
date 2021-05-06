using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        StartCoroutine(LoadGame());
    }

    public void ResumeGame()
    {
        StartCoroutine(LoadResumedGame());
    }
    
    public void StartTutorial()
    {
        StartCoroutine(LoadTutorial());
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }

    
    
    IEnumerator LoadGame()
    {
        if (SaveSystem.CurrentAccount == null || SaveSystem.CurrentAccount.Save == null) yield break;
        GameSave save = SaveSystem.CurrentAccount.Save;
        
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("MainGame");

        // returns control back to the caller until it's complete
        while (!asyncOperation.isDone)
        {
            yield return null;
        }

        GetComponent<LandedItems>().ResumeGame(save.Blocks);
        GetComponent<TetrominoManager>().ResumeGame(save.Points, save.ActiveTetromino);
    }

    IEnumerator LoadResumedGame()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("MainGame");
        
        // returns control back to the caller until it's complete
        while (!asyncOperation.isDone)
        {
            yield return null;
        }
    }
    
    IEnumerator LoadTutorial()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("Tutorial");
        
        // returns control back to the caller until it's complete
        while (!asyncOperation.isDone)
        {
            yield return null;
        }
    }
}
