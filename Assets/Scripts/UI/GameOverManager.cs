using System;
using UnityEngine;

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
        Application.Quit();
    }
}