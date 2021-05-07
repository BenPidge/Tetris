using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public static bool isLoadingResume = false;

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
        for (int i = 0; i < SaveSystem.Accounts.Count; i++)
        {
            SaveSystem.SaveAccount(SaveSystem.Accounts[i]);
        }
        Application.Quit();
    }

    
    
    IEnumerator LoadResumedGame()
    {
        if (SaveSystem.CurrentAccount == null || SaveSystem.CurrentAccount.Save == null) yield break;
        isLoadingResume = true;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("MainGame");

        // returns control back to the caller until it's complete
        while (!asyncOperation.isDone)
        {
            yield return null;
        }
    }

    IEnumerator LoadGame()
    {
        isLoadingResume = false;
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("MainGame");
        
        // returns control back to the caller until it's complete
        while (!asyncOperation.isDone)
        {
            yield return null;
        }
    }
    
    IEnumerator LoadTutorial()
    {
        isLoadingResume = false;
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("Tutorial");
        
        // returns control back to the caller until it's complete
        while (!asyncOperation.isDone)
        {
            yield return null;
        }
    }
}
