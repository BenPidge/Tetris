using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MenuManager
{
    public static event Action<float> GameRestarted;
    public static event Action<float> GameReplayed;
    public void Restart()
    {
        Sounds.PlayBtnClick();
        CommandController.Empty();
        GameRestarted?.Invoke(Time.timeSinceLevelLoad);
    }

    public void Replay()
    {
        Sounds.PlayBtnClick();
        GameReplayed?.Invoke(Time.timeSinceLevelLoad);
    }
    
    public void ExitGame()
    {
        if (SaveSystem.CurrentAccount != null)
        {
            SaveSystem.CurrentAccount.UpdateHighScore(PointsManager.GetPoints());
            SaveSystem.SaveAccount(SaveSystem.CurrentAccount);
        }
        BtnClicked("MainMenu");
    }
}