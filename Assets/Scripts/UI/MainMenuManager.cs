using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public static bool isLoadingResume = false;
    [SerializeField] private TransitionManager transition;

    public void StartGame()
    {
        isLoadingResume = false;
        StartCoroutine(transition.LoadSceneWithAnim("MainGame"));
    }

    public void ResumeGame()
    {
        if (SaveSystem.CurrentAccount == null || SaveSystem.CurrentAccount.Save == null) return;
        isLoadingResume = true;
        StartCoroutine(transition.LoadSceneWithAnim("MainGame"));
    }
    
    public void StartTutorial()
    {
        isLoadingResume = false;
        StartCoroutine(transition.LoadSceneWithAnim("Tutorial"));
    }
    
    public void ExitGame()
    {
        for (int i = 0; i < SaveSystem.Accounts.Count; i++)
        {
            SaveSystem.SaveAccount(SaveSystem.Accounts[i]);
        }
        Application.Quit();
    }
}
