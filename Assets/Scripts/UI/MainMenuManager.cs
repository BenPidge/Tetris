using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MenuManager
{
    public static bool isLoadingResume = false;

    public void StartGame()
    {
        isLoadingResume = false;
        BtnClicked("MainGame");
    }

    public void ResumeGame()
    {
        if (SaveSystem.CurrentAccount == null || SaveSystem.CurrentAccount.Save == null) return;
        isLoadingResume = true;
        BtnClicked("MainGame");
    }
    
    public void StartTutorial()
    {
        isLoadingResume = false;
        BtnClicked("Tutorial");
    }
    
    public void ExitGame()
    {
        Sounds.PlayBtnClick();
        for (int i = 0; i < SaveSystem.Accounts.Count; i++)
        {
            SaveSystem.SaveAccount(SaveSystem.Accounts[i]);
        }
        Application.Quit();
    }
}
