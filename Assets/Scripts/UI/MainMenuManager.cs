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
